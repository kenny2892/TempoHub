using MahApps.Metro.Controls;
using MahApps.Metro.Converters;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using TagLib.Id3v2;
using TempoHub.Converters;
using TempoHub.Models;
using TempoHub.User_Controls;
using File = System.IO.File;

namespace TempoHub
{
    /// <summary>
    /// Interaction logic for SongEditWindow.xaml
    /// </summary>
    public partial class SongEditWindow : MetroWindow
    {
        private SongFile Song { get; set; }
        public bool ChangesWereApplied { get { return TitleChanged || AlbumNameChanged || AlbumCoverChanged || ArtistChanged; } }
        public bool TitleChanged { get; private set; } = false;
        public bool AlbumNameChanged { get; private set; } = false;
        public bool AlbumCoverChanged { get; private set; } = false;
        public bool ArtistChanged { get; private set; } = false;

        public SongEditWindow(SongFile songFile)
        {
            Song = songFile;
            InitializeComponent();
            SetupDefaults();
            ShowDetailsTab();
            ShowPicByFileTab();
        }

        private void SetupDefaults()
        {
            // Setup Initial Main Tab States
            mainContentGrid.Visibility = Visibility.Visible;
            pictureAddGrid.Visibility = Visibility.Collapsed;
            LoadAlbumCover();

            // Load Main Tabs
            detailsStackPanel.LoadDetailsTab(Song);
            picturesTabContent.LoadPicturesTab(Song);
            picturesTabContent.OnAddPictureClickMethod = () => OnAddPictureClick();

            // Setup Main Tab Btns
            detailsTabBtn.OnMouseUpMethod = () => ShowDetailsTab();
            picturesTabBtn.OnMouseUpMethod = () => ShowPicturesTab();
            OnMainTabMouseUp(detailsTabBtn, null);

            // Setup Picture Add Tabs
            picByFileTab.OnMouseUpMethod = () => ShowPicByFileTab();
            picByMusicBrainzTab.OnMouseUpMethod = () => ShowPicByMusicBrainzTab();
            picByUrlTab.OnMouseUpMethod = () => ShowPicByUrlTab();
            OnPictureAddTabMouseUp(picByFileTab, null);

            // Setup Initial Picture Add States
            pictureAddUploadBtn.Visibility = Visibility.Collapsed;
            picByFileContent.UploadBtn = pictureAddUploadBtn;
            picByMusicBrainzContent.UploadBtn = pictureAddUploadBtn;
        }

        private void ShowDetailsTab()
        {
            detailsStackPanel.Visibility = Visibility.Visible;
            picturesTabContent.Visibility = Visibility.Collapsed;
            lyricsStackPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowPicturesTab()
        {
            detailsStackPanel.Visibility = Visibility.Collapsed;
            picturesTabContent.Visibility = Visibility.Visible;
            lyricsStackPanel.Visibility = Visibility.Collapsed;
        }

        private void LoadAlbumCover()
        {
            if(Song.TagLibFile.Tag.Pictures.Count() > 0)
            {
                albumCoverImg.SetImage(Song.TagLibFile.Tag.Pictures[0].Data.Data);
            }
        }

        private void OnTabMouseEnter(object sender, MouseEventArgs e)
        {
            if(sender is ToggleButton btn)
            {
                if(!btn.IsClicked)
                {
                    btn.Background = (Brush) FindResource("MahApps.Brushes.Button.Flat.Foreground");
                }
            }
        }

        private void OnTabMouseLeave(object sender, MouseEventArgs e)
        {
            if(sender is ToggleButton btn)
            {
                if(!btn.IsClicked)
                {
                    btn.Background = (Brush) FindResource("MahApps.Brushes.Button.AccentedSquare.Background.MouseOver");
                }
            }
        }

        private void OnMainTabMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(sender is ToggleButton btn)
            {
                // Deactivate all Tabs
                foreach(var element in tabStackPanel.Children)
                {
                    if(element is ToggleButton toDeactivate)
                    {
                        toDeactivate.IsClicked = false;
                        toDeactivate.Background = (Brush) FindResource("MahApps.Brushes.Button.AccentedSquare.Background.MouseOver");
                    }
                }

                if(!btn.IsClicked)
                {
                    btn.IsClicked = true;
                    btn.Background = (Brush) FindResource("MahApps.Brushes.Button.Flat.Background.Pressed");
                }
            }
        }

        private void OnApplyBtnClick(object sender, RoutedEventArgs e)
        {
            SaveStringTags();
            SaveIntTags();
            SaveArrayTags();
            SaveRatingsTags();
            SavePictureTags();

            Song.TagLibFile.Save();
            Close();
        }

        private void SaveStringTags()
        {
            TitleChanged = Song.TagLibFile.Tag.Title != detailsStackPanel.titleInput.Text;
            AlbumNameChanged = Song.TagLibFile.Tag.Album != detailsStackPanel.albumInput.Text;

            Song.TagLibFile.Tag.Title = detailsStackPanel.titleInput.Text;
            Song.TagLibFile.Tag.Album = detailsStackPanel.albumInput.Text;
            Song.TagLibFile.Tag.Publisher = detailsStackPanel.publisherInput.Text;
            Song.TagLibFile.Tag.Conductor = detailsStackPanel.conductorInput.Text;
            Song.TagLibFile.Tag.Grouping = detailsStackPanel.groupingInput.Text;
            Song.TagLibFile.Tag.Comment = detailsStackPanel.commentInput.Text;
        }

        private void SaveIntTags()
        {
            if(uint.TryParse(detailsStackPanel.yearInput.Text, out uint year) && year >= 0)
            {
                Song.TagLibFile.Tag.Year = year;
            }

            if(uint.TryParse(detailsStackPanel.trackCurrInput.Text, out uint trackCurr) && trackCurr >= 0)
            {
                Song.TagLibFile.Tag.Track = trackCurr;
            }

            if(uint.TryParse(detailsStackPanel.trackTotalInput.Text, out uint trackTotal) && trackTotal >= 0)
            {
                Song.TagLibFile.Tag.TrackCount = trackTotal;
            }

            if(uint.TryParse(detailsStackPanel.discCurrInput.Text, out uint discCurr) && discCurr >= 0)
            {
                Song.TagLibFile.Tag.Disc = discCurr;
            }

            if(uint.TryParse(detailsStackPanel.discTotalInput.Text, out uint discTotal) && discTotal >= 0)
            {
                Song.TagLibFile.Tag.DiscCount = discTotal;
            }

            if(uint.TryParse(detailsStackPanel.bpmInput.Text, out uint bpm) && bpm >= 0)
            {
                Song.TagLibFile.Tag.BeatsPerMinute = bpm;
            }
        }

        private void SaveArrayTags()
        {
            ArtistChanged = Song.TagLibFile.Tag.FirstPerformer != detailsStackPanel.artistInput.Text;

            if(!String.IsNullOrEmpty(detailsStackPanel.artistInput.Text))
            {
                if(Song.TagLibFile.Tag.Performers.Length == 0)
                {
                    Song.TagLibFile.Tag.Performers = new string[] { detailsStackPanel.artistInput.Text };
                }

                else
                {
                    Song.TagLibFile.Tag.Performers[0] = detailsStackPanel.artistInput.Text;
                }
            }

            if(!String.IsNullOrEmpty(detailsStackPanel.composerInput.Text))
            {
                if(Song.TagLibFile.Tag.Composers.Length == 0)
                {
                    Song.TagLibFile.Tag.Composers = new string[] { detailsStackPanel.composerInput.Text };
                }

                else
                {
                    Song.TagLibFile.Tag.Composers[0] = detailsStackPanel.composerInput.Text;
                }
            }

            if(!String.IsNullOrEmpty(detailsStackPanel.albumArtistInput.Text))
            {
                var parts = detailsStackPanel.albumArtistInput.Text.Split(", ");
                Song.TagLibFile.Tag.AlbumArtists = parts;
            }

            if(!String.IsNullOrEmpty(detailsStackPanel.genresInput.Text))
            {
                var parts = detailsStackPanel.genresInput.Text.Split(", ");
                Song.TagLibFile.Tag.Genres = parts;
            }
        }

        private void SaveRatingsTags()
        {
            if(detailsStackPanel.ratingsGrid.Visibility == Visibility.Visible)
            {
                var id3v2Tag = (TagLib.Id3v2.Tag) Song.TagLibFile.GetTag(TagLib.TagTypes.Id3v2, true);
                string ratingUserToUse = "Windows Media Player 9 Series";
                foreach(TagLib.Id3v2.Frame item in id3v2Tag)
                {
                    PopularimeterFrame popularimeterFrame = item as PopularimeterFrame;
                    if(popularimeterFrame != null)
                    {
                        ratingUserToUse = popularimeterFrame.User;
                        break;
                    }
                }

                PopularimeterFrame extraInfo = PopularimeterFrame.Get(id3v2Tag, ratingUserToUse, true);
                var percentage = 5.0 / detailsStackPanel.starRating.Rating;
                extraInfo.Rating = (byte) (percentage * 255);
            }
        }

        private void SavePictureTags()
        {
            int selectedIndex = -1;
            int index = 0;
            foreach(var element in picturesTabContent.pictureOptionsStackPanel.Children)
            {
                if(element is EditorPicture pic && pic.IsSelected)
                {
                    selectedIndex = index;
                    break;
                }

                index++;
            }

            if(selectedIndex > 0)
            {
                AlbumCoverChanged = true;
                var selectedPic = Song.TagLibFile.Tag.Pictures[selectedIndex];
                var newList = Song.TagLibFile.Tag.Pictures.ToList();
                newList.Remove(selectedPic);
                newList.Insert(0, selectedPic);
                Song.TagLibFile.Tag.Pictures = newList.ToArray();
            }
        }

        private void OnCancelBtnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowPicByFileTab()
        {
            picByFileContent.Visibility = Visibility.Visible;
            picByMusicBrainzContent.Visibility = Visibility.Collapsed;
            picByUrlGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowPicByMusicBrainzTab()
        {
            picByFileContent.Visibility = Visibility.Collapsed;
            picByMusicBrainzContent.Visibility = Visibility.Visible;
            picByUrlGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowPicByUrlTab()
        {
            picByFileContent.Visibility = Visibility.Collapsed;
            picByMusicBrainzContent.Visibility = Visibility.Collapsed;
            picByUrlGrid.Visibility = Visibility.Visible;
        }

        private void OnPictureAddTabMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(sender is ToggleButton btn)
            {
                // Deactivate all Tabs
                foreach(var element in picAddTabStackPanel.Children)
                {
                    if(element is ToggleButton toDeactivate)
                    {
                        toDeactivate.IsClicked = false;
                        toDeactivate.Background = (Brush) FindResource("MahApps.Brushes.Button.AccentedSquare.Background.MouseOver");
                    }
                }

                if(!btn.IsClicked)
                {
                    btn.IsClicked = true;
                    btn.Background = (Brush) FindResource("MahApps.Brushes.Button.Flat.Background.Pressed");
                }
            }
        }

        private void OnAddPictureClick()
        {
            mainContentGrid.Visibility = Visibility.Collapsed;
            pictureAddGrid.Visibility = Visibility.Visible;
        }

        private void OnAddPictureUploadClick(object sender, RoutedEventArgs e)
        {
            if(picByFileContent.Visibility == Visibility.Visible)
            {
                var mimeType = picByFileContent.ImagePathToImport.EndsWith(".png") ? "image/png" : "image/jpeg";

                AttachmentFrame picToAdd = new AttachmentFrame
                {
                    Type = TagLib.PictureType.FrontCover,
                    Description = "Album Cover",
                    MimeType = mimeType,
                    TextEncoding = TagLib.StringType.UTF16,
                    Data = File.ReadAllBytes(picByFileContent.ImagePathToImport)
                };

                var newList = Song.TagLibFile.Tag.Pictures.ToList();
                newList.Insert(0, picToAdd);
                Song.TagLibFile.Tag.Pictures = newList.ToArray();
                picturesTabContent.LoadPicturesTab(Song);
            }

            else if(picByMusicBrainzTab.Visibility == Visibility.Visible)
            {
                AttachmentFrame picToAdd = new AttachmentFrame
                {
                    Type = TagLib.PictureType.FrontCover,
                    Description = "Album Cover",
                    MimeType = picByMusicBrainzContent.ImageType,
                    TextEncoding = TagLib.StringType.UTF16,
                    Data = picByMusicBrainzContent.ImageDataToUse
                };

                var newList = Song.TagLibFile.Tag.Pictures.ToList();
                newList.Insert(0, picToAdd);
                Song.TagLibFile.Tag.Pictures = newList.ToArray();
                picturesTabContent.LoadPicturesTab(Song);
            }

            else
            {

            }

            AlbumCoverChanged = true;
            mainContentGrid.Visibility = Visibility.Visible;
            pictureAddGrid.Visibility = Visibility.Collapsed;
        }

        private void OnAddPictureCancelClick(object sender, RoutedEventArgs e)
        {
            mainContentGrid.Visibility = Visibility.Visible;
            pictureAddGrid.Visibility = Visibility.Collapsed;
        }
    }
}
