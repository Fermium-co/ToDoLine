using Bit.View;
using Syncfusion.XForms.BadgeView;
using System;
using System.Globalization;
using System.Linq;
using ToDoLine.Dto;
using ToDoLineApp.ViewModels;
using Xamarin.Forms;

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

    public class ToDoGroupIdToToDoItemsCountConverter : ValueConverter<Guid, int, MenuView>
    {
        protected override int Convert(Guid toDoGroupId, Type targetType, MenuView menuView, CultureInfo culture)
        {
            MenuViewModel viewModel = (MenuViewModel)menuView.BindingContext;

            return viewModel.ToDoService.ToDoItems.Count(tdi => tdi.ToDoGroupId == toDoGroupId);
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
