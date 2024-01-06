using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using TempoHub.Models;

namespace TempoHub.ViewModels
{
    public class ImageSearchResultRowViewModel : PropertyChangedBase
    {
        public double Height { get; set; }
        public string ImageType { get; set; }
        private byte[] imageData = new byte[0];
        public byte[] ImageData
        {
            get { return imageData; }
            set
            {
                if(imageData != value)
                {
                    imageData = value;
                    OnPropertyChanged(nameof(ImageData));
                }
            }
        }
        private string album = "";
        public string Album
        {
            get { return album; }
            set
            {
                if(album != value)
                {
                    album = value;
                    OnPropertyChanged(nameof(Album));
                }
            }
        }
        private string songTitle = "";
        public string SongTitle
        {
            get { return songTitle; }
            set
            {
                if(songTitle != value)
                {
                    songTitle = value;
                    OnPropertyChanged(nameof(SongTitle));
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
    }
}
