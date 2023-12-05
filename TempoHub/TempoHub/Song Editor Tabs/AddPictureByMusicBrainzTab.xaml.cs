using ControlzEx.Standard;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.CoverArt;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using TempoHub.User_Controls;

namespace TempoHub.Song_Editor_Tabs
{
    /// <summary>
    /// Interaction logic for AddPictureByMusicBrainzTab.xaml
    /// </summary>
    public partial class AddPictureByMusicBrainzTab : UserControl
    {
        private static readonly string ApplicationName = "TempoHub";
        private static readonly string Version = "0.3";
        private static readonly string Contact = "mailto:jtross2892@gmail.com";
        public byte[] ImageDataToUse
        {
            get
            {
                foreach(var element in searchResultsStackPanel.Children)
                {
                    if(element is ImageSearchResultRow toUse && toUse.IsClicked)
                    {
                        return toUse.ImageData;
                    }
                }

                return null;
            }
        }
        public string ImageType
        {
            get
            {
                foreach(var element in searchResultsStackPanel.Children)
                {
                    if(element is ImageSearchResultRow toUse && toUse.IsClicked)
                    {
                        return toUse.ImageType;
                    }
                }

                return "image/jpeg";
            }
        }
        public Button UploadBtn { get; set; }

        public AddPictureByMusicBrainzTab()
        {
            InitializeComponent();
        }

        private async void OnSearchBtnClick(object sender, RoutedEventArgs e)
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

            var searchTerms = new List<string>() { albumNameQuery, artistQuery, "(country:us OR country:xw)" };
            var queryStr = String.Join(" AND ", searchTerms.Where(term => !String.IsNullOrEmpty(term)));
            var albumResults = await albumSearchEngine.FindReleasesAsync(queryStr);

            List<(IRelease album, CoverArtImage cover)> pairs = new List<(IRelease, CoverArtImage)>();
            foreach(var result in albumResults.Results.ToList())
            {
                var id = result.Item.Id;

                try
                {
                    Console.WriteLine("Searching for: " + id);
                    var cover = await coverArtSearchEngine.FetchFrontAsync(id);
                    pairs.Add((result.Item, cover));
                }

                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Thread.Sleep(1000);
            }

            // Display Covers
            foreach(var pair in pairs)
            {
                if(pair.cover.Data is MemoryStream imageStream)
                {
                    ImageSearchResultRow row = new ImageSearchResultRow() { ImageData = imageStream.ToArray() };
                    row.Height = 250;
                    row.albumNameLbl.Content = pair.album.Title;
                    row.songNameLbl.Visibility = Visibility.Collapsed;
                    row.ImageType = pair.cover.ContentType;

                    if(pair.album.ArtistCredit != null && pair.album.ArtistCredit.Count > 0)
                    {
                        row.artistNameLbl.Content = String.Join(", ", pair.album.ArtistCredit.Select(credit => credit.Name));
                    }

                    else
                    {
                        row.artistNameLbl.Visibility = Visibility.Collapsed;
                    }

                    row.OnRowClickMethod = () =>
                    {
                        foreach(var element in searchResultsStackPanel.Children)
                        {
                            if(element is ImageSearchResultRow toUpdate)
                            {
                                toUpdate.IsClicked = false;
                            }
                        }

                        row.IsClicked = true;
                        UploadBtn.Visibility = Visibility.Visible;
                    };

                    imageStream.Dispose();
                    searchResultsStackPanel.Children.Add(row);
                }
            }
        }
    }
}
