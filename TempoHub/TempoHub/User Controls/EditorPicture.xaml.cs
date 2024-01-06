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
using TempoHub.ViewModels;

namespace TempoHub.User_Controls
{
    /// <summary>
    /// Interaction logic for EditorPicture.xaml
    /// </summary>
    public partial class EditorPicture : UserControl
    {
        public EditorPicture()
        {
            InitializeComponent();
            picture.image.VerticalAlignment = VerticalAlignment.Top;
        }

        private void OnRemovalClick(object sender, MouseButtonEventArgs e)
        {
            if(DataContext is EditorPictureViewModel vm)
            {
                if(vm.OnRemovalClickMethod != null)
                {
                    vm.OnRemovalClickMethod();
                }
            }
        }
    }
}
