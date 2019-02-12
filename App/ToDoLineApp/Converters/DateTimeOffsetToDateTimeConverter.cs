using Bit.View;
using System;
using System.Globalization;

namespace ToDoLineApp.Converters
{
    public class DateTimeOffsetToDateTimeConverter: ValueConverter<DateTimeOffset, DateTime>
    {
        protected override DateTime Convert(DateTimeOffset value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.DateTime;            
        }

        protected override DateTimeOffset ConvertBack(DateTime value, Type targetType, object parameter, CultureInfo culture)
        {
            return new DateTimeOffset(value);
        }
    }
}
