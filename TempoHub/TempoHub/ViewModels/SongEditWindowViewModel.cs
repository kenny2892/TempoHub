using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TempoHub.Models;
using TempoHub.User_Controls;
using TempoHub.ViewModels.Song_Editor_Tabs;

namespace TempoHub.ViewModels
{
    public class SongEditWindowViewModel : PropertyChangedBase
    {
        public List<SongFile> SongsToEdit { get; set; } = new List<SongFile>();
        private AspectRatioImageViewModel imageVm = null;
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
        private SongEditorDetailsTabViewModel detailsVm = null;
        public SongEditorDetailsTabViewModel DetailsVm
        {
            get { return detailsVm; }
            set
            {
                if(detailsVm != value)
                {
                    detailsVm = value;
                    OnPropertyChanged(nameof(DetailsVm));
                }
            }
        }
        private SongEditorPicturesTabViewModel picturesVm = null;
        public SongEditorPicturesTabViewModel PicturesVm
        {
            get { return picturesVm; }
            set
            {
                if(picturesVm != value)
                {
                    picturesVm = value;
                    OnPropertyChanged(nameof(PicturesVm));
                }
            }
        }
        private SongEditorLyricsTabViewModel lyricsVm = null;
        public SongEditorLyricsTabViewModel LyricsVm
        {
            get { return lyricsVm; }
            set
            {
                if(lyricsVm != value)
                {
                    lyricsVm = value;
                    OnPropertyChanged(nameof(LyricsVm));
                }
            }
        }
        private SongEditorSortingTabViewModel sortingVm = null;
        public SongEditorSortingTabViewModel SortingVm
        {
            get { return sortingVm; }
            set
            {
                if(sortingVm != value)
                {
                    sortingVm = value;
                    OnPropertyChanged(nameof(SortingVm));
                }
            }
        }
        private SongEditorInfoTabViewModel infoVm = null;
        public SongEditorInfoTabViewModel InfoVm
        {
            get { return infoVm; }
            set
            {
                if(infoVm != value)
                {
                    infoVm = value;
                    OnPropertyChanged(nameof(InfoVm));
                }
            }
        }
        private Visibility infoBtnVisibility = Visibility.Visible;
        public Visibility InfoBtnVisibility
        {
            get { return infoBtnVisibility; }
            set
            {
                if(infoBtnVisibility != value)
                {
                    infoBtnVisibility = value;
                    OnPropertyChanged(nameof(InfoBtnVisibility));
                }
            }
        }
        private AddPictureByFileTabViewModel addPictureByFileVm = null;
        public AddPictureByFileTabViewModel AddPictureByFileVm
        {
            get { return addPictureByFileVm; }
            set
            {
                if(addPictureByFileVm != value)
                {
                    addPictureByFileVm = value;
                    OnPropertyChanged(nameof(AddPictureByFileVm));
                }
            }
        }
        private AddPictureByMusicBrainzTabViewModel addPictureByMusicBrainzVm = null;
        public AddPictureByMusicBrainzTabViewModel AddPictureByMusicBrainzVm
        {
            get { return addPictureByMusicBrainzVm; }
            set
            {
                if(addPictureByMusicBrainzVm != value)
                {
                    addPictureByMusicBrainzVm = value;
                    OnPropertyChanged(nameof(AddPictureByMusicBrainzVm));
                }
            }
        }
        private AddPictureByUrlTabViewModel addPictureByUrlVm = null;
        public AddPictureByUrlTabViewModel AddPictureByUrlVm
        {
            get { return addPictureByUrlVm; }
            set
            {
                if(addPictureByUrlVm != value)
                {
                    addPictureByUrlVm = value;
                    OnPropertyChanged(nameof(AddPictureByUrlVm));
                }
            }
        }
        private Visibility pictureUploadBtnVisibility = Visibility.Collapsed;
        public Visibility PictureUploadBtnVisibility
        {
            get { return pictureUploadBtnVisibility; }
            set
            {
                if(pictureUploadBtnVisibility != value)
                {
                    pictureUploadBtnVisibility = value;
                    OnPropertyChanged(nameof(PictureUploadBtnVisibility));
                }
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

        public SongEditWindowViewModel(SongInfo songInfo, SongFile[] songsToEdit, Action onAddPictureClickMethod)
        {
            SongsToEdit.AddRange(songsToEdit);

            DetailsVm = new SongEditorDetailsTabViewModel(songInfo) { AllowSingleSongEdits = SongsToEdit.Count == 1 ? Visibility.Visible : Visibility.Collapsed };
            LyricsVm = new SongEditorLyricsTabViewModel(songInfo);
            PicturesVm = new SongEditorPicturesTabViewModel(songInfo) { OnAddPictureClickMethod = onAddPictureClickMethod };
            SortingVm = new SongEditorSortingTabViewModel(songInfo);
            InfoVm = new SongEditorInfoTabViewModel(songInfo);
            InfoBtnVisibility = SongsToEdit.Count == 1 ? Visibility.Visible : Visibility.Collapsed;

            AddPictureByFileVm = new AddPictureByFileTabViewModel() { SetUploadBtnVisibilityMethod = SetPictureUploadBtnVisibility };
            var musicBrainzVm = new AddPictureByMusicBrainzTabViewModel();
            musicBrainzVm.SetUploadBtnVisibilityMethod = SetPictureUploadBtnVisibility;
            musicBrainzVm.StartLoadingDisplay = (min, max, curr) => { LoadingVisibility = Visibility.Visible; LoadingDisplayVm.Start(min, max, curr); return true; };
            musicBrainzVm.AddValueLoadingDisplay = (toAdd) => { LoadingDisplayVm.CurrentValue += toAdd; };
            musicBrainzVm.StopLoadingDisplay = () => { LoadingDisplayVm.Stop(); LoadingVisibility = Visibility.Collapsed; };
            AddPictureByMusicBrainzVm = musicBrainzVm;
            AddPictureByUrlVm = new AddPictureByUrlTabViewModel() { SetUploadBtnVisibilityMethod = SetPictureUploadBtnVisibility };

            ImageVm = new AspectRatioImageViewModel();
            if(songInfo.Pictures.Length > 0)
            {
                ImageVm.ImageData = songInfo.Pictures[0].Data.Data;
            }
        }

        private void SetPictureUploadBtnVisibility(Visibility value)
        {
            PictureUploadBtnVisibility = value;
        }

        public void Clear()
        {
            DetailsVm.Clear();
            LyricsVm.Clear();
            PicturesVm.Clear();
            SortingVm.Clear();
        }
    }
}
