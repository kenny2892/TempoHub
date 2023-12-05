using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TempoHub.Converters
{
    public class GridHeightToFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double gridHeight)
            {
                double fontSize = gridHeight / 5;

                if(parameter is double multiplier)
                {
                    fontSize *= multiplier;
                }

                else if(parameter is string && double.TryParse((string) parameter, out double multiplier2))
                {
                    fontSize *= multiplier2;
                }

                if(fontSize > 35790)
                {
                    fontSize = 35790;
                }

                else if(fontSize == 0)
                {
                    return 1;
                }

                return (int) fontSize;
            }

            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
