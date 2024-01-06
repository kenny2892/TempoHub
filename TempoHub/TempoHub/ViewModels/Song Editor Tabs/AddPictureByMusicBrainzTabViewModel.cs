using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TempoHub.User_Controls;

namespace TempoHub.ViewModels.Song_Editor_Tabs
{
    public class AddPictureByMusicBrainzTabViewModel : PropertyChangedBase
    {
        public Func<double, double, double, bool> StartLoadingDisplay { get; set; }
        public Action<int> AddValueLoadingDisplay { get; set; }
        public Action StopLoadingDisplay { get; set; }
        public Action<Visibility> SetUploadBtnVisibilityMethod { get; set; }
        private ObservableCollection<ImageSearchResultRowViewModel> searchResults = new ObservableCollection<ImageSearchResultRowViewModel>();
        public ObservableCollection<ImageSearchResultRowViewModel> SearchResults
        {
            get { return searchResults; }
            set
            {
                if(searchResults != value)
                {
                    searchResults = value;
                    OnPropertyChanged(nameof(SearchResults));
                }
            }
        }
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
    }
}
