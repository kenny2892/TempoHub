using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoHub.Models
{
    public class SongFile
    {
        public string FilePath { get; set; }
        public TagLib.File TagLibFile { get; set; }
    }
}
