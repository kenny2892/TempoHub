using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TempoHub.Models;
using TempoHub.User_Controls;
using TempoHub.ViewModels;
using TempoHub.ViewModels.Content_Displays;

namespace TempoHub.User_Controls.Content_Displays
{
    /// <summary>
    /// Interaction logic for AlbumContentDisplay.xaml
    /// </summary>
    public partial class AlbumContentDisplay : UserControl
    {
        public event EventHandler<RoutedEventArgs> AddSongsDialogOpen;
        public event EventHandler<RoutedEventArgs> AddFolderSongsDialogOpen;
        public event EventHandler<AddToPlaylistEventArgs> AddToPlaylistClick;
        public event EventHandler<SongCopyPasteEventArgs> Copy;
        public event EventHandler<SongCopyPasteEventArgs> Paste;

        public AlbumContentDisplay()
        {
            InitializeComponent();
            displayAlbumCoverImg.image.VerticalAlignment = VerticalAlignment.Top;
        }

        private void OnSongSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DataContext is AlbumContentDisplayViewModel vm)
            {
                if(vm.ParentArtistContaier is null)
                {
                    vm.MultiSelectEnabled = vm.Songs.Count(song => song.IsSelected) > 1;
                }

                else
                {
                    var songThatWasClicked = (SongListRowViewModel) songListItemsControl.SelectedItem;
                    var shiftOrCtrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

                    // Check all other albums and see if they have a song selected
                    var allSelectedSongs = vm.ParentArtistContaier.AlbumDisplays
                        .SelectMany(album => album.Songs.Where(song => song.IsSelected)).ToList();

                    // If not holding shift or ctrl, deselect all songs not in this vm
                    if(!shiftOrCtrl)
                    {
                        foreach(var album in vm.ParentArtistContaier.AlbumDisplays)
                        {
                            if(album != vm)
                            {
                                foreach(var song in album.Songs)
                                {
                                    song.IsSelected = false;
                                }
                            }

                            album.MultiSelectEnabled = false;
                        }

                        // Re-select this song
                        if(songThatWasClicked != null)
                        {
                            songThatWasClicked.IsSelected = true;
                        }
                    }

                    else
                    {
                        var multiSelected = vm.ParentArtistContaier.AlbumDisplays.Sum(album => album.Songs.Count(song => song.IsSelected)) > 1 ;

                        foreach(var album in vm.ParentArtistContaier.AlbumDisplays)
                        {
                            album.MultiSelectEnabled = multiSelected;
                        }
                    }
                }
            }
        }

        // We need to redirect the scroll event to the parent
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(DataContext is AlbumContentDisplayViewModel vm && vm.ParentArtistContaier != null)
            {
                var parentListBox = FindParent<ScrollViewer>((DependencyObject) sender);

                if(parentListBox != null)
                {
                    e.Handled = true;
                    var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                    {
                        RoutedEvent = UIElement.MouseWheelEvent,
                        Source = sender
                    };

                    parentListBox.RaiseEvent(eventArg);
                }
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if(parentObject == null)
            {
                return null;
            }

            if(parentObject is T parent)
            {
                return parent;
            }

            return FindParent<T>(parentObject);
        }

        private void OnPlaySongClick(object sender, string e)
        {
            if(DataContext is AlbumContentDisplayViewModel vm && sender is SongListRowViewModel songVm)
            {
                vm.Play(songVm.Song);
            }
        }

        private void OnAddToQueueClick(object sender, string e)
        {
            if(DataContext is AlbumContentDisplayViewModel vm)
            {
                vm.AddToQueue();
            }
        }

        public void OnEditSongInfoClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is AlbumContentDisplayViewModel vm && sender is SongListRowViewModel songVm)
            {
                vm.EditSongInfo(songVm.Song);
            }
        }

        public void OnEditSongsInfoClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is AlbumContentDisplayViewModel vm)
            {
                vm.EditSongsInfo();
            }
        }

        private void OnRemoveSongsClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is AlbumContentDisplayViewModel vm)
            {
                vm.RemoveSongs();
            }
        }

        private void OnAddToPlaylistClick(object sender, AddToPlaylistEventArgs e)
        {
            // Need to adjust the Args so that it includes all selected songs
            if(DataContext is AlbumContentDisplayViewModel vm)
            {
                e.FilePaths.Clear();

                if(vm.ParentArtistContaier is null)
                {
                    e.FilePaths.AddRange(vm.Songs.Where(song => song.IsSelected).Select(song => song.Song.FilePath));
                }

                else
                {
                    e.FilePaths.Clear();
                    foreach(var album in vm.ParentArtistContaier.AlbumDisplays)
                    {
                        e.FilePaths.AddRange(album.Songs.Where(song => song.IsSelected).Select(song => song.Song.FilePath));
                    }
                }
            }

            AddToPlaylistClick?.Invoke(this, e);
        }

        private void OnCopyClick(object sender, SongCopyPasteEventArgs e)
        {
            Copy?.Invoke(this, e);
        }

        private void OnPasteClick(object sender, SongCopyPasteEventArgs e)
        {
            Paste?.Invoke(this, e);
        }

        private void OnAddSongContextMenu(object sender, RoutedEventArgs e)
        {
            AddSongsDialogOpen?.Invoke(this, e);
        }

        private void OnAddFolderSongContextMenu(object sender, RoutedEventArgs e)
        {
            AddFolderSongsDialogOpen?.Invoke(this, e);
        }
    }
}
