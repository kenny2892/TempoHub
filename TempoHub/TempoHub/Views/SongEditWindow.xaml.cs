using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System;
using System.Formats.Tar;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TagLib.Id3v2;
using TempoHub.Models;
using TempoHub.Services;
using TempoHub.Settings;
using TempoHub.User_Controls;
using TempoHub.ViewModels;
using File = System.IO.File;

namespace TempoHub.Views
{
    /// <summary>
    /// Interaction logic for SongEditWindow.xaml
    /// </summary>
    public partial class SongEditWindow : MetroWindow
    {
        public bool ChangesWereApplied { get { return TitleChanged || AlbumNameChanged || AlbumCoverChanged || ArtistChanged || TrackNumChanged; } }
        public bool TitleChanged { get; private set; } = false;
        public bool AlbumNameChanged { get; private set; } = false;
        public bool AlbumCoverChanged { get; private set; } = false;
        public bool ArtistChanged { get; private set; } = false;
        public bool TrackNumChanged { get; private set; } = false;
        private SongInfo InitialSongInfo { get; set; }
        private SongEditWindowViewModel SongEditorVm { get; set; }
        private Action OpenTagSearchMethod { get; set; }

        public SongEditWindow(ThemeBase themeBase, ThemeColor themeColor, Action openTagSearchMethod, SongInfo songInfo, params SongFile[] songsToEdit)
        {
            InitializeComponent();
            SongEditorVm = new SongEditWindowViewModel(songInfo, songsToEdit, OnAddPictureClick);
            InitialSongInfo = songInfo;
            DataContext = SongEditorVm;

            SetupDefaults();
            ShowDetailsTab();
            ShowPicByFileTab();
            ThemeManager.Current.ChangeTheme(this, $"{themeBase}.{themeColor}");
            OpenTagSearchMethod = openTagSearchMethod;
        }

        private void SetupDefaults()
        {
            // Setup Initial Main Tab States
            mainContentGrid.Visibility = Visibility.Visible;
            pictureAddGrid.Visibility = Visibility.Collapsed;

            // Setup Main Tab Btns
            detailsTabBtn.OnMouseUpMethod = () => ShowDetailsTab();
            picturesTabBtn.OnMouseUpMethod = () => ShowPicturesTab();
            lyricsTabBtn.OnMouseUpMethod = () => ShowLyricsTab();
            sortingTabBtn.OnMouseUpMethod = () => ShowSortingTab();
            infoTabBtn.OnMouseUpMethod = () => ShowInfoTab();

            // Setup Picture Add Tabs
            picByFileTab.OnMouseUpMethod = () => ShowPicByFileTab();
            picByMusicBrainzTab.OnMouseUpMethod = () => ShowPicByMusicBrainzTab();
            picByUrlTab.OnMouseUpMethod = () => ShowPicByUrlTab();

            // Setup Initial Picture Add States
            SongEditorVm.PictureUploadBtnVisibility = Visibility.Collapsed;
        }

        private void ShowDetailsTab()
        {
            HideTab();
            detailsTabBtn.IsPressed = true;
            detailsTabContent.Visibility = Visibility.Visible;
        }

        private void ShowPicturesTab()
        {
            HideTab();
            picturesTabBtn.IsPressed = true;
            picturesTabContent.Visibility = Visibility.Visible;
        }

        private void ShowLyricsTab()
        {
            HideTab();
            lyricsTabBtn.IsPressed = true;
            lyricsTabContent.Visibility = Visibility.Visible;
        }

        private void ShowSortingTab()
        {
            HideTab();
            sortingTabBtn.IsPressed = true;
            sortingTabContent.Visibility = Visibility.Visible;
        }

        private void ShowInfoTab()
        {
            HideTab();
            infoTabBtn.IsPressed = true;
            infoTabContent.Visibility = Visibility.Visible;
        }

        private void HideTab()
        {
            detailsTabBtn.IsPressed = false;
            picturesTabBtn.IsPressed = false;
            lyricsTabBtn.IsPressed = false;
            sortingTabBtn.IsPressed = false;
            infoTabBtn.IsPressed = false;

            detailsTabContent.Visibility = Visibility.Collapsed;
            picturesTabContent.Visibility = Visibility.Collapsed;
            lyricsTabContent.Visibility = Visibility.Collapsed;
            sortingTabContent.Visibility = Visibility.Collapsed;
            infoTabContent.Visibility = Visibility.Collapsed;
        }

        private void OnApplyBtnClick(object sender, RoutedEventArgs e)
        {
            SongInfo info = new SongInfo();
            info.Album = SongEditorVm.DetailsVm.Album;
            info.AlbumSort = SongEditorVm.SortingVm.AlbumSort;
            info.Publisher = SongEditorVm.DetailsVm.Publisher;
            info.Conductor = SongEditorVm.DetailsVm.Conductor;
            info.Grouping = SongEditorVm.DetailsVm.Grouping;
            info.Comment = SongEditorVm.DetailsVm.Comment;
            info.Lyrics = SongEditorVm.LyricsVm.Lyrics;
            info.Year = SongEditorVm.DetailsVm.Year;
            info.TrackCurr = SongEditorVm.DetailsVm.TrackCurr + "";
            info.TrackTotal = SongEditorVm.DetailsVm.TrackTotal + "";
            info.DiscCurr = SongEditorVm.DetailsVm.DiscCurr + "";
            info.DiscTotal = SongEditorVm.DetailsVm.DiscTotal + "";
            info.Bpm = SongEditorVm.DetailsVm.Bpm;
            info.Artist = SongEditorVm.DetailsVm.Artist;
            info.ArtistSort = SongEditorVm.SortingVm.ArtistSort;
            info.Composer = SongEditorVm.DetailsVm.Composer;
            info.ComposerSort = SongEditorVm.SortingVm.ComposerSort;
            info.AlbumArtist = SongEditorVm.DetailsVm.AlbumArtist;
            info.AlbumArtistSort = SongEditorVm.SortingVm.AlbumArtistSort;
            info.Genres = SongEditorVm.DetailsVm.Genres;
            info.StarRating = SongEditorVm.DetailsVm.StarRatingVm.Rating;
            info.Pictures = SongEditorVm.PicturesVm.Pictures;

            TitleChanged = InitialSongInfo.Title != SongEditorVm.DetailsVm.Title;
            AlbumNameChanged = InitialSongInfo.Album != SongEditorVm.DetailsVm.Album;
            TrackNumChanged = InitialSongInfo.TrackCurr != SongEditorVm.DetailsVm.TrackCurr.ToString();
            ArtistChanged = InitialSongInfo.Artist != SongEditorVm.DetailsVm.Artist || InitialSongInfo.AlbumArtist != SongEditorVm.DetailsVm.AlbumArtist;

            foreach(var song in SongEditorVm.SongsToEdit)
            {
                var tagFile = song.TagLibFile;
                SaveSongInfoToFileService.Save(tagFile, info, SongEditorVm.DetailsVm.AllowSingleSongEdits == Visibility.Visible);
            }
            
            Close();
        }

        private void SaveStringTags(TagLib.File tagFile)
        {
            TitleChanged = tagFile.Tag.Title != SongEditorVm.DetailsVm.Title;
            AlbumNameChanged = tagFile.Tag.Album != SongEditorVm.DetailsVm.Album;

            if(SongEditorVm.DetailsVm.AllowSingleSongEdits == Visibility.Visible)
            {
                tagFile.Tag.Title = SongEditorVm.DetailsVm.Title;
                tagFile.Tag.TitleSort = SongEditorVm.SortingVm.TitleSort;
            }

            AssignIfNotMultiple(SongEditorVm.DetailsVm, tagFile.Tag, nameof(tagFile.Tag.Album));
            AssignIfNotMultiple(SongEditorVm.SortingVm, tagFile.Tag, nameof(tagFile.Tag.AlbumSort));
            AssignIfNotMultiple(SongEditorVm.DetailsVm, tagFile.Tag, nameof(tagFile.Tag.Publisher));
            AssignIfNotMultiple(SongEditorVm.DetailsVm, tagFile.Tag, nameof(tagFile.Tag.Conductor));
            AssignIfNotMultiple(SongEditorVm.DetailsVm, tagFile.Tag, nameof(tagFile.Tag.Grouping));
            AssignIfNotMultiple(SongEditorVm.DetailsVm, tagFile.Tag, nameof(tagFile.Tag.Comment));
            AssignIfNotMultiple(SongEditorVm.LyricsVm, tagFile.Tag, nameof(tagFile.Tag.Lyrics));
        }

        private void SaveIntTags(TagLib.File tagFile)
        {
            if(SongEditorVm.DetailsVm.AllowSingleSongEdits == Visibility.Visible)
            {
                if(uint.TryParse(SongEditorVm.DetailsVm.Year, out uint year) && year >= 0)
                {
                    tagFile.Tag.Year = year;
                }

                if(SongEditorVm.DetailsVm.TrackCurr >= 0)
                {
                    if(tagFile.Tag.Track != (uint) SongEditorVm.DetailsVm.TrackCurr)
                    {
                        TrackNumChanged = true;
                        tagFile.Tag.Track = (uint) SongEditorVm.DetailsVm.TrackCurr;
                    }
                }

                if(SongEditorVm.DetailsVm.TrackTotal >= 0)
                {
                    tagFile.Tag.TrackCount = (uint) SongEditorVm.DetailsVm.TrackTotal;
                }

                if(SongEditorVm.DetailsVm.DiscCurr >= 0)
                {
                    tagFile.Tag.Disc = (uint) SongEditorVm.DetailsVm.DiscCurr;
                }

                if(SongEditorVm.DetailsVm.DiscTotal >= 0)
                {
                    tagFile.Tag.DiscCount = (uint) SongEditorVm.DetailsVm.DiscTotal;
                }

                if(uint.TryParse(SongEditorVm.DetailsVm.Bpm, out uint bpm) && bpm >= 0)
                {
                    tagFile.Tag.BeatsPerMinute = bpm;
                }
            }
        }

        private void SaveArrayTags(TagLib.File tagFile)
        {
            ArtistChanged = tagFile.Tag.FirstPerformer != SongEditorVm.DetailsVm.Artist || tagFile.Tag.FirstAlbumArtist != SongEditorVm.DetailsVm.AlbumArtist;

            if(!String.IsNullOrEmpty(SongEditorVm.DetailsVm.Artist) && SongEditorVm.DetailsVm.Artist != SongInfo.DefaultHasMultipleText)
            {
                if(tagFile.Tag.Performers.Length == 0)
                {
                    tagFile.Tag.Performers = new string[] { SongEditorVm.DetailsVm.Artist };
                }

                else
                {
                    // For some reason, just assigning a new value to tagFile.Tag.Performers[0] doesn't work. The value doesn't change
                    var performers = tagFile.Tag.Performers;
                    performers[0] = SongEditorVm.DetailsVm.Artist;
                    tagFile.Tag.Performers = performers;
                }
            }

            if(!String.IsNullOrEmpty(SongEditorVm.SortingVm.ArtistSort) && SongEditorVm.SortingVm.ArtistSort != SongInfo.DefaultHasMultipleText)
            {
                if(tagFile.Tag.PerformersSort.Length == 0)
                {
                    tagFile.Tag.PerformersSort = new string[] { SongEditorVm.SortingVm.ArtistSort };
                }

                else
                {
                    var performers = tagFile.Tag.PerformersSort;
                    performers[0] = SongEditorVm.SortingVm.ArtistSort;
                    tagFile.Tag.PerformersSort = performers;
                }
            }

            if(!String.IsNullOrEmpty(SongEditorVm.DetailsVm.Composer) && SongEditorVm.DetailsVm.Composer != SongInfo.DefaultHasMultipleText)
            {
                if(tagFile.Tag.Composers.Length == 0)
                {
                    tagFile.Tag.Composers = new string[] { SongEditorVm.DetailsVm.Composer };
                }

                else
                {
                    var composers = tagFile.Tag.Composers;
                    composers[0] = SongEditorVm.DetailsVm.Composer;
                    tagFile.Tag.Composers = composers;
                }
            }

            if(!String.IsNullOrEmpty(SongEditorVm.SortingVm.ComposerSort) && SongEditorVm.SortingVm.ComposerSort != SongInfo.DefaultHasMultipleText)
            {
                if(tagFile.Tag.ComposersSort.Length == 0)
                {
                    tagFile.Tag.ComposersSort = new string[] { SongEditorVm.SortingVm.ComposerSort };
                }

                else
                {
                    var composers = tagFile.Tag.ComposersSort;
                    composers[0] = SongEditorVm.SortingVm.ComposerSort;
                    tagFile.Tag.ComposersSort = composers;
                }
            }

            if(!String.IsNullOrEmpty(SongEditorVm.DetailsVm.AlbumArtist) && SongEditorVm.DetailsVm.AlbumArtist != SongInfo.DefaultHasMultipleText)
            {
                var parts = SongEditorVm.DetailsVm.AlbumArtist.Split(", ");
                tagFile.Tag.AlbumArtists = parts;
            }

            if(!String.IsNullOrEmpty(SongEditorVm.SortingVm.AlbumArtistSort) && SongEditorVm.SortingVm.AlbumArtistSort != SongInfo.DefaultHasMultipleText)
            {
                var parts = SongEditorVm.SortingVm.AlbumArtistSort.Split(", ");
                tagFile.Tag.AlbumArtistsSort = parts;
            }

            if(!String.IsNullOrEmpty(SongEditorVm.DetailsVm.Genres) && SongEditorVm.DetailsVm.Genres != SongInfo.DefaultHasMultipleText)
            {
                var parts = SongEditorVm.DetailsVm.Genres.Split(", ");
                tagFile.Tag.Genres = parts;
            }
        }

        private void SaveRatingsTags(TagLib.File tagFile)
        {
            if(SongEditorVm.DetailsVm.StarRatingVm.Rating > -1 && SongEditorVm.DetailsVm.AllowSingleSongEdits == Visibility.Visible)
            {
                var id3v2Tag = (TagLib.Id3v2.Tag) tagFile.GetTag(TagLib.TagTypes.Id3v2, true);
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
                extraInfo.Rating = (byte) SongEditorVm.DetailsVm.StarRatingVm.Rating;
            }
        }

        private void SavePictureTags(TagLib.File tagFile)
        {
            var allTheSame = SongEditorVm.PicturesVm.Pictures.Length == tagFile.Tag.Pictures.Length;

            if(allTheSame)
            {
                for(int i = 0; i < SongEditorVm.PicturesVm.Pictures.Length && allTheSame; i++)
                {
                    var editorPic = SongEditorVm.PicturesVm.Pictures[i];
                    var filePic = tagFile.Tag.Pictures[i];

                    allTheSame = editorPic.Data.Data.SequenceEqual(filePic.Data.Data);
                }
            }

            if(SongEditorVm.PicturesVm.SelectedIndex > -1 && !allTheSame)
            {
                var selectedPic = SongEditorVm.PicturesVm.Pictures[SongEditorVm.PicturesVm.SelectedIndex];
                var newList = SongEditorVm.PicturesVm.Pictures.ToList();
                newList.Remove(selectedPic);
                newList.Insert(0, selectedPic);
                tagFile.Tag.Pictures = newList.ToArray();
            }
        }

        private void AssignIfNotMultiple(object source, object target, string propertyName)
        {
            PropertyInfo sourceProperty = source.GetType().GetProperty(propertyName);
            PropertyInfo targetProperty = target.GetType().GetProperty(propertyName);

            if(sourceProperty != null && targetProperty != null)
            {
                object sourceValue = sourceProperty.GetValue(source);
                if(sourceValue != null && !sourceValue.Equals(SongInfo.DefaultHasMultipleText))
                {
                    targetProperty.SetValue(target, sourceValue);
                }
            }
        }

        private void OnCancelBtnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowPicByFileTab()
        {
            picByFileTab.IsPressed = true;
            picByMusicBrainzTab.IsPressed = false;
            picByUrlTab.IsPressed = false;
            picByFileContent.Visibility = Visibility.Visible;
            picByMusicBrainzContent.Visibility = Visibility.Collapsed;
            picByUrlContent.Visibility = Visibility.Collapsed;
        }

        private void ShowPicByMusicBrainzTab()
        {
            picByFileTab.IsPressed = false;
            picByMusicBrainzTab.IsPressed = true;
            picByUrlTab.IsPressed = false;
            picByFileContent.Visibility = Visibility.Collapsed;
            picByMusicBrainzContent.Visibility = Visibility.Visible;
            picByUrlContent.Visibility = Visibility.Collapsed;
        }

        private void ShowPicByUrlTab()
        {
            picByFileTab.IsPressed = false;
            picByMusicBrainzTab.IsPressed = false;
            picByUrlTab.IsPressed = true;
            picByFileContent.Visibility = Visibility.Collapsed;
            picByMusicBrainzContent.Visibility = Visibility.Collapsed;
            picByUrlContent.Visibility = Visibility.Visible;
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
                var mimeType = SongEditorVm.AddPictureByFileVm.ImagePathToImport.EndsWith(".png") ? "image/png" : "image/jpeg";

                AttachmentFrame picToAdd = new AttachmentFrame
                {
                    Type = TagLib.PictureType.FrontCover,
                    Description = "Album Cover",
                    MimeType = mimeType,
                    TextEncoding = TagLib.StringType.UTF16,
                    Data = File.ReadAllBytes(SongEditorVm.AddPictureByFileVm.ImagePathToImport)
                };

                var newList = SongEditorVm.PicturesVm.Pictures.ToList();
                newList.Insert(0, picToAdd);
                SongEditorVm.PicturesVm.Pictures = newList.ToArray();
                SongEditorVm.PicturesVm.CreateEditorPictures();
            }

            else if(picByMusicBrainzContent.Visibility == Visibility.Visible)
            {
                var selectedImage = SongEditorVm.AddPictureByMusicBrainzVm.SearchResults[SongEditorVm.AddPictureByMusicBrainzVm.SelectedIndex];

                AttachmentFrame picToAdd = new AttachmentFrame
                {
                    Type = TagLib.PictureType.FrontCover,
                    Description = "Album Cover",
                    MimeType = selectedImage.ImageType,
                    TextEncoding = TagLib.StringType.UTF16,
                    Data = selectedImage.ImageData
                };

                var newList = SongEditorVm.PicturesVm.Pictures.ToList();
                newList.Insert(0, picToAdd);
                SongEditorVm.PicturesVm.Pictures = newList.ToArray();
                SongEditorVm.PicturesVm.CreateEditorPictures();
            }

            else
            {
                AttachmentFrame picToAdd = new AttachmentFrame
                {
                    Type = TagLib.PictureType.FrontCover,
                    Description = "Album Cover",
                    MimeType = SongEditorVm.AddPictureByUrlVm.MimeType,
                    TextEncoding = TagLib.StringType.UTF16,
                    Data = SongEditorVm.AddPictureByUrlVm.EditorPicture.ImageVm.ImageData
                };

                var newList = SongEditorVm.PicturesVm.Pictures.ToList();
                newList.Insert(0, picToAdd);
                SongEditorVm.PicturesVm.Pictures = newList.ToArray();
                SongEditorVm.PicturesVm.CreateEditorPictures();
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

        private void OnSearchForTagsContextMenu(object sender, RoutedEventArgs e)
        {
            OpenTagSearchMethod();
            Close();
        }

        private void OnClearTagsContextMenu(object sender, RoutedEventArgs e)
        {
            SongEditorVm.Clear();
        }
    }
}
