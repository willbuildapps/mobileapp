using System;
using System.Collections.Immutable;
using System.Linq;
using Foundation;
using Toggl.Daneel.Cells.Calendar;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.ViewModels.Selectable;
using Toggl.Multivac;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class SelectUserCalendarsTableViewSource
        : SectionedListTableViewSource<SelectableUserCalendarViewModel, SelectableUserCalendarViewCell>
    {
        private const int rowHeight = 48;
        private const int headerHeight = 48;

        private const string cellIdentifier = nameof(SelectableUserCalendarViewCell);
        private const string headerIdentifier = nameof(UserCalendarListHeaderViewCell);

        public UIColor SectionHeaderBackgroundColor { get; set; } = UIColor.White;

        public SelectUserCalendarsTableViewSource(
            UITableView tableView,
            ISchedulerProvider schedulerProvider
        )
            : base(ImmutableList<IImmutableList<SelectableUserCalendarViewModel>>.Empty, schedulerProvider, cellIdentifier)
        {
            tableView.RegisterNibForCellReuse(SelectableUserCalendarViewCell.Nib, cellIdentifier);
            tableView.RegisterNibForHeaderFooterViewReuse(UserCalendarListHeaderViewCell.Nib, headerIdentifier);
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            => headerHeight;

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            => rowHeight;

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var header = (UserCalendarListHeaderViewCell)tableView.DequeueReusableHeaderFooterView(headerIdentifier);
            header.Item = Items[(int)section].First().SourceName;
            header.ContentView.BackgroundColor = SectionHeaderBackgroundColor;
            return header;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            base.RowSelected(tableView, indexPath);

            var cell = (SelectableUserCalendarViewCell)tableView.CellAt(indexPath);
            cell.ToggleSwitch();

            tableView.DeselectRow(indexPath, true);
        }
    }
}
