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
    public class ByteArrayToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is byte[] imageData && imageData.Length != 0)
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

        // Source: https://stackoverflow.com/a/6597746
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is BitmapImage imageSource)
            {
                byte[] data;
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imageSource));

                using MemoryStream memory = new MemoryStream();
                encoder.Save(memory);
                data = memory.ToArray();

                return data;
            }

            return null;
        }
    }
}
