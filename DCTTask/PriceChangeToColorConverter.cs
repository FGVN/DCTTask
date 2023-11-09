using System;
using System.Globalization;
using System.Windows.Data;

namespace DCTTask
{
    public class PriceChangeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal priceChange)
            {
                return priceChange >= 0 ? "Up" : "Down";
            }

            return "NoChange";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
