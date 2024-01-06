using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TempoHub.Models;
using TempoHub.ViewModels.Selection_Displays;

namespace TempoHub.ViewModels
{
    public class TagSearchWindowViewModel : PropertyChangedBase
    {
        public bool HitApply { get; set; } = false;
        public List<SongInfo> SongsToUpdate { get; set; } = new List<SongInfo>();

        private string pairingText = "";
        public string PairingText
        {
            get { return pairingText; }
            set
            {
                if(pairingText != value)
                {
                    pairingText = value;
                    OnPropertyChanged(nameof(PairingText));
                }
            }
        }

        private string albumNameSearch = "";
        public string AlbumNameSearch
        {
            get { return albumNameSearch; }
            set
            {
                if(albumNameSearch != value)
                {
                    albumNameSearch = value;
                    OnPropertyChanged(nameof(AlbumNameSearch));
                }
            }
        }
        private string artistNameSearch = "";
        public string ArtistNameSearch
        {
            get { return artistNameSearch; }
            set
            {
                if(artistNameSearch != value)
                {
                    artistNameSearch = value;
                    OnPropertyChanged(nameof(ArtistNameSearch));
                }
            }
        }
        private string yearSearch = "";
        public string YearSearch
        {
            get { return yearSearch; }
            set
            {
                if(yearSearch != value)
                {
                    yearSearch = value;
                    OnPropertyChanged(nameof(YearSearch));
                }
            }
        }

        private SelectionViewModel selectionVm = new SelectionViewModel();
        public SelectionViewModel SelectionVm
        {
            get { return selectionVm; }
            set
            {
                if(selectionVm != value)
                {
                    selectionVm = value;
                    OnPropertyChanged(nameof(SelectionVm));
                }
            }
        }

        #region Album Display
        private string albumName = "";
        public string AlbumName
        {
            get { return albumName; }
            set
            {
                if(albumName != value)
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
                if(artistName != value)
                {
                    artistName = value;
                    OnPropertyChanged(nameof(ArtistName));
                }
            }
        }
        private string albumArtists = "";
        public string AlbumArtists
        {
            get { return albumArtists; }
            set
            {
                if(value != albumArtists)
                {
                    albumArtists = value;
                    OnPropertyChanged(nameof(AlbumArtists));
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
        private ObservableCollection<MusicBrainzSongResultViewModel> songs = new ObservableCollection<MusicBrainzSongResultViewModel>();
        public ObservableCollection<MusicBrainzSongResultViewModel> Songs
        {
            get { return songs; }
            set
            {
                if(songs != value)
                {
                    songs = value;
                    OnPropertyChanged(nameof(Songs));
                }
            }
        }
        #endregion

        private Visibility loadingVisibility = Visibility.Collapsed;
        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set
            {
                if(loadingVisibility != value)
                {
                    loadingVisibility = value;
                    OnPropertyChanged(nameof(LoadingVisibility));
                }
            }
        }
        private LoadingDisplayViewModel loadingDisplayVm = new LoadingDisplayViewModel();
        public LoadingDisplayViewModel LoadingDisplayVm
        {
            get { return loadingDisplayVm; }
            set
            {
                if(loadingDisplayVm != value)
                {
                    loadingDisplayVm = value;
                    OnPropertyChanged(nameof(LoadingDisplayVm));
                }
            }
        }

        private bool overrideExistingData = false;
        public bool OverrideExistingData
        {
            get { return overrideExistingData; }
            set
            {
                if(overrideExistingData != value)
                {
                    overrideExistingData = value;
                    OnPropertyChanged(nameof(OverrideExistingData));
                }
            }
        }
    }
}
