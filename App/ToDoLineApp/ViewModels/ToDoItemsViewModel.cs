using Acr.UserDialogs;
using Bit.ViewModel;
using Prism.Navigation;
using Prism.Services;
using Syncfusion.ListView.XForms;
using System;
using System.Threading;
using System.Threading.Tasks;
using ToDoLine.Dto;
using ToDoLineApp.Contracts;
using ToDoLineApp.Resources.Strings;

namespace ToDoLineApp.ViewModels
{
    public class ToDoItemsViewModel : BitViewModelBase
    {
        public const string ItemCategoryParameterKey = "ItemCategory";
        public const string GroupParameterKey = "Group";

        private ItemCategory _itemCategory;

        public virtual IToDoService ToDoService { get; set; }
        public virtual IPageDialogService PageDialogService { get; set; }
        public virtual IUserDialogs UserDialogs { get; set; }
        public BitDelegateCommand AddNewItemCommand { get; set; }
        public BitDelegateCommand<ToDoItemDto> DeleteItemCommand { get; set; }
        public BitDelegateCommand<ToDoItemDto> EditItemCommand { get; set; }
        public BitDelegateCommand<ToDoItemDto> ReverseIsImportantCommand { get; set; }
        public BitDelegateCommand<ItemTappedEventArgs> ShowTodoItemStepsCommand { get; set; }

        public string Title { get; set; }
        public string NewItemTitle { get; set; }
        public ToDoGroupDto Group { get; set; }

        public ToDoItemsViewModel()
        {
            AddNewItemCommand = new BitDelegateCommand(AddNewItem , () => !string.IsNullOrEmpty(NewItemTitle));
            AddNewItemCommand.ObservesProperty(() => NewItemTitle);

            EditItemCommand = new BitDelegateCommand<ToDoItemDto>(EditItem);
            DeleteItemCommand = new BitDelegateCommand<ToDoItemDto>(DeleteItem);
            ReverseIsImportantCommand = new BitDelegateCommand<ToDoItemDto>(ReverseIsImportant);
            ShowTodoItemStepsCommand = new BitDelegateCommand<ItemTappedEventArgs>(ShowTodoItemSteps);
        }

        private async Task ShowTodoItemSteps(ItemTappedEventArgs eventArgs)
        {
            var todoItem = (ToDoItemDto)eventArgs.ItemData;
            await NavigationService.NavigateAsync("ToDoItemSteps", (TodoItemStepsViewModel.TodoItemParameterKey, todoItem));
        }

        private async Task ReverseIsImportant(ToDoItemDto todoItem)
        {
            todoItem.IsImportant = !todoItem.IsImportant;
            await ToDoService.UpdateItem(todoItem, CancellationToken.None);
        }

        private async Task DeleteItem(ToDoItemDto todoItem)
        {
            if (await PageDialogService.DisplayAlertAsync(Strings.DeleteItem, string.Format(Strings.DeleteItemForever, todoItem.Title), Strings.Delete, Strings.Cancel))
            {
                await ToDoService.DeleteItem(todoItem, CancellationToken.None);
            }
        }

        private async Task EditItem(ToDoItemDto toDoItem)
        {
            var editResult = await UserDialogs.PromptAsync(new PromptConfig()
            {
                InputType = InputType.Name,
                CancelText = Strings.Cancel,
                OkText = Strings.Edit,
                Placeholder = toDoItem.Title,
            });

            if (editResult.Ok)
            {
                using (UserDialogs.Loading(Strings.Edit, out CancellationToken cancellationToken))
                {
                    toDoItem.Title = editResult.Text;
                    await ToDoService.UpdateItem(toDoItem, cancellationToken);
                }
            }
        }

        async Task AddNewItem()
        {
            await ToDoService.AddNewItem(NewItemTitle, _itemCategory, Group?.Id, CancellationToken.None);

            NewItemTitle = string.Empty;
        }

        public async override Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            Group = parameters.GetValue<ToDoGroupDto>(GroupParameterKey);

            _itemCategory = parameters.GetValue<ItemCategory>(ItemCategoryParameterKey);

            switch (_itemCategory)
            {
                case ItemCategory.MyDay:
                    Title = Strings.MyDay;
                    break;
                case ItemCategory.Important:
                    Title = Strings.Important;
                    break;
                case ItemCategory.Planned:
                    Title = Strings.Planned;
                    break;
                case ItemCategory.WithoutGroup:
                    Title = Strings.ToDoItems;
                    break;
                case ItemCategory.UserDefinedGroup:
                    if (Group != null)                       
                        Title = Group.Title;                    
                    break;
            }
        }
    }
}
