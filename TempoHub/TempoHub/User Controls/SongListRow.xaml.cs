using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using TempoHub.ViewModels;

namespace TempoHub.User_Controls
{
    /// <summary>
    /// Interaction logic for SongListRow.xaml
    /// </summary>
    public partial class SongListRow : UserControl
    {
        public event EventHandler<MouseButtonEventArgs> DoubleClick;
        public event EventHandler<string> AddToQueueClick;
        public event EventHandler<string> PlayClick;
        public event EventHandler<RoutedEventArgs> RemoveClick;
        public event EventHandler<RoutedEventArgs> EditSongInfoClick;
        public event EventHandler<RoutedEventArgs> EditSongsInfoClick;
        public event EventHandler<AddToPlaylistEventArgs> AddToPlaylistClick;
        public event EventHandler<SongCopyPasteEventArgs> Copy;
        public event EventHandler<SongCopyPasteEventArgs> Paste;
        public SongListRow()
        {
            InitializeComponent();
        }

        public void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2 && !(e.OriginalSource is PackIconBase))
            {
                DoubleClick?.Invoke(this, e);
            }
        }

        public void OnAddToQueueClick(object sender, MouseButtonEventArgs e)
        {
            AddToQueueClick?.Invoke(this, "Button");
        }

        private void OnAddToQueueClick(object sender, RoutedEventArgs e)
        {
            AddToQueueClick?.Invoke(this, "Menu");
        }

        public void OnPlayClick(object sender, MouseButtonEventArgs e)
        {
            PlayClick?.Invoke(DataContext, "Button");
        }

        private void OnPlayClick(object sender, RoutedEventArgs e)
        {
            PlayClick?.Invoke(DataContext, "Menu");
        }

        public void OnEditSongInfoClick(object sender, RoutedEventArgs e)
        {
            EditSongInfoClick?.Invoke(DataContext, e);
        }

        public void OnEditSongsInfoClick(object sender, RoutedEventArgs e)
        {
            EditSongsInfoClick?.Invoke(this, e);
        }

        private void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            RemoveClick?.Invoke(this, e);
        }

        private void OnAddToPlaylistClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongListRowViewModel vm && sender is MenuItem playlistBtn)
            {
                AddToPlaylistEventArgs args = new AddToPlaylistEventArgs();
                args.FilePaths.Add(vm.Song.FilePath);
                args.PlaylistName = playlistBtn.Header as string;

                AddToPlaylistClick?.Invoke(this, args);
            }
        }

        private void OnCopyClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongListRowViewModel vm)
            {
                SongCopyPasteEventArgs args = new SongCopyPasteEventArgs();
                args.FilePath = vm.Song.FilePath;
                Copy?.Invoke(this, args);
            }
        }

        private void OnPasteClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongListRowViewModel vm)
            {
                SongCopyPasteEventArgs args = new SongCopyPasteEventArgs();
                args.FilePath = vm.Song.FilePath;
                Paste?.Invoke(this, args);
            }
        }
    }
}
