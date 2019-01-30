using System;
using System.Collections.Immutable;
using Foundation;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public class ListTableViewSource<TModel> : UITableViewSource
    {
        protected IImmutableList<TModel> items;

        public EventHandler<TModel> OnItemTapped { get; set; }
        public Func<ListTableViewSource<TModel>, UITableView, NSIndexPath, TModel, UITableViewCell> ConfigureCell;

        public ListTableViewSource(IImmutableList<TModel> items)
        {
            this.items = items;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return ConfigureCell(this, tableView, indexPath, items[indexPath.Row]);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            OnItemTapped?.Invoke(this, items[indexPath.Row]);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
            => items.Count;
    }
}
