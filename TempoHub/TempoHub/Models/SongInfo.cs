using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using TagLib.Id3v2;
using TempoHub.Converters;
using TempoHub.User_Controls;
using TempoHub.ViewModels;

namespace TempoHub.Models
{
    public class SongInfo
    {
        public string FilePath { get; set; }
        public string Title { get; set; }
        public string TitleSort { get; set; }
        public string Album { get; set; }
        public string AlbumSort { get; set; }
        public string Artist { get; set; }
        public string ArtistSort { get; set; }
        public string AlbumArtist { get; set; }
        public string AlbumArtistSort { get; set; }
        public string Genres { get; set; }
        public string Composer { get; set; }
        public string ComposerSort { get; set; }
        public string Publisher { get; set; }
        public string Conductor { get; set; }
        public string Grouping { get; set; }
        public string SongLength { get; set; }
        public double SongLengthMilliseconds { get; set; }
        public string Year { get; set; }
        public string TrackCurr { get; set; }
        public string TrackTotal { get; set; }
        public string DiscCurr { get; set; }
        public string DiscTotal { get; set; }
        public double StarRating { get; set; }
        public StarRatingViewModel StarRatingViewModel { get; set; }
        public string Bpm { get; set; }
        public string Comment { get; set; }
        public DateTime DateAdded { get; set; }
        public string Lyrics { get; set; }
        public bool HasLyrics { get { return !String.IsNullOrEmpty(Lyrics); } }
        public IPicture[] Pictures { get; set; }
        public bool HasAlbumCover { get { return Pictures != null && Pictures.Length > 0; } }
        public static string DefaultHasMultipleText { get; } = "*Multiple*";

        public SongInfo()
        {
            StarRatingViewModel = new StarRatingViewModel() { Rating = -1, Editable = false };
        }

        public SongInfo(params SongFile[] songsToEdit)
        {
            if(songsToEdit.Length == 1)
            {
                GenerateSingleSongInfo(songsToEdit[0]);
            }

            else if(songsToEdit.Length > 1)
            {
                GenerateMultiSongInfo(songsToEdit);
            }

            else
            {
                StarRatingViewModel = new StarRatingViewModel() { Rating = -1, Editable = false };
            }
        }

        private void GenerateMultiSongInfo(IEnumerable<SongFile> songsToEdit)
        {
            var tagFiles = songsToEdit.Select(song => song.TagLibFile);
            var first = tagFiles.First();
            FilePath = "";
            Album = tagFiles.All(toCheck => toCheck.Tag.Album == first.Tag.Album) ? first.Tag.Album : DefaultHasMultipleText;
            Artist = tagFiles.All(toCheck => !String.IsNullOrEmpty(toCheck.Tag.FirstPerformer) && 
                toCheck.Tag.FirstPerformer == first.Tag.FirstPerformer) ? first.Tag.FirstPerformer : DefaultHasMultipleText;
            AlbumArtist = tagFiles.All(toCheck =>
            {
                if(first.Tag.AlbumArtists.Length == 0 && toCheck.Tag.AlbumArtists.Length == 0)
                {
                    return true;
                }

                return first.Tag.AlbumArtists.Length == toCheck.Tag.AlbumArtists.Length && String.Join(", ", toCheck.Tag.AlbumArtists) == String.Join(", ", first.Tag.AlbumArtists);
            }) ? String.Join(", ", first.Tag.AlbumArtists) : DefaultHasMultipleText;
            Genres = tagFiles.All(toCheck =>
            {
                if(first.Tag.Genres.Length == 0 && toCheck.Tag.Genres.Length == 0)
                {
                    return true;
                }

                return first.Tag.Genres.Length == toCheck.Tag.Genres.Length && String.Join(", ", toCheck.Tag.Genres) == String.Join(", ", first.Tag.Genres);
            }) ? String.Join(", ", first.Tag.Genres) : DefaultHasMultipleText;
            Composer = tagFiles.All(toCheck => toCheck.Tag.FirstComposer == first.Tag.FirstComposer) ? first.Tag.FirstComposer : DefaultHasMultipleText;
            Publisher = tagFiles.All(toCheck => toCheck.Tag.Publisher == first.Tag.Publisher) ? first.Tag.Publisher : DefaultHasMultipleText;
            Conductor = tagFiles.All(toCheck => toCheck.Tag.Conductor == first.Tag.Conductor) ? first.Tag.Conductor : DefaultHasMultipleText;
            Grouping = tagFiles.All(toCheck => toCheck.Tag.Grouping == first.Tag.Grouping) ? first.Tag.Grouping : DefaultHasMultipleText;
            Year = tagFiles.All(toCheck => toCheck.Tag.Year == first.Tag.Year) ? first.Tag.Year.ToString() : DefaultHasMultipleText;
            Bpm = tagFiles.All(toCheck => toCheck.Tag.BeatsPerMinute == first.Tag.BeatsPerMinute) ? first.Tag.BeatsPerMinute.ToString() : DefaultHasMultipleText;
            Comment = tagFiles.All(toCheck => toCheck.Tag.Comment == first.Tag.Comment) ? first.Tag.Comment : DefaultHasMultipleText;
            Lyrics = tagFiles.All(toCheck => toCheck.Tag.Lyrics == first.Tag.Lyrics) ? first.Tag.Lyrics : DefaultHasMultipleText;
            Pictures = tagFiles.All(toCheck =>
            {
                if(toCheck.Tag.Pictures.Length != first.Tag.Pictures.Length)
                {
                    return false;
                }

                for(int i = 0; i < toCheck.Tag.Pictures.Length; i++)
                {
                    if(!toCheck.Tag.Pictures[i].Data.Data.SequenceEqual(first.Tag.Pictures[i].Data.Data))
                    {
                        return false;
                    }
                }

                return true;
            }) ? first.Tag.Pictures : new IPicture[0];
        }

        private void GenerateSingleSongInfo(SongFile song)
        {
            var tagFile = song.TagLibFile;

            FilePath = song.FilePath;
            Title = tagFile.Tag.Title;
            TitleSort = tagFile.Tag.TitleSort;
            Album = tagFile.Tag.Album;
            AlbumSort = tagFile.Tag.AlbumSort;
            Artist = tagFile.Tag.Performers.Length > 0 ? tagFile.Tag.Performers[0] : "";
            ArtistSort = tagFile.Tag.PerformersSort.Length > 0 ? tagFile.Tag.PerformersSort[0] : "";
            AlbumArtist = tagFile.Tag.AlbumArtists.Length > 0 ? String.Join(", ", tagFile.Tag.AlbumArtists) : "";
            AlbumArtistSort = tagFile.Tag.AlbumArtistsSort.Length > 0 ? String.Join(", ", tagFile.Tag.AlbumArtistsSort) : "";
            Genres = tagFile.Tag.Genres.Length > 0 ? String.Join(", ", tagFile.Tag.Genres) : "";
            Composer = tagFile.Tag.Composers.Length > 0 ? tagFile.Tag.Composers[0] : "";
            ComposerSort = tagFile.Tag.ComposersSort.Length > 0 ? tagFile.Tag.ComposersSort[0] : "";
            Publisher = tagFile.Tag.Publisher;
            Conductor = tagFile.Tag.Conductor;
            Grouping = tagFile.Tag.Grouping;
            SongLength = song.Duration.ToString(@"hh\:mm\:ss");
            SongLengthMilliseconds = song.Duration.TotalMilliseconds;
            Year = tagFile.Tag.Year.ToString();
            TrackCurr = tagFile.Tag.Track.ToString();
            TrackTotal = tagFile.Tag.TrackCount.ToString();
            DiscCurr = tagFile.Tag.Disc.ToString();
            DiscTotal = tagFile.Tag.DiscCount.ToString();
            Bpm = tagFile.Tag.BeatsPerMinute.ToString();
            Comment = tagFile.Tag.Comment;
            Lyrics = tagFile.Tag.Lyrics;
            Pictures = tagFile.Tag.Pictures;
            DateAdded = song.DateAdded;
            StarRating = GetSongRating(tagFile);
            StarRatingViewModel = new StarRatingViewModel() { Rating = StarRating, Editable = false };
        }

        private double GetSongRating(TagLib.File tagFile)
        {
            // Found info from here: https://stackoverflow.com/q/41252370
            var id3v2Tag = (TagLib.Id3v2.Tag) tagFile.GetTag(TagLib.TagTypes.Id3v2, true);
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
                return extraInfo.Rating;
            }

            return -1;
        }

        public string AsCsv()
        {
            var ratingConverter = new RatingsConverter();
            var rating = (double) ratingConverter.Convert(StarRating, null, null, null);

            var values = new List<string>
            {
                FilePath,
                Title,
                TitleSort,
                Album,
                AlbumSort,
                Artist,
                ArtistSort,
                AlbumArtist,
                AlbumArtistSort,
                Genres,
                Composer,
                ComposerSort,
                Publisher,
                Conductor,
                Grouping,
                SongLength,
                SongLengthMilliseconds.ToString(),
                Year,
                TrackCurr,
                TrackTotal,
                DiscCurr,
                DiscTotal,
                Bpm,
                Comment,
                Lyrics,
                Pictures.Count().ToString(),
                DateAdded.ToString("G"),
                rating.ToString()
            };

            return String.Join(",", values.Select(text => CsvProtect(text)));
        }

        private string CsvProtect(string text)
        {
            if(String.IsNullOrEmpty(text))
            {
                return "";
            }

            return $"\"{text.Replace("\"", "\"\"")}\"";
        }
    }
}
