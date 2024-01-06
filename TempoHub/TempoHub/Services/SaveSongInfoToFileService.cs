using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TagLib.Id3v2;
using TempoHub.Converters;
using TempoHub.Models;
using TempoHub.ViewModels.Song_Editor_Tabs;

namespace TempoHub.Services
{
    public class SaveSongInfoToFileService
    {
        public static void Save(TagLib.File tagFile, SongInfo info, bool allowSingleSongEdits)
        {
            if(allowSingleSongEdits)
            {
                AssignStringIfNotMultiple(info, tagFile.Tag, nameof(info.Title), nameof(tagFile.Tag.Title));
                AssignStringIfNotMultiple(info, tagFile.Tag, nameof(info.TitleSort), nameof(tagFile.Tag.TitleSort));
            }

            AssignStringIfNotMultiple(info, tagFile.Tag, nameof(info.Album), nameof(tagFile.Tag.Album));
            AssignStringIfNotMultiple(info, tagFile.Tag, nameof(info.AlbumSort), nameof(tagFile.Tag.AlbumSort));
            AssignStringIfNotMultiple(info, tagFile.Tag, nameof(info.Publisher), nameof(tagFile.Tag.Publisher));
            AssignStringIfNotMultiple(info, tagFile.Tag, nameof(info.Conductor), nameof(tagFile.Tag.Conductor));
            AssignStringIfNotMultiple(info, tagFile.Tag, nameof(info.Grouping), nameof(tagFile.Tag.Grouping));
            AssignStringIfNotMultiple(info, tagFile.Tag, nameof(info.Comment), nameof(tagFile.Tag.Comment));
            AssignStringIfNotMultiple(info, tagFile.Tag, nameof(info.Lyrics), nameof(tagFile.Tag.Lyrics));

            AssignArrayIfNotMultiple(info, tagFile.Tag, nameof(info.Artist), nameof(tagFile.Tag.Performers));
            AssignArrayIfNotMultiple(info, tagFile.Tag, nameof(info.ArtistSort), nameof(tagFile.Tag.PerformersSort));
            AssignArrayIfNotMultiple(info, tagFile.Tag, nameof(info.AlbumArtist), nameof(tagFile.Tag.AlbumArtists));
            AssignArrayIfNotMultiple(info, tagFile.Tag, nameof(info.AlbumArtistSort), nameof(tagFile.Tag.AlbumArtistsSort));
            AssignArrayIfNotMultiple(info, tagFile.Tag, nameof(info.Composer), nameof(tagFile.Tag.Composers));
            AssignArrayIfNotMultiple(info, tagFile.Tag, nameof(info.ComposerSort), nameof(tagFile.Tag.ComposersSort));

            if(allowSingleSongEdits)
            {
                AssignUintIfNotNull(info, tagFile.Tag, nameof(info.Year), nameof(tagFile.Tag.Year));
                AssignUintIfNotNull(info, tagFile.Tag, nameof(info.TrackCurr), nameof(tagFile.Tag.Track));
                AssignUintIfNotNull(info, tagFile.Tag, nameof(info.TrackTotal), nameof(tagFile.Tag.TrackCount));
                AssignUintIfNotNull(info, tagFile.Tag, nameof(info.DiscCurr), nameof(tagFile.Tag.Disc));
                AssignUintIfNotNull(info, tagFile.Tag, nameof(info.DiscTotal), nameof(tagFile.Tag.DiscCount));
                AssignUintIfNotNull(info, tagFile.Tag, nameof(info.Bpm), nameof(tagFile.Tag.BeatsPerMinute));
            }

            if(info.Genres != null)
            {
                var dividerRegex = new Regex(", {0,1}");
                tagFile.Tag.Genres = dividerRegex.Split(info.Genres);
            }

            if(info.StarRating != null && allowSingleSongEdits)
            {
                var ratingConverter = new RatingsConverter();

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
                extraInfo.Rating = (byte) info.StarRating;
            }

            tagFile.Save();
        }

        private static void AssignStringIfNotMultiple(object source, object target, string sourcePropertyName, string targetPropertyName)
        {
            PropertyInfo sourceProperty = source.GetType().GetProperty(sourcePropertyName);
            PropertyInfo targetProperty = target.GetType().GetProperty(targetPropertyName);

            if(sourceProperty != null && targetProperty != null)
            {
                object sourceValue = sourceProperty.GetValue(source);
                if(sourceValue != null && sourceValue is string text && text != SongInfo.DefaultHasMultipleText)
                {
                    targetProperty.SetValue(target, sourceValue);
                }
            }
        }

        private static void AssignArrayIfNotMultiple(object source, object target, string sourcePropertyName, string targetPropertyName)
        {
            PropertyInfo sourceProperty = source.GetType().GetProperty(sourcePropertyName);
            PropertyInfo targetProperty = target.GetType().GetProperty(targetPropertyName);

            if(sourceProperty != null && targetProperty != null)
            {
                object sourceValue = sourceProperty.GetValue(source);
                object targetValue = targetProperty.GetValue(target);

                if(sourceValue != null && targetValue is string[] targetArray && sourceValue is string toAdd && toAdd != SongInfo.DefaultHasMultipleText)
                {
                    var newList = targetArray.ToList();

                    if(newList.Count > 0)
                    {
                        newList[0] = toAdd;
                    }

                    else
                    {
                        newList.Add(toAdd);
                    }

                    targetProperty.SetValue(target, newList.ToArray());
                }
            }
        }

        private static void AssignUintIfNotNull(object source, object target, string sourcePropertyName, string targetPropertyName)
        {
            PropertyInfo sourceProperty = source.GetType().GetProperty(sourcePropertyName);
            PropertyInfo targetProperty = target.GetType().GetProperty(targetPropertyName);

            if(sourceProperty != null && targetProperty != null)
            {
                object sourceValue = sourceProperty.GetValue(source);
                if(sourceValue != null && sourceValue is string value && uint.TryParse(value, out uint valueNum) && valueNum >= 0)
                {
                    targetProperty.SetValue(target, valueNum);
                }
            }
        }
    }
}
