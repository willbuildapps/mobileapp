using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Immutable;
using System.Reactive.Disposables;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;
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

             // Calendar collection view
            ViewModel.MonthsObservable
                     .Subscribe(calendarCollectionViewSource.CollectionChanged)
                     .DisposedBy(DisposeBag);

            calendarCollectionViewSource.ItemTapped
                                        .Subscribe(ViewModel.CalendarDayTapped.Inputs)
                                        .DisposedBy(DisposeBag);

            CalendarCollectionView.DataSource = calendarCollectionViewSource;
            CalendarCollectionView.CollectionViewLayout = calendarCollectionViewLayout;

            var quickSelectCollectionViewSource = new ReportsCalendarQuickSelectCollectionViewSource(QuickSelectCollectionView);
            QuickSelectCollectionView.Source = quickSelectCollectionViewSource;

            ViewModel.DayHeadersObservable
                     .FirstAsync()
                     .Subscribe(setupDayHeaders)
                     .DisposedBy(DisposeBag);

            //Quick select collection view
            // bindingSet.Bind(quickSelectCollectionViewSource).To(vm => vm.QuickSelectShortcuts);
            // bindingSet.Bind(quickSelectCollectionViewSource)
            // .For(v => v.SelectionChangedCommand)
            // .To(vm => vm.QuickSelectCommand);

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

            //The constraint isn't available before DidMoveToParentViewController
            var heightConstraint = View
                .Superview
                .Constraints
                .Single(c => c.FirstAttribute == NSLayoutAttribute.Height);

            var rowHeight = ReportsCalendarCollectionViewLayout.CellHeight;
            var additionalHeight = View.Bounds.Height - CalendarCollectionView.Bounds.Height;

            ViewModel.RowsInCurrentMonthObservable
                     .Select(rows => rows * rowHeight + additionalHeight)
                     .Subscribe(heightConstraint.Rx().ConstantAnimated())
                     .DisposedBy(DisposeBag);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (calendarInitialized) return;

            ViewModel.CurrentPageObservable
                .Subscribe(CalendarCollectionView.Rx().CurrentPageObserver())
                .DisposedBy(DisposeBag);

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

