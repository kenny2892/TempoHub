using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TempoHub.ViewModels.Song_Editor_Tabs
{
    public class AddPictureByFileTabViewModel : PropertyChangedBase
    {
        public string ImagePathToImport { get; set; }
        public Action<Visibility> SetUploadBtnVisibilityMethod { get; set; }
        private EditorPictureViewModel editorPicture = new EditorPictureViewModel();
        public EditorPictureViewModel EditorPicture
        {
            get { return editorPicture; }
            set
            {
                if(editorPicture != value)
                {
                    editorPicture = value;
                    OnPropertyChanged(nameof(EditorPicture));
                }
            }
        }

        public AddPictureByFileTabViewModel()
        {
            EditorPicture.OnRemovalClickMethod = () =>
            {
                EditorPicture.ImageVm.ImageData = new byte[0];
                ImagePathToImport = "";

                if(SetUploadBtnVisibilityMethod != null)
                {
                    SetUploadBtnVisibilityMethod(Visibility.Collapsed);
                }
            };
        }
    }
}
