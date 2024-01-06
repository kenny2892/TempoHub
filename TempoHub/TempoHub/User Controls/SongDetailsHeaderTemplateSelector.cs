using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TempoHub.User_Controls
{
    public class SongDetailsHeaderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextBoxTemplate { get; set; }
        public DataTemplate ComboBoxTemplate { get; set; }
        public DataTemplate RatingTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item is string text)
            {
                if(text == "Has Lyrics" || text == "Has Album Cover")
                {
                    return ComboBoxTemplate;
                }

                else if(text == "Rating")
                {
                    return RatingTemplate;
                }
            }

            return TextBoxTemplate;
        }
    }
}
