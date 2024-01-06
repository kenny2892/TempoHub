using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoHub.ViewModels.Content_Displays
{
    public class SongContentDisplayViewModel : PropertyChangedBase
    {
        public string FilePath { get; set; }
        public Action OnPlayMethod { get; set; }
        public Action OnQueueMethod { get; set; }
        public Action OnEditSongMethod { get; set; }

        private string songTitle = "";
        public string SongTitle
        {
            get { return songTitle; }
            set
            {
                songTitle = value;
                OnPropertyChanged(nameof(SongTitle));
            }
        }
        private string albumName = "";
        public string AlbumName
        {
            get { return albumName; }
            set
            {
                albumName = value;
                OnPropertyChanged(nameof(AlbumName));
            }
        }
        private string artistName = "";
        public string ArtistName
        {
            get { return artistName; }
            set
            {
                artistName = value;
                OnPropertyChanged(nameof(ArtistName));
            }
        }

        private string songLength = "";
        public string SongLength
        {
            get { return songLength; }
            set
            {
                if(value != null && value != songLength)
                {
                    songLength = value;
                    OnPropertyChanged(nameof(SongLength));
                }
            }
        }
        private AspectRatioImageViewModel imageVm = new AspectRatioImageViewModel();
        public AspectRatioImageViewModel ImageVm
        {
            get { return imageVm; }
            set
            {
                imageVm = value;
                OnPropertyChanged(nameof(ImageVm));
            }
        }
    }
}
