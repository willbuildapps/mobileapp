using System;
using System.Collections.Immutable;
using Foundation;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public class SectionedListTableViewSource<TModel> : UITableViewSource
    {
        protected IImmutableList<IImmutableList<TModel>> sections;

        public EventHandler<TModel> OnItemTapped { get; set; }

        public Func<SectionedListTableViewSource<TModel>, UITableView, NSIndexPath, TModel, UITableViewCell> ConfigureCell;
        public Func<SectionedListTableViewSource<TModel>, UITableView, int, UIView> ViewForHeaderInSection;

        public SectionedListTableViewSource(IImmutableList<IImmutableList<TModel>> sections)
        {
            this.sections = sections;
        }

        public SectionedListTableViewSource(IImmutableList<TModel> items)
        {
            this.sections = ImmutableList.Create(items);
        }

        public void SetItems(IImmutableList<IImmutableList<TModel>> sections)
        {
            this.sections = sections;
        }

        public void SetItems(IImmutableList<TModel> items)
        {
            this.sections = ImmutableList.Create(items);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return ConfigureCell(this, tableView, indexPath, sections[indexPath.Section][indexPath.Row]);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            OnItemTapped?.Invoke(this, sections[indexPath.Section][indexPath.Row]);
        }

        public override nint NumberOfSections(UITableView tableView)
            => sections.Count;

        public override nint RowsInSection(UITableView tableview, nint section)
            => sections[(int)section].Count;

        public override UIView GetViewForHeader(UITableView tableView, nint section)
            => ViewForHeaderInSection(this, tableView, (int)section);
    }
}
