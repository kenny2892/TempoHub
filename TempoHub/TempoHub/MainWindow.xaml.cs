using MahApps.Metro.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TempoHub.Data;
using TempoHub.Models;
using TempoHub.User_Controls;

namespace TempoHub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool IsMouseDragging { get; set; } = false;
        private SongEditWindow SongEditPopup { get; set; }
        private List<SongFile> SongFiles { get; set; } = new List<SongFile>();
        private int CurrentSongIndex { get; set; } = -1;
        private List<SongFile> SongQueue { get; set; } = new List<SongFile>();
        private MediaPlayer Player { get; set; } = new MediaPlayer();
        private bool IsPaused { get; set; } = false;
        private Timer PlayerTimer { get; set; } = new Timer();
        private int PlayerTimerMsPerInterval { get; set; } = 100;
        private long ElapsedIntervals { get; set; } = 0;
        private double StoredVolume { get; set; } = -1;

        public MainWindow()
        {
            InitializeComponent();
            SetDefaultStates();
            Closing += (sender, e) => { PlayerTimer.Stop(); };
            UpdateAlbumList();
        }

        private void SetDefaultStates()
        {
            displayAlbumNameLbl.Content = "";
            volumeSlider.Value = 0.5;

            Player.MediaEnded += (sender, e) => NextTrack();
            PlayerTimer.Elapsed += new ElapsedEventHandler(TimelineProgression);
            PlayerTimer.Interval = PlayerTimerMsPerInterval;
            timeline.Visibility = Visibility.Hidden;
            timelineCurrTime.Content = "";
            timelineTotalTime.Content = "";
            currSongTitle.Content = "";
        }

        public void AddSongs(string[] filePaths)
        {
            DbContextOptionsBuilder<SongContext> options = new DbContextOptionsBuilder<SongContext>();
            options.UseSqlite("Data Source=SongData.sqlite");
            SongContext context = new SongContext(options.Options);
            context.Database.EnsureCreated();

            foreach(var musicFilePath in filePaths.Where(pathToAdd => SongFiles.All(existing => existing.FilePath != pathToAdd)))
            {
                try
                {
                    var musicFile = TagLib.File.Create(musicFilePath);
                    SongFiles.Add(new SongFile() { FilePath = musicFilePath, TagLibFile = musicFile });
                    context.SongPaths.Add(new SongPath() { FilePath = musicFilePath });
                }

                catch(Exception)
                {
                    Trace.WriteLine("File was not added: " + musicFilePath);
                }
            }

            context.SaveChanges();
            UpdateAlbumList(context);
        }

        private void UpdateAlbumList(SongContext context = null)
        {
            if(context is null)
            {
                DbContextOptionsBuilder<SongContext> options = new DbContextOptionsBuilder<SongContext>();
                options.UseSqlite("Data Source=SongData.sqlite");
                context = new SongContext(options.Options);
                context.Database.EnsureCreated();

                // Load in all song paths
                foreach(var song in context.SongPaths.ToList().Where(song => SongFiles.All(existing => existing.FilePath != song.FilePath)))
                {
                    try
                    {
                        var musicFile = TagLib.File.Create(song.FilePath);
                        SongFiles.Add(new SongFile() { FilePath = song.FilePath, TagLibFile = musicFile });
                    }

                    catch(Exception)
                    {
                        Trace.WriteLine("Could not parse file: " + song.FilePath);
                    }
                }
            }

            albumRowStackPanel.Children.Clear();

            var albums = SongFiles.GroupBy(song => song.TagLibFile.Tag.Album).OrderBy(group => group.Key);
            foreach(var album in albums)
            {
                var firstSong = album.FirstOrDefault(song => song.TagLibFile.Tag.Track == 1);
                if(firstSong is null)
                {
                    firstSong = album.First();
                }

                var albumRow = new AlbumRow();
                albumRow.Songs.AddRange(album);
                albumRow.UpdateInfo();

                albumRow.OnClickMethod = () => DisplayAlbum(album);
                albumRow.Height = 77;
                albumRowStackPanel.Children.Add(albumRow);
            }
        }

        private void DisplayAlbum(IGrouping<string, SongFile> album)
        {
            songListStackPanel.Children.Clear();
            displayAlbumCoverImg.image.VerticalAlignment = VerticalAlignment.Top;
            displayAlbumNameLbl.Content = album.First().TagLibFile.Tag.Album;

            int rowIndex = 0;
            var orderedSongs = album.OrderBy(song => song.TagLibFile.Tag.Track);
            foreach(var song in orderedSongs)
            {
                SongListRow songListRow = new SongListRow();
                songListRow.Song = song;
                songListRow.UpdateInfo();
                songListRow.RowIndex = rowIndex;
                songListRow.OnDoubleClickMethod = () => { AddToQueue(song); };
                songListRow.OnAddClickMethod = () => { AddToQueue(song); };
                songListRow.OnPlayClickMethod = () => 
                {
                    AddToQueue(song, 0);
                    CurrentSongIndex = 0;
                    ResetCurrentTrack();
                };
                songListRow.OnEditSongInfoClickMethod = () =>
                {
                    SongEditPopup = new SongEditWindow(song);
                    StopCurrentTrack();
                    SongEditPopup.Closing += (sender, e) => 
                    {
                        if(SongEditPopup.ChangesWereApplied)
                        {
                            try
                            {
                                var musicFile = TagLib.File.Create(song.FilePath);
                                song.TagLibFile = musicFile;

                                if(SongEditPopup.TitleChanged)
                                {
                                    SongEditUpdateSongListTitle(song);
                                    SongEditUpdateSongQueueTitle(song);
                                }

                                if(SongEditPopup.AlbumNameChanged)
                                {
                                    SongEditUpdateAlbumName(song);
                                }

                                if(SongEditPopup.AlbumCoverChanged)
                                {
                                    SongEditUpdateAlbumCover(song);
                                }
                            }

                            catch(Exception)
                            {
                                Trace.WriteLine("Could not update Tag File: " + song.FilePath);
                            }
                        }
                    };

                    SongEditPopup.ShowDialog();
                };

                songListStackPanel.Children.Add(songListRow);
                rowIndex++;
            }

            if(orderedSongs.First().TagLibFile.Tag.Pictures.Count() > 0)
            {
                displayAlbumCoverImg.SetImage(orderedSongs.First().TagLibFile.Tag.Pictures[0].Data.Data);
            }

            else
            {
                displayAlbumCoverImg.ClearImage();
            }
        }

        private void SongEditUpdateSongListTitle(SongFile song)
        {
            foreach(var element in songListStackPanel.Children)
            {
                if(element is SongListRow toUpdate && toUpdate.Song == song)
                {
                    toUpdate.UpdateInfo();
                }
            }
        }

        private void SongEditUpdateSongQueueTitle(SongFile song)
        {
            foreach(var element in songQueueStackPanel.Children)
            {
                if(element is SongQueueRow toUpdate && toUpdate.Song == song)
                {
                    toUpdate.UpdateInfo();
                }
            }
        }

        private void SongEditUpdateAlbumName(SongFile song)
        {
            int songCount = 0;
            foreach(var element in songListStackPanel.Children)
            {
                if(element is SongListRow toUpdate)
                {
                    songCount++;
                }
            }

            // If this is the only song in the album, update the display name, the song list entry, and the albums list
            if(songCount == 1)
            {
                displayAlbumNameLbl.Content = song.TagLibFile.Tag.Album;

                foreach(var element in songListStackPanel.Children)
                {
                    if(element is SongListRow toUpdate && toUpdate.Song == song)
                    {
                        toUpdate.UpdateInfo();
                    }
                }
            }

            // Else, remove the song from the list and update the album list
            else
            {
                for(int i = 0; i < songListStackPanel.Children.Count; i++)
                {
                    var element = songListStackPanel.Children[i];
                    if(element is SongListRow toUpdate && toUpdate.Song == song)
                    {
                        songListStackPanel.Children.RemoveAt(i);
                        i--;
                    }

                    UpdateSongListIndexes();
                }
            }

            UpdateAlbumList();
        }

        private void SongEditUpdateAlbumCover(SongFile song)
        {
            // First, check if the song is still present in the Song List
            int indexOfSong = -1;
            for(int i = 0; i < songListStackPanel.Children.Count; i++)
            {
                var element = songListStackPanel.Children[i];
                if(element is SongListRow toCheck && toCheck.Song == song)
                {
                    indexOfSong = i;
                    break;
                }
            }

            // If the song is not the first in the album, or is no longer in the album, exit.
            if(indexOfSong != 0)
            {
                return;
            }

            // Else, update the album cover in the song list
            if(song.TagLibFile.Tag.Pictures.Count() > 0)
            {
                displayAlbumCoverImg.SetImage(song.TagLibFile.Tag.Pictures[0].Data.Data);
            }

            else
            {
                displayAlbumCoverImg.ClearImage();
            }

            // And update the album cover in the album rows
            foreach(var element in albumRowStackPanel.Children)
            {
                if(element is AlbumRow toUpdate && toUpdate.Songs.Any(existing => existing == song))
                {
                    toUpdate.UpdateInfo();
                }
            }
        }

        private void UpdateSongListIndexes()
        {
            // Update Row Indexes
            int index = 0;
            foreach(var row in songListStackPanel.Children)
            {
                if(row is SongListRow toUpdate)
                {
                    toUpdate.RowIndex = index;
                    index++;
                }
            }
        }

        private void AddToQueue(SongFile song, int index = -1)
        {
            SongQueueRow songQueueRow = new SongQueueRow();
            songQueueRow.Song = song;
            songQueueRow.UpdateInfo();
            songQueueRow.SongName = song.TagLibFile.Tag.Title;
            songQueueRow.SongLength = $"{song.TagLibFile.Properties.Duration.Minutes}:{(song.TagLibFile.Properties.Duration.Seconds).ToString().PadLeft(2, '0')}";
            songQueueRow.RowIndex = songQueueStackPanel.Children.Count;
            songQueueRow.OnUpArrowClickMethod = () =>
            {
                int oldIndex = SongQueue.IndexOf(song);
                int newIndex = oldIndex - 1;
                int indexLimit = -1;
                if(ReorderSongQueue(song, songQueueRow, newIndex, indexLimit))
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
            songQueueRow.OnDownArrowClickMethod = () =>
            {
                int oldIndex = SongQueue.IndexOf(song);
                int newIndex = oldIndex + 1;
                int indexLimit = SongQueue.Count;
                if(ReorderSongQueue(song, songQueueRow, newIndex, indexLimit))
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
            songQueueRow.OnRemoveClickMethod = () =>
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
                songQueueStackPanel.Children.Remove(songQueueRow);
                UpdateSongQueueIndexes();
                ResetCurrentTrack();
            };
            songQueueRow.OnPlayClickMethod = () =>
            {
                var indexToUse = SongQueue.IndexOf(song);
                CurrentSongIndex = indexToUse;
                ResetCurrentTrack();
            };
            songQueueRow.OnDoubleClickMethod = songQueueRow.OnPlayClickMethod;

            if(index < 0)
            {
                songQueueStackPanel.Children.Add(songQueueRow);
                SongQueue.Add(song);
            }

            else
            {
                songQueueStackPanel.Children.Insert(index, songQueueRow);
                SongQueue.Insert(index, song);
                UpdateSongQueueIndexes();
            }
        }

        private bool ReorderSongQueue(SongFile song, SongQueueRow songQueueRow, int newIndex, int indexLimit)
        {
            if(newIndex == indexLimit)
            {
                return false;
            }

            // Reorder Queue
            SongQueue.Remove(song);
            SongQueue.Insert(newIndex, song);

            // Reorder UI
            songQueueStackPanel.Children.Remove(songQueueRow);
            songQueueStackPanel.Children.Insert(newIndex, songQueueRow);

            UpdateSongQueueIndexes();
            return true;
        }

        private void UpdateSongQueueIndexes()
        {
            // Update Row Indexes
            int index = 0;
            foreach(var row in songQueueStackPanel.Children)
            {
                var toUpdate = row as SongQueueRow;
                if(toUpdate != null)
                {
                    toUpdate.RowIndex = index;
                }

                index++;
            }
        }

        private void UpdateSongQueueSelection()
        {
            int index = 0;
            foreach(var child in songQueueStackPanel.Children)
            {
                if(child is SongQueueRow rowToDeselect)
                {
                    rowToDeselect.IsSelected = CurrentSongIndex == index++;
                }
            }
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

            CurrentSongIndex = -1;
            UpdateSongQueueSelection();

            currSongTitle.Content = "";
            timelineCurrTime.Content = "";
            timelineTotalTime.Content = "";
            timeline.Visibility = Visibility.Hidden;

            IsPaused = true;
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
            timelineTotalTime.Content = SongQueue[CurrentSongIndex].TagLibFile.Properties.Duration.ToString(@"hh\:mm\:ss");
            IsPaused = false;
            timeline.Visibility = Visibility.Visible;

            UpdateSongQueueSelection();
            Player.Play();
        }

        private void ResetCurrentTrack()
        {
            ElapsedIntervals = 0;

            if(Player is null)
            {
                Player = new MediaPlayer();
                Player.MediaEnded += (sender, e) => NextTrack();
            }

            if(CurrentSongIndex > -1)
            {
                Player.Open(new Uri(SongQueue[CurrentSongIndex].FilePath));
                PlayerTimer.Start();
                PlayCurrentTrack();
            }

            else
            {
                IsPaused = true;
            }
        }

        private void VolumeToggle(object sender, MouseButtonEventArgs e)
        {
            if(Player.Volume > 0)
            {
                StoredVolume = Player.Volume;
                volumeSlider.Value = 0;
            }

            else
            {
                volumeSlider.Value = StoredVolume;
            }
        }

        private void OnVolumeValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.Volume = volumeSlider.Value;

            if(Player.Volume > 0.5)
            {
                volumeBtnIcon.Kind = MahApps.Metro.IconPacks.PackIconBoxIconsKind.SolidVolumeFull;
            }

            else if(Player.Volume > 0)
            {
                volumeBtnIcon.Kind = MahApps.Metro.IconPacks.PackIconBoxIconsKind.SolidVolumeLow;
            }

            else
            {
                volumeBtnIcon.Kind = MahApps.Metro.IconPacks.PackIconBoxIconsKind.SolidVolumeMute;
            }
        }

        private void TimelineProgression(object source, ElapsedEventArgs e)
        {
            if(!IsPaused)
            {
                ElapsedIntervals++;
                var currTime = new TimeSpan(0, 0, 0, 0, (int) ElapsedIntervals * PlayerTimerMsPerInterval);
                var barPercentage = currTime.TotalMilliseconds / (double) SongQueue[CurrentSongIndex].TagLibFile.Properties.Duration.TotalMilliseconds;
                Dispatcher.Invoke((Action) (() =>
                {
                    timeline.Value = barPercentage * timeline.Maximum;
                    timelineCurrTime.Content = currTime.ToString(@"hh\:mm\:ss");
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

            var newTimelinePosition = new TimeSpan(0, 0, 0, 0, (int) (percentage * SongQueue[CurrentSongIndex].TagLibFile.Properties.Duration.TotalMilliseconds));
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Ctrl + O
            if(Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O)
            {
                OnAddSongsClick(null, null);
            }
        }

        private void OnAddSongContextMenu(object sender, RoutedEventArgs e)
        {
            OnAddSongsClick(null, null);
        }
    }
}
