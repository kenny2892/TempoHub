using MahApps.Metro.IconPacks;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TempoHub.Converters;
using TempoHub.User_Controls.Song_Editor_Tabs;
using TempoHub.ViewModels;

namespace TempoHub.User_Controls
{
    /// <summary>
    /// Interaction logic for StarRating.xaml
    /// </summary>
    public partial class StarRating : UserControl
    {
        public StarRating()
        {
            InitializeComponent();
        }

        private void OnStarGridMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(sender is PackIconRemixIcon star && DataContext is StarRatingViewModel vm)
            {
                if(!vm.Editable)
                {
                    return;
                }

                var clickPoint = e.GetPosition(star);
                var isOverHalf = clickPoint.X > (star.ActualWidth / 2);
                var decimalToAdd = isOverHalf ? 1.0 : 0.5;

                var converter = new RatingsConverter();
                switch(star.Name)
                {
                    case "starIconOne":
                        vm.Rating = (double) converter.ConvertBack(0 + decimalToAdd, null, null, null);
                        break;

                    case "starIconTwo":
                        vm.Rating = (double) converter.ConvertBack(1 + decimalToAdd, null, null, null);
                        break;

                    case "starIconThree":
                        vm.Rating = (double) converter.ConvertBack(2 + decimalToAdd, null, null, null);
                        break;

                    case "starIconFour":
                        vm.Rating = (double) converter.ConvertBack(3 + decimalToAdd, null, null, null);
                        break;

                    case "starIconFive":
                        vm.Rating = (double) converter.ConvertBack(4 + decimalToAdd, null, null, null);
                        break;
                }
            }
        }

        private void OnStarRatingChanged(object sender, TextChangedEventArgs e)
        {
            if(double.TryParse(ratingTextBlock.Text, out double rating))
            {
                UpdateStars(rating);
            }
        }

        private void UpdateStars(double rating)
        {
            if(rating < 0)
            {
                Visibility = Visibility.Collapsed;
                return;
            }

            var converter = new RatingsConverter();
            rating = (double) converter.Convert(rating, null, null, null);

            starIconOne.Kind = PackIconRemixIconKind.StarFill;
            starIconTwo.Kind = PackIconRemixIconKind.StarFill;
            starIconThree.Kind = PackIconRemixIconKind.StarFill;
            starIconFour.Kind = PackIconRemixIconKind.StarFill;
            starIconFive.Kind = PackIconRemixIconKind.StarFill;

            if(rating >= 5)
            {
                return;
            }

            if(rating == 4.5)
            {
                starIconFive.Kind = PackIconRemixIconKind.StarHalfLine;
            }

            else if(rating <= 4)
            {
                starIconFive.Kind = PackIconRemixIconKind.StarLine;
            }

            if(rating == 3.5)
            {
                starIconFour.Kind = PackIconRemixIconKind.StarHalfLine;
            }

            else if(rating <= 3)
            {
                starIconFour.Kind = PackIconRemixIconKind.StarLine;
            }

            if(rating == 2.5)
            {
                starIconThree.Kind = PackIconRemixIconKind.StarHalfLine;
            }

            else if(rating <= 2)
            {
                starIconThree.Kind = PackIconRemixIconKind.StarLine;
            }

            if(rating == 1.5)
            {
                starIconTwo.Kind = PackIconRemixIconKind.StarHalfLine;
            }

            else if(rating <= 1)
            {
                starIconTwo.Kind = PackIconRemixIconKind.StarLine;
            }

            if(rating == 0.5)
            {
                starIconOne.Kind = PackIconRemixIconKind.StarHalfLine;
            }

            else if(rating == 0)
            {
                starIconOne.Kind = PackIconRemixIconKind.StarLine;
            }
        }
    }
}
