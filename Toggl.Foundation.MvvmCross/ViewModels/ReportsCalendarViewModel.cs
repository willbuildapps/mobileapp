using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using PropertyChanged;
using Toggl.Foundation.Analytics;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.Extensions;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels.ReportsCalendar;
using Toggl.Foundation.MvvmCross.ViewModels.ReportsCalendar.QuickSelectShortcuts;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.Services;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class ReportsCalendarViewModel : MvxViewModel
    {
        public const int MonthsToShow = 13;

        //Fields
        private readonly ITimeService timeService;
        private readonly IDialogService dialogService;
        private readonly ITogglDataSource dataSource;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly IIntentDonationService intentDonationService;
        private readonly ISubject<ReportsDateRangeParameter> selectedDateRangeSubject = new Subject<ReportsDateRangeParameter>();
        private readonly ISubject<ReportsDateRangeParameter> highlightedDateRangeSubject = new Subject<ReportsDateRangeParameter>();
        private readonly string[] dayHeaders =
        {
            Resources.SundayInitial,
            Resources.MondayInitial,
            Resources.TuesdayInitial,
            Resources.WednesdayInitial,
            Resources.ThursdayInitial,
            Resources.FridayInitial,
            Resources.SaturdayInitial
        };

        private bool isInitialized;
        private CalendarMonth initialMonth;
        private CompositeDisposable disposableBag;
        private ReportsCalendarDayViewModel startOfSelection;
        private CompositeDisposable calendarDisposeBag;
        private CompositeDisposable shortcutDisposeBag;
        private QuickSelectShortcut weeklyQuickSelectShortcut;
        private ReportPeriod reportPeriod = ReportPeriod.ThisWeek;

        public BeginningOfWeek BeginningOfWeek { get; private set; }

        //Properties
        [DependsOn(nameof(CurrentPage))]
        [Obsolete("Use CurrentMonthObservable instead instead")]
        public CalendarMonth CurrentMonth => CurrentMonthObservable.LastOrDefault();
        public IObservable<CalendarMonth> CurrentMonthObservable { get; }

        [Obsolete("Use CurrentPageObservable instead")]
        public int CurrentPage => CurrentPageObservable.LastOrDefault();
        public IObservable<int> CurrentPageObservable { get; }
        private readonly ISubject<int> currentPageSubject = new Subject<int>();

        [DependsOn(nameof(Months), nameof(CurrentPage))]
        [Obsolete("Use RowsInCurrentMonthObservable instead")]
        public int RowsInCurrentMonth => RowsInCurrentMonthObservable.LastOrDefault();
        public IObservable<int> RowsInCurrentMonthObservable { get; }

        [Obsolete("Use MonthsObservable instead")]
        public IImmutableList<ReportsCalendarPageViewModel> Months => MonthsObservable.LastOrDefault();
        public IObservable<IImmutableList<ReportsCalendarPageViewModel>> MonthsObservable { get; }

        public IObservable<Unit> ReloadCalendar { get; }
        public IObservable<IImmutableList<string>> DayHeaders { get; }
        public IObservable<ReportsDateRangeParameter> SelectedDateRangeObservable { get; }

        public IObservable<IImmutableList<QuickSelectShortcut>> QuickSelectShortcuts { get; }

        [Obsolete("Use CalendarDayTapped instead")]
        public IMvxAsyncCommand<ReportsCalendarDayViewModel> CalendarDayTappedCommand { get; }
        [Obsolete("Use CalendarDayTapped instead")]
        public IMvxCommand<QuickSelectShortcut> QuickSelectCommand { get; }

        public ReportsCalendarViewModel(
            ITimeService timeService,
            IDialogService dialogService,
            ITogglDataSource dataSource,
            ISchedulerProvider schedulerProvider,
            IIntentDonationService intentDonationService)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(dialogService, nameof(dialogService));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(intentDonationService, nameof(intentDonationService));

            this.timeService = timeService;
            this.dialogService = dialogService;
            this.dataSource = dataSource;
            this.schedulerProvider = schedulerProvider;
            this.intentDonationService = intentDonationService;

            CalendarDayTappedCommand = new MvxAsyncCommand<ReportsCalendarDayViewModel>(calendarDayTapped);
            QuickSelectCommand = new MvxCommand<QuickSelectShortcut>(quickSelect);

            var currentDate = timeService.CurrentDateTime;
            initialMonth = new CalendarMonth(currentDate.Year, currentDate.Month).AddMonths(-MonthsToShow + 1);]

            calendarDisposeBag = new CompositeDisposable();
            shortcutDisposeBag = new CompositeDisposable();

            var beginningOfWeekObservable =
                dataSource.User.Current
                    .Select(user => user.BeginningOfWeek)
                    .DistinctUntilChanged()
                    .ConnectedReplay();

            var highlightObservable = highlightedDateRangeSubject.AsObservable();

            DayHeaders = beginningOfWeekObservable
                .Select(headers)
                .AsDriver(schedulerProvider);

            SelectedDateRangeObservable = selectedDateRangeSubject
                .AsObservable()
                .AsDriver(schedulerProvider);

            CurrentPageObservable = currentPageSubject
                .StartWith(MonthsToShow - 1)
                .AsObservable()
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            CurrentMonthObservable = CurrentPageObservable
                .Select(initialMonth.AddMonths)
                .AsDriver(schedulerProvider);

            ReloadCalendar = SelectedDateRangeObservable
                .Merge(highlightedDateRangeSubject.AsObservable())
                .SelectUnit()
                .AsDriver(schedulerProvider);

            QuickSelectShortcuts = beginningOfWeekObservable
                .Select(createQuickSelectShortcuts)
                .AsDriver(schedulerProvider);

            MonthsObservable = beginningOfWeekObservable
                .Select(calendarPages)
                .AsDriver(schedulerProvider);

            RowsInCurrentMonthObservable = CurrentPageObservable
                .CombineLatest(MonthsObservable,
                    (currentPage, months) => months[currentPage].RowCount)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            IImmutableList<string> headers(BeginningOfWeek beginningOfWeek)
                => Enumerable.Range(0, 7)
                    .Select(index => dayHeaders[(index + (int)beginningOfWeek + 7) % 7])
                    .ToImmutableList();

            IImmutableList<ReportsCalendarPageViewModel> calendarPages(BeginningOfWeek beginningOfWeek)
                => Enumerable.Range(0, MonthsToShow)
                    .Select(initialMonth.AddMonths)
                    .Select(calendarMonth => new ReportsCalendarPageViewModel(calendarMonth, beginningOfWeek, currentDate))
                    .Select(month => {
                        foreach (var day in month.Days)
                        {
                            SelectedDateRangeObservable
                                .Subscribe(day.OnSelectedRangeChanged)
                                .DisposedBy(calendarDisposeBag);

                            highlightObservable
                                .Subscribe(day.OnSelectedRangeChanged)
                                .DisposedBy(calendarDisposeBag);
                        }
                        return month;
                    })
                    .ToImmutableList();

            IImmutableList<QuickSelectShortcut> createQuickSelectShortcuts(BeginningOfWeek beginningOfWeek)
            {
                var shortcuts = ImmutableList.Create<QuickSelectShortcut>(
                    QuickSelectShortcut.ForToday(timeService),
                    QuickSelectShortcut.ForYesterday(timeService),
                    weeklyQuickSelectShortcut = QuickSelectShortcut.ForThisWeek(timeService, beginningOfWeek),
                    QuickSelectShortcut.ForLastWeek(timeService, beginningOfWeek),
                    QuickSelectShortcut.ForThisMonth(timeService),
                    QuickSelectShortcut.ForLastMonth(timeService),
                    QuickSelectShortcut.ForThisYear(timeService)
                );

                shortcuts.ForEach(shortcut =>
                {
                    SelectedDateRangeObservable
                        .Subscribe(shortcut.OnDateRangeChanged)
                        .DisposedBy(shortcutDisposeBag);

                });

                return shortcuts;
            }
        }

        public void QuickSelect(QuickSelectShortcut quickSelectShortCut)
        {
            intentDonationService.DonateShowReport(quickSelectShortCut.Period);
            changeDateRange(quickSelectShortCut.DateRange);
            currentPageSubject.OnNext(MonthsToShow - 1 - quickSelectShortCut.PageOffset);
        }

        public async Task CalendarDayTapped(ReportsCalendarDayViewModel tappedDay)
        {
            if (startOfSelection == null)
            {
                var date = tappedDay.DateTimeOffset;

                var dateRange = ReportsDateRangeParameter
                    .WithDates(date, date)
                    .WithSource(ReportsSource.Calendar);
                startOfSelection = tappedDay;
                highlightedDateRangeSubject.OnNext(dateRange);
            }
            else
            {
                var startDate = startOfSelection.DateTimeOffset;
                var endDate = tappedDay.DateTimeOffset;

                if (System.Math.Abs((endDate - startDate).Days) > 365)
                {
                    await dialogService.Alert(
                        Resources.ReportTooLongTitle,
                        Resources.ReportTooLongDescription,
                        Resources.Ok
                    );
                }
                else
                {
                    var dateRange = ReportsDateRangeParameter
                        .WithDates(startDate, endDate)
                        .WithSource(ReportsSource.Calendar);
                    startOfSelection = null;
                    changeDateRange(dateRange);
                }
            }
        }

        private async Task calendarDayTapped(ReportsCalendarDayViewModel tappedDay)
        {
            await CalendarDayTapped(tappedDay);
        }

        public override void Prepare()
        {
            base.Prepare();

            var now = timeService.CurrentDateTime;
            initialMonth = new CalendarMonth(now.Year, now.Month).AddMonths(-(MonthsToShow - 1));
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            BeginningOfWeek = (await dataSource.User.Current.FirstAsync()).BeginningOfWeek;
            RaisePropertyChanged(nameof(CurrentMonth));

            //QuickSelectShortcuts = createQuickSelectShortcuts();

            //QuickSelectShortcuts
                //.Select(quickSelectShortcut => SelectedDateRangeObservable.Subscribe(
                    //quickSelectShortcut.OnDateRangeChanged))
                //.ForEach(disposableBag.Add);

            var initialShortcut = QuickSelectShortcuts.LastOrDefault().Single(shortcut => shortcut.Period == reportPeriod);
            changeDateRange(initialShortcut.DateRange.WithSource(ReportsSource.Initial));
            isInitialized = true;
        }

        public void OnToggleCalendar() => selectStartOfSelectionIfNeeded();

        public void OnHideCalendar() => selectStartOfSelectionIfNeeded();

        [Obsolete("Use the DayHeaders observable instead")]
        public string DayHeaderFor(int index)
            => DayHeaders.LastOrDefault()[(index + (int)BeginningOfWeek + 7) % 7];

        public void SelectPeriod(ReportPeriod period)
        {
            reportPeriod = period;

            if (isInitialized)
            {
                var initialShortcut = QuickSelectShortcuts.LastOrDefault().Single(shortcut => shortcut.Period == period);
                changeDateRange(initialShortcut.DateRange.WithSource(ReportsSource.Initial));
            }
        }

        private void selectStartOfSelectionIfNeeded()
        {
            if (startOfSelection == null) return;

            var date = startOfSelection.DateTimeOffset;
            var dateRange = ReportsDateRangeParameter
                .WithDates(date, date)
                .WithSource(ReportsSource.Calendar);
            changeDateRange(dateRange);
        }

        private void changeDateRange(ReportsDateRangeParameter newDateRange)
        {
            startOfSelection = null;
            highlightDateRange(newDateRange);
            selectedDateRangeSubject.OnNext(newDateRange);
        }

        private void quickSelect(QuickSelectShortcut quickSelectShortCut)
        {
            QuickSelect(quickSelectShortCut);
        }

        private void highlightDateRange(ReportsDateRangeParameter dateRange)
        {
            Months.ForEach(month => month.Days.ForEach(day => day.OnSelectedRangeChanged(dateRange)));
        }
    }
}
