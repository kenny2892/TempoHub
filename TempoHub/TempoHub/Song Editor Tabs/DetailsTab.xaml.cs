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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TagLib.Id3v2;
using TempoHub.Models;

namespace TempoHub.Song_Editor_Tabs
{
    /// <summary>
    /// Interaction logic for DetailsTab.xaml
    /// </summary>
    public partial class DetailsTab : UserControl
    {
        private SongFile Song { get; set; }
        public static readonly DependencyProperty TitleTxtProperty =
            DependencyProperty.Register("TitleTxt", typeof(string), typeof(DetailsTab));
        public static readonly DependencyProperty AlbumTxtProperty =
            DependencyProperty.Register("AlbumTxt", typeof(string), typeof(DetailsTab));
        public static readonly DependencyProperty ArtistTxtProperty =
            DependencyProperty.Register("ArtistTxt", typeof(string), typeof(DetailsTab));

        public string TitleTxt
        {
            get { return (string) GetValue(TitleTxtProperty); }
            set { SetValue(TitleTxtProperty, value); }
        }
        public string AlbumTxt
        {
            get { return (string) GetValue(AlbumTxtProperty); }
            set { SetValue(AlbumTxtProperty, value); }
        }
        public string ArtistTxt
        {
            get { return (string) GetValue(ArtistTxtProperty); }
            set { SetValue(ArtistTxtProperty, value); }
        }

        public DetailsTab()
        {
            InitializeComponent();
        }

        public void LoadDetailsTab(SongFile song)
        {
            Song = song;

            TitleTxt = Song.TagLibFile.Tag.Title;
            AlbumTxt = Song.TagLibFile.Tag.Album;
            ArtistTxt = Song.TagLibFile.Tag.Performers.Length > 0 ? Song.TagLibFile.Tag.Performers[0] : "";
            albumArtistInput.Text = Song.TagLibFile.Tag.AlbumArtists.Length > 0 ? String.Join(", ", Song.TagLibFile.Tag.AlbumArtists) : "";
            genresInput.Text = Song.TagLibFile.Tag.Genres.Length > 0 ? String.Join(", ", Song.TagLibFile.Tag.Genres) : "";
            composerInput.Text = Song.TagLibFile.Tag.Composers.Length > 0 ? Song.TagLibFile.Tag.Composers[0] : "";
            publisherInput.Text = Song.TagLibFile.Tag.Publisher;
            conductorInput.Text = Song.TagLibFile.Tag.Conductor;
            groupingInput.Text = Song.TagLibFile.Tag.Grouping;
            yearInput.Text = Song.TagLibFile.Tag.Year.ToString();
            trackCurrInput.Text = Song.TagLibFile.Tag.Track.ToString();
            trackTotalInput.Text = Song.TagLibFile.Tag.TrackCount.ToString();
            discCurrInput.Text = Song.TagLibFile.Tag.Disc.ToString();
            discTotalInput.Text = Song.TagLibFile.Tag.DiscCount.ToString();

            // Found info from here: https://stackoverflow.com/q/41252370
            var id3v2Tag = (TagLib.Id3v2.Tag) Song.TagLibFile.GetTag(TagLib.TagTypes.Id3v2, true);
            if(id3v2Tag != null)
            {
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
                var percentage = extraInfo.Rating / 255.0;
                starRating.Rating = percentage * 5;
            }

            else
            {
                ratingsGrid.Visibility = Visibility.Collapsed;
            }

            bpmInput.Text = Song.TagLibFile.Tag.BeatsPerMinute.ToString();
            commentInput.Text = Song.TagLibFile.Tag.Comment;
        }
    }
}
