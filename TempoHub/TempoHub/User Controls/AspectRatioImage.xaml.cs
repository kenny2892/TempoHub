using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TempoHub.User_Controls
{
    /// <summary>
    /// Interaction logic for AspectRatioImage.xaml
    /// </summary>
    public partial class AspectRatioImage : UserControl
    {
        public AspectRatioImage()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
        }

        public void ClearImage()
        {
            image.Source = null;
        }

        public void SetImage(byte[] imageData)
        {
            if(imageData is null || imageData.Length == 0)
            {
                return;
            }

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

            image.Source = imageToUse;
            UpdateImageSizeAndPosition();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateImageSizeAndPosition();
        }

        private void UpdateImageSizeAndPosition()
        {
            if(image.Source is null)
            {
                return;
            }

            double canvasWidth = imageWrapperGrid.ActualWidth;
            double canvasHeight = imageWrapperGrid.ActualHeight;

            double aspectRatio = image.Source.Width / image.Source.Height;

            double scaledWidth = canvasWidth;
            double scaledHeight = canvasWidth / aspectRatio;

            if(scaledHeight > canvasHeight)
            {
                scaledHeight = canvasHeight;
                scaledWidth = canvasHeight * aspectRatio;
            }

            double left = (canvasWidth - scaledWidth) / 2;
            double top = (canvasHeight - scaledHeight) / 2;

            image.Width = scaledWidth;
            image.Height = scaledHeight;

            Canvas.SetLeft(image, left);
            Canvas.SetTop(image, top);
        }
    }
}
