using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoHub.Models;

namespace TempoHub.ViewModels.Selection_Displays
{
    public class SelectionRowBaseViewModel : PropertyChangedBase
    {
        public SelectionType Type { get; set; }
    }
}
