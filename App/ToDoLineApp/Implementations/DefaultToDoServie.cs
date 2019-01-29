using Bit.Model;
using Bit.ViewModel.Contracts;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToDoLine.Dto;
using ToDoLineApp.Contracts;

namespace ToDoLineApp.Implementations
{
    public class DefaultToDoServie : Bindable, IToDoService
    {
        public virtual IODataClient ODataClient { get; set; }

        public virtual IDateTimeProvider DateTimeProvider { get; set; }
        public virtual ObservableCollection<ToDoGroupDto> ToDoGroups { get; set; }

        public virtual ObservableCollection<ToDoItemDto> ToDoItems { get; set; }
        public virtual List<ToDoItemDto> MyDayToDoItems => ToDoItems?.Where(tdi => tdi.ShowInMyDay == true).ToList();
        public virtual List<ToDoItemDto> ImportantToDoItems => ToDoItems?.Where(tdi => tdi.IsImportant == true).ToList();
        public virtual List<ToDoItemDto> PlannedToDoItems => ToDoItems?.Where(tdi => tdi.RemindOn != null).ToList();
        public virtual List<ToDoItemDto> ToDoItemsWithoutToDoGroup => ToDoItems?.Where(tdi => tdi.ToDoGroupId == null).ToList();

        public virtual int MyDayToDoItemsCount => MyDayToDoItems?.Count ?? 0;
        public virtual int ImportantToDoItemsCount => ImportantToDoItems?.Count ?? 0;
        public virtual int PlannedToDoItemsCount => PlannedToDoItems?.Count ?? 0;
        public virtual int ToDoItemsWithoutToDoGroupCount => ToDoItemsWithoutToDoGroup?.Count ?? 0;

        public virtual bool AnyOverdueTask => PlannedToDoItems?.Any(tdi => tdi.RemindOn.Value < DateTimeProvider.GetCurrentUtcDateTime()) ?? false;

        public virtual UserDto User { get; set; }

        public virtual async Task LoadData(CancellationToken cancellationToken)
        {
            ODataBatch BatchClient = new ODataBatch(ODataClient, reuseSession: true);
            BatchClient += async client => ToDoGroups = new ObservableCollection<ToDoGroupDto>((await client.For<ToDoGroupDto>("ToDoGroups").Function("GetMyToDoGroups").FindEntriesAsync(cancellationToken)).ToList());
            BatchClient += async client => ToDoItems = new ObservableCollection<ToDoItemDto>((await client.For<ToDoItemDto>("ToDoItems").Function("GetMyToDoItems").FindEntriesAsync(cancellationToken)).ToList());
            BatchClient += async client => User = await client.For<UserDto>("User").Function("GetCurrentUser").FindEntryAsync(cancellationToken);
            await BatchClient.ExecuteAsync(cancellationToken);
        }

        private void OnToDoItemAddRemove(ToDoItemDto addedOrRemovedToDoItem)
        {
            RaisePropertyChanged(nameof(MyDayToDoItems));
            RaisePropertyChanged(nameof(ImportantToDoItems));
            RaisePropertyChanged(nameof(PlannedToDoItems));
            RaisePropertyChanged(nameof(ToDoItemsWithoutToDoGroup));
            RaisePropertyChanged(nameof(MyDayToDoItemsCount));
            RaisePropertyChanged(nameof(ImportantToDoItemsCount));
            RaisePropertyChanged(nameof(PlannedToDoItemsCount));
            RaisePropertyChanged(nameof(ToDoItemsWithoutToDoGroupCount));

            ToDoGroups.SingleOrDefault(tdg => tdg.Id == addedOrRemovedToDoItem.ToDoGroupId)?.RaiseThisChanged();
        }

        public virtual async Task<ToDoGroupDto> AddNewGroup(string groupTitle, CancellationToken cancellationToken)
        {
            ToDoGroupDto addedToDoGroup = await ODataClient
                .For<ToDoGroupDto>("ToDoGroups")
                .Action("CreateToDoGroup")
                .Set(new { title = groupTitle })
                .ExecuteAsSingleAsync(cancellationToken);

            ToDoGroups.Add(addedToDoGroup);
            return addedToDoGroup;
        }

        public virtual async Task DeleteGroup(ToDoGroupDto group, CancellationToken cancellationToken)
        {
            await ODataClient
                .For<ToDoGroupDto>("ToDoGroups")
                .Key(group.Id)
                .DeleteEntryAsync(cancellationToken);

            ToDoGroups.Remove(group);
        }

        public virtual async Task UpdateGroup(ToDoGroupDto group, CancellationToken cancellationToken)
        {
            // ToDo: Merge returned group with existing one using auto mapper!
            var serverGroup = await ODataClient
               .For<ToDoGroupDto>("ToDoGroups")
               .Key(group.Id)
               .Set(group)
               .UpdateEntryAsync(cancellationToken);

            group.Title = serverGroup.Title;
            group.ModifiedOn = serverGroup.ModifiedOn;
            group.SharedByCount = serverGroup.SharedByCount;
            group.SortedBy = serverGroup.SortedBy;
            group.Theme = serverGroup.Theme;
        }

        public virtual async Task<ToDoItemDto> AddNewItem(string newItemTitle, ItemCategory categoty, Guid? groupId, CancellationToken cancellationToken)
        {
            ToDoItemDto addedToDoItem = await ODataClient
                .For<ToDoItemDto>("ToDoItems")
                .Set(new ToDoItemDto
                {
                    Title = newItemTitle,
                    IsImportant = categoty == ItemCategory.Important,
                    ShowInMyDay = categoty == ItemCategory.MyDay,
                    IsCompleted = false,
                    ToDoGroupId = groupId,
                    DueDate = DateTimeOffset.Now
                }).InsertEntryAsync(cancellationToken);

            ToDoItems.Add(addedToDoItem);

            OnToDoItemAddRemove(addedToDoItem);

            return addedToDoItem;
        }

        public async Task DeleteItem(ToDoItemDto todoItem, CancellationToken cancellationToken)
        {
            await ODataClient
                .For<ToDoItemDto>("ToDoItems")
                .Key(todoItem.Id)
                .DeleteEntryAsync(cancellationToken);

            ToDoItems.Remove(todoItem);

            OnToDoItemAddRemove(todoItem);
        }

        public async Task UpdateItem(ToDoItemDto todoItem, CancellationToken cancellationToken)
        {
            // ToDo: Merge returned item with existing one using auto mapper!
            var serverItem = await ODataClient
               .For<ToDoItemDto>("TodoItems")
               .Key(todoItem.Id)
               .Set(todoItem)
               .UpdateEntryAsync(cancellationToken);

            todoItem.Title = serverItem.Title;
            todoItem.RemindOn = serverItem.RemindOn;
            todoItem.ShowInMyDay = serverItem.ShowInMyDay;
            todoItem.ToDoGroupId = serverItem.ToDoGroupId;
            todoItem.ToDoItemStepsCompletedCount = serverItem.ToDoItemStepsCompletedCount;
            todoItem.ToDoItemStepsCount = serverItem.ToDoItemStepsCount;
            todoItem.Notes = serverItem.Notes;
            todoItem.ModifiedOn = serverItem.ModifiedOn;
            todoItem.IsCompleted = serverItem.IsCompleted;
            todoItem.DueDate = serverItem.DueDate;
            todoItem.CompletedBy = serverItem.CompletedBy;
            todoItem.CreatedOn = serverItem.CreatedOn;
        }
    }
}
