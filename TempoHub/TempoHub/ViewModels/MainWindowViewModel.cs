using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TempoHub.Settings;
using TempoHub.ViewModels.Content_Displays;
using TempoHub.ViewModels.Selection_Displays;

namespace TempoHub.ViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        private SelectionViewModel albumSelectionVm = new SelectionViewModel();
        public SelectionViewModel AlbumSelectionVm
        {
            get { return albumSelectionVm; }
            set
            {
                if(albumSelectionVm != value)
                {
                    albumSelectionVm = value;
                    OnPropertyChanged(nameof(AlbumSelectionVm));
                }
            }
        }
        private SelectionViewModel artistSelectionVm = new SelectionViewModel();
        public SelectionViewModel ArtistSelectionVm
        {
            get { return artistSelectionVm; }
            set
            {
                if(artistSelectionVm != value)
                {
                    artistSelectionVm = value;
                    OnPropertyChanged(nameof(ArtistSelectionVm));
                }
            }
        }
        private SelectionViewModel songSelectionVm = new SelectionViewModel();
        public SelectionViewModel SongSelectionVm
        {
            get { return songSelectionVm; }
            set
            {
                if(songSelectionVm != value)
                {
                    songSelectionVm = value;
                    OnPropertyChanged(nameof(SongSelectionVm));
                }
            }
        }
        private SelectionViewModel playlistSelectionVm = new SelectionViewModel();
        public SelectionViewModel PlaylistSelectionVm
        {
            get { return playlistSelectionVm; }
            set
            {
                if(playlistSelectionVm != value)
                {
                    playlistSelectionVm = value;
                    OnPropertyChanged(nameof(PlaylistSelectionVm));
                }
            }
        }
        private ObservableCollection<SongQueueRowViewModel> songQueue = new ObservableCollection<SongQueueRowViewModel>();
        public ObservableCollection<SongQueueRowViewModel> SongQueue
        {
            get { return songQueue; }
            set
            {
                songQueue = value;
                OnPropertyChanged(nameof(SongQueue));
            }
        }
        private SongDetailsColumnTogglesViewModel songDetailsToggleVm = new SongDetailsColumnTogglesViewModel();
        public SongDetailsColumnTogglesViewModel SongDetailsToggleVm
        {
            get { return songDetailsToggleVm; }
            set
            {
                if(songDetailsToggleVm != value)
                {
                    songDetailsToggleVm = value;
                    OnPropertyChanged(nameof(SongDetailsToggleVm));
                }
            }
        }
        private AlbumContentDisplayViewModel albumContentDisplayVm = new AlbumContentDisplayViewModel();
        public AlbumContentDisplayViewModel AlbumContentDisplayVm
        {
            get { return albumContentDisplayVm; }
            set
            {
                albumContentDisplayVm = value;
                OnPropertyChanged(nameof(AlbumContentDisplayVm));
            }
        }
        private ArtistContentDisplayViewModel artistContentDisplayVm = new ArtistContentDisplayViewModel();
        public ArtistContentDisplayViewModel ArtistContentDisplayVm
        {
            get { return artistContentDisplayVm; }
            set
            {
                artistContentDisplayVm = value;
                OnPropertyChanged(nameof(ArtistContentDisplayVm));
            }
        }
        private SongContentDisplayViewModel songContentDisplayVm = new SongContentDisplayViewModel();
        public SongContentDisplayViewModel SongContentDisplayVm
        {
            get { return songContentDisplayVm; }
            set
            {
                songContentDisplayVm = value;
                OnPropertyChanged(nameof(SongContentDisplayVm));
            }
        }
        private SongDetailsContentDisplayViewModel songDetailsContentDisplayVm = new SongDetailsContentDisplayViewModel();
        public SongDetailsContentDisplayViewModel SongDetailsContentDisplayVm
        {
            get { return songDetailsContentDisplayVm; }
            set
            {
                songDetailsContentDisplayVm = value;
                OnPropertyChanged(nameof(SongDetailsContentDisplayVm));
            }
        }
        private PlaylistCustomContentDisplayViewModel playlistContentDisplayVm = new PlaylistCustomContentDisplayViewModel();
        public PlaylistCustomContentDisplayViewModel PlaylistContentDisplayVm
        {
            get { return playlistContentDisplayVm; }
            set
            {
                playlistContentDisplayVm = value;
                OnPropertyChanged(nameof(PlaylistContentDisplayVm));
            }
        }
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
                loadingDisplayVm = value;
                OnPropertyChanged(nameof(LoadingDisplayVm));
            }
        }
        private NotificationDisplayViewModel notificationDisplayVm = new NotificationDisplayViewModel();
        public NotificationDisplayViewModel NotificationDisplayVm
        {
            get { return notificationDisplayVm; }
            set
            {
                if(notificationDisplayVm != value)
                {
                    notificationDisplayVm = value;
                    OnPropertyChanged(nameof(NotificationDisplayVm));
                }
            }
        }
        public AppliedSettings Settings { get; set; } = new AppliedSettings();
    }
}
