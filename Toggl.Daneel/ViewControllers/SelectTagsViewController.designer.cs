// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Toggl.Daneel.ViewControllers
{
    [Register ("SelectTagsViewController")]
    partial class SelectTagsViewController
    {
        [Outlet]
        UIKit.NSLayoutConstraint BottomConstraint { get; set; }


        [Outlet]
        UIKit.UIImageView EmptyStateImage { get; set; }


        [Outlet]
        UIKit.UILabel EmptyStateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CloseButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SaveButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView TagsTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField TextField { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BottomConstraint != null) {
                BottomConstraint.Dispose ();
                BottomConstraint = null;
            }

            if (CloseButton != null) {
                CloseButton.Dispose ();
                CloseButton = null;
            }

            if (EmptyStateImage != null) {
                EmptyStateImage.Dispose ();
                EmptyStateImage = null;
            }

            if (EmptyStateLabel != null) {
                EmptyStateLabel.Dispose ();
                EmptyStateLabel = null;
            }

            if (SaveButton != null) {
                SaveButton.Dispose ();
                SaveButton = null;
            }

            if (TagsTableView != null) {
                TagsTableView.Dispose ();
                TagsTableView = null;
            }

            if (TextField != null) {
                TextField.Dispose ();
                TextField = null;
            }
        }
    }
}