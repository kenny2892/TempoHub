using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using TempoHub.ViewModels;
using TempoHub.ViewModels.Song_Editor_Tabs;

namespace TempoHub.User_Controls.Song_Editor_Tabs
{
    /// <summary>
    /// Interaction logic for AddPictureByUrlTab.xaml
    /// </summary>
    public partial class AddPictureByUrlTab : UserControl
    {
        public AddPictureByUrlTab()
        {
            InitializeComponent();
            picByUrlPreviewImage.picture.image.VerticalAlignment = VerticalAlignment.Top;
        }

        private async void OnLoadClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is AddPictureByUrlTabViewModel vm)
            {
                string urlToUse = urlTextBox.Text;

                // URL source: https://stackoverflow.com/a/7581824
                if(!String.IsNullOrEmpty(urlToUse) && Uri.TryCreate(urlToUse, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                {
                    using(var client = new HttpClient())
                    {
                        var response = await client.GetAsync(urlToUse);

                        if(response != null)
                        {
                            vm.EditorPicture.ImageVm = new AspectRatioImageViewModel() { ImageData = await response.Content.ReadAsByteArrayAsync() };
                            vm.MimeType = response.Content.Headers.ContentType.ToString();

                            if(vm.SetUploadBtnVisibilityMethod != null)
                            {
                                vm.SetUploadBtnVisibilityMethod(Visibility.Visible);
                            }
                        }
                    }
                }
            }
        }
    }
}
