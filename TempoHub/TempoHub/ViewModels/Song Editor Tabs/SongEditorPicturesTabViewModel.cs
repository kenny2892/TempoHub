using System;
using System.Collections.ObjectModel;
using System.Linq;
using TempoHub.Models;
using TagLib;

namespace TempoHub.ViewModels.Song_Editor_Tabs
{
    public class SongEditorPicturesTabViewModel : PropertyChangedBase
    {
        public Action OnAddPictureClickMethod { get; set; }
        public IPicture[] Pictures { get; set; }
        private int selectedIndex = -1;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if(selectedIndex != value)
                {
                    selectedIndex = value;
                    OnPropertyChanged(nameof(SelectedIndex));
                }
            }
        }
        private ObservableCollection<EditorPictureViewModel> editorPictures = new ObservableCollection<EditorPictureViewModel>();

        public ObservableCollection<EditorPictureViewModel> EditorPictures
        {
            get { return editorPictures; }
            set
            {
                if(editorPictures != value)
                {
                    editorPictures = value;
                    OnPropertyChanged(nameof(EditorPictures));
                }
            }
        }

        public SongEditorPicturesTabViewModel(SongInfo songInfo)
        {
            Pictures = songInfo.Pictures;
            CreateEditorPictures();
        }

        public void CreateEditorPictures()
        {
            EditorPictures.Clear();

            for(int i = 0; i < Pictures.Length; i++)
            {
                var pictureToDisplay = Pictures[i];
                var picData = pictureToDisplay.Data.Data;

                EditorPictureViewModel editorPicVm = new EditorPictureViewModel();
                editorPicVm.ImageVm = new AspectRatioImageViewModel() { ImageData = picData };
                editorPicVm.OnRemovalClickMethod = () =>
                {
                    var newList = Pictures.ToList();
                    newList.RemoveAt(i);
                    Pictures = newList.ToArray();
                    EditorPictures.Remove(editorPicVm);
                };

                EditorPictures.Add(editorPicVm);
            }

            if(Pictures.Length > 0)
            {
                SelectedIndex = 0;
            }
        }

        public void Clear()
        {
            Pictures = new IPicture[0];
            EditorPictures.Clear();
        }
    }
}
