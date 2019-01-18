using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;
using Toggl.Foundation.MvvmCross.Helper;
using System.Threading.Tasks;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Extensions.Reactive;
using Toggl.Daneel.Views.Tag;
using Toggl.Daneel.ViewSources;
using Toggl.Multivac.Extensions;

namespace Toggl.Daneel.ViewControllers
{
    [ModalCardPresentation]
    public sealed partial class SelectTagsViewController : KeyboardAwareViewController<SelectTagsViewModel>, IDismissableViewController
    {
        private SelectTagsTableViewSource tableViewSource = new SelectTagsTableViewSource();

        public SelectTagsViewController() 
            : base(nameof(SelectTagsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TagsTableView.RegisterNibForCellReuse(NewTagViewCell.Nib, NewTagViewCell.Identifier);
            TagsTableView.RegisterNibForCellReuse(CreateTagViewCell.Nib, CreateTagViewCell.Identifier);
            TagsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            TagsTableView.Source = tableViewSource;

            ViewModel.Tags
                .Subscribe(replaceTags)
                .DisposedBy(DisposeBag);

            ViewModel.HasTag
                .Invert()
                .Subscribe(EmptyStateImage.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.HasTag
                .Invert()
                .Subscribe(EmptyStateLabel.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.FilterText
                .Subscribe(TextField.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            tableViewSource.TagSelected
                .Subscribe(ViewModel.SelectTag.Inputs)
                .DisposedBy(DisposeBag);

            CloseButton.Rx()
                .BindAction(ViewModel.Close)
                .DisposedBy(DisposeBag);

            SaveButton.Rx()
                .BindAction(ViewModel.Save)
                .DisposedBy(DisposeBag);

            TextField.Rx().Text()
                .Subscribe(ViewModel.FilterText)
                .DisposedBy(DisposeBag);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            TextField.BecomeFirstResponder();
        }

        public async Task<bool> Dismiss()
        {
            await ViewModel.Close.Execute();
            return true;
        }

        private void replaceTags(IEnumerable<SelectableTagBaseViewModel> tags)
        {
            tableViewSource.SetNewTags(tags);
            TagsTableView.ReloadData();
        }

        protected override void KeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            BottomConstraint.Constant = e.FrameEnd.Height;
            UIView.Animate(Animation.Timings.EnterTiming, () => View.LayoutIfNeeded());
        }

        protected override void KeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            BottomConstraint.Constant = 0;
            UIView.Animate(Animation.Timings.EnterTiming, () => View.LayoutIfNeeded());
        }
    }
}
