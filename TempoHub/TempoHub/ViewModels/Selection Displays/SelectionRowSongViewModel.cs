using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;

namespace TempoHub.ViewModels.Selection_Displays
{
    public class SelectionRowSongViewModel : SelectionRowBaseViewModel
    {
        public string FilePath { get; set; }
        private string songTitle = "";
        public string SongTitle
        {
            get { return songTitle; }
            set
            {
                if(value != null && value != songTitle)
                {
                    songTitle = value;
                    OnPropertyChanged(nameof(SongTitle));
                }
            }
        }
        public string SongTitleSort { get; set; } = "";

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
        public string AlbumNameSort { get; set; } = "";

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
        public string AlbumArtistSort { get; set; } = "";

        private AspectRatioImageViewModel imageVm;
        public AspectRatioImageViewModel ImageVm
        {
            get { return imageVm; }
            set
            {
                if(imageVm != value)
                {
                    imageVm = value;
                    OnPropertyChanged(nameof(ImageVm));
                }
            }
        }

        public double RowHeight { get; set; } = 77;

        public SelectionRowSongViewModel()
        {
            Type = SelectionType.Song;
        }
    }
}
