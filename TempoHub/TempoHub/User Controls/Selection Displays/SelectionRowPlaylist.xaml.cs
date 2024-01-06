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
using TempoHub.ViewModels;
using TempoHub.ViewModels.Selection_Displays;

namespace TempoHub.User_Controls.Selection_Displays
{
    /// <summary>
    /// Interaction logic for SelectionRowPlaylist.xaml
    /// </summary>
    public partial class SelectionRowPlaylist : UserControl
    {
        public event EventHandler<int> DeleteClick;

        public SelectionRowPlaylist()
        {
            InitializeComponent();
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SelectionRowPlaylistViewModel vm)
            {
                DeleteClick?.Invoke(sender, vm.PlaylistID);
            }
        }
    }
}
