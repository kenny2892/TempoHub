using ControlzEx.Standard;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.CoverArt;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TagLib;
using TempoHub.Models;
using TempoHub.Settings;
using TempoHub.ViewModels;
using TempoHub.ViewModels.Selection_Displays;

namespace TempoHub.Views
{
    /// <summary>
    /// Interaction logic for TagSearchWindow.xaml
    /// </summary>
    public partial class TagSearchWindow : MetroWindow
    {
        private static readonly string ApplicationName = "TempoHub";
        private static readonly string Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
        private static readonly string Contact = "mailto:jtross2892@gmail.com";

        public TagSearchWindow(ThemeBase themeBase, ThemeColor themeColor)
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, $"{themeBase}.{themeColor}");
            albumCoverImg.image.VerticalAlignment = VerticalAlignment.Top;
        }

        private async void OnSearchBtnClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is TagSearchWindowViewModel vm)
            {
                if(String.IsNullOrEmpty(albumNameTextBox.Text) && String.IsNullOrEmpty(artistNameTextBox.Text))
                {
                    return;
                }

                // MusicBrainz Terms:
                // Recording = song
                // Release = album
                // Release Group = variants of an album (like the american and the japanese versions of the same album)

                // While the normal query is supposed to provide confirmation on whether or not a Cover Art is available, all my tests have come back incorrect
                // So I have to manually test each song to check for a Cover Art
                var albumSearchEngine = new Query(ApplicationName, Version, Contact);
                var coverArtSearchEngine = new CoverArt(ApplicationName, Version, Contact);

                var albumNameQuery = vm.AlbumNameSearch;
                var artistQuery = String.IsNullOrEmpty(vm.ArtistNameSearch) ? "" : "artist:" + vm.ArtistNameSearch;
                var countryQuery = allCountriesCheckBox.IsChecked == true ? "" : "(country:us OR country:xw)";
                var dateQuery = String.IsNullOrEmpty(vm.YearSearch) ? "" : "date:" + vm.YearSearch;

                var searchTerms = new List<string>() { albumNameQuery, artistQuery, countryQuery, dateQuery };
                var queryStr = String.Join(" AND ", searchTerms.Where(term => !String.IsNullOrEmpty(term)));
                var albumResults = await albumSearchEngine.FindReleasesAsync(queryStr, limit: (int?) maxResultCounter.Value);

                vm.LoadingVisibility = Visibility.Visible;
                vm.LoadingDisplayVm.Start(0, albumResults.Results.Count, 0);

                foreach(var result in albumResults.Results.ToList())
                {
                    var id = result.Item.Id;
                    var toAdd = new SelectionRowMusicBrainzResultViewModel();

                    try
                    {
                        var albumResult = result.Item;
                        await Task.Delay(1100); // This is to ensure I don't hit the API limit
                        var albumWithSongsResult = albumSearchEngine.LookupRelease(id, Include.Recordings);

                        if(albumResult != null && albumWithSongsResult != null)
                        {
                            toAdd.MusicBrainzAlbumSearchResult = albumResult;
                            toAdd.AlbumName = albumResult.Title;
                            toAdd.Year = albumResult.Date.Year + "";

                            if(albumWithSongsResult.Media?.Count > 0 && albumWithSongsResult.Media[0].Tracks?.Count > 0)
                            {
                                toAdd.SongCount = albumWithSongsResult.Media[0].Tracks.Count + " Songs";
                                toAdd.Songs.AddRange(albumWithSongsResult.Media[0].Tracks);
                                toAdd.Songs.OrderBy(track => track.Position);
                            }

                            if(albumResult.ArtistCredit != null && albumResult.ArtistCredit.Count > 0)
                            {
                                toAdd.ArtistName = String.Join(", ", albumResult.ArtistCredit.Select(credit => credit.Name));
                            }

                            var cover = await coverArtSearchEngine.FetchFrontAsync(id);
                            if(cover.Data is MemoryStream imageStream)
                            {
                                toAdd.ImageData = imageStream.ToArray();
                                toAdd.MimeType = cover.ContentType;
                                imageStream.Dispose();
                            }
                        }
                    }

                    catch(Exception)
                    {
                        //Console.WriteLine(ex);
                    }

                    if(toAdd.MusicBrainzAlbumSearchResult != null)
                    {
                        vm.SelectionVm.Items.Add(toAdd);
                    }

                    vm.LoadingDisplayVm.CurrentValue += 1;
                    await Task.Delay(1100); // This is to ensure I don't hit the API limit
                }

                vm.PairingText = "";
                vm.SelectionVm.Filter();
                vm.LoadingVisibility = Visibility.Collapsed;
                vm.LoadingDisplayVm.Stop();
            }
        }

        private void OnResultSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DataContext is TagSearchWindowViewModel vm)
            {
                if(vm.SelectionVm.SelectedIndex < 0)
                {
                    vm.AlbumName = "";
                    vm.Year = "";
                    vm.ArtistName = "";
                    vm.AlbumArtists = "";
                    vm.Songs.Clear();
                    vm.ImageVm = new AspectRatioImageViewModel();
                    return;
                }

                var selectedRow = vm.SelectionVm.ItemsFiltered[vm.SelectionVm.SelectedIndex] as SelectionRowMusicBrainzResultViewModel;

                if(selectedRow != null)
                {
                    vm.AlbumName = selectedRow.AlbumName;
                    vm.Year = selectedRow.Year;

                    // Artists
                    if(selectedRow.MusicBrainzAlbumSearchResult.ArtistCredit != null && selectedRow.MusicBrainzAlbumSearchResult.ArtistCredit.Count > 0)
                    {
                        vm.ArtistName = selectedRow.MusicBrainzAlbumSearchResult.ArtistCredit[0].Name;

                        if(selectedRow.MusicBrainzAlbumSearchResult.ArtistCredit.Count > 1)
                        {
                            vm.AlbumArtists = String.Join(", ", selectedRow.MusicBrainzAlbumSearchResult.ArtistCredit.Skip(1).Select(credit => credit.Name));
                        }

                        else
                        {
                            vm.AlbumArtists = "";
                        }
                    }

                    // Songs
                    vm.Songs.Clear();
                    for(int i = 0; i < selectedRow.Songs.Count; i++)
                    {
                        var songToAdd = selectedRow.Songs[i];
                        var genres = songToAdd.Recording?.Genres is null ? "" : String.Join(", ", songToAdd.Recording.Genres.Select(genre => genre.Name));
                        double songToAddTotalMilliseconds = songToAdd.Length != null ? songToAdd.Length.Value.TotalMilliseconds : 0;

                        var result = new MusicBrainzSongResultViewModel()
                        {
                            Index = (int) songToAdd.Position,
                            Song = songToAdd.Title,
                            Length = (songToAdd.Length != null ? new DateTime(songToAdd.Length.Value.Ticks).ToString("HH:mm:ss") : ""),
                            Genres = (songToAdd.Recording?.Genres is null ? new List<string>() : songToAdd.Recording.Genres.Select(genre => genre.Name).ToList()),
                            FilePathOptions = vm.SongsToUpdate.Select(song => song.FilePath).ToList()
                        };

                        // Check for Matches
                        var match = vm.SongsToUpdate.FirstOrDefault(songToCheck =>
                        {
                            var titleMatch = CheckIfTitlesMatch(songToCheck.Title, result.Song);
                            var lengthMatch = Math.Abs(songToCheck.SongLengthMilliseconds - songToAddTotalMilliseconds) < 10000;

                            return titleMatch && lengthMatch && vm.Songs.All(existing => existing.MatchingFilePath != songToCheck.FilePath);
                        });

                        if(match != null)
                        {
                            result.MatchingFilePath = match.FilePath;
                            result.Track = songToAdd;
                        }

                        vm.Songs.Add(result);
                    }

                    // Cover Art
                    if(selectedRow.ImageData != null && selectedRow.ImageData.Length > 0)
                    {
                        AspectRatioImageViewModel imageVm = new AspectRatioImageViewModel() { ImageData = selectedRow.ImageData };
                        vm.ImageVm = imageVm;
                    }

                    else
                    {
                        vm.ImageVm = new AspectRatioImageViewModel();
                    }

                    // Pairing Count
                    vm.PairingText = $"{vm.Songs.Count(song => !String.IsNullOrEmpty(song.MatchingFilePath))} / {vm.SongsToUpdate.Count} Paired";
                }
            }
        }

        private bool CheckIfTitlesMatch(string fileTitle, string musicBrainzResultTitle)
        {
            string fileTitleLower = fileTitle is null ? "" : fileTitle.ToLower();
            string musicBrainzResultTitleLower = musicBrainzResultTitle is null ? "" : musicBrainzResultTitle.ToLower();

            var match = fileTitleLower.Contains(musicBrainzResultTitleLower) || musicBrainzResultTitleLower.Contains(fileTitleLower);

            if(!match)
            {
                // Swap out And
                fileTitleLower = fileTitleLower.Replace("&", "and");
                musicBrainzResultTitleLower = musicBrainzResultTitleLower.Replace("&", "and");

                // Remove other characters
                Regex cleanUp = new Regex("[^a-zA-Z0-9 ]");
                fileTitleLower = cleanUp.Replace(fileTitleLower, "");
                musicBrainzResultTitleLower = cleanUp.Replace(musicBrainzResultTitleLower, "");

                match = fileTitleLower.Contains(musicBrainzResultTitleLower) || musicBrainzResultTitleLower.Contains(fileTitleLower);
            }

            return match;
        }

        private void OnRefreshRequested(object sender, string e)
        {
            if(DataContext is TagSearchWindowViewModel vm)
            {
                vm.SelectionVm.Filter();
            }
        }

        private void OnApplyBtnClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is TagSearchWindowViewModel vm)
            {
                vm.HitApply = true;
                Close();
            }
        }

        private void OnCancelBtnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnFilePathSelectionChanged(object sender, RoutedEventArgs e)
        {
            if(sender is MenuItem menuItem)
            {
                var contextMenu = FindParent<ContextMenu>((DependencyObject) sender);
                if(contextMenu != null && contextMenu.DataContext is MusicBrainzSongResultViewModel resultVm && 
                    menuItem.Header is string filePathSelected && DataContext is TagSearchWindowViewModel vm)
                {
                    resultVm.MatchingFilePath = resultVm.MatchingFilePath == filePathSelected ? "" : filePathSelected;

                    if(resultVm.MatchingFilePath == filePathSelected)
                    {
                        // Deselect it from all other songs
                        foreach(var toDeactivate in vm.Songs.Where(song => song != resultVm && song.MatchingFilePath == filePathSelected))
                        {
                            toDeactivate.MatchingFilePath = "";
                        }
                    }

                    // Pairing Count
                    vm.PairingText = $"{vm.Songs.Count(song => !String.IsNullOrEmpty(song.MatchingFilePath))} / {vm.SongsToUpdate.Count} Paired";
                }
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if(parentObject == null)
            {
                return null;
            }

            if(parentObject is T parent)
            {
                return parent;
            }

            return FindParent<T>(parentObject);
        }
    }
}
