using MahApps.Metro.IconPacks;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoHub.ViewModels
{
    public class MusicBrainzSongResultViewModel : PropertyChangedBase
    {
        public string matchingFilePath = "";
        public string MatchingFilePath
        {
            get { return matchingFilePath; }
            set
            {
                if(matchingFilePath != value)
                {
                    matchingFilePath = value;
                    OnPropertyChanged(nameof(MatchingFilePath));
                    OnPropertyChanged(nameof(IconType));
                }
            }
        }
        public PackIconMicronsKind IconType
        {
            get { return String.IsNullOrEmpty(MatchingFilePath) ? PackIconMicronsKind.BoxCross : PackIconMicronsKind.BoxTick; }
        }
        public List<string> FilePathOptions { get; set; } = new List<string>();
        public ITrack Track { get; set; }
        public int Index { get; set; }
        public string Song { get; set; }
        public string Length { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
    }
}
