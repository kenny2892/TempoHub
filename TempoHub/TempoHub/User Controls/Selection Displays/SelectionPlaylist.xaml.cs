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

namespace TempoHub.User_Controls.Selection_Displays
{
    /// <summary>
    /// Interaction logic for SelectionPlaylist.xaml
    /// </summary>
    public partial class SelectionPlaylist : UserControl
    {
        public event EventHandler<RoutedEventArgs> NewPlaylistClicked;
        public event EventHandler<string> RefreshRequested;
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        public event EventHandler<int> DeletePlaylistClick;

        public SelectionPlaylist()
        {
            InitializeComponent();
        }

        private void OnNewPlaylistClick(object sender, RoutedEventArgs e)
        {
            NewPlaylistClicked?.Invoke(sender, e);
        }

        private void OnRefreshRequested(object sender, string e)
        {
            RefreshRequested?.Invoke(this, e);
        }

        private void OnSelectionSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }

        private void OnPlaylistDeleteClick(object sender, int e)
        {
            DeletePlaylistClick?.Invoke(this, e);
        }
    }
}
