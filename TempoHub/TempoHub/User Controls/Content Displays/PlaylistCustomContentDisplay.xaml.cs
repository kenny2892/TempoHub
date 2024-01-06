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

namespace TempoHub.User_Controls.Content_Displays
{
    /// <summary>
    /// Interaction logic for PlaylistCustomContentDisplay.xaml
    /// </summary>
    public partial class PlaylistCustomContentDisplay : UserControl
    {
        public event EventHandler<RoutedEventArgs> PlayClick;
        public event EventHandler<RoutedEventArgs> ShuffleClick;
        public event EventHandler<RoutedEventArgs> AddClick;
        public event EventHandler<RoutedEventArgs> OrderChanged;
        public event EventHandler<RoutedEventArgs> SongRemoved;
        public event EventHandler<RoutedEventArgs> PlaylistNameChanged;

        public PlaylistCustomContentDisplay()
        {
            InitializeComponent();
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

        private void OnOrderChanged(object sender, RoutedEventArgs e)
        {
            OrderChanged?.Invoke(sender, e);
        }

        private void OnSongRemoved(object sender, RoutedEventArgs e)
        {
            SongRemoved?.Invoke(sender, e);
        }

        private void OnPlaylistNameChanged(object sender, TextChangedEventArgs e)
        {
            PlaylistNameChanged?.Invoke(sender, e);
        }
    }
}
