using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TempoHub.Converters
{
    public class EnumSpacingConverter : IValueConverter
    {
        private Regex WordPattern { get; set; } = new Regex(@"([A-Z][a-z]*)");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Enum toConvert && WordPattern.IsMatch(toConvert.ToString()))
            {
                var words = WordPattern.Matches(toConvert.ToString()).Select(pair => pair.Value);
                return String.Join(" ", words);
            }

            else if(value is Enum text)
            {
                return text.ToString();
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
