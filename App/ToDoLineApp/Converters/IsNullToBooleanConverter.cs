using Bit.View;
using System;
using System.Globalization;

namespace ToDoLineApp.Converters
{
    public class IsNullToBooleanConverter: ValueConverter<object, bool>
    {
        protected override bool Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }
    }
}
