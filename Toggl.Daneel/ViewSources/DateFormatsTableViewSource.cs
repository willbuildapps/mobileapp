using System;
using System.Collections.Immutable;
using System.Reactive.Linq;
using Foundation;
using Toggl.Daneel.Views.Settings;
using Toggl.Foundation.MvvmCross.ViewModels.Selectable;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class DateFormatsTableViewSource : ListTableViewSource<SelectableDateFormatViewModel>
    {
        private const int rowHeight = 48;

        public IObservable<SelectableDateFormatViewModel> DateFormatSelected
            => Observable
                .FromEventPattern<SelectableDateFormatViewModel>(e => OnItemTapped += e, e => OnItemTapped -= e)
                .Select(e => e.EventArgs);

        public DateFormatsTableViewSource(UITableView tableView, IImmutableList<SelectableDateFormatViewModel> items)
            : base(items)
        {
            tableView.RegisterNibForCellReuse(DateFormatViewCell.Nib, DateFormatViewCell.Identifier);
            ConfigureCell = configureCell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => rowHeight;

        private UITableViewCell configureCell(ListTableViewSource<SelectableDateFormatViewModel> source,
            UITableView tableView, NSIndexPath indexPath, SelectableDateFormatViewModel model)
        {
            var cell = tableView.DequeueReusableCell(DateFormatViewCell.Identifier) as DateFormatViewCell;
            cell.Item = model;
            return cell;
        }
    }
}
