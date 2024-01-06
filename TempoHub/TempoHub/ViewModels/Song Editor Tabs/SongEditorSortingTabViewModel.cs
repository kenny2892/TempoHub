using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TagLib;
using TempoHub.Models;

namespace TempoHub.ViewModels.Song_Editor_Tabs
{
    public class SongEditorSortingTabViewModel : PropertyChangedBase
    {
        private string titleSort = "";
        public string TitleSort
        {
            get { return titleSort; }
            set
            {
                if(value != titleSort)
                {
                    titleSort = value;
                    OnPropertyChanged(nameof(TitleSort));
                }
            }
        }
        private string albumSort = "";
        public string AlbumSort
        {
            get { return albumSort; }
            set
            {
                if(value != albumSort)
                {
                    albumSort = value;
                    OnPropertyChanged(nameof(AlbumSort));
                }
            }
        }
        private string artistSort = "";
        public string ArtistSort
        {
            get { return artistSort; }
            set
            {
                if(value != artistSort)
                {
                    artistSort = value;
                    OnPropertyChanged(nameof(ArtistSort));
                }
            }
        }
        private string albumArtistSort = "";
        public string AlbumArtistSort
        {
            get { return albumArtistSort; }
            set
            {
                if(value != albumArtistSort)
                {
                    albumArtistSort = value;
                    OnPropertyChanged(nameof(AlbumArtistSort));
                }
            }
        }
        private string composerSort = "";
        public string ComposerSort
        {
            get { return composerSort; }
            set
            {
                if(value != composerSort)
                {
                    composerSort = value;
                    OnPropertyChanged(nameof(ComposerSort));
                }
            }
        }
        private Visibility allowSingleSongEdits = Visibility.Visible;
        public Visibility AllowSingleSongEdits
        {
            get { return allowSingleSongEdits; }
            set
            {
                if(value != allowSingleSongEdits)
                {
                    allowSingleSongEdits = value;
                    OnPropertyChanged(nameof(AllowSingleSongEdits));
                }
            }
        }

        public SongEditorSortingTabViewModel(SongInfo songInfo)
        {
            TitleSort = songInfo.TitleSort;
            AlbumSort = songInfo.AlbumSort;
            ArtistSort = songInfo.ArtistSort;
            AlbumArtistSort = songInfo.AlbumArtistSort;
            ComposerSort = songInfo.ComposerSort;
        }

        public void Clear()
        {
            TitleSort = "";
            AlbumSort = "";
            ArtistSort = "";
            AlbumArtistSort = "";
            ComposerSort = "";
        }
    }
}
