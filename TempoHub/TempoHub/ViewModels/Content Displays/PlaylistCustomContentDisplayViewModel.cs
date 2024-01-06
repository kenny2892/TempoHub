using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;

namespace TempoHub.ViewModels.Content_Displays
{
    public class PlaylistCustomContentDisplayViewModel : PropertyChangedBase
    {
        public int PlaylistID { get; set; } = -1;
        private PlaylistType type = PlaylistType.Custom;
        public PlaylistType Type
        {
            get { return type; }
            set
            {
                if(type != value)
                {
                    type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }
        private string playlistName = "";
        public string PlaylistName
        {
            get { return playlistName; }
            set
            {
                if(playlistName != value)
                {
                    playlistName = value;
                    OnPropertyChanged(nameof(PlaylistName));
                }
            }
        }
        private PlaylistDisplayViewModel playlistDisplayVm = new PlaylistDisplayViewModel();
        public PlaylistDisplayViewModel PlaylistDisplayVm
        {
            get { return playlistDisplayVm; }
            set
            {
                if(playlistDisplayVm != value)
                {
                    playlistDisplayVm = value;
                    OnPropertyChanged(nameof(PlaylistDisplayVm));
                }
            }
        }
    }
}
