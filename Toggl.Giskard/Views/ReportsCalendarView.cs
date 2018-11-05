using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Widget;
using MvvmCross.WeakSubscription;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.ViewModels.ReportsCalendar.QuickSelectShortcuts;
using Toggl.Giskard.Adapters;
using Toggl.Giskard.Extensions;
using Toggl.Giskard.ViewHolders;
using Toggl.Multivac.Extensions;

namespace Toggl.Giskard.Views
{
    [Register("toggl.giskard.views.ReportsCalendarView")]
    public sealed class ReportsCalendarView : LinearLayout
    {
        private TextView monthYear;
        private LinearLayout daysHeader;
        private ViewPager monthsPager;
        private RecyclerView shortcutsRecyclerView;

        private int rowHeight;
        private int currentRowCount;
        private bool isInitialized;

        private ReportsCalendarViewModel viewModel;

        private CompositeDisposable disposableBag = new CompositeDisposable();

        public ReportsCalendarView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ReportsCalendarView(Context context) : base(context)
        {
            Init(Context);
        }

        public ReportsCalendarView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(Context);
        }

        public ReportsCalendarView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init(Context);
        }

        public ReportsCalendarView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init(Context);
        }

        private void Init(Context context)
        {
            Inflate(Context, Resource.Layout.ReportsCalendarView, this);
            SetBackgroundColor(new Android.Graphics.Color(ContextCompat.GetColor(Context, Resource.Color.toolbarBlack)));

            rowHeight = context.Resources.DisplayMetrics.WidthPixels / 7;

            monthYear = FindViewById<TextView>(Resource.Id.ReportsCalendarMonthYear);
            daysHeader = FindViewById<LinearLayout>(Resource.Id.ReportsCalendarFragmentHeader);
            monthsPager = FindViewById<ViewPager>(Resource.Id.ReportsCalendarFragmentViewPager);
            shortcutsRecyclerView = FindViewById<RecyclerView>(Resource.Id.ReportsCalendarFragmentShortcuts);
            shortcutsRecyclerView.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Horizontal, false));
        }

        public void SetupWith(ReportsCalendarViewModel reportsCalendarViewModel)
        {
            viewModel = reportsCalendarViewModel;

            monthYear.Text = "Some string"; //local:MvxBind="Text Format('{0} {1}', IntToMonthName(CurrentMonth.Month), CurrentMonth.Year)"

            setupWeekdaysLabels();

            monthsPager.Adapter = new ReportsCalendarPagerAdapter(Context, viewModel);
            monthsPager.SetCurrentItem(viewModel.Months.Count - 1, false);

            setupShortcuts();

            viewModel.WeakSubscribe<PropertyChangedEventArgs>(nameof(viewModel.RowsInCurrentMonth), onRowCountChanged)
                .DisposedBy(disposableBag);

            viewModel.SelectedDateRangeObservable.Subscribe(onDateRangeChanged)
                .DisposedBy(disposableBag);

            recalculatePagerHeight();
        }

        private void setupWeekdaysLabels()
        {
            daysHeader
                .GetChildren<TextView>()
                .Indexed()
                .ForEach((textView, index)
                    => textView.Text = viewModel.DayHeaderFor(index));
        }

        private void setupShortcuts()
        {
            var adapter = new SimpleAdapter<ReportsCalendarBaseQuickSelectShortcut>(
                Resource.Layout.ReportsCalendarShortcutCell,
                ReportsCalendarShortcutCellViewHolder.Create)
            {
                Items = viewModel.QuickSelectShortcuts,
//                OnItemTapped = viewModel.QuickSelectAction
            };
            shortcutsRecyclerView.SetAdapter(adapter);
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            recalculatePagerHeight();
        }

        private void onDateRangeChanged(ReportsDateRangeParameter dateRange)
        {
            var anyShortcutIsSelected = viewModel.QuickSelectShortcuts.Any(shortcut => shortcut.Selected);
            if (!anyShortcutIsSelected) return;

            var dateRangeStartDate = dateRange.StartDate;
            var monthToScroll = viewModel.Months.IndexOf(month => month.CalendarMonth.Month == dateRangeStartDate.Month);
            if (monthToScroll == monthsPager.CurrentItem) return;

            var dateRangeStartDateIsContaintedInCurrentMonthView = viewModel
                .Months[monthsPager.CurrentItem]
                .Days.Any(day => day.DateTimeOffset == dateRangeStartDate);

            if (!dateRangeStartDateIsContaintedInCurrentMonthView || dateRangeStartDate.Month == dateRange.EndDate.Month)
            {
                monthsPager.SetCurrentItem(monthToScroll, true);
            }
        }

        private void onRowCountChanged(object sender, PropertyChangedEventArgs e)
        {
            if (currentRowCount == viewModel.RowsInCurrentMonth)
                return;
            recalculatePagerHeight();
        }

        private void recalculatePagerHeight()
        {
            currentRowCount = viewModel.RowsInCurrentMonth;

            var layoutParams = monthsPager.LayoutParameters;
            layoutParams.Height = rowHeight * currentRowCount;
            monthsPager.LayoutParameters = layoutParams;

            var parent = Parent as ReportsLinearLayout;
            parent?.RecalculateCalendarHeight();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing || disposableBag == null) return;

            disposableBag.Dispose();
        }
    }
}
