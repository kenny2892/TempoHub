using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using TempoHub.ViewModels;

namespace TempoHub.User_Controls
{
    /// <summary>
    /// Interaction logic for PlaylistDisplay.xaml
    /// </summary>
    public partial class PlaylistDisplay : UserControl
    {
        public event EventHandler<RoutedEventArgs> PlayClick;
        public event EventHandler<RoutedEventArgs> ShuffleClick;
        public event EventHandler<RoutedEventArgs> AddClick;
        public event EventHandler<RoutedEventArgs> OrderChanged;
        public event EventHandler<RoutedEventArgs> SongRemoved;
        public PlaylistDisplay()
        {
            InitializeComponent();
        }

        // Based on here: https://stackoverflow.com/a/37195140
        private void OnSongDrop(object sender, DragEventArgs e)
        {
            if(DataContext is PlaylistDisplayViewModel vm)
            {
                var source = e.Data.GetData("Source") as PlaylistSongRowViewModel;

                if(source != null)
                {
                    var swapPlacesWith = (sender as PlaylistSongRow).DataContext as PlaylistSongRowViewModel;

                    if(swapPlacesWith != null)
                    {
                        int newIndex = vm.Songs.IndexOf(swapPlacesWith);
                        vm.Songs.RemoveAt(vm.Songs.IndexOf(source));
                        vm.Songs.Insert(newIndex, source);
                        UpdateSongRowIndexes(vm);
                        OrderChanged?.Invoke(sender, e);
                    }
                }
            }
        }

        private void OnSongPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Task.Factory.StartNew(new Action(() =>
                {
                    Thread.Sleep(200);
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if(e.LeftButton == MouseButtonState.Pressed)
                        {
                            var data = new DataObject();
                            data.SetData("Source", (sender as PlaylistSongRow).DataContext);
                            DragDrop.DoDragDrop(sender as DependencyObject, data, DragDropEffects.Move);
                            e.Handled = true;
                        }
                    }), null);
                }), CancellationToken.None);
            }
        }

        private void OnPlayBtnClick(object sender, RoutedEventArgs e)
        {
            PlayClick?.Invoke(sender, e);
        }

        private void OnShuffleBtnClick(object sender, RoutedEventArgs e)
        {
            ShuffleClick?.Invoke(sender, e);
        }

        private void OnAddBtnClick(object sender, RoutedEventArgs e)
        {
            AddClick?.Invoke(sender, e);
        }

        private void OnSongUpClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is PlaylistDisplayViewModel vm && sender is Button songBtn)
            {
                var songToMove = songBtn.DataContext as PlaylistSongRowViewModel;

                if(songToMove != null)
                {
                    int oldIndex = vm.Songs.IndexOf(songToMove);

                    if(oldIndex <= 0)
                    {
                        return;
                    }

                    int newIndex = oldIndex - 1;
                    vm.Songs.RemoveAt(oldIndex);
                    vm.Songs.Insert(newIndex, songToMove);
                    UpdateSongRowIndexes(vm);
                    OrderChanged?.Invoke(sender, e);
                }
            }
        }

        private void OnSongDownClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is PlaylistDisplayViewModel vm && sender is Button songBtn)
            {
                var songToMove = songBtn.DataContext as PlaylistSongRowViewModel;

                if(songToMove != null)
                {
                    int oldIndex = vm.Songs.IndexOf(songToMove);

                    if(oldIndex >= vm.Songs.Count)
                    {
                        return;
                    }

                    int newIndex = oldIndex + 1;
                    vm.Songs.RemoveAt(oldIndex);
                    vm.Songs.Insert(newIndex, songToMove);
                    UpdateSongRowIndexes(vm);
                    OrderChanged?.Invoke(sender, e);
                }
            }
        }

        private void OnSongRemoveClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is PlaylistDisplayViewModel vm && sender is Button songBtn)
            {
                var songToRemove = songBtn.DataContext as PlaylistSongRowViewModel;

                if(songToRemove != null)
                {
                    int index = vm.Songs.IndexOf(songToRemove);
                    vm.Songs.RemoveAt(index);
                    UpdateSongRowIndexes(vm);
                    SongRemoved?.Invoke(sender, e);
                }
            }
        }

        private void UpdateSongRowIndexes(PlaylistDisplayViewModel vm)
        {
            int index = 0;
            foreach(var songRow in vm.Songs)
            {
                songRow.Index = index++;
                songRow.SongEntry.Index = songRow.Index;
            }
        }
    }
}
