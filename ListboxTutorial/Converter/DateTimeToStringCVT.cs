using System;
using System.Windows;
using System.Windows.Data;

namespace NoteBook.Converter
{
    public class DateTimeToStringCVT : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime time;
            if (value is DateTime)
                time = (DateTime)value;
            else if (value is string)
                time = DateTime.Parse(value as string);
            else
                return DependencyProperty.UnsetValue;
            var dt = DateTime.Now - time;
            if (parameter == null)
                return time.ToShortTimeString();
            var strs = parameter.ToString().Split(new[] { ','});
            if (strs.Length < 2 || dt.Days > 7)
                return time.ToString(strs[0]);
            if (string.IsNullOrEmpty(strs[1]))
                return string.Empty;
            if (strs[1] == "d")
                return dt.Days;
            if (strs[1] == "m")
                return dt.TotalMinutes;
            if (strs[1] == "h")
                return dt.TotalHours;
            if (strs[1] == "s")
                return dt.TotalSeconds;
            if (strs[1] == "mm")
                return dt.TotalMilliseconds;
            return strs[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
