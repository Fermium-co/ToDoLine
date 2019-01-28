using Bit.ViewModel;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToDoLine.Dto;
using ToDoLineApp.Contracts;
using ToDoLineApp.Resources.Strings;

namespace ToDoLineApp.ViewModels
{
    public class ToDoItemsViewModel : BitViewModelBase
    {
        public virtual IToDoService ToDoService { get; set; }
        public List<ToDoItemDto> ToDoItems { get; set; }
        public string Title { get; set; }
        public string NewItemTitle { get; set; }
        public ToDoGroupDto Group { get; set; }

        public ToDoItemsViewModel()
        {

        }


        public async override Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            Group = parameters.GetValue<ToDoGroupDto>(Strings.Group);

            string groupName = parameters.GetValue<string>(Strings.GroupName);

            if (Group != null && groupName == Strings.List)
            {
                ToDoItems = ToDoService.ToDoItems?.Where(tdi => tdi.ToDoGroupId == Group.Id).ToList();
                Title = Group.Title;
            }
            else if (groupName == Strings.Important)
            {
                ToDoItems = ToDoService.ImportantToDoItems;
                Title = Strings.Important;
            }
            else if (groupName == Strings.Planned)
            {
                ToDoItems = ToDoService.PlannedToDoItems;
                Title = Strings.Planned;
            }
            else if (groupName == Strings.ToDoItems)
            {
                ToDoItems = ToDoService.ToDoItemsWithoutToDoGroup;
                Title = Strings.ToDoItems;
            }
            else 
            {
                ToDoItems = ToDoService.PlannedToDoItems;
                Title = Strings.MyDay;
            }
        }
    }
}
