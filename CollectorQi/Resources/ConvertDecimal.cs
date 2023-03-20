using System;
using System.Globalization;
using Xamarin.Forms;

namespace CollectorQi
{
    public class ConvertDecimal : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (value is Decimal)
            {
                return ((decimal)value).ToString().Replace(".", ",");
            }
            
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}