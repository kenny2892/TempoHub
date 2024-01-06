using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            image.SourceUpdated += (sender, e) => UpdateImageSizeAndPosition();
        }

        public void ClearImage()
        {
            image.Source = null;
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
