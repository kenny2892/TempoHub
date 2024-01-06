using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;
using TempoHub.User_Controls;

namespace TempoHub.ViewModels.Content_Displays
{
    public class AlbumContentDisplayViewModel : PropertyChangedBase
    {
        public ArtistContentDisplayViewModel ParentArtistContaier { get; set; }
        private bool multiSelectEnabled = false;
        public bool MultiSelectEnabled
        {
            get { return multiSelectEnabled; }
            set
            {
                if(value != multiSelectEnabled)
                {
                    multiSelectEnabled = value;

                    // Update all song list rows
                    foreach(var songRow in songs)
                    {
                        songRow.MultiSelectEnabled = value;
                    }

                    OnPropertyChanged(nameof(MultiSelectEnabled));
                }
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
        public string Artist { get; set; } = "";

        private ObservableCollection<SongListRowViewModel> songs = new ObservableCollection<SongListRowViewModel>();
        public ObservableCollection<SongListRowViewModel> Songs
        {
            get { return songs; }
            set
            {
                songs = value;
                OnPropertyChanged(nameof(Songs));
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
        public Action<SongFile> Play { get; set; }
        public Action AddToQueue { get; set; }
        public Action<SongFile> EditSongInfo { get; set; }
        public Action EditSongsInfo { get; set; }
        public Action RemoveSongs { get; set; }
    }
}
