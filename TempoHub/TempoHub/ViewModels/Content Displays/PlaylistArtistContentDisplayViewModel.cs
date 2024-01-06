using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;

namespace TempoHub.ViewModels.Content_Displays
{
    public class PlaylistArtistContentDisplayViewModel : PlaylistCustomContentDisplayViewModel
    {
        private List<string> artists = new List<string>();
        public List<string> Artists
        {
            get { return artists; }
            set
            {
                if(artists != value)
                {
                    artists = value;
                    OnPropertyChanged(nameof(Artists));
                }
            }
        }

        public PlaylistArtistContentDisplayViewModel()
        {
            Type = PlaylistType.Artists;
        }
    }
}
