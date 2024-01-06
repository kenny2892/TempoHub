using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Settings;

namespace TempoHub.ViewModels
{
    public class SongDetailsColumnTogglesViewModel
    {
        public bool FilePath { get; set; } = true;
        public bool Title { get; set; } = true;
        public bool Album { get; set; } = true;
        public bool Artist { get; set; } = true;
        public bool AlbumArtist { get; set; } = true;
        public bool Genres { get; set; } = true;
        public bool Composer { get; set; } = true;
        public bool Publisher { get; set; } = true;
        public bool Conductor { get; set; } = true;
        public bool Grouping { get; set; } = true;
        public bool SongLength { get; set; } = true;
        public bool Year { get; set; } = true;
        public bool TrackCurr { get; set; } = true;
        public bool TrackTotal { get; set; } = true;
        public bool DiscCurr { get; set; } = true;
        public bool DiscTotal { get; set; } = true;
        public bool Rating { get; set; } = true;
        public bool Bpm { get; set; } = true;
        public bool Comment { get; set; } = true;
        public bool HasLyrics { get; set; } = true;
        public bool HasAlbumCover { get; set; } = true;
        public bool DateAdded { get; set; } = true;

        public SongDetailsColumnTogglesViewModel()
        {

        }

        public SongDetailsColumnTogglesViewModel(AppliedSettings settings)
        {
            ApplyDefaultSettings(settings);
        }

        public void ApplyDefaultSettings(AppliedSettings settings)
        {
            FilePath = settings.FilePathIsEnabled;
            Title = settings.TitleIsEnabled;
            Album = settings.AlbumIsEnabled;
            Artist = settings.ArtistIsEnabled;
            AlbumArtist = settings.AlbumArtistIsEnabled;
            Genres = settings.GenresIsEnabled;
            Composer = settings.ComposerIsEnabled;
            Publisher = settings.PublisherIsEnabled;
            Conductor = settings.ConductorIsEnabled;
            Grouping = settings.GroupingIsEnabled;
            SongLength = settings.SongLengthIsEnabled;
            Year = settings.YearIsEnabled;
            TrackCurr = settings.TrackCurrIsEnabled;
            TrackTotal = settings.TrackTotalIsEnabled;
            DiscCurr = settings.DiscCurrIsEnabled;
            DiscTotal = settings.DiscTotalIsEnabled;
            Rating = settings.RatingIsEnabled;
            Bpm = settings.BpmIsEnabled;
            Comment = settings.CommentIsEnabled;
            HasLyrics = settings.HasLyricsIsEnabled;
            HasAlbumCover = settings.HasAlbumCoverIsEnabled;
            DateAdded = settings.DateAddedIsEnabled;
        }
    }
}
