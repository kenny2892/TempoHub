using System;
using System.Collections.Generic;
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
using TempoHub.ViewModels;

namespace TempoHub.User_Controls
{
    /// <summary>
    /// Interaction logic for PlaylistSongRow.xaml
    /// </summary>
    public partial class PlaylistSongRow : UserControl
    {
        public event EventHandler<RoutedEventArgs> UpClick;
        public event EventHandler<RoutedEventArgs> DownClick;
        public event EventHandler<RoutedEventArgs> RemoveClick;
        public PlaylistSongRow()
        {
            InitializeComponent();
        }

        private void OnUpClick(object sender, RoutedEventArgs e)
        {
            UpClick?.Invoke(sender, e);
        }

        private void OnDownClick(object sender, RoutedEventArgs e)
        {
            DownClick?.Invoke(sender, e);
        }

        private void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            RemoveClick?.Invoke(sender, e);
        }
    }
}
