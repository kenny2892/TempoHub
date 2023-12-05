using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for SongListRow.xaml
    /// </summary>
    public partial class SongListRow : UserControl, INotifyPropertyChanged
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
                addIcon.Background = DefaultBackgroundColor;
                addIcon.Foreground = DefaultAddIconColor;
                playIcon.Background = DefaultBackgroundColor;
                playIcon.Foreground = DefaultAddIconColor;
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
        public Action OnDoubleClickMethod { get; set; }
        public Action OnAddClickMethod { get; set; }
        public Action OnPlayClickMethod { get; set; }
        public Action OnEditSongInfoClickMethod { get; set; }

        public SongListRow()
        {
            InitializeComponent();
            DataContext = this;
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
            addIcon.Background = DarkGrey;
            addIcon.Foreground = LightGrey;
            playIcon.Background = DarkGrey;
            playIcon.Foreground = LightGrey;
        }

        public void OnMouseLeave(object sender, MouseEventArgs e)
        {
            songGrid.Background = DefaultBackgroundColor;
            addIcon.Background = DefaultBackgroundColor;
            addIcon.Foreground = DefaultAddIconColor;
            playIcon.Background = DefaultBackgroundColor;
            playIcon.Foreground = DefaultAddIconColor;
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(OnDoubleClickMethod != null && e.ClickCount == 2 && !(e.OriginalSource is PackIconBase))
            {
                OnDoubleClickMethod();
            }
        }

        private void OnAddClick(object sender, MouseButtonEventArgs e)
        {
            if(OnAddClickMethod != null)
            {
                OnAddClickMethod();
            }
        }

        private void OnPlayClick(object sender, MouseButtonEventArgs e)
        {
            if(OnPlayClickMethod != null)
            {
                OnPlayClickMethod();
            }
        }

        private void OnEditSongInfoClick(object sender, RoutedEventArgs e)
        {
            if(OnEditSongInfoClickMethod != null)
            {
                OnEditSongInfoClickMethod();
            }
        }
    }
}
