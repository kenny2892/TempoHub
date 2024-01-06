using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;

namespace TempoHub.ViewModels.Selection_Displays
{
    public class SelectionRowAlbumViewModel : SelectionRowBaseViewModel
    {
        public Action OpenTagSearchMethod { get; set; }
        private string albumName = "";
        public string AlbumName
        {
            get { return albumName; }
            set
            {
                if(value != null && value != albumName)
                {
                    albumName = value;
                    OnPropertyChanged(nameof(AlbumName));
                }
            }
        }

        private string albumArtist = "";
        public string AlbumArtist
        {
            get { return albumArtist; }
            set
            {
                if(value != null && value != albumArtist)
                {
                    albumArtist = value;
                    OnPropertyChanged(nameof(AlbumArtist));
                }
            }
        }

        private AspectRatioImageViewModel imageVm;
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

        public double RowHeight { get; set; } = 77;

        public SelectionRowAlbumViewModel()
        {
            Type = SelectionType.Album;
        }
    }
}
