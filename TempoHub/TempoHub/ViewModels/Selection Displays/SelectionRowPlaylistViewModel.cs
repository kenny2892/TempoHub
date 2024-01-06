using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TempoHub.Models;

namespace TempoHub.ViewModels.Selection_Displays
{
    public class SelectionRowPlaylistViewModel : SelectionRowBaseViewModel
    {
        public int PlaylistID { get; set; }
        public PlaylistType PlaylistDisplayType { get; set; } = PlaylistType.Custom;
        private Visibility deleteBtnVisibility = Visibility.Visible;
        public Visibility DeleteBtnVisibility
        {
            get { return deleteBtnVisibility; }
            set
            {
                if(value != deleteBtnVisibility)
                {
                    deleteBtnVisibility = value;
                    OnPropertyChanged(nameof(DeleteBtnVisibility));
                }
            }
        }
        private string playlistName = "";
        public string PlaylistName
        {
            get { return playlistName; }
            set
            {
                if(value != null && value != playlistName)
                {
                    playlistName = value;
                    OnPropertyChanged(nameof(PlaylistName));
                }
            }
        }
        private string songCount = "";
        public string SongCount
        {
            get { return songCount; }
            set
            {
                if(value != null && value != songCount)
                {
                    songCount = value;
                    OnPropertyChanged(nameof(SongCount));
                }
            }
        }
        public double RowHeight { get; set; } = 77;

        public SelectionRowPlaylistViewModel()
        {
            Type = SelectionType.Playlist;
        }
    }
}
