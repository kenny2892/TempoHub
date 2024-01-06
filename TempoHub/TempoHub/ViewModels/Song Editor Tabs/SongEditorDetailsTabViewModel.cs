using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TagLib.Id3v2;
using TempoHub.Models;
using TempoHub.User_Controls;

namespace TempoHub.ViewModels.Song_Editor_Tabs
{
    public class SongEditorDetailsTabViewModel : PropertyChangedBase
    {
        private string title = "";
        public string Title
        {
            get { return title; }
            set
            {
                if(value != title)
                {
                    title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }
        private string album = "";
        public string Album
        {
            get { return album; }
            set
            {
                if(value != album)
                {
                    album = value;
                    OnPropertyChanged(nameof(Album));
                }
            }
        }
        private string artist = "";
        public string Artist
        {
            get { return artist; }
            set
            {
                if(value != artist)
                {
                    artist = value;
                    OnPropertyChanged(nameof(Artist));
                }
            }
        }
        private string albumArtist = "";
        public string AlbumArtist
        {
            get { return albumArtist; }
            set
            {
                if(value != albumArtist)
                {
                    albumArtist = value;
                    OnPropertyChanged(nameof(AlbumArtist));
                }
            }
        }
        private string genres = "";
        public string Genres
        {
            get { return genres; }
            set
            {
                if(value != genres)
                {
                    genres = value;
                    OnPropertyChanged(nameof(Genres));
                }
            }
        }
        private string composer = "";
        public string Composer
        {
            get { return composer; }
            set
            {
                if(value != composer)
                {
                    composer = value;
                    OnPropertyChanged(nameof(Composer));
                }
            }
        }
        private string publisher = "";
        public string Publisher
        {
            get { return publisher; }
            set
            {
                if(value != publisher)
                {
                    publisher = value;
                    OnPropertyChanged(nameof(Publisher));
                }
            }
        }
        private string conductor = "";
        public string Conductor
        {
            get { return conductor; }
            set
            {
                if(value != conductor)
                {
                    conductor = value;
                    OnPropertyChanged(nameof(Conductor));
                }
            }
        }
        private string grouping = "";
        public string Grouping
        {
            get { return grouping; }
            set
            {
                if(value != grouping)
                {
                    grouping = value;
                    OnPropertyChanged(nameof(Grouping));
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
        private int trackCurr = 0;
        public int TrackCurr
        {
            get { return trackCurr; }
            set
            {
                if(value != trackCurr)
                {
                    trackCurr = value;
                    OnPropertyChanged(nameof(TrackCurr));
                }
            }
        }
        private int trackTotal = 0;
        public int TrackTotal
        {
            get { return trackTotal; }
            set
            {
                if(value != trackTotal)
                {
                    trackTotal = value;
                    OnPropertyChanged(nameof(TrackTotal));
                }
            }
        }
        private int discCurr = 0;
        public int DiscCurr
        {
            get { return discCurr; }
            set
            {
                if(value != discCurr)
                {
                    discCurr = value;
                    OnPropertyChanged(nameof(DiscCurr));
                }
            }
        }
        private int discTotal = 0;
        public int DiscTotal
        {
            get { return discTotal; }
            set
            {
                if(value != discTotal)
                {
                    discTotal = value;
                    OnPropertyChanged(nameof(DiscTotal));
                }
            }
        }
        private StarRatingViewModel starRatingVm = new StarRatingViewModel();
        public StarRatingViewModel StarRatingVm
        {
            get { return starRatingVm; }
            set
            {
                if(value != starRatingVm)
                {
                    starRatingVm = value;
                    OnPropertyChanged(nameof(StarRatingVm));
                }
            }
        }
        private string bpm = "";
        public string Bpm
        {
            get { return bpm; }
            set
            {
                if(value != bpm)
                {
                    bpm = value;
                    OnPropertyChanged(nameof(Bpm));
                }
            }
        }
        private string comment = "";
        public string Comment
        {
            get { return comment; }
            set
            {
                if(value != comment)
                {
                    comment = value;
                    OnPropertyChanged(nameof(Comment));
                }
            }
        }
        private Visibility allowSingleSongEdits = Visibility.Visible;
        public Visibility AllowSingleSongEdits
        {
            get { return allowSingleSongEdits; }
            set
            {
                if(value != allowSingleSongEdits)
                {
                    allowSingleSongEdits = value;
                    OnPropertyChanged(nameof(AllowSingleSongEdits));
                }
            }
        }

        public SongEditorDetailsTabViewModel(SongInfo songInfo)
        {
            Title = songInfo.Title;
            Album = songInfo.Album;
            Artist = songInfo.Artist;
            AlbumArtist = songInfo.AlbumArtist;
            Genres = songInfo.Genres;
            Composer = songInfo.Composer;
            Publisher = songInfo.Publisher;
            Conductor = songInfo.Conductor;
            Grouping = songInfo.Grouping;
            Year = songInfo.Year.ToString();

            if(int.TryParse(songInfo.TrackCurr, out int trackCurr))
            {
                TrackCurr = trackCurr;
            }

            if(int.TryParse(songInfo.TrackTotal, out int trackTotal))
            {
                TrackTotal = trackTotal;
            }

            if(int.TryParse(songInfo.DiscCurr, out int discCurr))
            {
                DiscCurr = discCurr;
            }

            if(int.TryParse(songInfo.DiscTotal, out int discTotal))
            {
                DiscTotal = discTotal;
            }

            StarRatingVm = new StarRatingViewModel() { Rating = songInfo.StarRating };
            Bpm = songInfo.Bpm;
            Comment = songInfo.Comment;
        }

        public void Clear()
        {
            Title = "";
            Album = "";
            Artist = "";
            AlbumArtist = "";
            Genres = "";
            Composer = "";
            Publisher = "";
            Conductor = "";
            Grouping = "";
            Year = "";
            TrackCurr = 0;
            TrackTotal = 0;
            DiscCurr = 0;
            DiscTotal = 0;
            Bpm = "";
            Comment = "";

            if(StarRatingVm.Rating >= 0)
            {
                StarRatingVm = new StarRatingViewModel() { Rating = 0 };
            }
        }
    }
}
