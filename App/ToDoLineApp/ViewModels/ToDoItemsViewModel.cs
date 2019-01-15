using Bit.ViewModel;
using Prism.Navigation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ToDoLine.Dto;
using ToDoLineApp.Contracts;

namespace ToDoLineApp.ViewModels
{
    public class ToDoItemsViewModel : BitViewModelBase
    {
        public const string ToDoItemsKey = "TodoItems";
        public ToDoGroupDto ToDoGroup { get; set; }

        public List<ToDoItemDto> ToDoItems { get; set; }

        public override async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);

            var canGetValue = parameters.TryGetValue(ToDoItemsKey, out List<ToDoItemDto> Items);

            if(canGetValue)
            {
                ToDoItems = Items;
            }
        }
    }
}
