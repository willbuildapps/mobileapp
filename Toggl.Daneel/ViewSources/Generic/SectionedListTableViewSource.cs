using System;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;
using Toggl.Daneel.Cells;
using Toggl.Foundation.MvvmCross.Extensions;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public abstract class SectionedListTableViewSource<TModel, TCell> : UITableViewSource
        where TCell : BaseTableViewCell<TModel>
    {
        private readonly ISubject<TModel> itemSelectedSubject = new Subject<TModel>();
        private readonly string cellIdentifier;

        public IObservable<TModel> ItemSelected { get; }

        protected IImmutableList<IImmutableList<TModel>> Items { get; private set; }

        protected SectionedListTableViewSource(
            IImmutableList<IImmutableList<TModel>> items,
            ISchedulerProvider schedulerProvider,
            string cellIdentifier)
        {
            Items = items;
            ItemSelected = itemSelectedSubject.AsDriver(schedulerProvider);
            this.cellIdentifier = cellIdentifier;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (TCell)tableView.DequeueReusableCell(cellIdentifier, indexPath);
            cell.Item = Items[indexPath.Section][indexPath.Row];
            return cell;
        }

        public override nint NumberOfSections(UITableView tableView)
            => Items.Count;

        public override nint RowsInSection(UITableView tableview, nint section)
            => Items[(int)section].Count;

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            itemSelectedSubject.OnNext(Items[indexPath.Section][indexPath.Row]);
        }

        public void ChangeData(UITableView tableView, IImmutableList<IImmutableList<TModel>> newItems)
        {
            Items = newItems;
            tableView.ReloadData();
        }
    }
}
