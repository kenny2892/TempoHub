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
    public class VisibilityBasedOnStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility hideState = Visibility.Hidden;

            if(parameter is string explicitHideState && Enum.TryParse(explicitHideState, true, out hideState))
            {

            }

            if(value is string content)
            {
                return String.IsNullOrEmpty(content) ? hideState : Visibility.Visible;
            }

            return hideState;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
