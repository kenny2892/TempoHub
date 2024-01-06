using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoHub.Models
{
    public class PlaylistSongEntry
    {
        [Key]
        public int ID { get; set; } = -1;
        public int Index { get; set; } = -1;
        public int PlaylistID { get; set; } = -1;
        public string SongPath { get; set; } = "";
    }
}
