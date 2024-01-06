using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TempoHub.ViewModels.Song_Editor_Tabs
{
    public class AddPictureByUrlTabViewModel : PropertyChangedBase
    {
        public Action<Visibility> SetUploadBtnVisibilityMethod { get; set; }
        public string MimeType { get; set; }
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

        public AddPictureByUrlTabViewModel()
        {
            EditorPicture.OnRemovalClickMethod = () =>
            {
                EditorPicture.ImageVm.ImageData = new byte[0];

                if(SetUploadBtnVisibilityMethod != null)
                {
                    SetUploadBtnVisibilityMethod(Visibility.Collapsed);
                }
            };
        }
    }
}
