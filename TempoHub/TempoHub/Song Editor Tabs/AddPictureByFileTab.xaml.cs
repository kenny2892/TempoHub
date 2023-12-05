using Microsoft.Win32;
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
using TagLib.Id3v2;
using TempoHub.Models;

namespace TempoHub.Song_Editor_Tabs
{
    /// <summary>
    /// Interaction logic for AddPictureByFileTab.xaml
    /// </summary>
    public partial class AddPictureByFileTab : UserControl
    {
        public string ImagePathToImport { get; set; }
        public Button UploadBtn { get; set; }

        public AddPictureByFileTab()
        {
            InitializeComponent();
            picByFilePreviewImage.OnRemovalClickMethod = () => ResetSelectFile();
            picByFilePreviewImage.picture.image.VerticalAlignment = VerticalAlignment.Top;
        }

        private void OnSelectFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png|All Files|*.*"
            };

            if(openFileDialog.ShowDialog() == true)
            {
                ImagePathToImport = openFileDialog.FileName;
                picByFilePreviewImage.SetPicture(File.ReadAllBytes(ImagePathToImport));

                if(UploadBtn != null)
                {
                    UploadBtn.Visibility = Visibility.Visible;
                }
            }
        }

        private void ResetSelectFile()
        {
            ImagePathToImport = "";
            picByFilePreviewImage.ClearPicture();

            if(UploadBtn != null)
            {
                UploadBtn.Visibility = Visibility.Collapsed;
            }
        }
    }
}
