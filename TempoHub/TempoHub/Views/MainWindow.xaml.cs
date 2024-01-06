using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using NAudio.Wave;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TagLib.Id3v2;
using TempoHub.User_Controls.Content_Displays;
using TempoHub.Data;
using TempoHub.Models;
using TempoHub.Services;
using TempoHub.Settings;
using TempoHub.User_Controls;
using TempoHub.ViewModels;
using TempoHub.ViewModels.Content_Displays;
using TempoHub.ViewModels.Selection_Displays;
using Timer = System.Timers.Timer;

namespace TempoHub.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool HasSetup { get; set; } = false;
        private bool IsMouseDragging { get; set; } = false;
        private SongEditWindow SongEditPopup { get; set; }
        private SettingsWindow SettingsPopup { get; set; }
        private List<SongFile> SongFiles { get; set; } = new List<SongFile>();
        private List<Playlist> Playlists { get; set; } = new List<Playlist>();
        private List<PlaylistSongEntry> PlaylistSongEntries { get; set; } = new List<PlaylistSongEntry>();
        private int CurrentSongIndex { get; set; } = -1;
        private List<SongFile> SongQueue { get; set; } = new List<SongFile>();
        private MediaPlayer Player { get; set; } = new MediaPlayer();
        private bool IsPaused { get; set; } = false;
        private Timer PlayerTimer { get; set; } = new Timer();
        private int PlayerTimerMsPerInterval { get; set; } = 100;
        private long ElapsedIntervals { get; set; } = 0;
        private double Volume { get; set; } = 0.5;
        private double StoredVolume { get; set; } = -1;
        private int GeneratedPlaylistID { get; } = -1;
        private MainWindowViewModel Vm { get; set; } = new MainWindowViewModel();
        private string CoreDirectory { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = Vm;
            SetDefaultStates();
            Closing += (sender, e) => { PlayerTimer.Stop(); };
            OnDisplaySelectionDropdownChange(null, null);
            ThemeManager.Current.ChangeTheme(this, $"{Vm.Settings.ThemeBase}.{Vm.Settings.ThemeColor}");
            HasSetup = true;
        }

        private SongContext GenerateSongContext()
        {
            DbContextOptionsBuilder<SongContext> options = new DbContextOptionsBuilder<SongContext>();
            options.UseSqlite("Data Source=SongData.sqlite");
            SongContext context = new SongContext(options.Options);
            context.Database.EnsureCreated();

            return context;
        }

        private void SetDefaultStates()
        {
            CoreDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TempoHub");

            if(!Directory.Exists(CoreDirectory))
            {
                Directory.CreateDirectory(CoreDirectory);
            }

            Directory.SetCurrentDirectory(CoreDirectory);

            volumeSlider.Value = 0.5;

            Player.MediaEnded += (sender, e) => NextTrack();
            PlayerTimer.Elapsed += new ElapsedEventHandler(TimelineProgression);
            PlayerTimer.Interval = PlayerTimerMsPerInterval;
            timeline.Visibility = Visibility.Hidden;
            timelineCurrTime.Content = "";
            timelineTotalTime.Content = "";
            currSongTitle.Content = "";

            // Set Default Selections
            albumSelection.Visibility = Visibility.Visible;
            artistSelection.Visibility = Visibility.Collapsed;
            songSelection.Visibility = Visibility.Collapsed;

            LoadInSettingsFile();
            LoadInPlaylists();

            displaySelectionDropdown.SelectedIndex = (int) Vm.Settings.DefaultDisplay;

            // Set the Song Details Methods
            Vm.SongDetailsContentDisplayVm.OnAddToQueueClickMethod = (filePaths) =>
            {
                foreach(var filePath in filePaths)
                {
                    var song = SongFiles.FirstOrDefault(song => song.FilePath == filePath);
                    if(song != null)
                    {
                        AddToQueue(song, -1);
                    }
                }
            };
            Vm.SongDetailsContentDisplayVm.OnPlayClickMethod = (filePath) =>
            {
                var song = SongFiles.FirstOrDefault(song => song.FilePath == filePath);
                if(song != null)
                {
                    AddToQueue(song, 0);
                    CurrentSongIndex = 0;
                    ResetCurrentTrack();
                }
            };
            Vm.SongDetailsContentDisplayVm.OnEditSongInfoClickMethod = (filePath) =>
            {
                var song = SongFiles.FirstOrDefault(song => song.FilePath == filePath);
                if(song != null)
                {
                    ShowSongEditor(null, song);
                }
            };
            Vm.SongDetailsContentDisplayVm.OnEditSongsInfoClickMethod = (filePaths) =>
            {
                List<SongFile> selectedSongs = new List<SongFile>();
                foreach(var filePath in filePaths)
                {
                    var song = SongFiles.FirstOrDefault(song => song.FilePath == filePath);
                    if(song != null)
                    {
                        selectedSongs.Add(song);
                    }
                }

                ShowSongEditor(null, selectedSongs.ToArray());
            };
            Vm.SongDetailsContentDisplayVm.OnRemoveClickMethod = (filePaths) =>
            {
                List<SongFile> selectedSongs = new List<SongFile>();
                foreach(var filePath in filePaths)
                {
                    var song = SongFiles.FirstOrDefault(song => song.FilePath == filePath);
                    if(song != null)
                    {
                        selectedSongs.Add(song);
                    }
                }

                PromptToRemoveSongs(selectedSongs);
            };
        }

        private void LoadInSettingsFile()
        {
            var savePath = Path.Join(Directory.GetCurrentDirectory(), "Configuration.json");
            if(File.Exists(savePath))
            {
                Vm.Settings = JsonSerializer.Deserialize<AppliedSettings>(File.ReadAllText(savePath));
                Vm.SongDetailsContentDisplayVm.ApplyDefaultSettings(Vm.Settings);
                Vm.SongDetailsToggleVm.ApplyDefaultSettings(Vm.Settings);
            }
        }

        private void LoadInPlaylists()
        {
            SongContext context = GenerateSongContext();
            Playlists.AddRange(context.Playlists);
            PlaylistSongEntries.AddRange(context.PlaylistSongEntries);
        }

        private bool ShowLoadingDisplay()
        {
            Vm.LoadingVisibility = Visibility.Visible;
            Vm.LoadingDisplayVm.Start();

            return true;
        }

        private bool ShowLoadingDisplay(double min, double max, double curr)
        {
            Vm.LoadingVisibility = Visibility.Visible;
            Vm.LoadingDisplayVm.Start(min, max, curr);

            return true;
        }

        private bool UpdateLoadingDisplay(double curr)
        {
            if(loadingDisplayGrid.Visibility != Visibility.Visible)
            {
                return false;
            }

            Vm.LoadingDisplayVm.CurrentValue = curr;
            return true;
        }

        private bool HideLoadingDisplay()
        {
            Vm.LoadingVisibility = Visibility.Collapsed;
            Vm.LoadingDisplayVm.Stop();
            return true;
        }

        private void ShowSongEditor(SongInfo songInfo, params SongFile[] selectedSongs)
        {
            if(songInfo is null)
            {
                songInfo = new SongInfo(selectedSongs);
            }

            SongEditPopup = new SongEditWindow(Vm.Settings.ThemeBase, Vm.Settings.ThemeColor, () => ShowTagSearch(selectedSongs), songInfo, selectedSongs);
            StopCurrentTrack();
            SongEditPopup.Closing += (sender, e) =>
            {
                if(SongEditPopup.ChangesWereApplied)
                {
                    try
                    {
                        foreach(var song in selectedSongs)
                        {
                            song.UpdateInfo();
                        }

                        if(albumSelection.Visibility == Visibility.Visible)
                        {
                            if(SongEditPopup.ArtistChanged)
                            {
                                Vm.AlbumContentDisplayVm.Artist = selectedSongs[0].Artist;
                            }

                            if(SongEditPopup.AlbumNameChanged || SongEditPopup.AlbumCoverChanged || SongEditPopup.ArtistChanged)
                            {
                                ShowLoadingDisplay();
                                SongEditUpdateAlbumName(selectedSongs[0]);
                                UpdateDisplaySelectionWithAlbums();
                            }

                            if(SongEditPopup.TitleChanged || SongEditPopup.AlbumNameChanged || SongEditPopup.AlbumCoverChanged || SongEditPopup.TrackNumChanged)
                            {
                                OnAlbumSelectionSelectedChanged(null, null);
                            }

                            if(SongEditPopup.TitleChanged)
                            {
                                SongEditUpdateSongQueueTitle(selectedSongs[0]);
                            }
                        }

                        else if(artistSelection.Visibility == Visibility.Visible)
                        {
                            if(SongEditPopup.ArtistChanged || SongEditPopup.AlbumNameChanged)
                            {
                                ShowLoadingDisplay();
                                SongEditUpdateArtistName(selectedSongs[0]);
                                UpdateDisplaySelectionWithArtists();
                            }

                            if(SongEditPopup.TitleChanged || SongEditPopup.AlbumNameChanged || SongEditPopup.AlbumCoverChanged || SongEditPopup.TrackNumChanged)
                            {
                                OnArtistSelectionSelectedChanged(null, null);
                            }

                            if(SongEditPopup.TitleChanged)
                            {
                                SongEditUpdateSongQueueTitle(selectedSongs[0]);
                            }
                        }

                        else if(songSelection.Visibility == Visibility.Visible)
                        {
                            if(SongEditPopup.ArtistChanged || SongEditPopup.TitleChanged || SongEditPopup.AlbumNameChanged || SongEditPopup.AlbumCoverChanged)
                            {
                                ShowLoadingDisplay();
                                UpdateDisplaySelectionWithSongs();
                                OnSongSelectionSelectedChanged(null, null);
                            }

                            if(SongEditPopup.TitleChanged)
                            {
                                SongEditUpdateSongQueueTitle(selectedSongs[0]);
                            }
                        }

                        if(songDetailsContentDisplay.Visibility == Visibility.Visible)
                        {
                            UpdateDisplaySelectionWithSongDetails();
                        }
                    }

                    catch(Exception)
                    {
                        HideLoadingDisplay();
                    }
                }
            };

            SongEditPopup.ShowDialog();
        }

        private void ShowTagSearch(params SongFile[] songsToEdit)
        {
            var tagSearchVm = new TagSearchWindowViewModel();
            tagSearchVm.SongsToUpdate.AddRange(songsToEdit.Select(song => new SongInfo(song)));

            if(tagSearchVm.SongsToUpdate.Count > 0)
            {
                var first = tagSearchVm.SongsToUpdate[0];
                string album = tagSearchVm.SongsToUpdate.All(song => song.Album == first.Album) ? first.Album : "";
                string artist = tagSearchVm.SongsToUpdate.All(song => song.Artist == first.Artist) ? first.Artist : "";
                string year = tagSearchVm.SongsToUpdate.All(song => song.Year == first.Year) ? first.Year : "";

                tagSearchVm.AlbumNameSearch = album;
                tagSearchVm.ArtistNameSearch = artist;
                tagSearchVm.YearSearch = year;
            }

            var tagSearchWindow = new TagSearchWindow(Vm.Settings.ThemeBase, Vm.Settings.ThemeColor);
            tagSearchWindow.DataContext = tagSearchVm;

            tagSearchWindow.Closing += (sender, e) =>
            {
                if(tagSearchVm.HitApply && tagSearchVm.SelectionVm.SelectedIndex >= 0)
                {
                    var selected = tagSearchVm.SelectionVm.ItemsFiltered[tagSearchVm.SelectionVm.SelectedIndex] as SelectionRowMusicBrainzResultViewModel;

                    if(selected is null)
                    {
                        return;
                    }

                    var albumMusicBrainz = selected.MusicBrainzAlbumSearchResult;
                    var songsToUpdateMusicBrainz = tagSearchVm.Songs.Where(song => !String.IsNullOrEmpty(song.MatchingFilePath));

                    foreach(var songFileToUpdate in songsToEdit)
                    {
                        var tagFile = songFileToUpdate.TagLibFile;

                        if(tagFile.Tag.Album != albumMusicBrainz.Title && (tagSearchVm.OverrideExistingData || !String.IsNullOrEmpty(albumMusicBrainz.Title)))
                        {
                            tagFile.Tag.Album = albumMusicBrainz.Title;
                        }

                        uint albumMusicBrainzYear = (uint) ((int) albumMusicBrainz.Date.Year);
                        if(tagFile.Tag.Year != albumMusicBrainzYear && (tagSearchVm.OverrideExistingData || albumMusicBrainz.Date.Year != null))
                        {
                            tagFile.Tag.Year = albumMusicBrainzYear;
                        }

                        // Album Cover
                        if(selected.ImageData.Length > 0 && !String.IsNullOrEmpty(selected.MimeType))
                        {
                            AttachmentFrame picToAdd = new AttachmentFrame
                            {
                                Type = TagLib.PictureType.FrontCover,
                                Description = "Album Cover",
                                MimeType = selected.MimeType,
                                TextEncoding = TagLib.StringType.UTF16,
                                Data = selected.ImageData
                            };

                            var newList = tagFile.Tag.Pictures.ToList();
                            newList.Insert(0, picToAdd);
                            tagFile.Tag.Pictures = newList.ToArray();
                        }

                        // Total Track Count
                        if(albumMusicBrainz.Media != null)
                        {
                            tagFile.Tag.TrackCount = (uint) albumMusicBrainz.Media[0].TrackCount;
                        }

                        // Artist
                        if(albumMusicBrainz.ArtistCredit?.Count > 0)
                        {
                            if(tagFile.Tag.Performers.Length == 0)
                            {
                                tagFile.Tag.Performers = new string[] { albumMusicBrainz.ArtistCredit[0].Name };
                            }

                            else
                            {
                                // For some reason, just assigning a new value to tagFile.Tag.Performers[0] doesn't work. The value doesn't change
                                var performers = tagFile.Tag.Performers;
                                performers[0] = albumMusicBrainz.ArtistCredit[0].Name;
                                tagFile.Tag.Performers = performers;
                            }
                        }

                        else if(tagSearchVm.OverrideExistingData)
                        {
                            tagFile.Tag.Performers = new string[] { "" };
                        }

                        // Album Artists
                        if(albumMusicBrainz.ArtistCredit?.Count > 1)
                        {
                            tagFile.Tag.AlbumArtists = albumMusicBrainz.ArtistCredit.Select(artist => artist.Name).Skip(1).ToArray();
                        }

                        else if(tagSearchVm.OverrideExistingData)
                        {
                            tagFile.Tag.AlbumArtists = new string[] { "" };
                        }

                        // Genres
                        if(albumMusicBrainz.Genres?.Count > 0)
                        {
                            tagFile.Tag.Genres = albumMusicBrainz.Genres.Select(genre => genre.Name).ToArray();
                        }

                        else if(tagSearchVm.OverrideExistingData)
                        {
                            tagFile.Tag.Genres = new string[] { "" };
                        }

                        // Title & Current Track
                        if(songsToUpdateMusicBrainz.Any(toCheck => toCheck.MatchingFilePath == songFileToUpdate.FilePath))
                        {
                            var songRow = songsToUpdateMusicBrainz.FirstOrDefault(toCheck => toCheck.MatchingFilePath == songFileToUpdate.FilePath);

                            if(tagFile.Tag.Title != songRow.Song && (tagSearchVm.OverrideExistingData || !String.IsNullOrEmpty(songRow.Song)))
                            {
                                tagFile.Tag.Title = songRow.Song;
                            }

                            tagFile.Tag.Track = (uint) songRow.Index;
                        }

                        tagFile.Save();
                    }

                    try
                    {
                        if(albumSelection.Visibility == Visibility.Visible)
                        {
                            Vm.AlbumContentDisplayVm.Artist = songsToEdit[0].TagLibFile.Tag.FirstPerformer;
                            ShowLoadingDisplay();
                            SongEditUpdateAlbumName(songsToEdit);
                            UpdateDisplaySelectionWithAlbums();
                            OnAlbumSelectionSelectedChanged(null, null);
                            SongEditUpdateSongQueueTitle(songsToEdit);
                        }

                        else if(artistSelection.Visibility == Visibility.Visible)
                        {
                            ShowLoadingDisplay();
                            SongEditUpdateArtistName(songsToEdit);
                            UpdateDisplaySelectionWithArtists();
                            OnArtistSelectionSelectedChanged(null, null);
                            SongEditUpdateSongQueueTitle(songsToEdit);
                        }

                        else if(songSelection.Visibility == Visibility.Visible)
                        {
                            if(SongEditPopup.ArtistChanged || SongEditPopup.TitleChanged || SongEditPopup.AlbumNameChanged || SongEditPopup.AlbumCoverChanged)
                            {
                                ShowLoadingDisplay();
                                UpdateDisplaySelectionWithSongs();
                                OnSongSelectionSelectedChanged(null, null);
                                SongEditUpdateSongQueueTitle(songsToEdit);
                            }
                        }

                        if(songDetailsContentDisplay.Visibility == Visibility.Visible)
                        {
                            UpdateDisplaySelectionWithSongDetails();
                        }
                    }

                    catch(Exception)
                    {
                        HideLoadingDisplay();
                    }
                }
            };

            tagSearchWindow.Show();
        }

        private void LoadInSongs(SongContext context = null)
        {
            if(context is null)
            {
                context = GenerateSongContext();

                // Load in all song paths
                foreach(var song in context.SongPaths.ToList().Where(song => SongFiles.All(existing => existing.FilePath != song.FilePath)))
                {
                    try
                    {
                        // Make sure the file is compatible with TagLib
                        var musicFile = TagLib.File.Create(song.FilePath);

                        // Get the Duration using NAudio since TagLib is bugged and will give a shorter durtion on many occasions
                        var reader = new AudioFileReader(song.FilePath);
                        TimeSpan duration = reader.TotalTime;

                        SongFiles.Add(new SongFile(song.FilePath, song.DateAdded, duration));
                    }

                    catch(Exception)
                    {
                        Trace.WriteLine("Could not parse file: " + song.FilePath);
                    }
                }
            }
        }

        public void AddSongs(string[] filePaths)
        {
            SongContext context = GenerateSongContext();

            foreach(var musicFilePath in filePaths.Where(pathToAdd => SongFiles.All(existing => existing.FilePath != pathToAdd)))
            {
                try
                {
                    // Test to make sure the file is compatible with TagLib
                    var musicFile = TagLib.File.Create(musicFilePath);

                    // Get the Duration using NAudio since TagLib is bugged and will give a shorter durtion on many occasions
                    var reader = new AudioFileReader(musicFilePath);
                    TimeSpan duration = reader.TotalTime;

                    SongFiles.Add(new SongFile(musicFilePath, DateTime.Now, duration));
                    context.SongPaths.Add(new SongPath() { FilePath = musicFilePath, DateAdded = DateTime.Now });
                }

                catch(Exception)
                {
                    Trace.WriteLine("File was not added: " + musicFilePath);
                }
            }

            context.SaveChanges();
            ShowLoadingDisplay();
            UpdateDisplaySelection(displaySelectionDropdown.SelectedIndex);
        }

        private void PromptToRemoveSongs(List<SongFile> songsToDelete)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure?", $"Remove {songsToDelete.Count} Song{(songsToDelete.Count != 1 ? "s" : "")}", MessageBoxButton.YesNo);

            if(result == MessageBoxResult.Yes)
            {
                RemoveSongs(songsToDelete);
            }
        }

        private void RemoveSongs(List<SongFile> songFiles)
        {
            SongContext context = GenerateSongContext();

            foreach(var pathToRemove in songFiles.Select(song => song.FilePath))
            {
                // Remove from Songs
                var songToRemove = context.SongPaths.FirstOrDefault(song => song.FilePath == pathToRemove);

                if(songToRemove != null)
                {
                    context.SongPaths.Remove(songToRemove);
                }

                // Remove from History
                var historyRecordToRemove = context.HistoryRecords.FirstOrDefault(record => record.FilePath == pathToRemove);

                if(historyRecordToRemove != null)
                {
                    context.HistoryRecords.Remove(historyRecordToRemove);
                }

                // Remove from Playlists
                var playlistSongToRemove = context.PlaylistSongEntries.FirstOrDefault(songEntry => songEntry.SongPath == pathToRemove);

                if(playlistSongToRemove != null)
                {
                    context.PlaylistSongEntries.Remove(playlistSongToRemove);
                }
            }

            context.SaveChanges();

            SongFiles.Clear();
            Playlists.Clear();
            PlaylistSongEntries.Clear();
            OnDisplaySelectionDropdownChange(null, null);
            LoadInPlaylists();
        }

        private void OnDisplaySelectionDropdownChange(object sender, SelectionChangedEventArgs e)
        {
            // Will fire before initialization if a default value is set
            if(!IsInitialized)
            {
                return;
            }

            ShowLoadingDisplay();
            UpdateDisplaySelection(displaySelectionDropdown.SelectedIndex);
        }

        private void UpdateDisplaySelection(int selectedIndex)
        {
            switch(selectedIndex)
            {
                case 0:
                    UpdateDisplaySelectionWithAlbums();
                    break;

                case 1:
                    UpdateDisplaySelectionWithArtists();
                    break;

                case 2:
                    UpdateDisplaySelectionWithSongs();
                    break;

                case 3:
                    UpdateDisplaySelectionWithSongDetails();
                    break;

                case 4:
                    UpdateDisplaySelectionWithPlaylist();
                    break;
            }
        }

        private void UpdateDisplaySelectionWithAlbums(SongContext context = null)
        {
            albumSelection.Visibility = Visibility.Visible;
            artistSelection.Visibility = Visibility.Collapsed;
            songSelection.Visibility = Visibility.Collapsed;
            songDetailsSelection.Visibility = Visibility.Collapsed;
            playlistSelection.Visibility = Visibility.Collapsed;

            ClearAllDisplaySelections();
            Vm.ArtistContentDisplayVm = new ArtistContentDisplayViewModel();
            Vm.SongContentDisplayVm = new SongContentDisplayViewModel();
            Vm.PlaylistContentDisplayVm = new PlaylistCustomContentDisplayViewModel();
            LoadInSongs(context);

            Thread makeAlbums = new Thread(() =>
            {
                int index = 0;
                int indexToSelect = -1;
                var albumsToAdd = new List<SelectionRowAlbumViewModel>();
                var albums = SongFiles.GroupBy(song => (song.Album, song.Artist)).OrderBy(group => group.Key);
                foreach(var album in albums)
                {
                    var firstSong = album.FirstOrDefault(song => song.TagLibFile.Tag.Track == 1);
                    if(firstSong is null)
                    {
                        firstSong = album.First();
                    }

                    var imageVm = new AspectRatioImageViewModel();
                    if(firstSong.TagLibFile.Tag.Pictures.Count() > 0)
                    {
                        imageVm.ImageData = firstSong.TagLibFile.Tag.Pictures[0].Data.Data;
                    }

                    var albumRowVm = new SelectionRowAlbumViewModel();
                    albumRowVm.AlbumName = firstSong.Album;
                    albumRowVm.AlbumArtist = firstSong.Artist;
                    albumRowVm.ImageVm = imageVm;
                    albumRowVm.OpenTagSearchMethod = () => ShowTagSearch(album.Select(song => song).ToArray());

                    // Reselect the current album
                    if(HasSetup)
                    {
                        var albumMatch = String.IsNullOrEmpty(Vm.AlbumContentDisplayVm.AlbumName) && String.IsNullOrEmpty(albumRowVm.AlbumName);
                        var artistMatch = String.IsNullOrEmpty(Vm.AlbumContentDisplayVm.Artist) && String.IsNullOrEmpty(albumRowVm.AlbumArtist);

                        if(!albumMatch)
                        {
                            albumMatch = albumRowVm.AlbumName == Vm.AlbumContentDisplayVm.AlbumName;
                        }

                        if(!artistMatch)
                        {
                            artistMatch = albumRowVm.AlbumArtist == Vm.AlbumContentDisplayVm.Artist;
                        }

                        if(albumMatch && artistMatch)
                        {
                            indexToSelect = index;
                        }
                    }

                    index++;
                    albumsToAdd.Add(albumRowVm);
                }

                Dispatcher.BeginInvoke(() =>
                {
                    foreach(var albumRowVm in albumsToAdd)
                    {
                        Vm.AlbumSelectionVm.Items.Add(albumRowVm);
                    }

                    if(indexToSelect == -1)
                    {
                        Vm.AlbumContentDisplayVm = new AlbumContentDisplayViewModel();
                        indexToSelect = 0;
                    }

                    Vm.AlbumSelectionVm.SelectedIndex = indexToSelect;
                    Vm.AlbumSelectionVm.Filter();
                    HideLoadingDisplay();
                });
            });

            makeAlbums.Start();
        }

        private void OnAlbumSelectionSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Vm.AlbumSelectionVm.SelectedIndex >= Vm.AlbumSelectionVm.ItemsFiltered.Count || Vm.AlbumSelectionVm.SelectedIndex < 0)
            {
                return;
            }

            var selectedAlbum = Vm.AlbumSelectionVm.ItemsFiltered[Vm.AlbumSelectionVm.SelectedIndex] as SelectionRowAlbumViewModel;

            if(selectedAlbum is null)
            {
                return;
            }

            var album = SongFiles.GroupBy(song => (song.Album, song.Artist))
                .FirstOrDefault(group =>
                {
                    var albumMatch = false;
                    var artistMatch = false;

                    if(String.IsNullOrEmpty(selectedAlbum.AlbumName) && String.IsNullOrEmpty(group.Key.Album))
                    {
                        albumMatch = true;
                    }

                    else
                    {
                        albumMatch = group.Key.Album == selectedAlbum.AlbumName;
                    }

                    if(String.IsNullOrEmpty(selectedAlbum.AlbumArtist) && String.IsNullOrEmpty(group.Key.Artist))
                    {
                        artistMatch = true;
                    }

                    else
                    {
                        artistMatch = group.Key.Artist == selectedAlbum.AlbumArtist;
                    }

                    return albumMatch && artistMatch;
                });

            if(album != null)
            {
                LoadAlbumContentDisplay(album);
            }
        }

        private void LoadAlbumContentDisplay(IGrouping<(string Album, string Artist), SongFile> album)
        {
            AlbumContentDisplayViewModel albumContentVm = new AlbumContentDisplayViewModel();
            albumContentVm.AlbumName = album.Key.Album;
            albumContentVm.Artist = album.Key.Artist;

            albumContentVm.EditSongInfo = (song) => ShowSongEditor(null, song);
            albumContentVm.Play = (song) =>
            {
                AddToQueue(song, 0);
                CurrentSongIndex = 0;
                ResetCurrentTrack();
            };
            albumContentVm.EditSongsInfo = () =>
            {
                ShowSongEditor(null, albumContentVm.Songs.Where(songRow => songRow.IsSelected).Select(songRow => songRow.Song).ToArray());
            };
            albumContentVm.AddToQueue = () =>
            {
                List<SongFile> selectedSongs = new List<SongFile>();
                foreach(var song in albumContentVm.Songs.Where(songRow => songRow.IsSelected).Select(songRow => songRow.Song))
                {
                    AddToQueue(song, -1);
                }
            };
            albumContentVm.RemoveSongs = () =>
            {
                var songsToDelete = albumContentVm.Songs.Where(songRow => songRow.IsSelected).Select(songRow => songRow.Song).ToList();
                PromptToRemoveSongs(songsToDelete);
            };

            var songs = album.ToList();                
            songs.Sort((x, y) =>
            {
                var xTag = x.TagLibFile.Tag;
                var yTag = y.TagLibFile.Tag;

                if(xTag.Track != yTag.Track)
                {
                    return xTag.Track.CompareTo(yTag.Track);
                }

                return string.Compare(xTag.Title, yTag.Title, true);
            });

            foreach(var song in songs)
            {
                var tagFile = song.TagLibFile;

                SongListRowViewModel songVm = new SongListRowViewModel();
                songVm.Song = song;
                songVm.SongName = tagFile.Tag.Title;
                songVm.SongLength = $"{song.Duration.Minutes}:{(song.Duration.Seconds).ToString().PadLeft(2, '0')}";
                songVm.TrackNum = tagFile.Tag.Track == 0 ? "" : $"{tagFile.Tag.Track}.";
                songVm.PlaylistNames.AddRange(Playlists.Select(playlist => playlist.Name).Distinct().Order());

                albumContentVm.Songs.Add(songVm);
            }

            if(songs.First().TagLibFile.Tag.Pictures.Count() > 0)
            {
                albumContentVm.ImageVm.ImageData = songs.First().TagLibFile.Tag.Pictures[0].Data.Data;
            }

            Vm.AlbumContentDisplayVm = albumContentVm;
        }

        private void UpdateDisplaySelectionWithArtists(SongContext context = null)
        {
            albumSelection.Visibility = Visibility.Collapsed;
            artistSelection.Visibility = Visibility.Visible;
            songSelection.Visibility = Visibility.Collapsed;
            songDetailsSelection.Visibility = Visibility.Collapsed;
            playlistSelection.Visibility = Visibility.Collapsed;

            ClearAllDisplaySelections();
            Vm.AlbumContentDisplayVm = new AlbumContentDisplayViewModel();
            Vm.SongContentDisplayVm = new SongContentDisplayViewModel();
            Vm.PlaylistContentDisplayVm = new PlaylistCustomContentDisplayViewModel();
            LoadInSongs(context);

            Thread makeArtists = new Thread(() =>
            {
                int index = 0;
                int indexToSelect = -1;
                var artistsToAdd = new List<SelectionRowArtistViewModel>();
                var artists = SongFiles.GroupBy(song => song.TagLibFile.Tag.FirstPerformer).OrderBy(group => group.Key);
                foreach(var artist in artists)
                {
                    var artistRowVm = new SelectionRowArtistViewModel();
                    artistRowVm.ArtistName = artist.Key;
                    artistRowVm.AlbumCount = $"{artist.Count()} Album{(artist.Count() > 1 ? "s" : "")}";

                    // Reselect the current aritst
                    if(String.IsNullOrEmpty(Vm.ArtistContentDisplayVm.ArtistName) && String.IsNullOrEmpty(artistRowVm.ArtistName))
                    {
                        indexToSelect = index;
                    }

                    else if(artistRowVm.ArtistName == Vm.ArtistContentDisplayVm.ArtistName && HasSetup)
                    {
                        indexToSelect = index;
                    }

                    index++;
                    artistsToAdd.Add(artistRowVm);
                }

                Dispatcher.BeginInvoke(() =>
                {
                    foreach(var artistRowVm in artistsToAdd)
                    {
                        Vm.ArtistSelectionVm.Items.Add(artistRowVm);
                    }

                    if(indexToSelect == -1)
                    {
                        Vm.ArtistContentDisplayVm = new ArtistContentDisplayViewModel();
                        indexToSelect = 0;
                    }

                    Vm.ArtistSelectionVm.SelectedIndex = indexToSelect;
                    Vm.ArtistSelectionVm.Filter();
                    HideLoadingDisplay();
                });
            });

            makeArtists.Start();
        }

        private void OnArtistSelectionSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Vm.ArtistSelectionVm.SelectedIndex >= Vm.ArtistSelectionVm.ItemsFiltered.Count || Vm.ArtistSelectionVm.SelectedIndex < 0)
            {
                return;
            }

            var selectedArtist = Vm.ArtistSelectionVm.ItemsFiltered[Vm.ArtistSelectionVm.SelectedIndex] as SelectionRowArtistViewModel;

            if(selectedArtist is null)
            {
                return;
            }

            var artist = SongFiles.GroupBy(song => song.TagLibFile.Tag.FirstPerformer)
                .FirstOrDefault(group =>
                {
                    if(String.IsNullOrEmpty(selectedArtist.ArtistName))
                    {
                        return String.IsNullOrEmpty(group.Key);
                    }

                    else
                    {
                        return group.Key == selectedArtist.ArtistName;
                    }
                });

            if(artist != null)
            {
                LoadArtistContentDisplay(artist);
            }
        }

        private void LoadArtistContentDisplay(IGrouping<string, SongFile> artist)
        {
            ArtistContentDisplayViewModel artistContentVm = new ArtistContentDisplayViewModel();
            artistContentVm.ArtistName = artist.Key;

            var albums = artist.GroupBy(song => song.TagLibFile.Tag.Album);
            foreach(var album in albums)
            {
                AlbumContentDisplayViewModel albumContentVm = new AlbumContentDisplayViewModel();
                albumContentVm.ParentArtistContaier = artistContentVm;
                albumContentVm.AlbumName = album.First().TagLibFile.Tag.Album;

                albumContentVm.EditSongInfo = (song) => ShowSongEditor(null, song);
                albumContentVm.Play = (song) =>
                {
                    AddToQueue(song, 0);
                    CurrentSongIndex = 0;
                    ResetCurrentTrack();
                };
                albumContentVm.EditSongsInfo = () =>
                {
                    List<SongFile> selectedSongs = new List<SongFile>();
                    foreach(var albumVm in Vm.ArtistContentDisplayVm.AlbumDisplays)
                    {
                        selectedSongs.AddRange(albumVm.Songs.Where(songRow => songRow.IsSelected).Select(songRow => songRow.Song).ToList());
                    }

                    ShowSongEditor(null, selectedSongs.ToArray());
                };
                albumContentVm.AddToQueue = () =>
                {
                    List<SongFile> selectedSongs = new List<SongFile>();
                    foreach(var albumVm in Vm.ArtistContentDisplayVm.AlbumDisplays)
                    {
                        foreach(var song in albumVm.Songs.Where(songRow => songRow.IsSelected).Select(songRow => songRow.Song))
                        {
                            AddToQueue(song, -1);
                        }
                    }
                };
                albumContentVm.RemoveSongs = () =>
                {
                    List<SongFile> songsToDelete = new List<SongFile>();
                    foreach(var albumVm in Vm.ArtistContentDisplayVm.AlbumDisplays)
                    {
                        songsToDelete.AddRange(albumVm.Songs.Where(songRow => songRow.IsSelected).Select(songRow => songRow.Song));
                    }

                    PromptToRemoveSongs(songsToDelete);
                };

                var orderedSongs = album.OrderBy(song => song.TagLibFile.Tag.Track);
                foreach(var song in orderedSongs)
                {
                    SongListRowViewModel songVm = new SongListRowViewModel();
                    songVm.Song = song;
                    songVm.SongName = song.TagLibFile.Tag.Title;
                    songVm.SongLength = $"{song.Duration.Minutes}:{(song.Duration.Seconds).ToString().PadLeft(2, '0')}";
                    songVm.TrackNum = song.TagLibFile.Tag.Track == 0 ? "" : $"{song.TagLibFile.Tag.Track}.";
                    songVm.PlaylistNames.AddRange(Playlists.Select(playlist => playlist.Name).Distinct().Order());

                    albumContentVm.Songs.Add(songVm);
                }

                if(orderedSongs.First().TagLibFile.Tag.Pictures.Count() > 0)
                {
                    albumContentVm.ImageVm.ImageData = orderedSongs.First().TagLibFile.Tag.Pictures[0].Data.Data;
                }

                artistContentVm.AlbumDisplays.Add(albumContentVm);
            }

            Vm.ArtistContentDisplayVm = artistContentVm;
        }

        private void UpdateDisplaySelectionWithSongs(SongContext context = null)
        {
            albumSelection.Visibility = Visibility.Collapsed;
            artistSelection.Visibility = Visibility.Collapsed;
            songSelection.Visibility = Visibility.Visible;
            songDetailsSelection.Visibility = Visibility.Collapsed;
            playlistSelection.Visibility = Visibility.Collapsed;

            ClearAllDisplaySelections();
            Vm.AlbumContentDisplayVm = new AlbumContentDisplayViewModel();
            Vm.ArtistContentDisplayVm = new ArtistContentDisplayViewModel();
            Vm.PlaylistContentDisplayVm = new PlaylistCustomContentDisplayViewModel();
            LoadInSongs(context);

            Thread makeSongs = new Thread(() =>
            {
                int index = 0;
                int indexToSelect = 0;

                var songs = new List<SelectionRowSongViewModel>();
                var orderedSongFiles = SongFiles.OrderBy(song =>
                {
                    var tagFile = song.TagLibFile;
                    return String.IsNullOrEmpty(tagFile.Tag.TitleSort) ? tagFile.Tag.Title : tagFile.Tag.TitleSort;
                });

                foreach(var song in orderedSongFiles)
                {
                    var tags = song.TagLibFile.Tag;

                    var songVm = new SelectionRowSongViewModel();
                    songVm.SongTitle = tags.Title;
                    songVm.SongTitleSort = tags.TitleSort;
                    songVm.AlbumName = tags.Album;
                    songVm.AlbumNameSort = tags.AlbumSort;
                    songVm.AlbumArtist = tags.FirstPerformer;
                    songVm.AlbumArtistSort = tags.FirstPerformerSort;
                    songVm.FilePath = song.FilePath;

                    var imageVm = new AspectRatioImageViewModel();
                    if(tags.Pictures.Count() > 0)
                    {
                        imageVm.ImageData = tags.Pictures[0].Data.Data;
                    }
                    songVm.ImageVm = imageVm;

                    // Reselect the current song
                    if(Vm.SongContentDisplayVm.FilePath == songVm.FilePath)
                    {
                        indexToSelect = index;
                    }

                    index++;
                    songs.Add(songVm);
                }

                Dispatcher.BeginInvoke(() =>
                {
                    foreach(var songVm in songs)
                    {
                        Vm.SongSelectionVm.Items.Add(songVm);
                    }

                    if(indexToSelect == -1)
                    {
                        Vm.SongContentDisplayVm = new SongContentDisplayViewModel();
                        indexToSelect = 0;
                    }

                    Vm.SongSelectionVm.SelectedIndex = indexToSelect;
                    Vm.SongSelectionVm.Filter();

                    HideLoadingDisplay();
                });
            });

            makeSongs.Start();
        }

        private void OnSongSelectionSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Vm.SongSelectionVm.SelectedIndex >= Vm.SongSelectionVm.ItemsFiltered.Count || Vm.SongSelectionVm.SelectedIndex < 0)
            {
                return;
            }

            var selectedSong = Vm.SongSelectionVm.ItemsFiltered[Vm.SongSelectionVm.SelectedIndex] as SelectionRowSongViewModel;

            if(selectedSong is null)
            {
                return;
            }

            var song = SongFiles.FirstOrDefault(existing => existing.FilePath == selectedSong.FilePath);

            if(song != null)
            {
                LoadSongContentDisplay(song);
            }
        }

        private void LoadSongContentDisplay(SongFile song)
        {
            SongContentDisplayViewModel songContentVm = new SongContentDisplayViewModel();

            var tags = song.TagLibFile.Tag;
            songContentVm.SongTitle = tags.Title;
            songContentVm.AlbumName = tags.Album;
            songContentVm.ArtistName = tags.FirstPerformer;
            songContentVm.SongLength = song.Duration.ToString(@"hh\:mm\:ss");
            songContentVm.FilePath = song.FilePath;
            songContentVm.OnQueueMethod = () => AddToQueue(song, -1);
            songContentVm.OnPlayMethod = () =>
            {
                AddToQueue(song, 0);
                CurrentSongIndex = 0;
                ResetCurrentTrack();
            };
            songContentVm.OnEditSongMethod = () => ShowSongEditor(null, song);

            if(tags.Pictures.Count() > 0)
            {
                songContentVm.ImageVm.ImageData = tags.Pictures[0].Data.Data;
            }

            Vm.SongContentDisplayVm = songContentVm;
        }

        private void UpdateDisplaySelectionWithSongDetails(SongContext context = null)
        {
            albumSelection.Visibility = Visibility.Collapsed;
            artistSelection.Visibility = Visibility.Collapsed;
            songSelection.Visibility = Visibility.Collapsed;
            songDetailsSelection.Visibility = Visibility.Visible;
            playlistSelection.Visibility = Visibility.Collapsed;

            ClearAllDisplaySelections();
            Vm.AlbumContentDisplayVm = new AlbumContentDisplayViewModel();
            Vm.ArtistContentDisplayVm = new ArtistContentDisplayViewModel();
            Vm.SongContentDisplayVm = new SongContentDisplayViewModel();
            Vm.PlaylistContentDisplayVm = new PlaylistCustomContentDisplayViewModel();
            LoadInSongs(context);

            var songDetailsVm = Vm.SongDetailsContentDisplayVm;
            songDetailsVm.PlaylistNames.Clear();
            songDetailsVm.PlaylistNames.AddRange(Playlists.Select(playlist => playlist.Name).Distinct().Order());
            songDetailsVm.SongDetailsToggleVm = Vm.SongDetailsToggleVm;

            // Load in Songs
            songDetailsVm.Songs.Clear();
            SongFiles.ForEach(song => songDetailsVm.Songs.Add(new SongInfo(song)));

            HideLoadingDisplay();
        }

        private void UpdateDisplaySelectionWithPlaylist(SongContext context = null)
        {
            albumSelection.Visibility = Visibility.Collapsed;
            artistSelection.Visibility = Visibility.Collapsed;
            songSelection.Visibility = Visibility.Collapsed;
            songDetailsSelection.Visibility = Visibility.Collapsed;
            playlistSelection.Visibility = Visibility.Visible;

            ClearAllDisplaySelections();
            Vm.AlbumContentDisplayVm = new AlbumContentDisplayViewModel();
            Vm.ArtistContentDisplayVm = new ArtistContentDisplayViewModel();
            Vm.SongContentDisplayVm = new SongContentDisplayViewModel();
            LoadInSongs(context);

            Thread makePlaylists = new Thread(() =>
            {
                int index = -1;
                int selectedIndex = -1;
                var playlistsToAdd = new List<SelectionRowPlaylistViewModel>();

                // Make the By Artist Playlists
                SelectionRowPlaylistViewModel artistPlaylist = new SelectionRowPlaylistViewModel();
                artistPlaylist.DeleteBtnVisibility = Visibility.Collapsed;
                artistPlaylist.PlaylistDisplayType = PlaylistType.Artists;
                artistPlaylist.PlaylistName = "Artists";
                artistPlaylist.PlaylistID = GeneratedPlaylistID;
                playlistsToAdd.Add(artistPlaylist);
                index++;

                if(Vm.PlaylistContentDisplayVm.PlaylistName == "Artists" && selectedIndex == -1)
                {
                    selectedIndex = index;
                }

                // Make the By Genre Playlists
                SelectionRowPlaylistViewModel genrePlaylist = new SelectionRowPlaylistViewModel();
                genrePlaylist.DeleteBtnVisibility = Visibility.Collapsed;
                genrePlaylist.PlaylistDisplayType = PlaylistType.Genres;
                genrePlaylist.PlaylistName = "Genres";
                genrePlaylist.PlaylistID = GeneratedPlaylistID;
                playlistsToAdd.Add(genrePlaylist);
                index++;

                if(Vm.PlaylistContentDisplayVm.PlaylistName == "Genres" && selectedIndex == -1)
                {
                    selectedIndex = index;
                }

                // Make the Recently Played Playlist
                SelectionRowPlaylistViewModel recentPlaylist = new SelectionRowPlaylistViewModel();
                recentPlaylist.DeleteBtnVisibility = Visibility.Collapsed;
                recentPlaylist.PlaylistDisplayType = PlaylistType.Recent;
                recentPlaylist.PlaylistName = "Recently Played";
                recentPlaylist.PlaylistID = GeneratedPlaylistID;
                playlistsToAdd.Add(recentPlaylist);
                index++;

                if(Vm.PlaylistContentDisplayVm.PlaylistName == "Recently Played" && selectedIndex == -1)
                {
                    selectedIndex = index;
                }

                // Show Custom Playlists
                foreach(var toAdd in Playlists.OrderBy(playlist => playlist.Name))
                {
                    var playlistRowVm = new SelectionRowPlaylistViewModel();
                    playlistRowVm.PlaylistName = toAdd.Name;
                    playlistRowVm.PlaylistID = toAdd.ID;

                    int songCount = PlaylistSongEntries.Count(song => song.PlaylistID == toAdd.ID);
                    playlistRowVm.SongCount = $"{songCount} Track{(songCount > 1 ? "s" : "")}";
                    index++;

                    if(Vm.PlaylistContentDisplayVm.PlaylistName == playlistRowVm.PlaylistName && selectedIndex == -1)
                    {
                        selectedIndex = index;
                    }

                    playlistsToAdd.Add(playlistRowVm);
                }

                Dispatcher.BeginInvoke(() =>
                {
                    foreach(var playlistRowVm in playlistsToAdd)
                    {
                        Vm.PlaylistSelectionVm.Items.Add(playlistRowVm);
                    }

                    if(selectedIndex == -1)
                    {
                        selectedIndex = 0;
                    }

                    Vm.PlaylistSelectionVm.SelectedIndex = selectedIndex;
                    Vm.PlaylistSelectionVm.Filter();
                    HideLoadingDisplay();
                });
            });

            makePlaylists.Start();
        }

        private void OnPlaylistNameChanged(object sender, RoutedEventArgs e)
        {
            UpdateDisplaySelectionCurrentPlaylist();
        }

        private void UpdateDisplaySelectionCurrentPlaylist()
        {
            var currPlaylist = Playlists.FirstOrDefault(playlist => playlist.ID == Vm.PlaylistContentDisplayVm.PlaylistID);

            // Should only be null for Auto Playlists
            if(currPlaylist != null)
            {
                var rows = Vm.PlaylistSelectionVm.Items.Select(row => row as SelectionRowPlaylistViewModel).ToList();
                var currPlaylistSelectionRow = rows.FirstOrDefault(row => row.PlaylistID == currPlaylist.ID);

                currPlaylistSelectionRow.PlaylistName = Vm.PlaylistContentDisplayVm.PlaylistName;

                int songCount = PlaylistSongEntries.Count(song => song.PlaylistID == currPlaylist.ID);
                currPlaylistSelectionRow.SongCount = $"{songCount} Track{(songCount > 1 ? "s" : "")}";

                if(currPlaylist.Name != currPlaylistSelectionRow.PlaylistName)
                {
                    SongContext context = GenerateSongContext();

                    currPlaylist.Name = currPlaylistSelectionRow.PlaylistName;
                    context.Playlists.Update(currPlaylist);
                    context.SaveChanges();
                }
            }
        }

        private void OnPlaylistSelectionSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Vm.PlaylistSelectionVm.SelectedIndex >= Vm.PlaylistSelectionVm.ItemsFiltered.Count || Vm.PlaylistSelectionVm.SelectedIndex < 0)
            {
                return;
            }

            var selectedPlaylist = Vm.PlaylistSelectionVm.ItemsFiltered[Vm.PlaylistSelectionVm.SelectedIndex] as SelectionRowPlaylistViewModel;

            if(selectedPlaylist != null)
            {
                LoadPlaylistContentDisplay(selectedPlaylist);
            }
        }

        private void LoadPlaylistContentDisplay(SelectionRowPlaylistViewModel playlist)
        {
            switch(playlist.PlaylistDisplayType)
            {
                case PlaylistType.Custom:
                    PlaylistCustomContentDisplayViewModel playlistCustomContentVm = new PlaylistCustomContentDisplayViewModel();
                    playlistCustomContentVm.PlaylistName = playlist.PlaylistName;

                    var songEntries = PlaylistSongEntries.Where(songEntry => songEntry.PlaylistID == playlist.PlaylistID).OrderBy(songEntry => songEntry.Index);
                    foreach(var songEntry in songEntries)
                    {
                        var song = SongFiles.FirstOrDefault(song => songEntry.SongPath == song.FilePath);
                        if(song != null)
                        {
                            var tagFile = song.TagLibFile;
                            PlaylistSongRowViewModel songVm = new PlaylistSongRowViewModel();
                            songVm.SongName = tagFile.Tag.Title;
                            songVm.Artist = tagFile.Tag.FirstPerformer;
                            songVm.Length = song.Duration.ToString(@"hh\:mm\:ss");
                            songVm.Index = songEntry.Index;

                            var imageVm = new AspectRatioImageViewModel();
                            if(tagFile.Tag.Pictures.Count() > 0)
                            {
                                imageVm.ImageData = tagFile.Tag.Pictures[0].Data.Data;
                            }
                            songVm.ImageVm = imageVm;
                            songVm.SongEntry = songEntry;

                            playlistCustomContentVm.PlaylistDisplayVm.Songs.Add(songVm);
                        }
                    }

                    playlistCustomContentVm.PlaylistID = playlist.PlaylistID;
                    playlistCustomContentVm.PlaylistDisplayVm.ButtonVisibility = Visibility.Visible;
                    playlistCustomContentVm.PlaylistDisplayVm.AddButtonVisibility = Visibility.Visible;
                    Vm.PlaylistContentDisplayVm = playlistCustomContentVm;
                    break;

                case PlaylistType.Artists:
                    PlaylistArtistContentDisplayViewModel playlistArtistContentVm = new PlaylistArtistContentDisplayViewModel();
                    playlistArtistContentVm.PlaylistName = playlist.PlaylistName;
                    playlistArtistContentVm.Artists.AddRange(SongFiles.Select(song => song.TagLibFile.Tag.FirstPerformer).Where(artist => !String.IsNullOrEmpty(artist)).Distinct().Order());
                    playlistArtistContentVm.PlaylistDisplayVm.ButtonVisibility = Visibility.Collapsed;
                    Vm.PlaylistContentDisplayVm = playlistArtistContentVm;
                    break;

                case PlaylistType.Genres:
                    PlaylistGenreContentDisplayViewModel playlistGenreContentVm = new PlaylistGenreContentDisplayViewModel();
                    playlistGenreContentVm.PlaylistName = playlist.PlaylistName;
                    playlistGenreContentVm.Genres.AddRange(SongFiles.SelectMany(song => song.TagLibFile.Tag.Genres).Where(genre => !String.IsNullOrEmpty(genre)).Distinct().Order());
                    playlistGenreContentVm.PlaylistDisplayVm.ButtonVisibility = Visibility.Collapsed;
                    Vm.PlaylistContentDisplayVm = playlistGenreContentVm;
                    break;

                case PlaylistType.Recent:
                    PlaylistRecentContentDisplayViewModel playlistRecentContentVm = new PlaylistRecentContentDisplayViewModel();
                    playlistRecentContentVm.PlaylistName = playlist.PlaylistName;
                    playlistRecentContentVm.PlaylistDisplayVm.ButtonVisibility = Visibility.Visible;
                    playlistRecentContentVm.PlaylistDisplayVm.AddButtonVisibility = Visibility.Collapsed;

                    var context = GenerateSongContext();
                    foreach(var record in context.HistoryRecords.OrderBy(record => record.Order))
                    {
                        var song = SongFiles.FirstOrDefault(song => song.FilePath == record.FilePath);
                        var tagFile = song.TagLibFile;
                        PlaylistSongRowViewModel songVm = new PlaylistSongRowViewModel();
                        songVm.SongName = tagFile.Tag.Title;
                        songVm.Artist = tagFile.Tag.FirstPerformer;
                        songVm.Length = song.Duration.ToString(@"hh\:mm\:ss");

                        var imageVm = new AspectRatioImageViewModel();
                        if(tagFile.Tag.Pictures.Count() > 0)
                        {
                            imageVm.ImageData = tagFile.Tag.Pictures[0].Data.Data;
                        }
                        songVm.ImageVm = imageVm;
                        songVm.SongEntry = new PlaylistSongEntry() { SongPath = song.FilePath };

                        playlistRecentContentVm.PlaylistDisplayVm.Songs.Add(songVm);
                    }

                    Vm.PlaylistContentDisplayVm = playlistRecentContentVm;
                    break;
            }
        }

        private void OnPlaylistArtistSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ComboBox artistComboBox && Vm.PlaylistContentDisplayVm is PlaylistArtistContentDisplayViewModel artistPlaylistVm)
            {
                var artistName = artistComboBox.SelectedItem as string;
                if(artistName != null)
                {
                    var songs = SongFiles.Where(song => song.TagLibFile.Tag.FirstPerformer == artistName);
                    artistPlaylistVm.PlaylistDisplayVm.Songs.Clear();

                    foreach(var song in songs)
                    {
                        var tagFile = song.TagLibFile;
                        PlaylistSongRowViewModel songVm = new PlaylistSongRowViewModel();
                        songVm.SongName = tagFile.Tag.Title;
                        songVm.Artist = tagFile.Tag.FirstPerformer;
                        songVm.Length = song.Duration.ToString(@"hh\:mm\:ss");

                        var imageVm = new AspectRatioImageViewModel();
                        if(tagFile.Tag.Pictures.Count() > 0)
                        {
                            imageVm.ImageData = tagFile.Tag.Pictures[0].Data.Data;
                        }
                        songVm.ImageVm = imageVm;
                        songVm.SongEntry = new PlaylistSongEntry() { SongPath = song.FilePath };

                        artistPlaylistVm.PlaylistDisplayVm.Songs.Add(songVm);
                    }

                    Vm.PlaylistContentDisplayVm.PlaylistDisplayVm.ButtonVisibility = Visibility.Visible;
                    Vm.PlaylistContentDisplayVm.PlaylistDisplayVm.AddButtonVisibility = Visibility.Collapsed;
                }
            }
        }

        private void OnPlaylistGenreSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ComboBox genreComboBox && Vm.PlaylistContentDisplayVm is PlaylistGenreContentDisplayViewModel genrePlaylistVm)
            {
                var genreName = genreComboBox.SelectedItem as string;
                if(genreName != null)
                {
                    var songs = SongFiles.Where(song => song.TagLibFile.Tag.Genres.Contains(genreName));
                    genrePlaylistVm.PlaylistDisplayVm.Songs.Clear();

                    foreach(var song in songs)
                    {
                        var tagFile = song.TagLibFile;
                        PlaylistSongRowViewModel songVm = new PlaylistSongRowViewModel();
                        songVm.SongName = tagFile.Tag.Title;
                        songVm.Artist = tagFile.Tag.FirstPerformer;
                        songVm.Length = song.Duration.ToString(@"hh\:mm\:ss");

                        var imageVm = new AspectRatioImageViewModel();
                        if(tagFile.Tag.Pictures.Count() > 0)
                        {
                            imageVm.ImageData = tagFile.Tag.Pictures[0].Data.Data;
                        }
                        songVm.ImageVm = imageVm;
                        songVm.SongEntry = new PlaylistSongEntry() { SongPath = song.FilePath };

                        genrePlaylistVm.PlaylistDisplayVm.Songs.Add(songVm);
                    }

                    Vm.PlaylistContentDisplayVm.PlaylistDisplayVm.ButtonVisibility = Visibility.Visible;
                    Vm.PlaylistContentDisplayVm.PlaylistDisplayVm.AddButtonVisibility = Visibility.Collapsed;
                }
            }
        }

        private void OnPlaylistPlayBtnClick(object sender, RoutedEventArgs e)
        {
            foreach(var songFile in Vm.PlaylistContentDisplayVm.PlaylistDisplayVm.Songs
                .OrderBy(songRow => songRow.Index)
                .Select(songRow => SongFiles.FirstOrDefault(song => song.FilePath == songRow.SongEntry.SongPath)))
            {
                AddToQueue(songFile);
            }

            if(CurrentSongIndex < 0)
            {
                CurrentSongIndex = 0;
                ResetCurrentTrack();
            }
        }

        private void OnPlaylistShuffleBtnClick(object sender, RoutedEventArgs e)
        {
            Random rng = new Random();
            var songFiles = Vm.PlaylistContentDisplayVm.PlaylistDisplayVm.Songs
                .Select(songRow => SongFiles.FirstOrDefault(song => song.FilePath == songRow.SongEntry.SongPath)).ToList();

            int index = songFiles.Count;
            while(index > 1)
            {
                index--;
                int newIndex = rng.Next(index + 1);
                SongFile temp = songFiles[newIndex];
                songFiles[newIndex] = songFiles[index];
                songFiles[index] = temp;
            }

            foreach(var songFile in songFiles)
            {
                AddToQueue(songFile);
            }

            if(CurrentSongIndex < 0)
            {
                CurrentSongIndex = 0;
                ResetCurrentTrack();
            }
        }

        private void OnPlaylistAddBtnClick(object sender, RoutedEventArgs e)
        {
            var playlistAddVm = new PlaylistAddSongWindowViewModel();
            foreach(var song in SongFiles.OrderBy(song => song.TagLibFile.Tag.Title))
            {
                playlistAddVm.Songs.Add(new PlaylistSongAddRowViewModel(song));
            }
            playlistAddVm.Filter();

            var playlistAddWindow = new PlaylistAddSongWindow(Vm.Settings.ThemeBase, Vm.Settings.ThemeColor);
            playlistAddWindow.DataContext = playlistAddVm;
            playlistAddWindow.Show();

            playlistAddWindow.Closing += (sender, e) =>
            {
                if(!playlistAddVm.Canceled)
                {
                    SongContext context = GenerateSongContext();

                    foreach(var songToAdd in playlistAddVm.Songs.Where(song => song.IsSelected))
                    {
                        PlaylistSongEntry entry = new PlaylistSongEntry();
                        entry.SongPath = songToAdd.Song.FilePath;
                        entry.PlaylistID = Vm.PlaylistContentDisplayVm.PlaylistID;
                        entry.Index = PlaylistSongEntries.Count(existing => existing.PlaylistID == entry.PlaylistID) + 1;
                        entry.ID = PlaylistSongEntries.Count == 0 ? 0 : PlaylistSongEntries.Max(entry => entry.ID) + 1;

                        PlaylistSongEntries.Add(entry);
                        context.PlaylistSongEntries.Add(entry);
                    }

                    context.SaveChanges();
                    UpdateDisplaySelectionCurrentPlaylist();
                    OnPlaylistSelectionSelectedChanged(null, null);
                }
            };
        }

        private void OnPlaylistOrderChanged(object sender, RoutedEventArgs e)
        {
            if(Vm.PlaylistContentDisplayVm.Type == PlaylistType.Custom)
            {
                List<PlaylistSongEntry> entries = new List<PlaylistSongEntry>();
                foreach(var songRow in Vm.PlaylistContentDisplayVm.PlaylistDisplayVm.Songs)
                {
                    var songEntry = songRow.SongEntry;
                    if(songEntry != null)
                    {
                        songEntry.Index = songRow.Index;
                        entries.Add(songEntry);
                    }
                }

                SongContext context = GenerateSongContext();

                context.PlaylistSongEntries.UpdateRange(entries);
                context.SaveChanges();
            }
        }

        private void OnPlaylistSongRemoved(object sender, RoutedEventArgs e)
        {
            if(Vm.PlaylistContentDisplayVm.Type == PlaylistType.Custom)
            {
                var toRemove = PlaylistSongEntries.FirstOrDefault(entry => entry.PlaylistID == Vm.PlaylistContentDisplayVm.PlaylistID &&
                    Vm.PlaylistContentDisplayVm.PlaylistDisplayVm.Songs.All(songRow => songRow.SongEntry.ID != entry.ID));

                if(toRemove != null)
                {
                    // Reorder existing songs
                    List<PlaylistSongEntry> entries = Vm.PlaylistContentDisplayVm.PlaylistDisplayVm.Songs.Select(songRow => songRow.SongEntry).ToList();

                    // Update Database
                    SongContext context = GenerateSongContext();

                    context.PlaylistSongEntries.UpdateRange(entries);
                    context.PlaylistSongEntries.Remove(toRemove);
                    PlaylistSongEntries.Remove(toRemove);

                    context.SaveChanges();
                    UpdateDisplaySelectionCurrentPlaylist();
                }
            }
        }

        private void OnDeletePlaylistClick(object sender, int e)
        {
            var matchingSongEntries = PlaylistSongEntries.Where(songEntry => songEntry.PlaylistID == e);
            var matchingPlaylist = Playlists.FirstOrDefault(playlist => playlist.ID == e);

            SongContext context = GenerateSongContext();

            context.PlaylistSongEntries.RemoveRange(matchingSongEntries);
            context.Playlists.Remove(matchingPlaylist);
            context.SaveChanges();

            Playlists.Remove(matchingPlaylist);
            PlaylistSongEntries.RemoveAll(songEntry => songEntry.PlaylistID == e);

            UpdateDisplaySelectionWithPlaylist();
        }

        private void ClearAllDisplaySelections()
        {
            Vm.AlbumSelectionVm.ClearItems();
            Vm.AlbumSelectionVm.SelectedIndex = -1;
            Vm.ArtistSelectionVm.ClearItems();
            Vm.ArtistSelectionVm.SelectedIndex = -1;
            Vm.SongSelectionVm.ClearItems();
            Vm.SongSelectionVm.SelectedIndex = -1;
            Vm.PlaylistSelectionVm.ClearItems();
            Vm.PlaylistSelectionVm.SelectedIndex = -1;
        }

        private void SongEditUpdateSongQueueTitle(params SongFile[] songs)
        {
            foreach(var toUpdate in Vm.SongQueue)
            {
                if(songs.Contains(toUpdate.Song))
                {
                    toUpdate.SongName = toUpdate.Song.TagLibFile.Tag.Title;
                    toUpdate.SongLength = $"{toUpdate.Song.Duration.Minutes}:{(toUpdate.Song.Duration.Seconds).ToString().PadLeft(2, '0')}";
                }
            }
        }

        private void SongEditUpdateAlbumName(params SongFile[] songs)
        {
            // If the Album Selection is enabled, then we have to update the Album Name in AlbumContent
            // because that is used to re-select the album in Album Selection
            if(albumSelection.Visibility == Visibility.Visible)
            {
                // But only do this if the song is the first/only song in the album
                if(Vm.AlbumContentDisplayVm.Songs.Count > 0 && songs.Contains(Vm.AlbumContentDisplayVm.Songs[0].Song))
                {
                    Vm.AlbumContentDisplayVm.AlbumName = Vm.AlbumContentDisplayVm.Songs[0].Song.TagLibFile.Tag.Album;
                }
            }
        }

        private void SongEditUpdateArtistName(params SongFile[] songs)
        {
            // If the Artist Selection is enabled, then we have to update the Artist Name in ArtistContent
            // because that is used to re-select the artist in Artist Selection
            if(artistSelection.Visibility == Visibility.Visible)
            {
                // But only do this if there is only one album
                if(Vm.ArtistContentDisplayVm.AlbumDisplays.Count == 1 && songs.Length == 1)
                {
                    Vm.ArtistContentDisplayVm.ArtistName = songs[0].TagLibFile.Tag.FirstPerformer;
                }
            }
        }

        private bool AddToQueue(SongFile song, int index = -1)
        {
            var songQueueVm = new SongQueueRowViewModel();
            songQueueVm.Song = song;
            songQueueVm.SongName = song.TagLibFile.Tag.Title;
            songQueueVm.SongLength = $"{song.Duration.Minutes}:{(song.Duration.Seconds).ToString().PadLeft(2, '0')}";
            songQueueVm.RowIndex = Vm.SongQueue.Count.ToString();

            songQueueVm.OnUpArrowClickMethod = () =>
            {
                int oldIndex = Vm.SongQueue.IndexOf(songQueueVm);
                int newIndex = oldIndex - 1;
                int indexLimit = -1;
                if(ReorderSongQueue(song, songQueueVm, newIndex, indexLimit))
                {
                    // If moving the current song
                    if(oldIndex == CurrentSongIndex)
                    {
                        CurrentSongIndex = newIndex;
                    }

                    // If current song was the one above this one (and is now below it)
                    else if(oldIndex - 1 == CurrentSongIndex)
                    {
                        CurrentSongIndex = oldIndex;
                    }
                }
            };
            songQueueVm.OnDownArrowClickMethod = () =>
            {
                int oldIndex = SongQueue.IndexOf(song);
                int newIndex = oldIndex + 1;
                int indexLimit = SongQueue.Count;
                if(ReorderSongQueue(song, songQueueVm, newIndex, indexLimit))
                {
                    // If moving the current song
                    if(oldIndex == CurrentSongIndex)
                    {
                        CurrentSongIndex = newIndex;
                    }

                    // If current song was the one below this one (and is now above it)
                    else if(oldIndex + 1 == CurrentSongIndex)
                    {
                        CurrentSongIndex = oldIndex;
                    }
                }
            };
            songQueueVm.OnRemoveClickMethod = () =>
            {
                // If the current song index is after this one
                if(CurrentSongIndex > SongQueue.IndexOf(song))
                {
                    CurrentSongIndex--;
                }

                // If the current song is the one being removed, set the index to be the previous song
                // If the current index is 0 (aka no previous song), then set it to the next song (aka leave it at 0).
                // If there is no next one, then set it to -1;
                if(CurrentSongIndex == SongQueue.IndexOf(song))
                {
                    if(CurrentSongIndex > 0)
                    {
                        CurrentSongIndex--;
                    }

                    else if(SongQueue.Count - 1 > 0)
                    {
                        CurrentSongIndex = 0;
                    }

                    else
                    {
                        CurrentSongIndex = -1;
                        IsPaused = true;
                        StopCurrentTrack();
                    }
                }

                SongQueue.Remove(song);
                Vm.SongQueue.Remove(songQueueVm);
                SelectFromSongQueue(CurrentSongIndex);
                UpdateSongQueueIndexes();
                ResetCurrentTrack();
            };
            songQueueVm.OnPlayClickMethod = () =>
            {
                var indexToUse = SongQueue.IndexOf(song);
                CurrentSongIndex = indexToUse;
                ResetCurrentTrack();
            };
            songQueueVm.OnDoubleClickMethod = songQueueVm.OnPlayClickMethod;

            if(index < 0)
            {
                Vm.SongQueue.Add(songQueueVm);
                SongQueue.Add(song);
            }

            else
            {
                Vm.SongQueue.Insert(index, songQueueVm);
                SongQueue.Insert(index, song);
                UpdateSongQueueIndexes();
            }

            return true;
        }

        private bool ReorderSongQueue(SongFile song, SongQueueRowViewModel songQueueVm, int newIndex, int indexLimit)
        {
            if(newIndex == indexLimit)
            {
                return false;
            }

            // Reorder Queue
            SongQueue.Remove(song);
            SongQueue.Insert(newIndex, song);

            // Reorder UI
            Vm.SongQueue.Remove(songQueueVm);
            Vm.SongQueue.Insert(newIndex, songQueueVm);

            UpdateSongQueueIndexes();
            return true;
        }

        private void UpdateSongQueueIndexes()
        {
            // Update Row Indexes
            int index = 0;
            foreach(var row in Vm.SongQueue)
            {
                row.RowIndex = index.ToString();
                index++;
            }
        }

        private void SelectFromSongQueue(int indexToSelect)
        {
            for(int i = 0; i < Vm.SongQueue.Count; i++)
            {
                Vm.SongQueue[i].IsSelected = i == indexToSelect;
            }
        }

        private void OnClearQueueClick(object sender, RoutedEventArgs e)
        {
            StopCurrentTrack();
            Vm.SongQueue.Clear();
            SongQueue.Clear();
        }

        private void OnPreviousTrackClick(object sender, MouseButtonEventArgs e)
        {
            PreviousTrack();
        }

        private void OnNextTrackClick(object sender, MouseButtonEventArgs e)
        {
            NextTrack();
        }

        private void OnPauseTrackClick(object sender, MouseButtonEventArgs e)
        {
            PauseCurrentTrack();
        }

        private void OnPlayTrackClick(object sender, MouseButtonEventArgs e)
        {
            if(CurrentSongIndex < 0)
            {
                ResetCurrentTrack();
            }

            else
            {
                PlayCurrentTrack();
            }
        }

        private void PreviousTrack()
        {
            if(SongQueue.Count == 0)
            {
                return;
            }

            if(--CurrentSongIndex < 0)
            {
                CurrentSongIndex = SongQueue.Count - 1;
            }

            ResetCurrentTrack();
        }

        private void NextTrack()
        {
            if(SongQueue.Count == 0)
            {
                return;
            }

            if(++CurrentSongIndex >= SongQueue.Count)
            {
                CurrentSongIndex = 0;
            }

            ResetCurrentTrack();
        }

        private void StopCurrentTrack()
        {
            playTrackBtn.Visibility = Visibility.Visible;
            pauseTrackBtn.Visibility = Visibility.Collapsed;

            if(Player != null)
            {
                Player.Close();
                Player = null;
            }

            IsPaused = true;
            CurrentSongIndex = -1;
            SelectFromSongQueue(-1);

            currSongTitle.Content = "";
            timelineCurrTime.Content = "";
            timelineTotalTime.Content = "";
            timeline.Visibility = Visibility.Hidden;
        }

        private void PauseCurrentTrack()
        {
            playTrackBtn.Visibility = Visibility.Visible;
            pauseTrackBtn.Visibility = Visibility.Collapsed;

            if(Player != null)
            {
                Player.Pause();
            }

            IsPaused = true;
        }

        private void PlayCurrentTrack()
        {
            // Toggle Buttons
            playTrackBtn.Visibility = Visibility.Collapsed;
            pauseTrackBtn.Visibility = Visibility.Visible;

            // Update Timeline Display
            currSongTitle.Content = SongQueue[CurrentSongIndex].TagLibFile.Tag.Title;
            timelineTotalTime.Content = SongQueue[CurrentSongIndex].Duration.ToString(@"hh\:mm\:ss");
            IsPaused = false;
            timeline.Visibility = Visibility.Visible;

            Player.Play();
        }

        private void ResetCurrentTrack()
        {
            ElapsedIntervals = 0;

            if(Player is null)
            {
                Player = new MediaPlayer();
                Player.MediaEnded += (sender, e) => NextTrack();
                Player.Volume = Volume;
            }

            if(CurrentSongIndex > -1)
            {
                Player.Open(new Uri(SongQueue[CurrentSongIndex].FilePath));
                AddSongToHistory(SongQueue[CurrentSongIndex]);
                PlayerTimer.Start();
                PlayCurrentTrack();
            }

            else
            {
                IsPaused = true;
            }

            SelectFromSongQueue(CurrentSongIndex);
        }

        private void AddSongToHistory(SongFile songFile)
        {
            var context = GenerateSongContext();
            var historyRecords = context.HistoryRecords.OrderBy(record => record.Order).ToList();

            historyRecords.RemoveAll(record => record.FilePath == songFile.FilePath);
            historyRecords.Insert(0, new HistoryRecord() { Order = 0, FilePath = songFile.FilePath });

            if(historyRecords.Count > Vm.Settings.MaxHistoryRecords)
            {
                historyRecords = historyRecords.GetRange(0, Vm.Settings.MaxHistoryRecords);
            }

            int index = 0;
            historyRecords.ForEach(record => record.Order = index++);

            // Have to Detach or else EF thinks the records still exist after deleting
            foreach(var entity in context.ChangeTracker.Entries<HistoryRecord>())
            {
                entity.State = EntityState.Detached;
            }

            if(context.HistoryRecords.Count() > 0)
            {
                context.Database.ExecuteSqlRaw($"DELETE FROM {nameof(context.HistoryRecords)}");
            }

            context.HistoryRecords.AddRange(historyRecords);
            context.SaveChanges();
        }

        private void VolumeToggle(object sender, MouseButtonEventArgs e)
        {
            if(Volume > 0)
            {
                StoredVolume = Volume;
                volumeSlider.Value = 0;
            }

            else
            {
                volumeSlider.Value = StoredVolume;
            }
        }

        private void OnVolumeValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Volume = volumeSlider.Value;

            if(Volume > 0.5)
            {
                volumeBtnIcon.Kind = MahApps.Metro.IconPacks.PackIconBoxIconsKind.SolidVolumeFull;
            }

            else if(Volume > 0)
            {
                volumeBtnIcon.Kind = MahApps.Metro.IconPacks.PackIconBoxIconsKind.SolidVolumeLow;
            }

            else
            {
                volumeBtnIcon.Kind = MahApps.Metro.IconPacks.PackIconBoxIconsKind.SolidVolumeMute;
            }

            if(Player != null)
            {
                Player.Volume = Volume;
            }
        }

        private void TimelineProgression(object source, ElapsedEventArgs e)
        {
            if(!IsPaused)
            {
                ElapsedIntervals++;
                var currTime = new TimeSpan(0, 0, 0, 0, (int) ElapsedIntervals * PlayerTimerMsPerInterval);
                var barPercentage = currTime.TotalMilliseconds / (double) SongQueue[CurrentSongIndex].Duration.TotalMilliseconds;
                Dispatcher.Invoke((Action) (() =>
                {
                    if(!IsPaused)
                    {
                        timeline.Value = barPercentage * timeline.Maximum;
                        timelineCurrTime.Content = currTime.ToString(@"hh\:mm\:ss");
                    }
                }));
            }
        }

        private void OnTimelineClick(MouseEventArgs e)
        {
            if(CurrentSongIndex < 0 || SongQueue.Count == 0 || CurrentSongIndex >= SongQueue.Count)
            {
                return;
            }

            var clickPoint = e.GetPosition(timeline);
            var percentage = clickPoint.X / timeline.ActualWidth;
            timeline.Value = percentage * timeline.Maximum;

            var newTimelinePosition = new TimeSpan(0, 0, 0, 0, (int) (percentage * SongQueue[CurrentSongIndex].Duration.TotalMilliseconds));
            Player.Position = newTimelinePosition;
            timelineCurrTime.Content = newTimelinePosition.ToString(@"hh\:mm\:ss");
            ElapsedIntervals = (long) (newTimelinePosition.TotalMilliseconds / PlayerTimerMsPerInterval);
        }

        private void OnTimelinePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDragging = true;
        }

        private void OnTimelinePreviewMouseMove(object sender, MouseEventArgs e)
        {
            if(IsMouseDragging)
            {
                OnTimelineClick(e);
            }
        }

        private void OnTimelineMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(IsMouseDragging)
            {
                OnTimelineClick(e);
                IsMouseDragging = false;
            }
        }

        private void OnAddSongsClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Audio Files|*.mp3;*.m4a;*.wav|All Files|*.*"
            };

            if(openFileDialog.ShowDialog() == true)
            {
                AddSongs(openFileDialog.FileNames);
            }
        }

        private void OnAddSongContextMenu(object sender, RoutedEventArgs e)
        {
            OnAddSongsClick(null, null);
        }

        private void OnAddFolderSongsClick(object sender, RoutedEventArgs e)
        {
            var selectFolderDialog = new VistaFolderBrowserDialog();
            selectFolderDialog.Multiselect = true;
            selectFolderDialog.Description = "Please select a folder";
            selectFolderDialog.UseDescriptionForTitle = true;

            if(selectFolderDialog.ShowDialog(this) == true)
            {
                var dirPathsToSearch = selectFolderDialog.SelectedPaths;

                List<string> songFilePathsToCheck = new List<string>();
                foreach(var dirPath in dirPathsToSearch)
                {
                    RecursiveSearchForSongs(dirPath, songFilePathsToCheck);
                }

                AddSongs(songFilePathsToCheck.ToArray());
            }
        }

        private void OnAddFolderSongContextMenu(object sender, RoutedEventArgs e)
        {
            OnAddFolderSongsClick(null, null);
        }

        private void OnDropOnWindow(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] droppedFiles = (string[]) e.Data.GetData(DataFormats.FileDrop);
                
                List<string> songFiles = new List<string>();
                foreach(var droppedFile in droppedFiles)
                {
                    if(Directory.Exists(droppedFile))
                    {
                        RecursiveSearchForSongs(droppedFile, songFiles);
                    }

                    else if(droppedFile.EndsWith(".mp3") || droppedFile.EndsWith(".m4a") || droppedFile.EndsWith(".wav"))
                    {
                        songFiles.Add(droppedFile);
                    }
                }

                AddSongs(songFiles.ToArray());
            }
        }

        private List<string> RecursiveSearchForSongs(string dirPath, List<string> filePaths = null)
        {
            if(filePaths is null)
            {
                filePaths = new List<string>();
            }

            if(Directory.Exists(dirPath))
            {
                filePaths.AddRange(Directory.GetFiles(dirPath)
                    .Where(path => path.EndsWith(".mp3") || path.EndsWith(".m4a") || path.EndsWith(".wav")));

                foreach(var dirPathToSearch in Directory.GetDirectories(dirPath))
                {
                    RecursiveSearchForSongs(dirPathToSearch, filePaths);
                }
            }

            return filePaths;
        }

        private void OnSettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsPopup = new SettingsWindow(Vm.Settings.ThemeBase, Vm.Settings.ThemeColor);
            var settingsVm = new SettingsWindowViewModel(Vm.Settings);
            SettingsPopup.DataContext = settingsVm;
            SettingsPopup.Closing += (sender, e) => ThemeManager.Current.ChangeTheme(this, $"{Vm.Settings.ThemeBase}.{Vm.Settings.ThemeColor}"); ;
            SettingsPopup.Show();
        }

        private void OnNewPlaylistClicked(object sender, RoutedEventArgs e)
        {
            var playlistPopUpVm = new PlaylistNewWindowViewModel();
            var playlistPopUp = new PlaylistNewWindow(Vm.Settings.ThemeBase, Vm.Settings.ThemeColor);
            playlistPopUp.DataContext = playlistPopUpVm;
            playlistPopUp.Closing += (sender, e) =>
            {
                if(!playlistPopUpVm.Canceled)
                {
                    SongContext context = GenerateSongContext();

                    Playlist toAdd = new Playlist();
                    toAdd.Name = playlistPopUpVm.PlaylistName;
                    toAdd.ID = context.Playlists.Count() == 0 ? 0 : context.Playlists.Max(playlist => playlist.ID) + 1;

                    context.Playlists.Add(toAdd);
                    Playlists.Add(toAdd);

                    context.SaveChanges();
                    UpdateDisplaySelectionWithPlaylist();
                }
            };

            playlistPopUp.Show();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // Ctrl + O
            if(Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O)
            {
                OnAddSongsClick(null, null);
                e.Handled = true;
            }

            // Ctrl + Shift + O
            else if(Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.O)
            {
                OnAddFolderSongsClick(null, null);
                e.Handled = true;
            }

            else if(Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Right && CurrentSongIndex >= 0)
            {
                NextTrack();
            }

            else if(e.Key == Key.Right && CurrentSongIndex >= 0)
            {
                SkipTime(true, 5);
            }

            else if(Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Left && CurrentSongIndex >= 0)
            {
                PreviousTrack();
            }

            else if(e.Key == Key.Left && CurrentSongIndex >= 0)
            {
                SkipTime(false, 5);
            }

            else if(e.Key == Key.Space && !IsPaused && CurrentSongIndex >= 0)
            {
                PauseCurrentTrack();
            }

            else if(e.Key == Key.Space && IsPaused && CurrentSongIndex >= 0)
            {
                PlayCurrentTrack();
            }

            else
            {
                base.OnPreviewKeyDown(e);
            }
        }

        private void SkipTime(bool isAdd, int seconds)
        {
            var currPos = Player.Position;
            var newPos = isAdd ? currPos.Add(new TimeSpan(0, 0, seconds)) : currPos.Subtract(new TimeSpan(0, 0, seconds));
            var totalDur = SongQueue[CurrentSongIndex].Duration.TotalMilliseconds;

            if(newPos.TotalMilliseconds >= totalDur)
            {
                NextTrack();
                return;
            }

            else if(newPos.TotalMilliseconds <= 0)
            {
                PreviousTrack();
                return;
            }

            var percentage = newPos.TotalMilliseconds / totalDur;
            timeline.Value = percentage * timeline.Maximum;

            Player.Position = newPos;
            timelineCurrTime.Content = newPos.ToString(@"hh\:mm\:ss");
            ElapsedIntervals = (long) (newPos.TotalMilliseconds / PlayerTimerMsPerInterval);
        }

        private void OnAlbumRefreshRequested(object sender, string e)
        {
            Vm.AlbumSelectionVm.Filter();
        }

        private void OnArtistRefreshRequested(object sender, string e)
        {
            Vm.ArtistSelectionVm.Filter();
        }

        private void OnSongRefreshRequested(object sender, string e)
        {
            Vm.SongSelectionVm.Filter();
        }

        private void OnPlaylistRefreshRequested(object sender, string e)
        {
            Vm.PlaylistSelectionVm.Filter();
        }

        private void OnAddToPlaylistClick(object sender, AddToPlaylistEventArgs e)
        {
            var context = GenerateSongContext();
            
            foreach(var filePathToAdd in e.FilePaths)
            {
                PlaylistSongEntry newEntry = new PlaylistSongEntry();
                newEntry.SongPath = filePathToAdd;

                var playlistToAddTo = Playlists.FirstOrDefault(playlist => playlist.Name == e.PlaylistName);
                if(playlistToAddTo != null)
                {
                    newEntry.PlaylistID = playlistToAddTo.ID;
                }

                newEntry.Index = PlaylistSongEntries.Count(existing => existing.PlaylistID == newEntry.PlaylistID) + 1;
                newEntry.ID = PlaylistSongEntries.Count == 0 ? 0 : PlaylistSongEntries.Max(entry => entry.ID) + 1;

                PlaylistSongEntries.Add(newEntry);
                context.PlaylistSongEntries.Add(newEntry);
                context.SaveChanges();
            }
        }

        private void OnCopyClick(object sender, SongCopyPasteEventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetData("SongFilePath", e.FilePath);
        }

        private void OnPasteClick(object sender, SongCopyPasteEventArgs e)
        {
            string filePathToCopyFrom = "";
            if(Clipboard.ContainsData("SongFilePath"))
            {
                filePathToCopyFrom = (string) Clipboard.GetData("SongFilePath");

                var songToPasteTo = SongFiles.FirstOrDefault(song => song.FilePath == e.FilePath);
                var songToCopyFrom = SongFiles.FirstOrDefault(song => song.FilePath == filePathToCopyFrom);
                if(songToCopyFrom != null && songToPasteTo != null && songToCopyFrom != songToPasteTo)
                {
                    SongInfo copyFrom = new SongInfo(songToCopyFrom);
                    SongInfo pasteTo = new SongInfo(songToPasteTo);

                    copyFrom.FilePath = pasteTo.FilePath;
                    copyFrom.SongLengthMilliseconds = pasteTo.SongLengthMilliseconds;
                    copyFrom.SongLength = pasteTo.SongLength;

                    ShowSongEditor(copyFrom, songToPasteTo);
                }
            }
        }

        private void OnImportClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Csv Files|*.csv"
            };

            if(openFileDialog.ShowDialog() == true)
            {
                ShowLoadingDisplay();

                Thread importingThread = new Thread(() =>
                {
                    string importPath = openFileDialog.FileName;
                    ImportParserService.Parse(GenerateSongContext(), importPath);
                    SongFiles.Clear();

                    Dispatcher.BeginInvoke(() =>
                    {
                        OnDisplaySelectionDropdownChange(null, null);
                        Vm.NotificationDisplayVm.DisplayNotification("File Imported!", Brushes.Green, 1, 5);
                    });
                });

                importingThread.Start();
            }
        }

        private void OnExportClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = ".csv";
            saveDialog.FileName = $"TempoHub Data Export {DateTime.Now.ToString("M-d-yyyy")}.csv";
            saveDialog.Title = "Export TempoHub Data";
            saveDialog.Filter = "Csv|*.csv";

            if(saveDialog.ShowDialog(this) == true)
            {
                ShowLoadingDisplay();
                var savePath = saveDialog.FileName;
                List<string> lines = new List<string>();
                lines.Add(String.Join(",", "File Path", "Title", "Title Sort", "Album", "Album Sort", "Artist", "Artist Sort",
                    "Album Artist", "Album Artist Sort", "Genres", "Composer", "Composer Sort", "Publisher", "Conductor", 
                    "Grouping", "Song Length", "Song Length In Milliseconds", "Year", "Track #", "Track Total", "Disc #", "Disc Total", 
                    "Bpm", "Comment", "Lyrics", "Number of Pictures", "Date Added", "Rating"));

                SongFiles.ForEach(songFile => lines.Add(new SongInfo(songFile).AsCsv()));
                File.WriteAllLines(savePath, lines);
                HideLoadingDisplay();
                Vm.NotificationDisplayVm.DisplayNotification("File Exported!", Brushes.Green, 1, 5);
            }
        }
    }
}
