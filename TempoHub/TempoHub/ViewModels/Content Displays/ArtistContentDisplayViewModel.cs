using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoHub.ViewModels.Content_Displays
{
    public class ArtistContentDisplayViewModel : PropertyChangedBase
    {
        private ObservableCollection<AlbumContentDisplayViewModel> albumDisplays = new ObservableCollection<AlbumContentDisplayViewModel>();
        public ObservableCollection<AlbumContentDisplayViewModel> AlbumDisplays
        {
            get { return albumDisplays; }
            set
            {
                albumDisplays = value;
                OnPropertyChanged(nameof(AlbumDisplays));
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
    }
}
