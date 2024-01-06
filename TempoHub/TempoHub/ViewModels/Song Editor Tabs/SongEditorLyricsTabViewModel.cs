using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;

namespace TempoHub.ViewModels.Song_Editor_Tabs
{
    public class SongEditorLyricsTabViewModel : PropertyChangedBase
    {
        private string lyrics = "";
        public string Lyrics
        {
            get { return lyrics; }
            set
            {
                if(lyrics != value)
                {
                    lyrics = value;
                    OnPropertyChanged(nameof(Lyrics));
                }
            }
        }

        public SongEditorLyricsTabViewModel(SongInfo songInfo)
        {
            Lyrics = songInfo.Lyrics;
        }

        public void Clear()
        {
            Lyrics = "";
        }
    }
}
