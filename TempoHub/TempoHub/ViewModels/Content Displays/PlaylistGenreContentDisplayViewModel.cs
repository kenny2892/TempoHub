using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;

namespace TempoHub.ViewModels.Content_Displays
{
    public class PlaylistGenreContentDisplayViewModel : PlaylistCustomContentDisplayViewModel
    {
        private List<string> genres = new List<string>();
        public List<string> Genres
        {
            get { return genres; }
            set
            {
                if(genres != value)
                {
                    genres = value;
                    OnPropertyChanged(nameof(Genres));
                }
            }
        }

        public PlaylistGenreContentDisplayViewModel()
        {
            Type = PlaylistType.Genres;
        }
    }
}
