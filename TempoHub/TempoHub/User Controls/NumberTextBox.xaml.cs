using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for NumberTextBox.xaml
    /// </summary>
    public partial class NumberTextBox : UserControl
    {
        private static Regex NumberRegex { get; set; } = new Regex("^-{0,1}[0-9]+\\.{0,1}[0-9]*$");
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NumberTextBox));
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public NumberTextBox()
        {
            InitializeComponent();
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !NumberRegex.IsMatch(e.Text);
        }

        private void OnTextPasting(object sender, DataObjectPastingEventArgs e)
        {
            if(e.DataObject.GetDataPresent(typeof(string)))
            {
                var pastedText = (string) e.DataObject.GetData(typeof(string));

                if(!NumberRegex.IsMatch(pastedText))
                {
                    e.CancelCommand();
                }
            }

            else
            {
                e.CancelCommand();
            }
        }
    }
}
