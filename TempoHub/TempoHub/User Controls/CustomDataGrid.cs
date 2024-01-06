using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TempoHub.User_Controls
{
    public class CustomDataGrid : DataGrid
    {
        // Source: https://stackoverflow.com/a/51896877
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            INotifyCollectionChanged oldView = oldValue as INotifyCollectionChanged;
            if(oldView != null)
            {
                oldView.CollectionChanged -= View_CollectionChanged;
            }

            INotifyCollectionChanged newView = newValue as INotifyCollectionChanged;
            if(newView != null)
            {
                newView.CollectionChanged += View_CollectionChanged;
            }
        }

        private void View_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ICollectionView view = sender as ICollectionView;
            if(view != null)
            {
                foreach(var sd in view.SortDescriptions)
                {
                    if(sd != null)
                    {
                        DataGridColumn column = Columns.FirstOrDefault(x => x.SortMemberPath == sd.PropertyName);
                        if(column != null)
                        {
                            column.SortDirection = sd.Direction;
                        }
                    }
                }
            }
        }
    }
}
