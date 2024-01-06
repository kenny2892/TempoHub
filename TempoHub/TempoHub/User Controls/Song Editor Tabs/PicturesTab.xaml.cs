using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TempoHub.ViewModels;
using TempoHub.ViewModels.Song_Editor_Tabs;

namespace TempoHub.User_Controls.Song_Editor_Tabs
{
    /// <summary>
    /// Interaction logic for PicturesTab.xaml
    /// </summary>
    public partial class PicturesTab : UserControl
    {
        public PicturesTab()
        {
            InitializeComponent();
        }

        private void OnAddPictureClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongEditorPicturesTabViewModel vm && vm.OnAddPictureClickMethod != null)
            {
                vm.OnAddPictureClickMethod();
            }
        }
    }
}
