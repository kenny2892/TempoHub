using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using TempoHub.Models;

namespace TempoHub.User_Controls
{
    /// <summary>
    /// Interaction logic for SongQueueRow.xaml
    /// </summary>
    public partial class SongQueueRow : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
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

        private int rowIndex = 0;
        public int RowIndex
        {
            get { return rowIndex; }
            set
            {
                rowIndex = value;
                songGrid.Background = DefaultBackgroundColor;
                removeIcon.Background = DefaultBackgroundColor;
                removeIcon.Foreground = DefaultAddIconColor;
                playIcon.Background = DefaultBackgroundColor;
                playIcon.Foreground = DefaultAddIconColor;
                upIcon.Background = DefaultBackgroundColor;
                upIcon.Foreground = DefaultAddIconColor;
                downIcon.Background = DefaultBackgroundColor;
                downIcon.Foreground = DefaultAddIconColor;
                songBorder.BorderBrush = DefaultBorderColor;
            }
        }
        private static Brush DarkGrey { get; set; } = new SolidColorBrush(Color.FromRgb(40, 40, 40));
        private static Brush MediumGrey { get; set; } = new SolidColorBrush(Color.FromRgb(76, 76, 76));
        private static Brush LightGrey { get; set; } = new SolidColorBrush(Color.FromRgb(132, 132, 132));
        private Brush DefaultBackgroundColor
        {
            get
            {
                if((double) RowIndex / 2.0 == RowIndex / 2)
                {
                    return MediumGrey;
                }

                return LightGrey;
            }
        }
        private Brush DefaultAddIconColor
        {
            get
            {
                if((double) RowIndex / 2.0 == RowIndex / 2)
                {
                    return LightGrey;
                }

                return MediumGrey;
            }
        }
        private Brush DefaultBorderColor
        {
            get
            {
                if(!IsSelected)
                {
                    return DefaultBackgroundColor;
                }
                
                return (Brush) FindResource("MahApps.Brushes.Accent");
            }
        }
        public bool isSelected = false;
        public bool IsSelected 
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                songBorder.BorderBrush = DefaultBorderColor;
            }
        }
        public Action OnDoubleClickMethod { get; set; }
        public Action OnPlayClickMethod { get; set; }
        public Action OnRemoveClickMethod { get; set; }
        public Action OnUpArrowClickMethod { get; set; }
        public Action OnDownArrowClickMethod { get; set; }

        public SongQueueRow()
        {
            InitializeComponent();
            DataContext = this;
            IsSelected = false;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateInfo()
        {
            SongName = Song.TagLibFile.Tag.Title;
            SongLength = $"{Song.TagLibFile.Properties.Duration.Minutes}:{(Song.TagLibFile.Properties.Duration.Seconds).ToString().PadLeft(2, '0')}";
        }

        public void OnMouseEnter(object sender, MouseEventArgs e)
        {
            songGrid.Background = DarkGrey;
            removeIcon.Background = DarkGrey;
            removeIcon.Foreground = LightGrey;
            playIcon.Background = DarkGrey;
            playIcon.Foreground = LightGrey;
            upIcon.Background = DarkGrey;
            upIcon.Foreground = LightGrey;
            downIcon.Background = DarkGrey;
            downIcon.Foreground = LightGrey;
        }

        public void OnMouseLeave(object sender, MouseEventArgs e)
        {
            songGrid.Background = DefaultBackgroundColor;
            removeIcon.Background = DefaultBackgroundColor;
            removeIcon.Foreground = DefaultAddIconColor;
            playIcon.Background = DefaultBackgroundColor;
            playIcon.Foreground = DefaultAddIconColor;
            upIcon.Background = DefaultBackgroundColor;
            upIcon.Foreground = DefaultAddIconColor;
            downIcon.Background = DefaultBackgroundColor;
            downIcon.Foreground = DefaultAddIconColor;
        }

        private void OnRemoveClick(object sender, MouseButtonEventArgs e)
        {
            if(OnRemoveClickMethod != null)
            {
                OnRemoveClickMethod();
            }
        }

        private void OnUpArrowClick(object sender, MouseButtonEventArgs e)
        {
            if(OnUpArrowClickMethod != null)
            {
                OnUpArrowClickMethod();
            }
        }

        private void OnDownArrowClick(object sender, MouseButtonEventArgs e)
        {
            if(OnDownArrowClickMethod != null)
            {
                OnDownArrowClickMethod();
            }
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(OnDoubleClickMethod != null && e.ClickCount == 2 && !(e.OriginalSource is PackIconBase))
            {
                OnDoubleClickMethod();
            }
        }

        private void OnPlayClick(object sender, MouseButtonEventArgs e)
        {
            if(OnPlayClickMethod != null)
            {
                OnPlayClickMethod();
            }
        }
    }
}
