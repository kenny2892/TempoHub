using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;

namespace TempoHub.ViewModels
{
    public class SongQueueRowViewModel : PropertyChangedBase
    {
        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if(value != isSelected)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
        private string rowIndex = "0";
        public string RowIndex
        {
            get { return rowIndex; }
            set
            {
                if(value != rowIndex && int.TryParse(value, out int validIndex))
                {
                    rowIndex = value;
                    OnPropertyChanged(nameof(RowIndex));
                }
            }
        }
        public SongFile Song { get; set; }
        private string songName = "";
        public string SongName
        {
            get { return songName; }
            set
            {
                if(value != null && value != songName)
                {
                    songName = value;
                    OnPropertyChanged(nameof(SongName));
                }
            }
        }
        private string songLength = "";
        public string SongLength
        {
            get { return songLength; }
            set
            {
                if(value != null && value != songLength)
                {
                    songLength = value;
                    OnPropertyChanged(nameof(SongLength));
                }
            }
        }
        public Action OnDoubleClickMethod { get; set; }
        public Action OnPlayClickMethod { get; set; }
        public Action OnRemoveClickMethod { get; set; }
        public Action OnUpArrowClickMethod { get; set; }
        public Action OnDownArrowClickMethod { get; set; }
    }
}
