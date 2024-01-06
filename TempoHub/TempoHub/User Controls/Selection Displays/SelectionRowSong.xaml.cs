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
    /// Interaction logic for SelectionRowSong.xaml
    /// </summary>
    public partial class SelectionRowSong : UserControl
    {
        public SelectionRowSong()
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
    }
}
