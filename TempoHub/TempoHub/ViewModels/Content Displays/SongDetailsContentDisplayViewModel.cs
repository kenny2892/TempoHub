using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using TempoHub.Models;
using TempoHub.Settings;

namespace TempoHub.ViewModels.Content_Displays
{
    public class SongDetailsContentDisplayViewModel : PropertyChangedBase
    {
        public Action<List<string>> OnAddToQueueClickMethod { get; set; }
        public Action<string> OnPlayClickMethod { get; set; }
        public Action<List<string>> OnRemoveClickMethod { get; set; }
        public Action<string> OnEditSongInfoClickMethod { get; set; }
        public Action<List<string>> OnEditSongsInfoClickMethod { get; set; }
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
        private SongDetailsColumnTogglesViewModel songDetailsToggleVm = new SongDetailsColumnTogglesViewModel();
        public SongDetailsColumnTogglesViewModel SongDetailsToggleVm
        {
            get { return songDetailsToggleVm; }
            set
            {
                if(songDetailsToggleVm != value)
                {
                    songDetailsToggleVm = value;
                    OnPropertyChanged(nameof(SongDetailsToggleVm));
                }
            }
        }

        private ObservableCollection<SongInfo> songs = new ObservableCollection<SongInfo>();
        public ObservableCollection<SongInfo> Songs
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
        private CollectionViewSource songCollection = new CollectionViewSource();
        public CollectionViewSource SongCollection
        { 
            get { return songCollection; }
            set
            {
                if(songCollection != value)
                {
                    songCollection = value;
                    OnPropertyChanged(nameof(SongCollection));
                }
            }
        }

        private bool multiSelectEnabled = false;
        public bool MultiSelectEnabled
        { 
            get { return multiSelectEnabled; }
            set
            {
                if(multiSelectEnabled != value)
                {
                    multiSelectEnabled = value;
                    OnPropertyChanged(nameof(MultiSelectEnabled));
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

        public SongDetailsContentDisplayViewModel()
        {
            SongCollection.Source = Songs;
            SongCollection.IsLiveSortingRequested = true;

            SongCollection.Filter += (sender, e) =>
            {
                SongInfo song = e.Item as SongInfo;
                if(song != null)
                {
                    bool isMatch = true;

                    foreach(string propName in Filters.Keys)
                    {
                        var searchText = Filters[propName];
                        switch(propName)
                        {
                            case "File Path":
                                isMatch = song.FilePath != null && song.FilePath.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Title":
                                isMatch = song.Title != null && song.Title.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Album":
                                isMatch = song.Album != null && song.Album.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Artist":
                                isMatch = song.Artist != null && song.Artist.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Album Artist":
                                isMatch = song.AlbumArtist != null && song.AlbumArtist.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Genres":
                                isMatch = song.Genres != null && song.Genres.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Composer":
                                isMatch = song.Composer != null && song.Composer.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Publisher":
                                isMatch = song.Publisher != null && song.Publisher.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Conductor":
                                isMatch = song.Conductor != null && song.Conductor.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Grouping":
                                isMatch = song.Grouping != null && song.Grouping.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Length":
                                isMatch = song.SongLength != null && song.SongLength.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Year":
                                isMatch = song.Year != null && song.Year.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Track #":
                                isMatch = song.TrackCurr != null && song.TrackCurr.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Track Total":
                                isMatch = song.TrackTotal != null && song.TrackTotal.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Disc #":
                                isMatch = song.DiscCurr != null && song.DiscCurr.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Disc Total":
                                isMatch = song.DiscTotal != null && song.DiscTotal.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Rating":
                                isMatch = song.StarRating.ToString() == searchText;
                                break;

                            case "Bpm":
                                isMatch = song.Bpm != null && song.Bpm.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Comment":
                                isMatch = song.Comment != null && song.Comment.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Has Lyrics":
                                isMatch = song.HasLyrics.ToString().Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Has Album Cover":
                                isMatch = song.HasAlbumCover.ToString().Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;

                            case "Date Added":
                                isMatch = song.DateAdded.ToString().Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
                                break;
                        }

                        if(!isMatch)
                        {
                            break;
                        }
                    }

                    e.Accepted = isMatch;
                }
            };
        }

        public void ApplyDefaultSettings(AppliedSettings settings)
        {
            List<(int index, SortDescription sort)> sorts = new List<(int, SortDescription)>();

            CheckForDefault(sorts, "FilePath", settings.FilePathIsEnabled, settings.FilePathSort, settings.FilePathIndex);
            CheckForDefault(sorts, "Title", settings.TitleIsEnabled, settings.TitleSort, settings.TitleIndex);
            CheckForDefault(sorts, "Album", settings.AlbumIsEnabled, settings.AlbumSort, settings.AlbumIndex);
            CheckForDefault(sorts, "Artist", settings.ArtistIsEnabled, settings.ArtistSort, settings.ArtistIndex);
            CheckForDefault(sorts, "AlbumArtist", settings.AlbumArtistIsEnabled, settings.AlbumArtistSort, settings.AlbumArtistIndex);
            CheckForDefault(sorts, "Genres", settings.GenresIsEnabled, settings.GenresSort, settings.GenresIndex);
            CheckForDefault(sorts, "Composer", settings.ComposerIsEnabled, settings.ComposerSort, settings.ComposerIndex);
            CheckForDefault(sorts, "Publisher", settings.PublisherIsEnabled, settings.PublisherSort, settings.PublisherIndex);
            CheckForDefault(sorts, "Conductor", settings.ConductorIsEnabled, settings.ConductorSort, settings.ConductorIndex);
            CheckForDefault(sorts, "Grouping", settings.GroupingIsEnabled, settings.GroupingSort, settings.GroupingIndex);
            CheckForDefault(sorts, "SongLength", settings.SongLengthIsEnabled, settings.SongLengthSort, settings.SongLengthIndex);
            CheckForDefault(sorts, "Year", settings.YearIsEnabled, settings.YearSort, settings.YearIndex);
            CheckForDefault(sorts, "TrackCurr", settings.TrackCurrIsEnabled, settings.TrackCurrSort, settings.TrackCurrIndex);
            CheckForDefault(sorts, "TrackTotal", settings.TrackTotalIsEnabled, settings.TrackTotalSort, settings.TrackTotalIndex);
            CheckForDefault(sorts, "DiscCurr", settings.DiscCurrIsEnabled, settings.DiscCurrSort, settings.DiscCurrIndex);
            CheckForDefault(sorts, "DiscTotal", settings.DiscTotalIsEnabled, settings.DiscTotalSort, settings.DiscTotalIndex);
            CheckForDefault(sorts, "Rating", settings.RatingIsEnabled, settings.RatingSort, settings.RatingIndex);
            CheckForDefault(sorts, "Bpm", settings.BpmIsEnabled, settings.BpmSort, settings.BpmIndex);
            CheckForDefault(sorts, "Comment", settings.CommentIsEnabled, settings.CommentSort, settings.CommentIndex);
            CheckForDefault(sorts, "HasLyrics", settings.HasLyricsIsEnabled, settings.HasLyricsSort, settings.HasLyricsIndex);
            CheckForDefault(sorts, "HasAlbumCover", settings.HasAlbumCoverIsEnabled, settings.HasAlbumCoverSort, settings.HasAlbumCoverIndex);
            CheckForDefault(sorts, "DateAdded", settings.HasAlbumCoverIsEnabled, settings.HasAlbumCoverSort, settings.HasAlbumCoverIndex);

            sorts = sorts.OrderBy(pair => pair.index < 0 ? 100 : pair.index).ToList();
            SongCollection.SortDescriptions.Clear();
            SongCollection.LiveSortingProperties.Clear();
            sorts.ForEach(pair =>
            {
                SongCollection.SortDescriptions.Add(pair.sort);
                SongCollection.LiveSortingProperties.Add(pair.sort.PropertyName);
            });

            SongCollection.View.Refresh();
        }

        private void CheckForDefault(List<(int, SortDescription)> sorts, string name, bool isEnabled, SortOptions sort, int index)
        {
            if(isEnabled && sort != SortOptions.None)
            {
                Enum.TryParse<ListSortDirection>(sort.ToString(), true, out ListSortDirection order);
                sorts.Add((index, new SortDescription(name, order)));
            }
        }
    }
}
