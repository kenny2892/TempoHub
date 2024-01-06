using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoHub.Models
{
    public class ImportEntry
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
        public string Year { get; set; }
        public string TrackCurr { get; set; }
        public string TrackTotal { get; set; }
        public string DiscCurr { get; set; }
        public string DiscTotal { get; set; }
        public string Bpm { get; set; }
        public string Comment { get; set; }
        public string Lyrics { get; set; }
        public string Rating { get; set; }
    }
}
