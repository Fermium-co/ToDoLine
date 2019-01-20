using Bit.ViewModel;
using Prism.Navigation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ToDoLine.Dto;
using ToDoLineApp.Contracts;
using ToDoLineApp.Resources.Strings;

namespace ToDoLineApp.ViewModels
{
    public class MenuViewModel : BitViewModelBase
    {
        public virtual IToDoService ToDoService { get; set; }
        public string NewGroupName { get; set; } = "";

        public BitDelegateCommand AddNewGroupCommand { get; set; }
        public BitDelegateCommand CancelAddNewGroupCommand { get; set; }
        public BitDelegateCommand ShowNewGroupStackCommand { get; set; }

        public MenuViewModel()
        {
            AddNewGroupCommand = new BitDelegateCommand(AddNewGroupAsync, () => !string.IsNullOrEmpty(NewGroupName));
            AddNewGroupCommand.ObservesProperty(() => NewGroupName);
            CancelAddNewGroupCommand = new BitDelegateCommand(CancelAddNewGroupAsync);
            ShowNewGroupStackCommand = new BitDelegateCommand(async () => { NewGroupName = Strings.NewGroupName; });
        }

        private async Task CancelAddNewGroupAsync()
        {
            NewGroupName = string.Empty;
        }

        private async Task AddNewGroupAsync()
        {
            ToDoGroupDto group = await ToDoService.AddNewGroup(NewGroupName, CancellationToken.None);
            ToDoService.ToDoGroups.Add(group);
            ToDoService.ToDoGroups = new List<ToDoGroupDto>(ToDoService.ToDoGroups);
            NewGroupName = string.Empty;
        }

        public async override Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);

            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                await ToDoService.LoadData(CancellationToken.None);
            }
        }
    }
}
