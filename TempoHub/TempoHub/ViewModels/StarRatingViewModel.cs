using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoHub.ViewModels
{
    public class StarRatingViewModel : PropertyChangedBase
    {
        private bool editable { get; set; } = true;
        public bool Editable
        {
            get { return editable; }
            set
            {
                if(editable != value)
                {
                    editable = value;
                    OnPropertyChanged(nameof(Editable));
                }
            }
        }
        private double rating { get; set; } = 0;
        public double Rating
        {
            get { return rating; }
            set
            {
                if(rating != value && value <= 255 && value >= 0)
                {
                    rating = value;
                    OnPropertyChanged(nameof(Rating));
                }
            }
        }
    }
}
