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
using TempoHub.Settings;
using TempoHub.ViewModels;

namespace TempoHub.User_Controls.Selection_Displays
{
    /// <summary>
    /// Interaction logic for Selection.xaml
    /// </summary>
    public partial class Selection : UserControl
    {
        public event EventHandler<string> RefreshRequested;
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        public event EventHandler<int> PlaylistDeleteClick;
        public Selection()
        {
            InitializeComponent();
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
            PlaylistDeleteClick?.Invoke(sender, e);
        }
    }
}
