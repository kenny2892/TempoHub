using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Settings;

namespace TempoHub.ViewModels
{
    public class SettingsWindowViewModel
    {
        public AppliedSettings Settings { get; set; } = new AppliedSettings();
        public string VersionLabel { get; set; }
        public int MaxHistoryRecords { get; set; } = 0;
        public ThemeBase ThemeBase { get; set; } = ThemeBase.Light;
        public IEnumerable<ThemeBase> ThemeBaseValues
        {
            get { return Enum.GetValues(typeof(ThemeBase)).Cast<ThemeBase>(); }
        }
        public ThemeColor ThemeColor { get; set; } = ThemeColor.Blue;
        public IEnumerable<ThemeColor> ThemeColorValues
        {
            get { return Enum.GetValues(typeof(ThemeColor)).Cast<ThemeColor>(); }
        }
        public DisplayOptions DefaultDisplay { get; set; }
        public IEnumerable<DisplayOptions> DisplayOptionsValues
        {
            get { return Enum.GetValues(typeof(DisplayOptions)).Cast<DisplayOptions>(); }
        }
        public SettingsSongDetailsViewModel FilePath { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel Title { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel Album { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel Artist { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel AlbumArtist { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel Genres { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel Composer { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel Publisher { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel Conductor { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel Grouping { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel SongLength { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel Year { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel TrackCurr { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel TrackTotal { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel DiscCurr { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel DiscTotal { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel Rating { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel Bpm { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel Comment { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel HasLyrics { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel HasAlbumCover { get; set; } = new SettingsSongDetailsViewModel();
        public SettingsSongDetailsViewModel DateAdded { get; set; } = new SettingsSongDetailsViewModel();
        public IEnumerable<SortOptions> SortOptionsValues
        {
            get { return Enum.GetValues(typeof(SortOptions)).Cast<SortOptions>(); }
        }

        public SettingsWindowViewModel(AppliedSettings settings)
        {
            Settings = settings;
            VersionLabel = "Application Version " + Assembly.GetEntryAssembly().GetName().Version.ToString();

            MaxHistoryRecords = Settings.MaxHistoryRecords;

            ThemeBase = Settings.ThemeBase;
            ThemeColor = Settings.ThemeColor;

            DefaultDisplay = Settings.DefaultDisplay;

            var options = SortOptionsValues;

            FilePath.SettingName = "File Path";
            FilePath.Index = Settings.FilePathIndex;
            FilePath.Sort = Settings.FilePathSort;
            FilePath.IsEnabled = Settings.FilePathIsEnabled;
            FilePath.SortOptionsValues = options;

            Title.SettingName = "Title";
            Title.Index = Settings.TitleIndex;
            Title.Sort = Settings.TitleSort;
            Title.IsEnabled = Settings.TitleIsEnabled;
            Title.SortOptionsValues = options;

            Album.SettingName = "Album";
            Album.Index = Settings.AlbumIndex;
            Album.Sort = Settings.AlbumSort;
            Album.IsEnabled = Settings.AlbumIsEnabled;
            Album.SortOptionsValues = options;

            Artist.SettingName = "Artist";
            Artist.Index = Settings.ArtistIndex;
            Artist.Sort = Settings.ArtistSort;
            Artist.IsEnabled = Settings.ArtistIsEnabled;
            Artist.SortOptionsValues = options;

            AlbumArtist.SettingName = "Album Artist";
            AlbumArtist.Index = Settings.AlbumArtistIndex;
            AlbumArtist.Sort = Settings.AlbumArtistSort;
            AlbumArtist.IsEnabled = Settings.AlbumArtistIsEnabled;
            AlbumArtist.SortOptionsValues = options;

            Genres.SettingName = "Genres";
            Genres.Index = Settings.GenresIndex;
            Genres.Sort = Settings.GenresSort;
            Genres.IsEnabled = Settings.GenresIsEnabled;
            Genres.SortOptionsValues = options;

            Composer.SettingName = "Composer";
            Composer.Index = Settings.ComposerIndex;
            Composer.Sort = Settings.ComposerSort;
            Composer.IsEnabled = Settings.ComposerIsEnabled;
            Composer.SortOptionsValues = options;

            Publisher.SettingName = "Publisher";
            Publisher.Index = Settings.PublisherIndex;
            Publisher.Sort = Settings.PublisherSort;
            Publisher.IsEnabled = Settings.PublisherIsEnabled;
            Publisher.SortOptionsValues = options;

            Conductor.SettingName = "Conductor";
            Conductor.Index = Settings.ConductorIndex;
            Conductor.Sort = Settings.ConductorSort;
            Conductor.IsEnabled = Settings.ConductorIsEnabled;
            Conductor.SortOptionsValues = options;

            Grouping.SettingName = "Grouping";
            Grouping.Index = Settings.GroupingIndex;
            Grouping.Sort = Settings.GroupingSort;
            Grouping.IsEnabled = Settings.GroupingIsEnabled;
            Grouping.SortOptionsValues = options;

            SongLength.SettingName = "Length";
            SongLength.Index = Settings.SongLengthIndex;
            SongLength.Sort = Settings.SongLengthSort;
            SongLength.IsEnabled = Settings.SongLengthIsEnabled;
            SongLength.SortOptionsValues = options;

            Year.SettingName = "Year";
            Year.Index = Settings.YearIndex;
            Year.Sort = Settings.YearSort;
            Year.IsEnabled = Settings.YearIsEnabled;
            Year.SortOptionsValues = options;

            TrackCurr.SettingName = "Track #";
            TrackCurr.Index = Settings.TrackCurrIndex;
            TrackCurr.Sort = Settings.TrackCurrSort;
            TrackCurr.IsEnabled = Settings.TrackCurrIsEnabled;
            TrackCurr.SortOptionsValues = options;

            TrackTotal.SettingName = "Track Total";
            TrackTotal.Index = Settings.TrackTotalIndex;
            TrackTotal.Sort = Settings.TrackTotalSort;
            TrackTotal.IsEnabled = Settings.TrackTotalIsEnabled;
            TrackTotal.SortOptionsValues = options;

            DiscCurr.SettingName = "Disc #";
            DiscCurr.Index = Settings.DiscCurrIndex;
            DiscCurr.Sort = Settings.DiscCurrSort;
            DiscCurr.IsEnabled = Settings.DiscCurrIsEnabled;
            DiscCurr.SortOptionsValues = options;

            DiscTotal.SettingName = "Disc Total";
            DiscTotal.Index = Settings.DiscTotalIndex;
            DiscTotal.Sort = Settings.DiscTotalSort;
            DiscTotal.IsEnabled = Settings.DiscTotalIsEnabled;
            DiscTotal.SortOptionsValues = options;

            Rating.SettingName = "Rating";
            Rating.Index = Settings.RatingIndex;
            Rating.Sort = Settings.RatingSort;
            Rating.IsEnabled = Settings.RatingIsEnabled;
            Rating.SortOptionsValues = options;

            Bpm.SettingName = "Bpm";
            Bpm.Index = Settings.BpmIndex;
            Bpm.Sort = Settings.BpmSort;
            Bpm.IsEnabled = Settings.BpmIsEnabled;
            Bpm.SortOptionsValues = options;

            Comment.SettingName = "Comment";
            Comment.Index = Settings.CommentIndex;
            Comment.Sort = Settings.CommentSort;
            Comment.IsEnabled = Settings.CommentIsEnabled;
            Comment.SortOptionsValues = options;

            HasLyrics.SettingName = "Has Lyrics";
            HasLyrics.Index = Settings.HasLyricsIndex;
            HasLyrics.Sort = Settings.HasLyricsSort;
            HasLyrics.IsEnabled = Settings.HasLyricsIsEnabled;
            HasLyrics.SortOptionsValues = options;

            HasAlbumCover.SettingName = "Has Album Cover";
            HasAlbumCover.Index = Settings.HasAlbumCoverIndex;
            HasAlbumCover.Sort = Settings.HasAlbumCoverSort;
            HasAlbumCover.IsEnabled = Settings.HasAlbumCoverIsEnabled;
            HasAlbumCover.SortOptionsValues = options;

            DateAdded.SettingName = "Date Added";
            DateAdded.Index = Settings.DateAddedIndex;
            DateAdded.Sort = Settings.DateAddedSort;
            DateAdded.IsEnabled = Settings.DateAddedIsEnabled;
            DateAdded.SortOptionsValues = options;
        }
    }
}
