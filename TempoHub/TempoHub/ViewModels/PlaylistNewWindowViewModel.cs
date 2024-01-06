using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoHub.ViewModels
{
    public class PlaylistNewWindowViewModel : PropertyChangedBase
    {
        public bool Canceled { get; set; } = false;
        private string playlsitName = "";
        public string PlaylistName
        {
            get { return playlsitName; }
            set
            {
                if(playlsitName != value)
                {
                    playlsitName = value;
                    OnPropertyChanged(nameof(PlaylistName));
                }
            }
        }
    }
}
