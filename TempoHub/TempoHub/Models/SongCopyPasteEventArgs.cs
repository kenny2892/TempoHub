using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoHub.Models
{
    public class SongCopyPasteEventArgs : EventArgs
    {
        public string FilePath { get; set; } = "";
    }
}
