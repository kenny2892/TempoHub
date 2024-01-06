using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;

namespace TempoHub.ViewModels
{
    public class PlaylistSongRowViewModel : PropertyChangedBase
    {
        public PlaylistSongEntry SongEntry { get; set; }
        public int Index { get; set; } = 0;
        private AspectRatioImageViewModel imageVm = new AspectRatioImageViewModel();
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
        private string songName = "";
        public string SongName
        {
            get { return songName; }
            set
            {
                if(songName != value)
                {
                    songName = value;
                    OnPropertyChanged(nameof(SongName));
                }
            }
        }
        private string artist = "";
        public string Artist
        {
            get { return artist; }
            set
            {
                if(artist != value)
                {
                    artist = value;
                    OnPropertyChanged(nameof(Artist));
                }
            }
        }
        private string length = "";
        public string Length
        {
            get { return length; }
            set
            {
                if(length != value)
                {
                    length = value;
                    OnPropertyChanged(nameof(Length));
                }
            }
        }
    }
}
