using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;

namespace TempoHub.ViewModels
{
    public class PlaylistSongAddRowViewModel : PropertyChangedBase
    {
        public SongFile Song { get; set; } = null;
        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if(isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
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

        public PlaylistSongAddRowViewModel(SongFile song)
        {
            Song = song;
            var tagFile = Song.TagLibFile;
            SongName = tagFile.Tag.Title;
            Artist = tagFile.Tag.FirstPerformer;
            Length = song.Duration.ToString(@"hh\:mm\:ss");

            var imageVm = new AspectRatioImageViewModel();
            if(tagFile.Tag.Pictures.Count() > 0)
            {
                imageVm.ImageData = tagFile.Tag.Pictures[0].Data.Data;
            }
            ImageVm = imageVm;
        }
    }
}
