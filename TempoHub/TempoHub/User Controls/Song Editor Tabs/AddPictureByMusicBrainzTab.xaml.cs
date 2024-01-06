using MetaBrainz.MusicBrainz.CoverArt;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TempoHub.ViewModels;
using TempoHub.ViewModels.Song_Editor_Tabs;
using Query = MetaBrainz.MusicBrainz.Query;

namespace TempoHub.User_Controls.Song_Editor_Tabs
{
    /// <summary>
    /// Interaction logic for AddPictureByMusicBrainzTab.xaml
    /// </summary>
    public partial class AddPictureByMusicBrainzTab : UserControl
    {
        private static readonly string ApplicationName = "TempoHub";
        private static readonly string Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
        private static readonly string Contact = "mailto:jtross2892@gmail.com";

        public AddPictureByMusicBrainzTab()
        {
            InitializeComponent();
        }

        private async void OnSearchBtnClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is AddPictureByMusicBrainzTabViewModel vm)
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

                var albumNameQuery = albumNameTextBox.Text;
                var artistQuery = String.IsNullOrEmpty(artistNameTextBox.Text) ? "" : "artist:" + artistNameTextBox.Text;
                var countryQuery = allCountriesCheckBox.IsChecked == true ? "" : "(country:us OR country:xw)";

                var searchTerms = new List<string>() { albumNameQuery, artistQuery, countryQuery };
                var queryStr = String.Join(" AND ", searchTerms.Where(term => !String.IsNullOrEmpty(term)));
                var albumResults = await albumSearchEngine.FindReleasesAsync(queryStr, limit: (int?) maxResultCounter.Value);

                vm.StartLoadingDisplay(0, albumResults.Results.Count, 0);

                List<(IRelease album, CoverArtImage cover)> pairs = new List<(IRelease, CoverArtImage)>();
                foreach(var result in albumResults.Results.ToList())
                {
                    var id = result.Item.Id;

                    try
                    {
                        Trace.WriteLine("Searching for: " + id);
                        var cover = await coverArtSearchEngine.FetchFrontAsync(id);
                        pairs.Add((result.Item, cover));
                    }

                    catch(Exception)
                    {
                        //Console.WriteLine(ex);
                    }

                    vm.AddValueLoadingDisplay(1);
                    await Task.Delay(1100); // This is to ensure I don't hit the API limit
                }

                // Display Covers
                foreach(var pair in pairs)
                {
                    if(pair.cover.Data is MemoryStream imageStream)
                    {
                        ImageSearchResultRowViewModel rowVm = new ImageSearchResultRowViewModel();
                        rowVm.ImageData = imageStream.ToArray();
                        rowVm.ImageType = pair.cover.ContentType;
                        rowVm.Height = 250;
                        rowVm.Album = pair.album.Title;
                        rowVm.SongTitle = "";

                        if(pair.album.ArtistCredit != null && pair.album.ArtistCredit.Count > 0)
                        {
                            rowVm.Artist = String.Join(", ", pair.album.ArtistCredit.Select(credit => credit.Name));
                        }

                        imageStream.Dispose();
                        vm.SearchResults.Add(rowVm);
                    }
                }

                vm.StopLoadingDisplay();
            }
        }

        private void OnSearchResultsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DataContext is AddPictureByMusicBrainzTabViewModel vm)
            {
                if(vm.SetUploadBtnVisibilityMethod != null)
                {
                    vm.SetUploadBtnVisibilityMethod(Visibility.Visible);
                }
            }
        }
    }
}
