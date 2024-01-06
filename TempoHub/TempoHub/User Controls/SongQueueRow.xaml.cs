using MahApps.Metro.IconPacks;
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

namespace TempoHub.User_Controls
{
    /// <summary>
    /// Interaction logic for SongQueueRow.xaml
    /// </summary>
    public partial class SongQueueRow : UserControl
    {
        public SongQueueRow()
        {
            InitializeComponent();
            OnCheckedChange(null, null);
        }

        private void OnRemoveClick(object sender, MouseButtonEventArgs e)
        {
            if(DataContext is SongQueueRowViewModel vm)
            {
                if(vm.OnRemoveClickMethod != null)
                {
                    vm.OnRemoveClickMethod();
                }
            }
        }

        private void OnUpArrowClick(object sender, MouseButtonEventArgs e)
        {
            if(DataContext is SongQueueRowViewModel vm)
            {
                if(vm.OnUpArrowClickMethod != null)
                {
                    vm.OnUpArrowClickMethod();
                }
            }
        }

        private void OnDownArrowClick(object sender, MouseButtonEventArgs e)
        {
            if(DataContext is SongQueueRowViewModel vm)
            {
                if(vm.OnDownArrowClickMethod != null)
                {
                    vm.OnDownArrowClickMethod();
                }
            }
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(DataContext is SongQueueRowViewModel vm)
            {
                if(vm.OnDoubleClickMethod != null && e.ClickCount == 2 && !(e.OriginalSource is PackIconBase))
                {
                    vm.OnDoubleClickMethod();
                }
            }
        }

        private void OnPlayClick(object sender, MouseButtonEventArgs e)
        {
            if(DataContext is SongQueueRowViewModel vm)
            {
                if(vm.OnPlayClickMethod != null)
                {
                    vm.OnPlayClickMethod();
                }
            }
        }

        private void OnCheckedChange(object sender, RoutedEventArgs e)
        {
            if(isSelectedCheckBox.IsChecked == true)
            {
                songBorder.BorderBrush = (Brush) FindResource("MahApps.Brushes.Accent");
            }

            else
            {
                songBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
        }
    }
}
