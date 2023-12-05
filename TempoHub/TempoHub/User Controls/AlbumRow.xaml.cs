using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using TempoHub.Models;

namespace TempoHub.User_Controls
{
    /// <summary>
    /// Interaction logic for AlbumRow.xaml
    /// </summary>
    public partial class AlbumRow : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public List<SongFile> Songs { get; set; } = new List<SongFile>();

        private string albumName = "";
        public string AlbumName
        {
            get { return albumName; }
            set
            {
                if(value != null && value != albumName)
                {
                    albumName = value;
                    OnPropertyChanged(nameof(AlbumName));
                }
            }
        }

        private string albumArtist = "";
        public string AlbumArtist
        {
            get { return albumArtist; }
            set
            {
                if(value != null && value != albumArtist)
                {
                    albumArtist = value;
                    OnPropertyChanged(nameof(AlbumArtist));
                }
            }
        }

        public Action OnClickMethod { get; set; }

        public AlbumRow()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateInfo()
        {
            var firstSong = Songs[0];

            // Turns out, the Type assigned to a Picture isn't respected by things like iTunes, Windows Media Player, and Windows Explorer.
            // They all just use the first picture
            if(firstSong.TagLibFile.Tag.Pictures.Count() > 0)
            {
                SetAlbumCover(firstSong.TagLibFile.Tag.Pictures[0].Data.Data);
            }

            else
            {
                albumCover.ClearImage();
            }

            AlbumName = firstSong.TagLibFile.Tag.Album;
            AlbumArtist = firstSong.TagLibFile.Tag.Performers.Length > 0 ? firstSong.TagLibFile.Tag.Performers[0] : "";
        }

        public void SetAlbumCover(byte[] imageData)
        {
            albumCover.SetImage(imageData);
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            detailsGrid.SetResourceReference(Control.BackgroundProperty, "MahApps.Brushes.Gray10");
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            detailsGrid.SetResourceReference(Control.BackgroundProperty, "MahApps.Brushes.ThemeBackground");
        }

        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            if(OnClickMethod != null)
            {
                OnClickMethod();
            }
        }
    }
}
