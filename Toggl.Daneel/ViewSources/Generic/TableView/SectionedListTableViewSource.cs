using System;
using System.Collections.Immutable;
using Foundation;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public class SectionedListTableViewSource<TModel> : UITableViewSource
    {
        public delegate UITableViewCell CellConfiguration(SectionedListTableViewSource<TModel> source, UITableView tableView, NSIndexPath indexPath, TModel model);
        public delegate UIView HeaderConfiguration(SectionedListTableViewSource<TModel> source, UITableView tableView, int section);

        protected IImmutableList<IImmutableList<TModel>> sections;

        public EventHandler<TModel> OnItemTapped { get; set; }

        public CellConfiguration configureCell;
        public HeaderConfiguration configureHeader;

        public SectionedListTableViewSource(CellConfiguration configureCell, IImmutableList<IImmutableList<TModel>> sections,  HeaderConfiguration configureHeader = null)
        {
            this.sections = sections;
            this.configureCell = configureCell;
            this.configureHeader = configureHeader;
        }

        public SectionedListTableViewSource(CellConfiguration configureCell, IImmutableList<TModel> items = null, HeaderConfiguration configureHeader = null)
        {
            sections = items == null ? ImmutableList<IImmutableList<TModel>>.Empty : ImmutableList.Create(items);
            this.configureCell = configureCell;
            this.configureHeader = configureHeader;
        }

        public void SetItems(IImmutableList<IImmutableList<TModel>> sections)
        {
            this.sections = sections;
        }

        public void SetItems(IImmutableList<TModel> items)
        {
            sections = ImmutableList.Create(items);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            => configureCell(this, tableView, indexPath, sections[indexPath.Section][indexPath.Row]);

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
            => configureHeader(this, tableView, (int)section);
    }
}
