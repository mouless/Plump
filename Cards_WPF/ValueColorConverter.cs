using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Cards_WPF
{
    public class ValueColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            if (str == null) return null;

            int intValue;
            if (!int.TryParse(str, out intValue)) return null;

            if (intValue <= 1) return Brushes.Red;
            else if (intValue <= 2) return Brushes.Yellow;
            else return Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
