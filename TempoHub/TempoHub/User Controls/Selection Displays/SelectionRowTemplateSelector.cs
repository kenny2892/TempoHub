using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TempoHub.Models;
using TempoHub.ViewModels;
using TempoHub.ViewModels.Selection_Displays;

namespace TempoHub.User_Controls.Selection_Displays
{
    public class SelectionRowTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ArtistTemplate { get; set; }
        public DataTemplate AlbumTemplate { get; set; }
        public DataTemplate SongTemplate { get; set; }
        public DataTemplate PlaylistTemplate { get; set; }
        public DataTemplate MusicBrainzTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item is SelectionRowBaseViewModel row)
            {
                switch(row.Type)
                {
                    case SelectionType.Artist:
                        return ArtistTemplate;

                    case SelectionType.Album:
                        return AlbumTemplate;

                    case SelectionType.Song:
                        return SongTemplate;

                    case SelectionType.Playlist:
                        return PlaylistTemplate;

                    case SelectionType.MusicBrainzResult:
                        return MusicBrainzTemplate;

                    default:
                        return base.SelectTemplate(item, container);
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
