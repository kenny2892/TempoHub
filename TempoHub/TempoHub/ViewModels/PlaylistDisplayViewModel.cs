using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TempoHub.ViewModels
{
    public class PlaylistDisplayViewModel : PropertyChangedBase
    {
        private ObservableCollection<PlaylistSongRowViewModel> songs = new ObservableCollection<PlaylistSongRowViewModel>();
        public ObservableCollection<PlaylistSongRowViewModel> Songs
        {
            get { return songs; }
            set
            {
                songs = value;
                OnPropertyChanged(nameof(Songs));
            }
        }
        private Visibility buttonVisibility = Visibility.Visible;
        public Visibility ButtonVisibility
        {
            get { return buttonVisibility; }
            set
            {
                if(buttonVisibility != value)
                {
                    buttonVisibility = value;
                    OnPropertyChanged(nameof(ButtonVisibility));
                }
            }
        }
        private Visibility addButtonVisibility = Visibility.Visible;
        public Visibility AddButtonVisibility
        {
            get { return addButtonVisibility; }
            set
            {
                if(addButtonVisibility != value)
                {
                    addButtonVisibility = value;
                    OnPropertyChanged(nameof(AddButtonVisibility));
                }
            }
        }
    }
}
