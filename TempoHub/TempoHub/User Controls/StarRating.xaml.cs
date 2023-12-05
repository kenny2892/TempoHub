using MahApps.Metro.IconPacks;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TempoHub.User_Controls
{
    /// <summary>
    /// Interaction logic for StarRating.xaml
    /// </summary>
    public partial class StarRating : UserControl
    {
        public double Rating
        {
            get
            {
                if(starIconFive.Kind == PackIconRemixIconKind.StarFill)
                {
                    return 5;
                }

                else if(starIconFive.Kind == PackIconRemixIconKind.StarHalfFill)
                {
                    return 4.5;
                }

                else if(starIconFour.Kind == PackIconRemixIconKind.StarFill)
                {
                    return 4.5;
                }

                else if(starIconFour.Kind == PackIconRemixIconKind.StarHalfFill)
                {
                    return 3.5;
                }

                else if(starIconThree.Kind == PackIconRemixIconKind.StarFill)
                {
                    return 3;
                }

                else if(starIconThree.Kind == PackIconRemixIconKind.StarHalfFill)
                {
                    return 2.5;
                }

                else if(starIconTwo.Kind == PackIconRemixIconKind.StarFill)
                {
                    return 2;
                }

                else if(starIconTwo.Kind == PackIconRemixIconKind.StarHalfFill)
                {
                    return 1.5;
                }

                else if(starIconOne.Kind == PackIconRemixIconKind.StarFill)
                {
                    return 1;
                }

                else if(starIconOne.Kind == PackIconRemixIconKind.StarHalfFill)
                {
                    return 0.5;
                }

                return 0;
            }

            set
            {
                starIconOne.Kind = PackIconRemixIconKind.StarFill;
                starIconTwo.Kind = PackIconRemixIconKind.StarFill;
                starIconThree.Kind = PackIconRemixIconKind.StarFill;
                starIconFour.Kind = PackIconRemixIconKind.StarFill;
                starIconFive.Kind = PackIconRemixIconKind.StarFill;

                if(value <= 4.5 && value > 4)
                {
                    starIconFive.Kind = PackIconRemixIconKind.StarLine;
                }

                else if(value < 5)
                {
                    starIconFive.Kind = PackIconRemixIconKind.StarLine;
                }

                if(value <= 3.5 && value > 3)
                {
                    starIconFour.Kind = PackIconRemixIconKind.StarLine;
                }

                else if(value < 4)
                {
                    starIconFour.Kind = PackIconRemixIconKind.StarLine;
                }

                if(value <= 2.5 && value > 2)
                {
                    starIconThree.Kind = PackIconRemixIconKind.StarLine;
                }

                else if(value < 3)
                {
                    starIconThree.Kind = PackIconRemixIconKind.StarLine;
                }

                if(value <= 1.5 && value > 1)
                {
                    starIconTwo.Kind = PackIconRemixIconKind.StarLine;
                }

                else if(value < 2)
                {
                    starIconTwo.Kind = PackIconRemixIconKind.StarLine;
                }

                if(value <= 0.5 && value > 0)
                {
                    starIconOne.Kind = PackIconRemixIconKind.StarLine;
                }

                else if(value <= 0)
                {
                    starIconOne.Kind = PackIconRemixIconKind.StarLine;
                }
            }
        }

        public StarRating()
        {
            InitializeComponent();
        }

        private void OnStarGridMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(sender is Grid starGrid)
            {
                var clickPoint = e.GetPosition(starGrid);
                var isOverHalf = clickPoint.X > (starGrid.ActualWidth / 2);

                starIconOne.Kind = PackIconRemixIconKind.StarLine;
                starIconTwo.Kind = PackIconRemixIconKind.StarLine;
                starIconThree.Kind = PackIconRemixIconKind.StarLine;
                starIconFour.Kind = PackIconRemixIconKind.StarLine;
                starIconFive.Kind = PackIconRemixIconKind.StarLine;

                switch(starGrid.Name)
                {
                    case "starGridOne":
                        starIconOne.Kind = isOverHalf ? PackIconRemixIconKind.StarFill : PackIconRemixIconKind.StarHalfLine;
                        break;

                    case "starGridTwo":
                        starIconOne.Kind = PackIconRemixIconKind.StarFill;
                        starIconTwo.Kind = isOverHalf ? PackIconRemixIconKind.StarFill : PackIconRemixIconKind.StarHalfLine;
                        break;

                    case "starGridThree":
                        starIconOne.Kind = PackIconRemixIconKind.StarFill;
                        starIconTwo.Kind = PackIconRemixIconKind.StarFill;
                        starIconThree.Kind = isOverHalf ? PackIconRemixIconKind.StarFill : PackIconRemixIconKind.StarHalfLine;
                        break;

                    case "starGridFour":
                        starIconOne.Kind = PackIconRemixIconKind.StarFill;
                        starIconTwo.Kind = PackIconRemixIconKind.StarFill;
                        starIconThree.Kind = PackIconRemixIconKind.StarFill;
                        starIconFour.Kind = isOverHalf ? PackIconRemixIconKind.StarFill : PackIconRemixIconKind.StarHalfLine;
                        break;

                    case "starGridFive":
                        starIconOne.Kind = PackIconRemixIconKind.StarFill;
                        starIconTwo.Kind = PackIconRemixIconKind.StarFill;
                        starIconThree.Kind = PackIconRemixIconKind.StarFill;
                        starIconFour.Kind = PackIconRemixIconKind.StarFill;
                        starIconFive.Kind = isOverHalf ? PackIconRemixIconKind.StarFill : PackIconRemixIconKind.StarHalfLine;
                        break;
                }
            }
        }
    }
}
