using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using TempoHub.Models;
using TempoHub.User_Controls;

namespace TempoHub.ViewModels.Song_Editor_Tabs
{
    public class SongEditorInfoTabViewModel : PropertyChangedBase
    {
        private string filePath = "";
        public string FilePath
        {
            get { return filePath; }
            set
            {
                if(value != filePath)
                {
                    filePath = value;
                    OnPropertyChanged(nameof(FilePath));
                }
            }
        }
        private string songLength = "";
        public string SongLength
        {
            get { return songLength; }
            set
            {
                if(value != songLength)
                {
                    songLength = value;
                    OnPropertyChanged(nameof(SongLength));
                }
            }
        }

        public SongEditorInfoTabViewModel(SongInfo songInfo)
        {
            FilePath = songInfo.FilePath;
            SongLength = songInfo.SongLength;
        }
    }
}
