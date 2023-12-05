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
    public class EditorPictureRemoveBtnMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double heightToBaseOffOf)
            {
                return new Thickness(-1 * (heightToBaseOffOf / 2), -1 * (heightToBaseOffOf / 2), 0, 0);
            }

            return 50;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
