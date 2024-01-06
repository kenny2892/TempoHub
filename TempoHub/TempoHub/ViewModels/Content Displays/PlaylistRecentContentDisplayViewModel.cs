using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;

namespace TempoHub.ViewModels.Content_Displays
{
    public class PlaylistRecentContentDisplayViewModel : PlaylistCustomContentDisplayViewModel
    {
        public PlaylistRecentContentDisplayViewModel()
        {
            Type = PlaylistType.Recent;
        }
    }
}
