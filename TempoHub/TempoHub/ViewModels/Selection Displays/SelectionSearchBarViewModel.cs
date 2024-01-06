using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Settings;

namespace TempoHub.ViewModels.Selection_Displays
{
    public class SelectionSearchBarViewModel : PropertyChangedBase
    {
        private SortOptions sortOption = SortOptions.Ascending;
        public SortOptions SortOption
        {
            get { return sortOption; }
            set
            {
                if(sortOption != value)
                {
                    sortOption = value;
                    OnPropertyChanged(nameof(SortOption));
                }
            }
        }
        private string searchText = "";
        public string SearchText
        {
            get { return searchText; }
            set
            {
                if(searchText != value)
                {
                    searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                }
            }
        }
    }
}
