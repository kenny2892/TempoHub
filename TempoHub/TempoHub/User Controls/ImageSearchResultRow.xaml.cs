using System;
using System.Collections.Generic;
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
    /// Interaction logic for ImageSearchResultRow.xaml
    /// </summary>
    public partial class ImageSearchResultRow : UserControl
    {
        private bool isClicked = false;
        public bool IsClicked
        {
            get { return isClicked; }
            set
            {
                isClicked = value;
                selectedCheckBox.IsChecked = isClicked;
            }
        }
        private byte[] imageData = new byte[0];
        public byte[] ImageData
        {
            get { return imageData; }
            set
            {
                imageData = value;

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

                albumImage.Source = imageToUse;
            }
        }
        public string ImageType { get; set; }
        public Action OnRowClickMethod { get; set; }

        public ImageSearchResultRow()
        {
            InitializeComponent();
            MouseUp += (sender, e) => OnMouseUp(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if(OnRowClickMethod != null && e.ChangedButton == MouseButton.Left)
            {
                OnRowClickMethod();
                e.Handled = true;
            }
        }

        private void OnSelectedCheckBoxClick(object sender, RoutedEventArgs e)
        {
            if(OnRowClickMethod != null)
            {
                OnRowClickMethod();
                e.Handled = true;
            }
        }
    }
}
