using System;
using Prism.Navigation;
using ToDoLine.Dto;
using ToDoLineApp.Contracts;

namespace ToDoLineApp.Views
{
    public partial class ToDoItemsView : INavigatedAware
    {
        private ToDoGroupDto _group;
        private ItemCategory _itemCategory;

        public ToDoItemsView()
        {
            InitializeComponent();
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            _group = parameters.GetValue<ToDoGroupDto>("Group");
            _itemCategory = parameters.GetValue<ItemCategory>("ItemCategory");

            itemsListView.DataSource.Filter = FilterItems;
            itemsListView.DataSource.RefreshFilter();
        }

        private bool FilterItems(object todoItem)
        {            
            switch (_itemCategory)
            {
                case ItemCategory.MyDay:
                    return ((ToDoItemDto)todoItem).ShowInMyDay;

                case ItemCategory.Important:
                    return ((ToDoItemDto)todoItem).IsImportant;

                case ItemCategory.Planned:
                    return ((ToDoItemDto)todoItem).RemindOn != null;
                    
                case ItemCategory.WithoutGroup:
                    return ((ToDoItemDto)todoItem).ToDoGroupId == null;
                    
                case ItemCategory.UserDefinedGroup:
                    if (_group != null)
                        return ((ToDoItemDto)todoItem).ToDoGroupId == _group.Id;
                    else
                        return false;
                    
                default:
                    return false;
            }
        }
    }
}
