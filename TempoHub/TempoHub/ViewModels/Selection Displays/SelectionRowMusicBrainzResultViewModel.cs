using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MetaBrainz.MusicBrainz.Interfaces.Searches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;

namespace TempoHub.ViewModels.Selection_Displays
{
    public class SelectionRowMusicBrainzResultViewModel : SelectionRowBaseViewModel
    {
        public IRelease MusicBrainzAlbumSearchResult { get; set; }
        public string MimeType { get; set; }
        public byte[] ImageData { get; set; }
        public List<ITrack> Songs { get; set; } = new List<ITrack>();
        private string albumName = "";
        public string AlbumName
        {
            get { return albumName; }
            set
            {
                if(value != albumName)
                {
                    albumName = value;
                    OnPropertyChanged(nameof(AlbumName));
                }
            }
        }
        private string artistName = "";
        public string ArtistName
        {
            get { return artistName; }
            set
            {
                if(value != artistName)
                {
                    artistName = value;
                    OnPropertyChanged(nameof(ArtistName));
                }
            }
        }
        private string songCount = "";
        public string SongCount
        {
            get { return songCount; }
            set
            {
                if(value != songCount)
                {
                    songCount = value;
                    OnPropertyChanged(nameof(SongCount));
                }
            }
        }
        private string year = "";
        public string Year
        {
            get { return year; }
            set
            {
                if(value != year)
                {
                    year = value;
                    OnPropertyChanged(nameof(Year));
                }
            }
        }
        public double RowHeight { get; set; } = 77;

        public SelectionRowMusicBrainzResultViewModel()
        {
            Type = SelectionType.MusicBrainzResult;
        }
    }
}
