using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using TagLib.Id3v2;
using TempoHub.Converters;
using TempoHub.Data;
using TempoHub.Models;
using static System.Windows.Forms.LinkLabel;

namespace TempoHub.Services
{
    public class ImportParserService
    {
        public static void Parse(SongContext context, string importPath)
        {
            var entries = ParseFile(importPath);

            var existingSongs = context.SongPaths.Select(songPath => songPath.FilePath).ToList();
            var entriesToUpdate = entries.Where(entry => existingSongs.Contains(entry.FilePath)).ToList();
            var entriesToCreate = entries.Where(entry => !entriesToUpdate.Contains(entry)).ToList();

            foreach(var entry in entriesToUpdate)
            {
                UpdateSong(entry);
            }

            CreateSongs(context, entriesToCreate);
        }

        private static List<ImportEntry> ParseFile(string importPath)
        {
            var entries = new List<ImportEntry>();
            var lines = File.ReadAllText(importPath).Split("\r\n").Where(line => !String.IsNullOrWhiteSpace(line.Replace(",", ""))).ToList();
            var regex = new Regex("(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
            var headerMap = ParseHeaders(importPath, typeof(ImportEntry), lines, regex);

            foreach(string line in lines.Skip(1))
            {
                var entry = new ImportEntry();
                int index = 0;

                foreach(string value in regex.Matches(line).Select(match => match.Value))
                {
                    if(headerMap.ContainsKey(index))
                    {
                        string toUse = value;

                        // If the value starts and ends with " and contains "", then clean it
                        if(toUse.StartsWith("\"") && toUse.EndsWith("\"") && toUse.Contains("\"\""))
                        {
                            toUse = toUse[1..^1].Replace("\"\"", "\"");
                        }

                        else if(toUse.StartsWith("\"") && toUse.EndsWith("\""))
                        {
                            toUse = toUse[1..^1];
                        }

                        var propToAdd = headerMap[index];
                        propToAdd.SetValue(entry, toUse);
                    }

                    index++;
                }

                entries.Add(entry);
            }

            return entries;
        }

        private static Dictionary<int, PropertyInfo> ParseHeaders(string path, Type classType, List<string> lines, Regex regex)
        {
            var headers = regex.Matches(lines[0]).Select(match => match.Value).ToList();

            Dictionary<int, PropertyInfo> map = new Dictionary<int, PropertyInfo>();
            PropertyInfo[] propertyInfos = classType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach(var propertyInfo in propertyInfos)
            {
                string match = headers.Where(header => header.Replace(" ", "").Replace("#", "Curr").ToLower() == propertyInfo.Name.ToLower()).FirstOrDefault();

                if(!string.IsNullOrEmpty(match))
                {
                    map.Add(headers.IndexOf(match), propertyInfo);
                }
            }

            return map;
        }

        private static void CreateSongs(SongContext context, List<ImportEntry> entriesToCreate)
        {
            var existingSongs = context.SongPaths.ToList();

            foreach(var musicFilePath in entriesToCreate)
            {
                try
                {
                    // Test to make sure the file is compatible with TagLib
                    var musicFile = TagLib.File.Create(musicFilePath.FilePath);
                    UpdateSong(musicFilePath);

                    var toAdd = new SongPath() { FilePath = musicFilePath.FilePath, DateAdded = DateTime.Now };
                    context.SongPaths.Add(toAdd);
                }

                catch(Exception)
                {
                    Trace.WriteLine("File was not added: " + musicFilePath);
                }
            }

            context.SaveChanges();
        }

        private static void UpdateSong(ImportEntry entry)
        {
            var tagFile = TagLib.File.Create(entry.FilePath);

            AssignStringIfNotNull(entry, tagFile.Tag, nameof(entry.Title), nameof(tagFile.Tag.Title));
            AssignStringIfNotNull(entry, tagFile.Tag, nameof(entry.TitleSort), nameof(tagFile.Tag.TitleSort));
            AssignStringIfNotNull(entry, tagFile.Tag, nameof(entry.Album), nameof(tagFile.Tag.Album));
            AssignStringIfNotNull(entry, tagFile.Tag, nameof(entry.AlbumSort), nameof(tagFile.Tag.AlbumSort));
            AssignStringIfNotNull(entry, tagFile.Tag, nameof(entry.Publisher), nameof(tagFile.Tag.Publisher));
            AssignStringIfNotNull(entry, tagFile.Tag, nameof(entry.Conductor), nameof(tagFile.Tag.Conductor));
            AssignStringIfNotNull(entry, tagFile.Tag, nameof(entry.Grouping), nameof(tagFile.Tag.Grouping));
            AssignStringIfNotNull(entry, tagFile.Tag, nameof(entry.Comment), nameof(tagFile.Tag.Comment));
            AssignStringIfNotNull(entry, tagFile.Tag, nameof(entry.Lyrics), nameof(tagFile.Tag.Lyrics));

            AssignArrayIfNotNull(entry, tagFile.Tag, nameof(entry.Artist), nameof(tagFile.Tag.Performers));
            AssignArrayIfNotNull(entry, tagFile.Tag, nameof(entry.ArtistSort), nameof(tagFile.Tag.PerformersSort));
            AssignArrayIfNotNull(entry, tagFile.Tag, nameof(entry.AlbumArtist), nameof(tagFile.Tag.AlbumArtists));
            AssignArrayIfNotNull(entry, tagFile.Tag, nameof(entry.AlbumArtistSort), nameof(tagFile.Tag.AlbumArtistsSort));
            AssignArrayIfNotNull(entry, tagFile.Tag, nameof(entry.Composer), nameof(tagFile.Tag.Composers));
            AssignArrayIfNotNull(entry, tagFile.Tag, nameof(entry.ComposerSort), nameof(tagFile.Tag.ComposersSort));

            AssignUintIfNotNull(entry, tagFile.Tag, nameof(entry.Year), nameof(tagFile.Tag.Year));
            AssignUintIfNotNull(entry, tagFile.Tag, nameof(entry.TrackCurr), nameof(tagFile.Tag.Track));
            AssignUintIfNotNull(entry, tagFile.Tag, nameof(entry.TrackTotal), nameof(tagFile.Tag.TrackCount));
            AssignUintIfNotNull(entry, tagFile.Tag, nameof(entry.DiscCurr), nameof(tagFile.Tag.Disc));
            AssignUintIfNotNull(entry, tagFile.Tag, nameof(entry.DiscTotal), nameof(tagFile.Tag.DiscCount));
            AssignUintIfNotNull(entry, tagFile.Tag, nameof(entry.Bpm), nameof(tagFile.Tag.BeatsPerMinute));

            if(entry.Genres != null)
            {
                var dividerRegex = new Regex(", {0,1}");
                tagFile.Tag.Genres = dividerRegex.Split(entry.Genres);
            }

            if(entry.Rating != null && double.TryParse(entry.Rating, out double rating) && rating > -1)
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
                double convertedRating = (double) ratingConverter.ConvertBack(rating, null, null, null);
                extraInfo.Rating = (byte) convertedRating;
            }

            tagFile.Save();
        }

        private static void AssignStringIfNotNull(object source, object target, string sourcePropertyName, string targetPropertyName)
        {
            PropertyInfo sourceProperty = source.GetType().GetProperty(sourcePropertyName);
            PropertyInfo targetProperty = target.GetType().GetProperty(targetPropertyName);

            if(sourceProperty != null && targetProperty != null)
            {
                object sourceValue = sourceProperty.GetValue(source);
                if(sourceValue != null)
                {
                    targetProperty.SetValue(target, sourceValue);
                }
            }
        }

        private static void AssignArrayIfNotNull(object source, object target, string sourcePropertyName, string targetPropertyName)
        {
            PropertyInfo sourceProperty = source.GetType().GetProperty(sourcePropertyName);
            PropertyInfo targetProperty = target.GetType().GetProperty(targetPropertyName);

            if(sourceProperty != null && targetProperty != null)
            {
                object sourceValue = sourceProperty.GetValue(source);
                object targetValue = targetProperty.GetValue(target);

                if(sourceValue != null && targetValue is string[] targetArray && sourceValue is string toAdd)
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
