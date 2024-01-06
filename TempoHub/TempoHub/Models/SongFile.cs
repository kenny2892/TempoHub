using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TempoHub.Models
{
    public class SongFile
    {
        public string FilePath { get; set; }
        public DateTime DateAdded { get; set; }
        public string Album { get; private set; }
        public string Artist { get; private set; }
        public TimeSpan Duration { get; private set; }
        public TagLib.File TagLibFile
        {
            get { return TagLib.File.Create(FilePath); }
        }

        public SongFile(string filePath, DateTime dateAdded, TimeSpan duration)
        {
            FilePath = filePath;
            DateAdded = dateAdded;
            Duration = duration;
            UpdateInfo();
        }

        public void UpdateInfo()
        {
            var tagFile = TagLibFile;
            Album = tagFile.Tag.Album;
            Artist = !String.IsNullOrEmpty(tagFile.Tag.FirstAlbumArtist) ? tagFile.Tag.FirstAlbumArtist : tagFile.Tag.FirstPerformer;
        }
    }
}
