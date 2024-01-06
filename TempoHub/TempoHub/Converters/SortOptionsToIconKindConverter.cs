using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TempoHub.Settings;

namespace TempoHub.Converters
{
    public class SortOptionsToIconKindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is SortOptions sort)
            {
                switch(sort)
                {
                    case SortOptions.None:
                        return "RegularSortAZ";

                    case SortOptions.Ascending:
                        return "RegularSortAZ";

                    case SortOptions.Descending:
                        return "RegularSortZA";
                }
            }

            return "RegularSortAZ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
