using System.Collections.Immutable;
using System.Linq;
using Foundation;
using Toggl.Daneel.Cells.Calendar;
using Toggl.Foundation.MvvmCross.ViewModels.Selectable;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class SelectUserCalendarsTableViewSource : SectionedListTableViewSource<SelectableUserCalendarViewModel>
    {
        private const int rowHeight = 48;
        private const int headerHeight = 48;

        public UIColor SectionHeaderBackgroundColor { get; set; } = UIColor.White;

        public SelectUserCalendarsTableViewSource(UITableView tableView) : base(ImmutableList<IImmutableList<SelectableUserCalendarViewModel>>.Empty)
        {
            tableView.RegisterNibForCellReuse(SelectableUserCalendarViewCell.Nib, SelectableUserCalendarViewCell.Identifier);
            tableView.RegisterNibForHeaderFooterViewReuse(UserCalendarListHeaderViewCell.Nib, UserCalendarListHeaderViewCell.Identifier);
            tableView.SectionHeaderHeight = headerHeight;
            tableView.RowHeight = rowHeight;

            ConfigureCell = configureCell;
            ViewForHeaderInSection = viewForHeaderInSection;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            base.RowSelected(tableView, indexPath);

            var cell = (SelectableUserCalendarViewCell)tableView.CellAt(indexPath);
            cell.ToggleSwitch();

            tableView.DeselectRow(indexPath, true);
        }

        private UITableViewCell configureCell(SectionedListTableViewSource<SelectableUserCalendarViewModel> source,
            UITableView tableView, NSIndexPath indexPath, SelectableUserCalendarViewModel model)
        {
            var cell = tableView.DequeueReusableCell(SelectableUserCalendarViewCell.Identifier, indexPath) as SelectableUserCalendarViewCell;
            cell.Item = model;
            return cell;
        }

        private UIView viewForHeaderInSection(SectionedListTableViewSource<SelectableUserCalendarViewModel> source, UITableView tableView, int section)
        {
            var header = tableView.DequeueReusableHeaderFooterView(UserCalendarListHeaderViewCell.Identifier) as UserCalendarListHeaderViewCell;
            header.Item = items[(int)section].First().SourceName;
            header.ContentView.BackgroundColor = SectionHeaderBackgroundColor;
            return header;
        }
    }
}
