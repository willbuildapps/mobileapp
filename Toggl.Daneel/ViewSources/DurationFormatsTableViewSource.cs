using System;
using System.Collections.Immutable;
using System.Reactive.Linq;
using Foundation;
using Toggl.Daneel.Views.Settings;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class DurationFormatsTableViewSource : ListTableViewSource<SelectableDurationFormatViewModel>
    {
        private const int rowHeight = 48;

        public IObservable<SelectableDurationFormatViewModel> DurationFormatSelected
            => Observable
                .FromEventPattern<SelectableDurationFormatViewModel>(e => OnItemTapped += e, e => OnItemTapped -= e)
                .Select(e => e.EventArgs);

        public DurationFormatsTableViewSource(UITableView tableView, IImmutableList<SelectableDurationFormatViewModel> items)
            : base(items)
        {
            tableView.RegisterNibForCellReuse(DurationFormatViewCell.Nib, DurationFormatViewCell.Identifier);
            ConfigureCell = configureCell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => rowHeight;

        private UITableViewCell configureCell(ListTableViewSource<SelectableDurationFormatViewModel> source,
            UITableView tableView, NSIndexPath indexPath, SelectableDurationFormatViewModel model)
        {
            var cell = tableView.DequeueReusableCell(DurationFormatViewCell.Identifier) as DurationFormatViewCell;
            cell.Item = model;
            return cell;
        }
    }
}
