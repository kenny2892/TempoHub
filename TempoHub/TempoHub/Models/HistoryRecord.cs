using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoHub.Models
{
    public class HistoryRecord
    {
        [Key]
        public string FilePath { get; set; } = "";
        public int Order { get; set; } = 0;
    }
}
