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
using TempoHub.ViewModels;
using TempoHub.ViewModels.Selection_Displays;

namespace TempoHub.User_Controls.Selection_Displays
{
    /// <summary>
    /// Interaction logic for AlbumRow.xaml
    /// </summary>
    public partial class SelectionRowAlbum : UserControl
    {
        public SelectionRowAlbum()
        {
            InitializeComponent();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            detailsGrid.SetResourceReference(Control.BackgroundProperty, "MahApps.Brushes.Gray10");
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            detailsGrid.SetResourceReference(Control.BackgroundProperty, "MahApps.Brushes.ThemeBackground");
        }

        private void OnSearchForTagsContextMenu(object sender, RoutedEventArgs e)
        {
            if(DataContext is SelectionRowAlbumViewModel vm)
            {
                vm.OpenTagSearchMethod();
            }
        }
    }
}
