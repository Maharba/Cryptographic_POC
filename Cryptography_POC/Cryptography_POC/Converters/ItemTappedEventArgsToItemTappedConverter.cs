using System;
using System.Globalization;
using Xamarin.Forms;

namespace Cryptography_POC.Converters
{
    public class ItemTappedEventArgsToItemTappedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as ItemTappedEventArgs).Item;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}