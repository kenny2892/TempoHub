using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TempoHub.Converters
{
    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is byte[] imageData)
            {
                var imageToUse = new BitmapImage();
                using var memory = new MemoryStream(imageData);
                memory.Position = 0;
                imageToUse.BeginInit();
                imageToUse.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                imageToUse.CacheOption = BitmapCacheOption.OnLoad;
                imageToUse.UriSource = null;
                imageToUse.StreamSource = memory;
                imageToUse.EndInit();
                imageToUse.Freeze();

                return imageToUse;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
