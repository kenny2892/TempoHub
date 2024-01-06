using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace TempoHub.Converters
{
    public class StringEqualsToBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values is string[] strValues && values.Length > 1)
            {
                return strValues.All(value => value == strValues[0]);
            }

            else if(values.All(value => value is string) && values.Length > 1)
            {
                return values.Select(value => value as string).All(value => value == values[0] as string);
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
