using Bit.Model;
using Bit.ViewModel.Contracts;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
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

        public virtual List<ToDoGroupDto> ToDoGroups { get; set; }
        public virtual List<ToDoItemDto> ToDoItems { get; set; }

        public virtual List<ToDoItemDto> MyDayToDoItems => ToDoItems?.Where(tdi => tdi.ShowInMyDay == true).ToList();
        public virtual List<ToDoItemDto> ImportantToDoItems => ToDoItems?.Where(tdi => tdi.IsImportant == true).ToList();
        public virtual List<ToDoItemDto> PlannedToDoItems => ToDoItems?.Where(tdi => tdi.RemindOn != null).ToList();
        public virtual List<ToDoItemDto> ToDoItemsWithoutToDoGroup => ToDoItems?.Where(tdi => tdi.ToDoGroupId == null).ToList();

        public virtual int MyDayToDoItemsCount => MyDayToDoItems?.Count ?? 0;
        public virtual int ImportantToDoItemsCount => ImportantToDoItems?.Count ?? 0;
        public virtual int PlannedToDoItemsCount => PlannedToDoItems?.Count ?? 0;
        public virtual int ToDoItemsWithoutToDoGroupCount => ToDoItemsWithoutToDoGroup?.Count ?? 0;

        public virtual bool AnyOverdueTask => PlannedToDoItems?.Any(tdi => tdi.RemindOn.Value < DateTimeProvider.GetCurrentUtcDateTime()) ?? false;


        public virtual async Task LoadData(CancellationToken cancellationToken)
        {
            ODataBatch BatchClient = new ODataBatch(ODataClient, reuseSession: true);
            BatchClient += async client => ToDoGroups = (await client.For<ToDoGroupDto>("ToDoGroups").Function("GetMyToDoGroups").FindEntriesAsync(cancellationToken)).ToList();
            BatchClient += async client => ToDoItems = (await client.For<ToDoItemDto>("ToDoItems").Function("GetMyToDoItems").FindEntriesAsync(cancellationToken)).ToList();
            await BatchClient.ExecuteAsync(cancellationToken);
        }

        public virtual async Task<ToDoGroupDto> AddNewGroup(string groupName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentException(nameof(groupName));

            return await ODataClient
                .For<ToDoGroupDto>("ToDoGroups")
                .Action("CreateToDoGroup")
                .Set(new { title = groupName })
                .ExecuteAsSingleAsync(cancellationToken);
        }
    }
}
