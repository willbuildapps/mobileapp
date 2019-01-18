using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using Foundation;
using Toggl.Daneel.Cells;
using Toggl.Daneel.Views.Tag;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class SelectTagsTableViewSource : ListTableViewSource<SelectableTagBaseViewModel, NewTagViewCell>
    {
        public IObservable<SelectableTagBaseViewModel> TagSelected
            => Observable
                .FromEventPattern<SelectableTagBaseViewModel>(e => OnItemTapped += e, e => OnItemTapped -= e)
                .Select(e => e.EventArgs);

        private const int rowHeight = 48;
        public SelectTagsTableViewSource()
            : base(ImmutableArray<SelectableTagBaseViewModel>.Empty, NewTagViewCell.Identifier)
        {
        }

        public void SetNewTags(IEnumerable<SelectableTagBaseViewModel> tags)
        {
            items = tags.ToImmutableList();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var item = items[indexPath.Row];
            var identifier = item is SelectableTagCreationViewModel ? CreateTagViewCell.Identifier : cellIdentifier;
            var cell = tableView.DequeueReusableCell(identifier) as BaseTableViewCell<SelectableTagBaseViewModel>;
            cell.Item = item;
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => rowHeight;
    }
}
