using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using TempoHub.Models;
using MahApps.Metro.IconPacks;
using TempoHub.User_Controls;

namespace TempoHub.ViewModels
{
    public class SongListRowViewModel : PropertyChangedBase
    {
        public SongFile Song { get; set; }

        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if(value != isSelected)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
        private bool multiSelectEnabled = false;
        public bool MultiSelectEnabled
        {
            get { return multiSelectEnabled; }
            set
            {
                if(value != multiSelectEnabled)
                {
                    multiSelectEnabled = value;
                    OnPropertyChanged(nameof(MultiSelectEnabled));
                }
            }
        }

        private string songName = "";
        public string SongName
        {
            get { return songName; }
            set
            {
                if(value != null && value != songName)
                {
                    songName = value;
                    OnPropertyChanged(nameof(SongName));
                }
            }
        }

        private string songLength = "";
        public string SongLength
        {
            get { return songLength; }
            set
            {
                if(value != null && value != songLength)
                {
                    songLength = value;
                    OnPropertyChanged(nameof(SongLength));
                }
            }
        }

        private string trackNum = "";
        public string TrackNum
        {
            get { return trackNum; }
            set
            {
                if(value != trackNum)
                {
                    trackNum = value;
                    OnPropertyChanged(nameof(TrackNum));
                }
            }
        }

        private List<string> playlistNames = new List<string>();
        public List<string> PlaylistNames
        {
            get { return playlistNames; }
            set
            {
                if(value != playlistNames)
                {
                    playlistNames = value;
                    OnPropertyChanged(nameof(PlaylistNames));
                    OnPropertyChanged(nameof(HasPlaylists));
                }
            }
        }
        public bool HasPlaylists
        {
            get { return PlaylistNames.Count > 0; }
        }
    }
}
