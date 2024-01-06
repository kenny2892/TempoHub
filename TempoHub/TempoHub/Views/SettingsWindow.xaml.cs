using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using TempoHub.ViewModels;
using TempoHub.Settings;
using Newtonsoft.Json;
using System.IO;
using Path = System.IO.Path;
using ControlzEx.Theming;

namespace TempoHub.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow(ThemeBase themeBase, ThemeColor themeColor)
        {
            InitializeComponent();
            errorMsgGrid.Visibility = Visibility.Collapsed;
            ThemeManager.Current.ChangeTheme(this, $"{themeBase}.{themeColor}");
        }

        private void OnApplyClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SettingsWindowViewModel vm)
            {
                var savePath = Path.Join(Directory.GetCurrentDirectory(), "Configuration.json");

                if(IsFileOpen(savePath))
                {
                    errorMsgGrid.Visibility = Visibility.Visible;
                    return;
                }

                vm.Settings.DefaultDisplay = vm.DefaultDisplay;
                vm.Settings.ThemeBase = vm.ThemeBase;
                vm.Settings.ThemeColor = vm.ThemeColor;
                vm.Settings.MaxHistoryRecords = vm.MaxHistoryRecords;

                vm.Settings.FilePathSort = vm.FilePath.Sort;
                vm.Settings.FilePathIndex = vm.FilePath.Index;
                vm.Settings.FilePathIsEnabled = vm.FilePath.IsEnabled;

                vm.Settings.TitleSort = vm.Title.Sort;
                vm.Settings.TitleIndex = vm.Title.Index;
                vm.Settings.TitleIsEnabled = vm.Title.IsEnabled;

                vm.Settings.AlbumSort = vm.Album.Sort;
                vm.Settings.AlbumIndex = vm.Album.Index;
                vm.Settings.AlbumIsEnabled = vm.Album.IsEnabled;

                vm.Settings.ArtistSort = vm.Artist.Sort;
                vm.Settings.ArtistIndex = vm.Artist.Index;
                vm.Settings.ArtistIsEnabled = vm.Artist.IsEnabled;

                vm.Settings.AlbumArtistSort = vm.AlbumArtist.Sort;
                vm.Settings.AlbumArtistIndex = vm.AlbumArtist.Index;
                vm.Settings.AlbumArtistIsEnabled = vm.AlbumArtist.IsEnabled;

                vm.Settings.GenresSort = vm.Genres.Sort;
                vm.Settings.GenresIndex = vm.Genres.Index;
                vm.Settings.GenresIsEnabled = vm.Genres.IsEnabled;

                vm.Settings.ComposerSort = vm.Composer.Sort;
                vm.Settings.ComposerIndex = vm.Composer.Index;
                vm.Settings.ComposerIsEnabled = vm.Composer.IsEnabled;

                vm.Settings.PublisherSort = vm.Publisher.Sort;
                vm.Settings.PublisherIndex = vm.Publisher.Index;
                vm.Settings.PublisherIsEnabled = vm.Publisher.IsEnabled;

                vm.Settings.ConductorSort = vm.Conductor.Sort;
                vm.Settings.ConductorIndex = vm.Conductor.Index;
                vm.Settings.ConductorIsEnabled = vm.Conductor.IsEnabled;

                vm.Settings.GroupingSort = vm.Grouping.Sort;
                vm.Settings.GroupingIndex = vm.Grouping.Index;
                vm.Settings.GroupingIsEnabled = vm.Grouping.IsEnabled;

                vm.Settings.SongLengthSort = vm.SongLength.Sort;
                vm.Settings.SongLengthIndex = vm.SongLength.Index;
                vm.Settings.SongLengthIsEnabled = vm.SongLength.IsEnabled;

                vm.Settings.YearSort = vm.Year.Sort;
                vm.Settings.YearIndex = vm.Year.Index;
                vm.Settings.YearIsEnabled = vm.Year.IsEnabled;

                vm.Settings.TrackCurrSort = vm.TrackCurr.Sort;
                vm.Settings.TrackCurrIndex = vm.TrackCurr.Index;
                vm.Settings.TrackCurrIsEnabled = vm.TrackCurr.IsEnabled;

                vm.Settings.TrackTotalSort = vm.TrackTotal.Sort;
                vm.Settings.TrackTotalIndex = vm.TrackTotal.Index;
                vm.Settings.TrackTotalIsEnabled = vm.TrackTotal.IsEnabled;

                vm.Settings.DiscCurrSort = vm.DiscCurr.Sort;
                vm.Settings.DiscCurrIndex = vm.DiscCurr.Index;
                vm.Settings.DiscCurrIsEnabled = vm.DiscCurr.IsEnabled;

                vm.Settings.DiscTotalSort = vm.DiscTotal.Sort;
                vm.Settings.DiscTotalIndex = vm.DiscTotal.Index;
                vm.Settings.DiscTotalIsEnabled = vm.DiscTotal.IsEnabled;

                vm.Settings.RatingSort = vm.Rating.Sort;
                vm.Settings.RatingIndex = vm.Rating.Index;
                vm.Settings.RatingIsEnabled = vm.Rating.IsEnabled;

                vm.Settings.BpmSort = vm.Bpm.Sort;
                vm.Settings.BpmIndex = vm.Bpm.Index;
                vm.Settings.BpmIsEnabled = vm.Bpm.IsEnabled;

                vm.Settings.CommentSort = vm.Comment.Sort;
                vm.Settings.CommentIndex = vm.Comment.Index;
                vm.Settings.CommentIsEnabled = vm.Comment.IsEnabled;

                vm.Settings.HasLyricsSort = vm.HasLyrics.Sort;
                vm.Settings.HasLyricsIndex = vm.HasLyrics.Index;
                vm.Settings.HasLyricsIsEnabled = vm.HasLyrics.IsEnabled;

                vm.Settings.HasAlbumCoverSort = vm.HasAlbumCover.Sort;
                vm.Settings.HasAlbumCoverIndex = vm.HasAlbumCover.Index;
                vm.Settings.HasAlbumCoverIsEnabled = vm.HasAlbumCover.IsEnabled;

                vm.Settings.DateAddedSort = vm.DateAdded.Sort;
                vm.Settings.DateAddedIndex = vm.DateAdded.Index;
                vm.Settings.DateAddedIsEnabled = vm.DateAdded.IsEnabled;

                string json = JsonConvert.SerializeObject(vm.Settings);
                File.WriteAllText(savePath, json);

                Close();
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool IsFileOpen(string filePath)
        {
            if(!File.Exists(filePath))
            {
                return false;
            }

            FileInfo fileInfo = new FileInfo(filePath);
            bool isOpen = false;

            try
            {
                using(FileStream stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }

            catch(IOException)
            {
                isOpen = true;
            }

            return isOpen;
        }

        private void OnCloseErrorMsgClick(object sender, RoutedEventArgs e)
        {
            errorMsgGrid.Visibility = Visibility.Collapsed;
        }

        private void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DataContext is SettingsWindowViewModel vm)
            {
                ThemeManager.Current.ChangeTheme(this, $"{vm.ThemeBase}.{vm.ThemeColor}");
            }
        }
    }
}
