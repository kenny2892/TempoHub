using System;
using System.Collections.Generic;
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
using TempoHub.Converters;
using TempoHub.Models;
using TempoHub.User_Controls;

namespace TempoHub.Song_Editor_Tabs
{
    /// <summary>
    /// Interaction logic for PicturesTab.xaml
    /// </summary>
    public partial class PicturesTab : UserControl
    {
        private SongFile Song { get; set; }
        public Action OnAddPictureClickMethod { get; set; }

        public PicturesTab()
        {
            InitializeComponent();
        }

        public void LoadPicturesTab(SongFile song)
        {
            Song = song;
            pictureOptionsStackPanel.Children.Clear();

            for(int i = 0; i < Song.TagLibFile.Tag.Pictures.Length; i++)
            {
                var pictureToDisplay = Song.TagLibFile.Tag.Pictures[i];
                var picData = pictureToDisplay.Data.Data;
                EditorPicture editorPic = new EditorPicture();
                editorPic.SetPicture(picData);
                editorPic.IsSelected = i == 0;

                Binding heightBinding = new Binding("ActualHeight");
                heightBinding.Source = pictureScrollView;
                heightBinding.Converter = new SubtractWithMinConverter();
                heightBinding.ConverterParameter = 50;
                editorPic.SetBinding(EditorPicture.HeightProperty, heightBinding);

                Binding widthBinding = new Binding("ActualWidth");
                widthBinding.Source = pictureScrollView;
                widthBinding.Converter = new SubtractWithMinConverter();
                widthBinding.ConverterParameter = 50;
                editorPic.SetBinding(EditorPicture.WidthProperty, widthBinding);

                editorPic.picture.image.VerticalAlignment = VerticalAlignment.Top;
                editorPic.OnRemovalClickMethod = () =>
                {
                    var newList = Song.TagLibFile.Tag.Pictures.ToList();
                    newList.Remove(pictureToDisplay);
                    Song.TagLibFile.Tag.Pictures = newList.ToArray();

                    pictureOptionsStackPanel.Children.Remove(editorPic);
                };
                editorPic.picture.image.MouseUp += (sender, e) =>
                {
                    foreach(var element in pictureOptionsStackPanel.Children)
                    {
                        if(element is EditorPicture toUpdate && toUpdate.IsSelected)
                        {
                            toUpdate.IsSelected = false;
                        }
                    }

                    editorPic.IsSelected = true;
                };

                pictureOptionsStackPanel.Children.Add(editorPic);
            }
        }

        private void OnAddPictureClick(object sender, RoutedEventArgs e)
        {
            if(OnAddPictureClickMethod != null)
            {
                OnAddPictureClickMethod();
            }
        }
    }
}
