using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Plugin.Color.Platforms.Ios;
using Toggl.Daneel.Cells;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.MvvmCross.Converters;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.Views
{
    public sealed partial class SelectableTagViewCell : BaseTableViewCell<SelectableTagViewModel>
    {
        private static readonly UIColor selectedBackgroundColor
            = Color.Common.LightGray.ToNativeColor();

        public static readonly NSString Identifier = new NSString(nameof(SelectableTagViewCell));
        public static readonly UINib Nib;

        private static UIImage checkBoxCheckedImage = UIImage.FromBundle("icCheckBoxChecked");
        private static UIImage checkBoxUncheckedImage = UIImage.FromBundle("icCheckBoxUnchecked");

        static SelectableTagViewCell()
        {
            Nib = UINib.FromName(nameof(SelectableTagViewCell), NSBundle.MainBundle);
        }

        public override void SetSelected(bool selected, bool animated)
            => setBackgroundColor(selected, animated);

        public override void SetHighlighted(bool highlighted, bool animated)
            => setBackgroundColor(highlighted, animated);

        private void setBackgroundColor(bool selected, bool animated)
        {
            var targetColor = selected ? selectedBackgroundColor : UIColor.White;

            if (animated)
                animateBackgroundColor(targetColor);
            else
                BackgroundColor = targetColor;
        }

        private void animateBackgroundColor(UIColor color)
        {
            AnimationExtensions.Animate(
                Animation.Timings.EnterTiming,
                Animation.Curves.EaseIn,
                () => BackgroundColor = color
            );
        }

        protected override void UpdateView()
        {
            TagLabel.Text = Item.Name;
            SelectedImage.Image = Item.Selected ? checkBoxCheckedImage : checkBoxUncheckedImage;
        }
    }
}
