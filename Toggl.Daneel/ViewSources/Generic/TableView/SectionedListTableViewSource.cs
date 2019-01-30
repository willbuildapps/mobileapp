using System;
using System.Collections.Immutable;
using Foundation;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public class SectionedListTableViewSource<TModel> : UITableViewSource
    {
        protected IImmutableList<IImmutableList<TModel>> items;

        public EventHandler<TModel> OnItemTapped { get; set; }

        public Func<SectionedListTableViewSource<TModel>, UITableView, NSIndexPath, TModel, UITableViewCell> ConfigureCell;
        public Func<SectionedListTableViewSource<TModel>, UITableView, int, UIView> ViewForHeaderInSection;

        public SectionedListTableViewSource(IImmutableList<IImmutableList<TModel>> items)
        {
            this.items = items;
        }

        public SectionedListTableViewSource(IImmutableList<TModel> section)
        {
            this.items = ImmutableList.Create(section);
        }

        public void SetItems(IImmutableList<IImmutableList<TModel>> items)
        {
            this.items = items;
        }

        public void SetItems(IImmutableList<TModel> section)
        {
            this.items = ImmutableList.Create(section);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return ConfigureCell(this, tableView, indexPath, items[indexPath.Section][indexPath.Row]);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            OnItemTapped?.Invoke(this, items[indexPath.Section][indexPath.Row]);
        }

        public override nint NumberOfSections(UITableView tableView)
            => items.Count;

        public override nint RowsInSection(UITableView tableview, nint section)
            => items[(int)section].Count;

        public override UIView GetViewForHeader(UITableView tableView, nint section)
            => ViewForHeaderInSection(this, tableView, (int)section);
    }
}
