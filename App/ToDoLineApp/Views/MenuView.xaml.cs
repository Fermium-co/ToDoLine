using Bit.View;
using Syncfusion.XForms.BadgeView;
using System;
using System.Globalization;

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
}
