using ControlzEx.Theming;
using MahApps.Metro.Controls;
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
using System.Windows.Shapes;
using TempoHub.Settings;
using TempoHub.ViewModels;

namespace TempoHub.Views
{
    /// <summary>
    /// Interaction logic for PlaylistAddSongWindow.xaml
    /// </summary>
    public partial class PlaylistAddSongWindow : MetroWindow
    {
        public PlaylistAddSongWindow(ThemeBase themeBase, ThemeColor themeColor)
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, $"{themeBase}.{themeColor}");
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if(DataContext is PlaylistAddSongWindowViewModel vm)
            {
                vm.Filter();
            }
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is PlaylistAddSongWindowViewModel vm)
            {
                vm.Canceled = false;
            }

            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is PlaylistAddSongWindowViewModel vm)
            {
                vm.Canceled = true;
            }

            Close();
        }
    }
}
