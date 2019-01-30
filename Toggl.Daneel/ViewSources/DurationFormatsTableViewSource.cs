using System;
using System.Collections.Immutable;
using System.Reactive.Linq;
using Foundation;
using Toggl.Daneel.Views.Settings;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class DurationFormatsTableViewSource : SectionedListTableViewSource<SelectableDurationFormatViewModel>
    {
        private const int rowHeight = 48;

        public DurationFormatsTableViewSource(UITableView tableView, IImmutableList<SelectableDurationFormatViewModel> items)
            : base(items)
        {
            tableView.RegisterNibForCellReuse(DurationFormatViewCell.Nib, DurationFormatViewCell.Identifier);
            tableView.RowHeight = rowHeight;

            ConfigureCell = configureCell;
        }

        private UITableViewCell configureCell(SectionedListTableViewSource<SelectableDurationFormatViewModel> source,
            UITableView tableView, NSIndexPath indexPath, SelectableDurationFormatViewModel model)
        {
            var cell = tableView.DequeueReusableCell(DurationFormatViewCell.Identifier) as DurationFormatViewCell;
            cell.Item = model;
            return cell;
        }
    }
}
