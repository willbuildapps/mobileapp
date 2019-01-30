﻿using CoreGraphics;
using Foundation;
using Toggl.Daneel.Cells;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewControllers
{
    [ModalDialogPresentation]
    public sealed partial class SelectDefaultWorkspaceViewController : ReactiveViewController<SelectDefaultWorkspaceViewModel>
    {
        private const int heightAboveTableView = 127;
        private const int width = 288;
        private readonly int maxHeight = UIScreen.MainScreen.Bounds.Width > 320 ? 627 : 528;

        public SelectDefaultWorkspaceViewController() : base(nameof(SelectDefaultWorkspaceViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            HeadingLabel.Text = Resources.SetDefaultWorkspace;
            DescriptionLabel.Text = Resources.SelectDefaultWorkspaceDescription;

            View.ClipsToBounds = true;

            WorkspacesTableView.RegisterNibForCellReuse(SelectDefaultWorkspaceTableViewCell.Nib, SelectDefaultWorkspaceTableViewCell.Identifier);
            var tableViewSource = new ListTableViewSource<SelectableWorkspaceViewModel>(ViewModel.Workspaces);
            tableViewSource.ConfigureCell = configureCell;
            tableViewSource.OnItemTapped = onWorkspaceTapped;
            WorkspacesTableView.Source = tableViewSource;
            WorkspacesTableView.TableFooterView = new UIKit.UIView(new CoreGraphics.CGRect(0, 0, UIKit.UIScreen.MainScreen.Bounds.Width, 24));
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            setDialogSize();
        }

        private void onWorkspaceTapped(object sender, SelectableWorkspaceViewModel workspace)
        {
            ViewModel.SelectWorkspace.Execute(workspace);
        }

        private UITableViewCell configureCell(ListTableViewSource<SelectableWorkspaceViewModel> source,
            UITableView tableView, NSIndexPath indexPath, SelectableWorkspaceViewModel model)
        {
            var cell = tableView.DequeueReusableCell(SelectDefaultWorkspaceTableViewCell.Identifier, indexPath) as SelectDefaultWorkspaceTableViewCell;
            cell.Item = model;
            return cell;
        }

        private void setDialogSize()
        {
            var targetHeight = calculateTargetHeight();
            PreferredContentSize = new CGSize(
                width,
                targetHeight > maxHeight ? maxHeight : targetHeight
            );

            //Implementation in ModalPresentationController
            View.Frame = PresentationController.FrameOfPresentedViewInContainerView;

            WorkspacesTableView.ScrollEnabled = targetHeight > maxHeight;
        }

        private int calculateTargetHeight()
            => heightAboveTableView + (int)WorkspacesTableView.ContentSize.Height;
    }
}
