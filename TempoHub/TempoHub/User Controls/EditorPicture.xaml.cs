using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TempoHub.User_Controls
{
    /// <summary>
    /// Interaction logic for EditorPicture.xaml
    /// </summary>
    public partial class EditorPicture : UserControl
    {
        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if(value != isSelected)
                {
                    isSelected = value;

                    if(isSelected)
                    {
                        picture.imageBorder.BorderThickness = new Thickness(5, 5, 5, 5);
                    }

                    else
                    {
                        picture.imageBorder.BorderThickness = new Thickness(0, 0, 0, 0);
                    }
                }
            }
        }

        public static readonly DependencyProperty RemovalBtnBackgroundProperty =
            DependencyProperty.Register("RemovalBtnBackground", typeof(Brush), typeof(EditorPicture));
        public static readonly DependencyProperty RemovalBtnForegroundProperty =
            DependencyProperty.Register("RemovalBtnForeground", typeof(Brush), typeof(EditorPicture));
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(EditorPicture));
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(EditorPicture));
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(EditorPicture));
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(EditorPicture));

        public Brush RemovalBtnBackground
        {
            get { return (Brush) GetValue(RemovalBtnBackgroundProperty); }
            set { SetValue(RemovalBtnBackgroundProperty, value); }
        }
        public Brush RemovalBtnForeground
        {
            get { return (Brush) GetValue(RemovalBtnForegroundProperty); }
            set { SetValue(RemovalBtnForegroundProperty, value); }
        }
        public Thickness BorderThickness
        {
            get { return (Thickness) GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public Brush BorderBrush
        {
            get { return (Brush) GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }
        public double Height
        {
            get { return (double) GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public double Width
        {
            get { return (double) GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public Action OnRemovalClickMethod { get; set; }
        public EditorPicture()
        {
            InitializeComponent();
            picture.imageBorder.BorderBrush = (Brush) FindResource("MahApps.Brushes.Accent2");
            removalIcon.Visibility = Visibility.Hidden;
        }

        public void SetPicture(byte[] imageData)
        {
            removalIcon.Visibility = Visibility.Visible;
            picture.SetImage(imageData);
        }

        public void ClearPicture()
        {
            removalIcon.Visibility = Visibility.Hidden;
            picture.ClearImage();
        }

        private void OnRemovalClick(object sender, MouseButtonEventArgs e)
        {
            if(OnRemovalClickMethod != null)
            {
                OnRemovalClickMethod();
            }
        }
    }
}
