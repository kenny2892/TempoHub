using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TempoHub.User_Controls
{
    /// <summary>
    /// Interaction logic for ToggleButton.xaml
    /// </summary>
    public partial class ToggleButton : UserControl
    {
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(ToggleButton));

        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(ToggleButton));

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(ToggleButton));

        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(ToggleButton));

        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(ToggleButton));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ToggleButton));

        public static readonly DependencyProperty LabelContentProperty =
            DependencyProperty.Register("LabelContent", typeof(string), typeof(ToggleButton));

        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(ToggleButton));
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

        public Brush HoverBackground
        {
            get { return (Brush) GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        public Brush PressedBackground
        {
            get { return (Brush) GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public Brush Background
        {
            get { return (Brush) GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius) GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public string LabelContent
        {
            get { return (string) GetValue(LabelContentProperty); }
            set { SetValue(LabelContentProperty, value); }
        }

        public bool IsPressed
        {
            get { return (bool) GetValue(IsPressedProperty); }
            set { SetValue(IsPressedProperty, value); }
        }

        public Action OnMouseUpMethod { get; set; }

        public ToggleButton()
        {
            InitializeComponent();
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(OnMouseUpMethod != null)
            {
                OnMouseUpMethod();
            }
        }
    }
}
