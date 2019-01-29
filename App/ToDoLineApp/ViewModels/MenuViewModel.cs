using Acr.UserDialogs;
using Bit.ViewModel;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using ToDoLine.Dto;
using ToDoLineApp.Contracts;
using ToDoLineApp.Resources.Strings;

namespace ToDoLineApp.ViewModels
{
    public class MenuViewModel : BitViewModelBase
    {
        public virtual IPageDialogService PageDialogService { get; set; }
        public virtual IUserDialogs UserDialogs { get; set; }
        public virtual IToDoService ToDoService { get; set; }
        public virtual string NewGroupTitle { get; set; } = "";

        public BitDelegateCommand AddNewGroupCommand { get; set; }
        public BitDelegateCommand<ToDoGroupDto> DeleteGroupCommand { get; set; }
        public BitDelegateCommand<ToDoGroupDto> EditGroupNameCommand { get; set; }
        public BitDelegateCommand CancelAddNewGroupCommand { get; set; }
        public BitDelegateCommand BeginAddNewGroupCommand { get; set; }
        public BitDelegateCommand<object> OpenToDoItemsCommand { get; set; }

        public MenuViewModel()
        {
            BeginAddNewGroupCommand = new BitDelegateCommand(BeginAddNewGroup);
            AddNewGroupCommand = new BitDelegateCommand(AddNewGroupAsync, () => !string.IsNullOrEmpty(NewGroupTitle));
            AddNewGroupCommand.ObservesProperty(() => NewGroupTitle);
            DeleteGroupCommand = new BitDelegateCommand<ToDoGroupDto>(DeleteGroupAsync);
            EditGroupNameCommand = new BitDelegateCommand<ToDoGroupDto>(EditGroupNameAsync);
            CancelAddNewGroupCommand = new BitDelegateCommand(CancelAddNewGroupAsync);
            OpenToDoItemsCommand = new BitDelegateCommand<object>(OpenToDoItems);
        }

        async Task OpenToDoItems(object group)
        {
            ToDoGroupDto ToDoGroup = null;
            string Title = Strings.MyDay;

            if (group is ToDoGroupDto)
            {
                ToDoGroup = (ToDoGroupDto)group;
                Title = Strings.List;
            }
            else if(group is string)
            {
                Title = (string)group;
            }

            await NavigationService.NavigateAsync("Nav/ToDoItems",(Strings.Group, ToDoGroup),(Strings.GroupName, Title) );
        }

        async Task EditGroupNameAsync(ToDoGroupDto group)
        {
             var editResult = await UserDialogs.PromptAsync(new PromptConfig()
            {
                InputType = InputType.Name,
                CancelText = Strings.Cancel,
                OkText = Strings.Edit,
                Placeholder = group.Title,
            });

            if (editResult.Ok)
            {
                using (UserDialogs.Loading(Strings.Login, out CancellationToken cancellationToken))
                {
                    group.Title = editResult.Text;
                    await ToDoService.UpdateGroup(group, cancellationToken);
                }
            }
        }

        async Task DeleteGroupAsync(ToDoGroupDto group)
        {
            if (await PageDialogService.DisplayAlertAsync(Strings.DeleteGroup, string.Format(Strings.DeleteGroupForever, group.Title), Strings.Delete, Strings.Cancel))
            {
                await ToDoService.DeleteGroup(group, CancellationToken.None);
            }
        }

        async Task CancelAddNewGroupAsync()
        {
            NewGroupTitle = string.Empty;
        }

        async Task BeginAddNewGroup()
        {
            NewGroupTitle = Strings.NewGroupTitle;
        }

        async Task AddNewGroupAsync()
        {
            await ToDoService.AddNewGroup(NewGroupTitle, CancellationToken.None);
            NewGroupTitle = string.Empty;
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
