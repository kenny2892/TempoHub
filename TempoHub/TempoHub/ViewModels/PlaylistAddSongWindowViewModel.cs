using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib.Ape;
using TempoHub.Settings;

namespace TempoHub.ViewModels
{
    public class PlaylistAddSongWindowViewModel : PropertyChangedBase
    {
        public bool Canceled { get; set; } = false;
        private string titleSearchText = "";
        public string TitleSearchText
        {
            get { return titleSearchText; }
            set
            {
                if(titleSearchText != value)
                {
                    titleSearchText = value;
                    OnPropertyChanged(nameof(TitleSearchText));
                }
            }
        }
        private string artistSearchText = "";
        public string ArtistSearchText
        {
            get { return artistSearchText; }
            set
            {
                if(artistSearchText != value)
                {
                    artistSearchText = value;
                    OnPropertyChanged(nameof(ArtistSearchText));
                }
            }
        }
        private ObservableCollection<PlaylistSongAddRowViewModel> songs = new ObservableCollection<PlaylistSongAddRowViewModel>();
        public ObservableCollection<PlaylistSongAddRowViewModel> Songs
        {
            get { return songs; }
            set
            {
                if(songs != value)
                {
                    songs = value;
                    OnPropertyChanged(nameof(Songs));
                }
            }
        }
        private ObservableCollection<PlaylistSongAddRowViewModel> songsFiltered = new ObservableCollection<PlaylistSongAddRowViewModel>();
        public ObservableCollection<PlaylistSongAddRowViewModel> SongsFiltered
        {
            get { return songsFiltered; }
            set
            {
                if(songsFiltered != value)
                {
                    songsFiltered = value;
                    OnPropertyChanged(nameof(SongsFiltered));
                }
            }
        }

        public void Filter()
        {
            IEnumerable<PlaylistSongAddRowViewModel> filtered = null;
            bool searchForTitle = !String.IsNullOrEmpty(TitleSearchText) && !String.IsNullOrWhiteSpace(TitleSearchText);
            bool searchForArtist = !String.IsNullOrEmpty(ArtistSearchText) && !String.IsNullOrWhiteSpace(ArtistSearchText);

            if(Songs != null && (searchForTitle || searchForArtist))
            {
                filtered = Songs.Where(songRow =>
                {
                    bool titleMatch = true;
                    bool artistMatch = true;

                    if(searchForTitle)
                    {
                        titleMatch = String.IsNullOrEmpty(songRow.SongName) ? 
                            false : songRow.SongName.Contains(TitleSearchText, StringComparison.CurrentCultureIgnoreCase);
                    }

                    if(searchForArtist && titleMatch)
                    {
                        artistMatch = String.IsNullOrEmpty(songRow.Artist) ? 
                            false : songRow.Artist.Contains(ArtistSearchText, StringComparison.CurrentCultureIgnoreCase);
                    }

                    return titleMatch && artistMatch;
                });
            }

            else
            {
                filtered = Songs.AsEnumerable();
            }

            SongsFiltered = new ObservableCollection<PlaylistSongAddRowViewModel>(filtered);
        }
    }
}
