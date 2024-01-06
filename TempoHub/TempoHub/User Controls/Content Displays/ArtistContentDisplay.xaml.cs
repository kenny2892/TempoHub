using System;
using System.Collections.Generic;
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

namespace TempoHub.User_Controls.Content_Displays
{
    /// <summary>
    /// Interaction logic for ArtistContentDisplay.xaml
    /// </summary>
    public partial class ArtistContentDisplay : UserControl
    {
        public event EventHandler<RoutedEventArgs> AddSongsDialogOpen;
        public event EventHandler<RoutedEventArgs> AddFolderSongsDialogOpen;
        public event EventHandler<AddToPlaylistEventArgs> AddToPlaylistClick;
        public event EventHandler<SongCopyPasteEventArgs> Copy;
        public event EventHandler<SongCopyPasteEventArgs> Paste;

        public ArtistContentDisplay()
        {
            InitializeComponent();
        }

        private void OnAddToPlaylistClick(object sender, AddToPlaylistEventArgs e)
        {
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
