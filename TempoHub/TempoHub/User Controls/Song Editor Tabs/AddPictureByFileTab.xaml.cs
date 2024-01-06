using Microsoft.Win32;
using System.IO;
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
    /// Interaction logic for AddPictureByFileTab.xaml
    /// </summary>
    public partial class AddPictureByFileTab : UserControl
    {
        public AddPictureByFileTab()
        {
            InitializeComponent();
        }

        private void OnSelectFileClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is AddPictureByFileTabViewModel vm)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png|All Files|*.*"
                };

                if(openFileDialog.ShowDialog() == true)
                {
                    vm.ImagePathToImport = openFileDialog.FileName;
                    vm.EditorPicture.ImageVm = new AspectRatioImageViewModel() { ImageData = File.ReadAllBytes(vm.ImagePathToImport) };
                    
                    if(vm.SetUploadBtnVisibilityMethod != null)
                    {
                        vm.SetUploadBtnVisibilityMethod(Visibility.Visible);
                    }
                }
            }
        }
    }
}
