using Bit.View;
using Syncfusion.XForms.BadgeView;
using System;
using System.Globalization;
using System.Linq;
using ToDoLine.Dto;
using ToDoLineApp.ViewModels;

namespace ToDoLineApp.Views
{
    public partial class MenuView
    {
        public MenuView()
        {
            InitializeComponent();
        }
    }

    public class AnyOverdueTaskToBadgeTypeConverter : ValueConverter<bool, BadgeType>
    {
        protected override BadgeType Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == true ? BadgeType.Error : BadgeType.Success;
        }
    }

    public class ToDoGroupIdToToDoItemsCountConverter : ValueConverter<ToDoGroupDto, int, MenuView>
    {
        protected override int Convert(ToDoGroupDto group, Type targetType, MenuView menuView, CultureInfo culture)
        {
            if (group == null)
                return 0;

            MenuViewModel viewModel = (MenuViewModel)menuView.BindingContext;

            return viewModel.ToDoService.ToDoItems.Count(tdi => tdi.ToDoGroupId == group.Id);
        }
    }

    public class NewGroupNameToVisibilityConverter : ValueConverter<string, bool, string>
    {
        protected override bool Convert(string newGroupName, Type targetType, string viewPart, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(newGroupName))
            {
                if (viewPart == "AddButton")
                    return true;
                else
                    return false;
            }
            else
            {
                if (viewPart == "AddButton")
                    return false;
                else
                    return true;
            }
        }
    }
}
