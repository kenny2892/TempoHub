using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoHub.Settings
{
    public class AppliedSettings
    {
        public int MaxHistoryRecords { get; set; } = 100;
        public ThemeBase ThemeBase { get; set; } = ThemeBase.Light;
        public ThemeColor ThemeColor { get; set; } = ThemeColor.Blue;
        public DisplayOptions DefaultDisplay { get; set; } = DisplayOptions.Albums;
        public SortOptions FilePathSort { get; set; }
        public int FilePathIndex { get; set; } = -1;
        public bool FilePathIsEnabled { get; set; } = true;
        public SortOptions TitleSort { get; set; } = SortOptions.Ascending;
        public int TitleIndex { get; set; } = 1;
        public bool TitleIsEnabled { get; set; } = true;
        public SortOptions AlbumSort { get; set; } = SortOptions.Ascending;
        public int AlbumIndex { get; set; } = 0;
        public bool AlbumIsEnabled { get; set; } = true;
        public SortOptions ArtistSort { get; set; }
        public int ArtistIndex { get; set; } = -1;
        public bool ArtistIsEnabled { get; set; } = true;
        public SortOptions AlbumArtistSort { get; set; }
        public int AlbumArtistIndex { get; set; } = -1;
        public bool AlbumArtistIsEnabled { get; set; } = false;
        public SortOptions GenresSort { get; set; }
        public int GenresIndex { get; set; } = -1;
        public bool GenresIsEnabled { get; set; } = true;
        public SortOptions ComposerSort { get; set; }
        public int ComposerIndex { get; set; } = -1;
        public bool ComposerIsEnabled { get; set; } = false;
        public SortOptions PublisherSort { get; set; }
        public int PublisherIndex { get; set; } = -1;
        public bool PublisherIsEnabled { get; set; } = false;
        public SortOptions ConductorSort { get; set; }
        public int ConductorIndex { get; set; } = -1;
        public bool ConductorIsEnabled { get; set; } = false;
        public SortOptions GroupingSort { get; set; }
        public int GroupingIndex { get; set; } = -1;
        public bool GroupingIsEnabled { get; set; } = false;
        public SortOptions SongLengthSort { get; set; }
        public int SongLengthIndex { get; set; } = -1;
        public bool SongLengthIsEnabled { get; set; } = false;
        public SortOptions YearSort { get; set; }
        public int YearIndex { get; set; } = -1;
        public bool YearIsEnabled { get; set; } = false;
        public SortOptions TrackCurrSort { get; set; }
        public int TrackCurrIndex { get; set; } = -1;
        public bool TrackCurrIsEnabled { get; set; } = false;
        public SortOptions TrackTotalSort { get; set; }
        public int TrackTotalIndex { get; set; } = -1;
        public bool TrackTotalIsEnabled { get; set; } = false;
        public SortOptions DiscCurrSort { get; set; }
        public int DiscCurrIndex { get; set; } = -1;
        public bool DiscCurrIsEnabled { get; set; } = false;
        public SortOptions DiscTotalSort { get; set; }
        public int DiscTotalIndex { get; set; } = -1;
        public bool DiscTotalIsEnabled { get; set; } = false;
        public SortOptions RatingSort { get; set; }
        public int RatingIndex { get; set; } = -1;
        public bool RatingIsEnabled { get; set; } = true;
        public SortOptions BpmSort { get; set; }
        public int BpmIndex { get; set; } = -1;
        public bool BpmIsEnabled { get; set; } = false;
        public SortOptions CommentSort { get; set; }
        public int CommentIndex { get; set; } = -1;
        public bool CommentIsEnabled { get; set; } = false;
        public SortOptions HasLyricsSort { get; set; }
        public int HasLyricsIndex { get; set; } = -1;
        public bool HasLyricsIsEnabled { get; set; } = false;
        public SortOptions HasAlbumCoverSort { get; set; }
        public int HasAlbumCoverIndex { get; set; } = -1;
        public bool HasAlbumCoverIsEnabled { get; set; } = false;
        public SortOptions DateAddedSort { get; set; }
        public int DateAddedIndex { get; set; } = -1;
        public bool DateAddedIsEnabled { get; set; } = false;
    }
}
