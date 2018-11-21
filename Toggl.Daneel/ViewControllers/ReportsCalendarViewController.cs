using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Immutable;
using System.Reactive.Disposables;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;
using Toggl.Daneel.Converters;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Extensions.Reactive;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Daneel.Views.Reports;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.Converters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac.Extensions;
using UIKit;

namespace Toggl.Daneel.ViewControllers
{
    [NestedPresentation]
    public partial class ReportsCalendarViewController : ReactiveViewController<ReportsCalendarViewModel>
    {
        private bool calendarInitialized;

        public ReportsCalendarViewController()
            : base(nameof(ReportsCalendarViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var calendarCollectionViewSource = new ReportsCalendarCollectionViewSource(CalendarCollectionView);
            var calendarCollectionViewLayout = new ReportsCalendarCollectionViewLayout();
            CalendarCollectionView.DataSource = calendarCollectionViewSource;
            CalendarCollectionView.CollectionViewLayout = calendarCollectionViewLayout;

            var quickSelectCollectionViewSource = new ReportsCalendarQuickSelectCollectionViewSource(QuickSelectCollectionView);
            QuickSelectCollectionView.Source = quickSelectCollectionViewSource;

            ViewModel.DayHeadersObservable
                     .FirstAsync()
                     .Subscribe(setupDayHeaders)
                     .DisposedBy(DisposeBag);

            var bindingSet = this.CreateBindingSet<ReportsCalendarViewController, ReportsCalendarViewModel>();

            //Calendar collection view
            bindingSet.Bind(calendarCollectionViewSource).To(vm => vm.Months);
            bindingSet.Bind(calendarCollectionViewSource)
                      .For(v => v.CellTappedCommand)
                      .To(vm => vm.CalendarDayTappedCommand);

            //Quick select collection view
            bindingSet.Bind(quickSelectCollectionViewSource).To(vm => vm.QuickSelectShortcuts);
            bindingSet.Bind(quickSelectCollectionViewSource)
                      .For(v => v.SelectionChangedCommand)
                      .To(vm => vm.QuickSelectCommand);

            bindingSet.Apply();

            ViewModel.CurrentYearObservable
                     .Subscribe(CurrentYearLabel.Rx().Text())
                     .DisposedBy(DisposeBag);

            ViewModel.CurrentMonthNameObservable
                     .Subscribe(CurrentMonthLabel.Rx().Text())
                     .DisposedBy(DisposeBag);
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            var rowCountConverter = new ReportsCalendarRowCountToCalendarHeightConverter(
                ReportsCalendarCollectionViewLayout.CellHeight,
                View.Bounds.Height - CalendarCollectionView.Bounds.Height
            );
            //The constraint isn't available before DidMoveToParentViewController
            var heightConstraint = View
                .Superview
                .Constraints
                .Single(c => c.FirstAttribute == NSLayoutAttribute.Height);

            this.CreateBinding(heightConstraint)
                .For(v => v.BindAnimatedConstant())
                .To<ReportsCalendarViewModel>(vm => vm.RowsInCurrentMonth)
                .WithConversion(rowCountConverter, null)
                .Apply();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (calendarInitialized) return;

            //This binding needs the calendar to be in it's final size to work properly
            this.CreateBinding(CalendarCollectionView)
                .For(v => v.BindCurrentPage())
                .To<ReportsCalendarViewModel>(vm => vm.CurrentPage)
                .Apply();

            calendarInitialized = true;
        }

        private void setupDayHeaders(IImmutableList<string> headers)
        {
            DayHeader0.Text = headers[0];
            DayHeader1.Text = headers[1];
            DayHeader2.Text = headers[2];
            DayHeader3.Text = headers[3];
            DayHeader4.Text = headers[4];
            DayHeader5.Text = headers[5];
            DayHeader6.Text = headers[6];
        }
    }
}

