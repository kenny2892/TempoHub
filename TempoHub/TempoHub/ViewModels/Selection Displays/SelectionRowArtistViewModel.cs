using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;

namespace TempoHub.ViewModels.Selection_Displays
{
    public class SelectionRowArtistViewModel : SelectionRowBaseViewModel
    {
        private string artistName = "";
        public string ArtistName
        {
            get { return artistName; }
            set
            {
                if(value != null && value != artistName)
                {
                    artistName = value;
                    OnPropertyChanged(nameof(ArtistName));
                }
            }
        }

        private string albumCount = "";
        public string AlbumCount
        {
            get { return albumCount; }
            set
            {
                if(value != null && value != albumCount)
                {
                    albumCount = value;
                    OnPropertyChanged(nameof(AlbumCount));
                }
            }
        }
        public double RowHeight { get; set; } = 77;

        public SelectionRowArtistViewModel()
        {
            Type = SelectionType.Artist;
        }
    }
}
