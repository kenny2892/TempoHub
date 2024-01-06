using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Settings;

namespace TempoHub.ViewModels
{
    public class SettingsSongDetailsViewModel : PropertyChangedBase
    {
        private string settingName = "";
        public string SettingName
        {
            get { return settingName; }
            set
            {
                if(settingName != value)
                {
                    settingName = value;
                    OnPropertyChanged(nameof(SettingName));
                }
            }
        }
        private int index = 0;
        public int Index
        {
            get { return index; }
            set
            {
                if(index != value)
                {
                    index = value;
                    OnPropertyChanged(nameof(Index));
                }
            }
        }
        private SortOptions sort = 0;
        public SortOptions Sort
        {
            get { return sort; }
            set
            {
                if(sort != value)
                {
                    sort = value;
                    OnPropertyChanged(nameof(Sort));
                }
            }
        }
        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if(isEnabled != value)
                {
                    isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }
        private IEnumerable<SortOptions> sortOptionsValues = new List<SortOptions>();
        public IEnumerable<SortOptions> SortOptionsValues
        {
            get { return sortOptionsValues; }
            set
            {
                if(sortOptionsValues != value)
                {
                    sortOptionsValues = value;
                    OnPropertyChanged(nameof(SortOptionsValues));
                }
            }
        }
    }
}
