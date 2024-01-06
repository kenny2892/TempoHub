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
using TempoHub.Settings;
using TempoHub.ViewModels;
using TempoHub.ViewModels.Selection_Displays;

namespace TempoHub.User_Controls.Selection_Displays
{
    /// <summary>
    /// Interaction logic for SelectionSearchBar.xaml
    /// </summary>
    public partial class SelectionSearchBar : UserControl
    {
        public event EventHandler<string> RefreshRequested;
        public static readonly DependencyProperty PromptTextProperty =
            DependencyProperty.Register("PromptText", typeof(string), typeof(SelectionSearchBar));
        public string PromptText
        {
            get { return (string) GetValue(PromptTextProperty); }
            set { SetValue(PromptTextProperty, value); }
        }

        public SelectionSearchBar()
        {
            InitializeComponent();
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SelectionSearchBarViewModel vm)
            {
                vm.SearchText = "";
            }

            RefreshRequested?.Invoke(this, "Clear");
        }

        private void OnSortClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is SelectionSearchBarViewModel vm)
            {
                vm.SortOption = vm.SortOption == SortOptions.Ascending ? SortOptions.Descending : SortOptions.Ascending;
            }

            RefreshRequested?.Invoke(this, "Sort");
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshRequested?.Invoke(this, "Text");
        }
    }
}
