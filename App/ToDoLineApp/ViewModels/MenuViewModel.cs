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
            AddNewGroupCommand = new BitDelegateCommand(AddNewGroup, () => !string.IsNullOrEmpty(NewGroupTitle));
            AddNewGroupCommand.ObservesProperty(() => NewGroupTitle);
            DeleteGroupCommand = new BitDelegateCommand<ToDoGroupDto>(DeleteGroup);
            EditGroupNameCommand = new BitDelegateCommand<ToDoGroupDto>(EditGroupName);
            CancelAddNewGroupCommand = new BitDelegateCommand(CancelAddNewGroup);
            OpenToDoItemsCommand = new BitDelegateCommand<object>(OpenToDoItems);
        }

        async Task OpenToDoItems(object group)
        {
            ToDoGroupDto ToDoGroup = null;
            ItemCategory category;

            if (group is ToDoGroupDto)
            {
                ToDoGroup = (ToDoGroupDto)group;
                category = ItemCategory.UserDefinedGroup;
            }
            else if(group is ItemCategory)
            {
                category = (ItemCategory)group;
            }
            else
            {
                throw new NotSupportedException("GroupType is not supported.");
            }


            await NavigationService.NavigateAsync("Nav/ToDoItems",(ToDoItemsViewModel.GroupParameterKey, ToDoGroup),(ToDoItemsViewModel.ItemCategoryParameterKey, category) );
        }

        async Task EditGroupName(ToDoGroupDto group)
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

        async Task DeleteGroup(ToDoGroupDto group)
        {
            if (await PageDialogService.DisplayAlertAsync(Strings.DeleteGroup, string.Format(Strings.DeleteGroupForever, group.Title), Strings.Delete, Strings.Cancel))
            {
                await ToDoService.DeleteGroup(group, CancellationToken.None);
            }
        }

        async Task CancelAddNewGroup()
        {
            NewGroupTitle = string.Empty;
        }

        async Task BeginAddNewGroup()
        {
            NewGroupTitle = Strings.NewGroupTitle;
        }

        async Task AddNewGroup()
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
