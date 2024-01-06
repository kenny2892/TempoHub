using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TempoHub.ViewModels
{
    public class EditorPictureViewModel : PropertyChangedBase
    {
        public Action OnRemovalClickMethod { get; set; }
        private AspectRatioImageViewModel imageVm = new AspectRatioImageViewModel();
        public AspectRatioImageViewModel ImageVm
        {
            get { return imageVm; }
            set
            {
                if(imageVm != value)
                {
                    imageVm = value;

                    if(imageVm.ImageData.Length > 1)
                    {
                        RemovalIconVisibility = Visibility.Visible;
                    }

                    else
                    {
                        RemovalIconVisibility = Visibility.Collapsed;
                    }

                    OnPropertyChanged(nameof(ImageVm));
                }
            }
        }
        private Brush removalBtnBackground = Brushes.Transparent;
        public Brush RemovalBtnBackground
        {
            get { return removalBtnBackground; }
            set
            {
                if(removalBtnBackground != value)
                {
                    removalBtnBackground = value;
                    OnPropertyChanged(nameof(RemovalBtnBackground));
                }
            }
        }
        private Brush removalBtnForeground = Brushes.Transparent;
        public Brush RemovalBtnForeground
        {
            get { return removalBtnForeground; }
            set
            {
                if(removalBtnForeground != value)
                {
                    removalBtnForeground = value;
                    OnPropertyChanged(nameof(RemovalBtnForeground));
                }
            }
        }
        private Brush borderBrush = Brushes.Transparent;
        public Brush BorderBrush
        {
            get { return borderBrush; }
            set
            {
                if(borderBrush != value)
                {
                    borderBrush = value;
                    OnPropertyChanged(nameof(BorderBrush));
                }
            }
        }
        private Thickness borderThickness = new Thickness(0, 0, 0, 0);
        public Thickness BorderThickness
        {
            get { return borderThickness; }
            set
            {
                if(borderThickness != value)
                {
                    borderThickness = value;
                    OnPropertyChanged(nameof(BorderThickness));
                }
            }
        }
        private Visibility removalIconVisibility = Visibility.Collapsed;
        public Visibility RemovalIconVisibility
        {
            get { return removalIconVisibility; }
            set
            {
                if(removalIconVisibility != value)
                {
                    removalIconVisibility = value;
                    OnPropertyChanged(nameof(RemovalIconVisibility));
                }
            }
        }
    }
}
