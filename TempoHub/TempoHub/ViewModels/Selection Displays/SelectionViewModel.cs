using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;
using TempoHub.Settings;

namespace TempoHub.ViewModels.Selection_Displays
{
    public class SelectionViewModel : PropertyChangedBase
    {

        private SelectionSearchBarViewModel searchVm = new SelectionSearchBarViewModel();
        public SelectionSearchBarViewModel SearchVm
        {
            get { return searchVm; }
            set
            {
                if(searchVm != value)
                {
                    searchVm = value;
                    OnPropertyChanged(nameof(SearchVm));
                }
            }
        }
        private ObservableCollection<SelectionRowBaseViewModel> items = new ObservableCollection<SelectionRowBaseViewModel>();
        public ObservableCollection<SelectionRowBaseViewModel> Items
        {
            get { return items; }
            set
            {
                if(items != value)
                {
                    items = value;
                    OnPropertyChanged(nameof(Items));
                }
            }
        }
        private ObservableCollection<SelectionRowBaseViewModel> itemsFiltered = new ObservableCollection<SelectionRowBaseViewModel>();
        public ObservableCollection<SelectionRowBaseViewModel> ItemsFiltered
        {
            get { return itemsFiltered; }
            set
            {
                if(itemsFiltered != value)
                {
                    itemsFiltered = value;
                    OnPropertyChanged(nameof(ItemsFiltered));
                }
            }
        }
        private int selectedIndex = -1;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if(selectedIndex != value)
                {
                    selectedIndex = value;
                    OnPropertyChanged(nameof(SelectedIndex));
                }
            }
        }

        public void ClearItems()
        {
            Items.Clear();
            itemsFiltered.Clear();
        }

        public void Filter()
        {
            IEnumerable<SelectionRowBaseViewModel> filtered = null;
            var searchText = SearchVm.SearchText;

            if(Items != null && !String.IsNullOrEmpty(searchText) && !String.IsNullOrWhiteSpace(searchText))
            {
                filtered = Items.Where(item => ContainsSearchText(item, searchText));
            }

            else
            {
                filtered = Items.AsEnumerable();
            }

            switch(SearchVm.SortOption)
            {
                case SortOptions.Ascending:
                    filtered = filtered.OrderBy(item => OrderValue(item));
                    break;

                case SortOptions.Descending:
                    filtered = filtered.OrderByDescending(item => OrderValue(item));
                    break;
            }

            ItemsFiltered = new ObservableCollection<SelectionRowBaseViewModel>(filtered);
        }

        private bool ContainsSearchText(SelectionRowBaseViewModel row, string searchText)
        {
            string textToCheck = "";

            if(row is SelectionRowAlbumViewModel album)
            {
                textToCheck = album.AlbumName;
            }

            else if(row is SelectionRowArtistViewModel artist)
            {
                textToCheck = artist.ArtistName;
            }

            else if(row is SelectionRowSongViewModel song)
            {
                textToCheck = song.SongTitle;
            }

            else if(row is SelectionRowPlaylistViewModel playlist)
            {
                textToCheck = playlist.PlaylistName;
            }

            else if(row is SelectionRowMusicBrainzResultViewModel musicBrainz)
            {
                textToCheck = musicBrainz.AlbumName + musicBrainz.ArtistName + musicBrainz.Year;
            }

            return textToCheck.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
        }

        private string OrderValue(SelectionRowBaseViewModel row)
        {
            if(row is SelectionRowAlbumViewModel album)
            {
                return album.AlbumName;
            }

            else if(row is SelectionRowArtistViewModel artist)
            {
                return artist.ArtistName;
            }

            else if(row is SelectionRowSongViewModel song)
            {
                return String.IsNullOrEmpty(song.SongTitleSort) ? song.SongTitle : song.SongTitleSort;
            }

            else if(row is SelectionRowPlaylistViewModel playlist)
            {
                // We want to keep the auto generated playlists at the top
                if(playlist.PlaylistDisplayType != PlaylistType.Custom)
                {
                    switch(SearchVm.SortOption)
                    {
                        case SortOptions.Ascending:
                            return "AAAA";

                        case SortOptions.Descending:
                            return "ZZZZ";
                    }
                }

                else
                {
                    return playlist.PlaylistName;
                }
            }

            else if(row is SelectionRowMusicBrainzResultViewModel musicBrainz)
            {
                return musicBrainz.AlbumName;
            }

            return "";
        }
    }
}
