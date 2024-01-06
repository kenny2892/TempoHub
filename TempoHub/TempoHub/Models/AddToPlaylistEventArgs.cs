using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoHub.Models
{
    public class AddToPlaylistEventArgs : EventArgs
    {
        public string PlaylistName { get; set; } = "";
        public List<string> FilePaths { get; set; } = new List<string>();
    }
}
