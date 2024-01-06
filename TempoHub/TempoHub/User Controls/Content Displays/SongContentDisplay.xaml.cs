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
using TempoHub.ViewModels.Content_Displays;

namespace TempoHub.User_Controls.Content_Displays
{
    /// <summary>
    /// Interaction logic for SongContentDisplay.xaml
    /// </summary>
    public partial class SongContentDisplay : UserControl
    {
        public event EventHandler<RoutedEventArgs> AddSongsDialogOpen;
        public event EventHandler<RoutedEventArgs> AddFolderSongsDialogOpen;

        public SongContentDisplay()
        {
            InitializeComponent();
        }

        private void OnPlayBtnClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongContentDisplayViewModel vm && vm.OnPlayMethod != null)
            {
                vm.OnPlayMethod();
            }
        }

        private void OnQueueBtnClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongContentDisplayViewModel vm && vm.OnQueueMethod != null)
            {
                vm.OnQueueMethod();
            }
        }

        private void OnEditBtnClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongContentDisplayViewModel vm && vm.OnEditSongMethod != null)
            {
                vm.OnEditSongMethod();
            }
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
