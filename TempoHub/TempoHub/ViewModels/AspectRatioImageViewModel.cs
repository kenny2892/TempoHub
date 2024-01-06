using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoHub.ViewModels
{
    public class AspectRatioImageViewModel : PropertyChangedBase
    {
        private byte[] imageData;
        public byte[] ImageData
        {
            get { return imageData; }
            set
            {
                if(imageData != value)
                {
                    imageData = value;
                    OnPropertyChanged("ImageData");
                }
            }
        }
    }
}
