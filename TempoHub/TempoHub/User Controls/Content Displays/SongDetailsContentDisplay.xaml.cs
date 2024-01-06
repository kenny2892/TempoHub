using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TempoHub.Converters;
using TempoHub.Models;
using TempoHub.ViewModels;
using TempoHub.ViewModels.Content_Displays;

namespace TempoHub.User_Controls.Content_Displays
{
    /// <summary>
    /// Interaction logic for SongDetailsContentDisplay.xaml
    /// </summary>
    public partial class SongDetailsContentDisplay : UserControl
    {
        public event EventHandler<AddToPlaylistEventArgs> AddToPlaylistClick;
        public event EventHandler<SongCopyPasteEventArgs> Copy;
        public event EventHandler<SongCopyPasteEventArgs> Paste;

        public SongDetailsContentDisplay()
        {
            InitializeComponent();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DataContext is SongDetailsContentDisplayViewModel vm)
            {
                vm.MultiSelectEnabled = songDataGrid.SelectedItems.Count > 1;
            }
        }

        public void OnAddToQueueClick(object sender, MouseButtonEventArgs e)
        {
            if(DataContext is SongDetailsContentDisplayViewModel vm)
            {
                if(vm.OnAddToQueueClickMethod != null)
                {
                    List<string> filePaths = new List<string>();
                    foreach(var selected in songDataGrid.SelectedItems)
                    {
                        if(selected is SongInfo songInfo)
                        {
                            filePaths.Add(songInfo.FilePath);
                        }
                    }

                    vm.OnAddToQueueClickMethod(filePaths);
                }
            }
        }

        private void OnAddToQueueClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongDetailsContentDisplayViewModel vm)
            {
                if(vm.OnAddToQueueClickMethod != null)
                {
                    List<string> filePaths = new List<string>();
                    foreach(var selected in songDataGrid.SelectedItems)
                    {
                        if(selected is SongInfo songInfo)
                        {
                            filePaths.Add(songInfo.FilePath);
                        }
                    }

                    vm.OnAddToQueueClickMethod(filePaths);
                }
            }
        }

        public void OnPlayClick(object sender, MouseButtonEventArgs e)
        {
            if(DataContext is SongDetailsContentDisplayViewModel vm && songDataGrid.SelectedItem is SongInfo songInfo)
            {
                if(vm.OnPlayClickMethod != null)
                {
                    vm.OnPlayClickMethod(songInfo.FilePath);
                }
            }
        }

        private void OnPlayClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongDetailsContentDisplayViewModel vm && songDataGrid.SelectedItem is SongInfo songInfo)
            {
                if(vm.OnPlayClickMethod != null)
                {
                    vm.OnPlayClickMethod(songInfo.FilePath);
                }
            }
        }

        private void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongDetailsContentDisplayViewModel vm)
            {
                if(vm.OnRemoveClickMethod != null)
                {
                    List<string> filePaths = new List<string>();
                    foreach(var selected in songDataGrid.SelectedItems)
                    {
                        if(selected is SongInfo songInfo)
                        {
                            filePaths.Add(songInfo.FilePath);
                        }
                    }

                    vm.OnRemoveClickMethod(filePaths);
                }
            }
        }

        public void OnEditSongInfoClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongDetailsContentDisplayViewModel vm && songDataGrid.SelectedItem is SongInfo songInfo)
            {
                if(vm.OnEditSongInfoClickMethod != null)
                {
                    vm.OnEditSongInfoClickMethod(songInfo.FilePath);
                }
            }
        }

        public void OnEditSongsInfoClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongDetailsContentDisplayViewModel vm)
            {
                if(vm.OnEditSongsInfoClickMethod != null)
                {
                    List<string> filePaths = new List<string>();
                    foreach(var selected in songDataGrid.SelectedItems)
                    {
                        if(selected is SongInfo songInfo)
                        {
                            filePaths.Add(songInfo.FilePath);
                        }
                    }

                    vm.OnEditSongsInfoClickMethod(filePaths);
                }
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if(DataContext is SongDetailsContentDisplayViewModel vm)
            {
                if(sender is TextBox searchBox)
                {
                    var propName = searchBox.DataContext.ToString();
                    
                    if(vm.Filters.ContainsKey(propName))
                    {
                        if(String.IsNullOrEmpty(searchBox.Text))
                        {
                            vm.Filters.Remove(propName);
                        }

                        else
                        {
                            vm.Filters[propName] = searchBox.Text;
                        }
                    }

                    else if(!String.IsNullOrEmpty(searchBox.Text))
                    {
                        vm.Filters.Add(propName, searchBox.Text);
                    }
                }

                vm.SongCollection.View.Refresh();
            }
        }

        private void OnSearchBoolSelectionChanged(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongDetailsContentDisplayViewModel vm)
            {
                if(sender is ComboBox searchComboBox)
                {
                    var propName = searchComboBox.DataContext.ToString();
                    bool? toggle = null;
                    if(searchComboBox.SelectedIndex == 1)
                    {
                        toggle = true;
                    }

                    else if(searchComboBox.SelectedIndex == 2)
                    {
                        toggle = false;
                    }

                    if(vm.Filters.ContainsKey(propName))
                    {
                        if(toggle is null)
                        {
                            vm.Filters.Remove(propName);
                        }

                        else
                        {
                            vm.Filters[propName] = toggle.ToString();
                        }
                    }

                    else if(toggle != null)
                    {
                        vm.Filters.Add(propName, toggle.ToString());
                    }
                }

                vm.SongCollection.View.Refresh();
            }
        }

        private void OnRatingValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if(DataContext is SongDetailsContentDisplayViewModel vm)
            {
                if(sender is NumericUpDown numBox)
                {
                    RatingsConverter converter = new RatingsConverter();

                    if(vm.Filters.ContainsKey("Rating"))
                    {
                        if(numBox.Value is null)
                        {
                            vm.Filters.Remove("Rating");
                        }

                        else
                        {
                            vm.Filters["Rating"] = converter.ConvertBack(numBox.Value, null, null, null).ToString();
                        }
                    }

                    else if(numBox.Value != null)
                    {
                        vm.Filters.Add("Rating", converter.ConvertBack(numBox.Value, null, null, null).ToString());
                    }

                    vm.SongCollection.View.Refresh();
                }
            }
        }

        private void OnAddToPlaylistClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongDetailsContentDisplayViewModel vm && sender is MenuItem playlistBtn)
            {
                AddToPlaylistEventArgs args = new AddToPlaylistEventArgs();
                args.PlaylistName = playlistBtn.Header as string;

                foreach(var selected in songDataGrid.SelectedItems)
                {
                    if(selected is SongInfo songInfo)
                    {
                        args.FilePaths.Add(songInfo.FilePath);
                    }
                }

                AddToPlaylistClick?.Invoke(this, args);
            }
        }

        private void OnCopyClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongDetailsContentDisplayViewModel vm && songDataGrid.SelectedItem is SongInfo songInfo)
            {
                SongCopyPasteEventArgs args = new SongCopyPasteEventArgs();
                args.FilePath = songInfo.FilePath;
                Copy?.Invoke(this, args);
            }
        }

        private void OnPasteClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SongDetailsContentDisplayViewModel vm && songDataGrid.SelectedItem is SongInfo songInfo)
            {
                SongCopyPasteEventArgs args = new SongCopyPasteEventArgs();
                args.FilePath = songInfo.FilePath;
                Paste?.Invoke(this, args);
            }
        }
    }
}
