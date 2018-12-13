using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Commands;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Plugin.Color.Platforms.Ios;
using MvvmCross.Plugin.Visibility;
using Toggl.Daneel.Combiners;
using Toggl.Foundation.MvvmCross.Converters;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels.ReportsCalendar;
using UIKit;

namespace Toggl.Daneel.Views
{
    public sealed partial class ReportsCalendarViewCell : ReactiveCollectionViewCell<ReportsCalendarDayViewModel>
    {
        private const int cornerRadius = 16;

        public static readonly NSString Key = new NSString(nameof(ReportsCalendarViewCell));
        public static readonly UINib Nib;

        static ReportsCalendarViewCell()
        {
            Nib = UINib.FromName(nameof(ReportsCalendarViewCell), NSBundle.MainBundle);
        }

        public ReportsCalendarViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            prepareViews();
        }

        private void prepareViews()
        {
            //Background view
            BackgroundView.CornerRadius = cornerRadius;

            //Today background indicator
            TodayBackgroundView.CornerRadius = cornerRadius;
            TodayBackgroundView.RoundLeft = true;
            TodayBackgroundView.RoundRight = true;
            TodayBackgroundView.BackgroundColor = Color.ReportsCalendar.Today.ToNativeColor();
        }

        private readonly UIColor otherMonthColor = Color.ReportsCalendar.CellTextColorOutOfCurrentMonth.ToNativeColor();
        private readonly UIColor thisMonthColor = Color.ReportsCalendar.CellTextColorInCurrentMonth.ToNativeColor();
        private readonly UIColor selectedColor = Color.ReportsCalendar.CellTextColorSelected.ToNativeColor();

        protected override void UpdateView()
        {
            Text.Text = Item.Day.ToString();
            if (Item.Selected)
            {
                Text.TextColor = selectedColor;
            }
            else
            {
                Text.TextColor = Item.IsInCurrentMonth ? thisMonthColor : otherMonthColor;
            }

            BackgroundView.BackgroundColor = Item.Selected
                ? Color.ReportsCalendar.SelectedDayBackgoundColor.ToNativeColor()
                : Color.Common.Transparent.ToNativeColor();

            BackgroundView.RoundLeft = Item.IsStartOfSelectedPeriod;
            BackgroundView.RoundRight = Item.IsEndOfSelectedPeriod;
            TodayBackgroundView.Hidden = !Item.IsToday;
        }
    }
}
