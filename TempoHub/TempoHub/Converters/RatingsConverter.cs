using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TempoHub.Converters
{
    public class RatingsConverter : IValueConverter
    {
        // Value = 0 - 255
        // Return = -1 (if invalid) - 5
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // These follow the standard followed by MusicBee and Windows
            if(value is double rating)
            {
                switch(rating)
                {
                    case 0:
                        return 0.0;

                    case 13:
                        return 0.5;

                    case 1:
                        return 1.0;

                    case 54:
                        return 1.5;

                    case 64:
                        return 2.0;

                    case 118:
                        return 2.5;

                    case 128:
                        return 3.0;

                    case 186:
                        return 3.5;

                    case 196:
                        return 4.0;

                    case 242:
                        return 4.5;

                    case 255:
                        return 5.0;
                }
            }

            return -1.0;
        }

        // Value = 0 - 5
        // Return = 0 - 255
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double rating)
            {
                switch(rating)
                {
                    case 0:
                        return 0.0;

                    case 0.5:
                        return 13.0;

                    case 1:
                        return 1.0;

                    case 1.5:
                        return 54.0;

                    case 2:
                        return 64.0;

                    case 2.5:
                        return 118.0;

                    case 3:
                        return 128.0;

                    case 3.5:
                        return 186.0;

                    case 4:
                        return 196.0;

                    case 4.5:
                        return 242.0;

                    case 5:
                        return 255.0;
                }
            }

            return 0.0;
        }
    }
}
