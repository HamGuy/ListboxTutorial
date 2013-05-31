using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;

namespace NoteBook.Converter
{
    public class DateTimeToBrushCVT : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan time;
            if (value is DateTime)
                time = DateTime.Now - (DateTime) value;
            else if (value is string)
                time = DateTime.Now - DateTime.Parse(value as string);
            else
                return DependencyProperty.UnsetValue;
            var color = (Color)Application.Current.Resources["PhoneAccentColor"];
            var icolor = new Color
                         {
                             R = GetColor(color.R),
                             G = GetColor(color.G),
                             B = GetColor(color.B),
                             A = color.A
                         };
            return new SolidColorBrush(time.Days <= 7 ? icolor:color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private byte GetColor(byte color)
        {
            return (byte) (255 - color);
        }
    }
}
