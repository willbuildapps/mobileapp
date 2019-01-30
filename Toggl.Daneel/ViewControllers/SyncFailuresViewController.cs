﻿using UIKit;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Daneel.Cells;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation.Models;

namespace Toggl.Daneel.ViewControllers
{
    [MvxChildPresentation]
    public sealed class SyncFailuresViewController : MvxTableViewController<SyncFailuresViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.RegisterNibForCellReuse(SyncFailureCell.Nib, SyncFailureCell.Identifier);
            var tableViewSource = new ListTableViewSource<SyncFailureItem>(ViewModel.SyncFailures);
            tableViewSource.ConfigureCell = (source, tableView, indexPath, model) =>
            {
                var cell = tableView.DequeueReusableCell(SyncFailureCell.Identifier, indexPath) as SyncFailureCell;
                cell.Item = model;
                return cell;
            };
            TableView.Source = tableViewSource;
        }
    }
}
