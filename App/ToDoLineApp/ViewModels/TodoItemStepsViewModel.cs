using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Bit.ViewModel;
using Prism.Navigation;
using Prism.Services;
using ToDoLine.Dto;
using ToDoLineApp.Contracts;
using ToDoLineApp.Resources.Strings;

namespace ToDoLineApp.ViewModels
{
    public class TodoItemStepsViewModel : BitViewModelBase
    {
        public const string TodoItemParameterKey = "TodoItem";

        public TodoItemStepsViewModel()
        {
            UpdateTodoItemCommand = new BitDelegateCommand(UpdateTodoItem);
            UpdateTodoItemStepCommand = new BitDelegateCommand<ToDoItemStepDto>(UpdateTodoItemStep);
            ReverseIsImportantCommand = new BitDelegateCommand(ReverseIsImportant);
            DeleteTodoItemStepCommand = new BitDelegateCommand<ToDoItemStepDto>(DeleteTodoItemStep);
            AddNewItemStepCommand = new BitDelegateCommand(AddNewItemStep);
        }

        public virtual IToDoService ToDoService { get; set; }

        public string NewItemStepTitle { get; set; }
        public ToDoItemDto TodoItem { get; set; }
        public ObservableCollection<ToDoItemStepDto> Steps { get; set; }
        public BitDelegateCommand UpdateTodoItemCommand { get; set; }
        public BitDelegateCommand<ToDoItemStepDto> UpdateTodoItemStepCommand { get; set; }
        public BitDelegateCommand ReverseIsImportantCommand { get; set; }
        public BitDelegateCommand<ToDoItemStepDto> DeleteTodoItemStepCommand { get; set; }
        public BitDelegateCommand AddNewItemStepCommand { get; set; }

        public override async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            TodoItem = parameters.GetValue<ToDoItemDto>(TodoItemParameterKey);

            Steps = new ObservableCollection<ToDoItemStepDto>(await ToDoService.GetToDoItemSteps(TodoItem, CancellationToken.None));
        }

        private async Task UpdateTodoItem()
        {
            await ToDoService.UpdateItem(TodoItem, CancellationToken.None);
        }

        private async Task UpdateTodoItemStep(ToDoItemStepDto todoItemStep)
        {
            await ToDoService.UpdateTodoItemStep(todoItemStep, CancellationToken.None);
        }

        private async Task ReverseIsImportant()
        {
            TodoItem.IsImportant = !TodoItem.IsImportant;
            await ToDoService.UpdateItem(TodoItem, CancellationToken.None);
        }

        private async Task DeleteTodoItemStep(ToDoItemStepDto toDoItemStep)
        {            
            await ToDoService.DeleteItemStep(toDoItemStep, CancellationToken.None);
            Steps.Remove(toDoItemStep);
        }

        private async Task AddNewItemStep()
        {
            var todoItemStep = await ToDoService.AddNewItemStep(NewItemStepTitle, TodoItem,  CancellationToken.None);
            Steps.Add(todoItemStep);
            NewItemStepTitle = string.Empty;
        }
    }
}
