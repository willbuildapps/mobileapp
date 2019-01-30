using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using Foundation;
using Toggl.Daneel.Views;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class WorkspaceTableViewSource : SectionedListTableViewSource<SelectableWorkspaceViewModel>
    {
        private const int rowHeight = 64;

        public WorkspaceTableViewSource()
            : base(ImmutableArray<SelectableWorkspaceViewModel>.Empty)
        {
            ConfigureCell = configureCell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => rowHeight;

        public void SetNewWorkspaces(IEnumerable<SelectableWorkspaceViewModel> workspaces)
            => SetItems(workspaces.ToImmutableList());

        private UITableViewCell configureCell(SectionedListTableViewSource<SelectableWorkspaceViewModel> source,
            UITableView tableView, NSIndexPath indexPath, SelectableWorkspaceViewModel model)
        {
            var cell = tableView.DequeueReusableCell(WorkspaceViewCell.Identifier) as WorkspaceViewCell;
            cell.Item = model;
            return cell;
        }
    }
}
