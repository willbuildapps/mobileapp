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

        private CalendarMonth initialMonth;
        private CompositeDisposable disposableBag;
        private ReportsCalendarDayViewModel startOfSelection;
        private CompositeDisposable calendarDisposeBag;
        private CompositeDisposable shortcutDisposeBag;
        private CompositeDisposable oldVariablesDisposeBag;
        private QuickSelectShortcut weeklyQuickSelectShortcut;
        private ReportPeriod reportPeriod = ReportPeriod.ThisWeek;

        public BeginningOfWeek BeginningOfWeek { get; private set; }

        //Properties
        [Obsolete("Use CurrentMonthObservable instead instead")]
        public CalendarMonth CurrentMonth { get; private set; }
        public IObservable<CalendarMonth> CurrentMonthObservable { get; }
        public IObservable<string> CurrentMonthNameObservable { get; }
        public IObservable<string> CurrentYearObservable { get; }

        [Obsolete("Use CurrentPageObservable instead")]
        public int CurrentPage { get; private set; }
        public IObservable<int> CurrentPageObservable { get; }
        private readonly ISubject<int> currentPageSubject = new Subject<int>();

        [Obsolete("Use RowsInCurrentMonthObservable instead")]
        public int RowsInCurrentMonth { get; private set; }
        public IObservable<int> RowsInCurrentMonthObservable { get; }

        [Obsolete("Use MonthsObservable instead")]
        public IImmutableList<ReportsCalendarPageViewModel> Months { get; private set; }
        public IObservable<IImmutableList<ReportsCalendarPageViewModel>> MonthsObservable { get; }

        public IObservable<Unit> ReloadCalendar { get; }
        public IImmutableList<string> DayHeaders { get; private set; }
        public IObservable<IImmutableList<string>> DayHeadersObservable { get; }
        public IObservable<ReportsDateRangeParameter> SelectedDateRangeObservable { get; }

        [Obsolete("Use QuickSelectShortcutsObservable instead")]
        public IImmutableList<QuickSelectShortcut> QuickSelectShortcuts { get; private set; }
        public IObservable<IImmutableList<QuickSelectShortcut>> QuickSelectShortcutsObservable { get; }

        public InputAction<ReportsCalendarDayViewModel> CalendarDayTapped { get; }
        public InputAction<QuickSelectShortcut> QuickSelect { get; }

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

            CalendarDayTapped = InputAction<ReportsCalendarDayViewModel>.FromAsync(calendarDayTapped);
            QuickSelect = InputAction<QuickSelectShortcut>.FromAction(quickSelect);

            var currentDate = timeService.CurrentDateTime;
            initialMonth = new CalendarMonth(currentDate.Year, currentDate.Month).AddMonths(-MonthsToShow + 1);

            calendarDisposeBag = new CompositeDisposable();
            shortcutDisposeBag = new CompositeDisposable();
            oldVariablesDisposeBag = new CompositeDisposable();

            var beginningOfWeekObservable =
                dataSource.User.Current
                    .Select(user => user.BeginningOfWeek)
                    .DistinctUntilChanged()
                    .ConnectedReplay();

            var highlightObservable = highlightedDateRangeSubject.AsObservable();

            DayHeadersObservable = beginningOfWeekObservable
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

            CurrentMonthNameObservable = CurrentMonthObservable
                .Select(month => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month.Month))
                .AsDriver(schedulerProvider);

            CurrentYearObservable = CurrentMonthObservable
                .Select(month => month.Year.ToString())
                .AsDriver(schedulerProvider);

            ReloadCalendar = SelectedDateRangeObservable
                .Merge(highlightedDateRangeSubject.AsObservable())
                .SelectUnit()
                .AsDriver(schedulerProvider);

            QuickSelectShortcutsObservable = beginningOfWeekObservable
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

            CurrentMonthObservable
                .Subscribe(month => CurrentMonth = month)
                .DisposedBy(oldVariablesDisposeBag);

            CurrentPageObservable
                .Subscribe(page => CurrentPage = page)
                .DisposedBy(oldVariablesDisposeBag);

            RowsInCurrentMonthObservable
                .Subscribe(rows => RowsInCurrentMonth = rows)
                .DisposedBy(oldVariablesDisposeBag);

            MonthsObservable
                .Subscribe(months => Months = months)
                .DisposedBy(oldVariablesDisposeBag);

            QuickSelectShortcutsObservable
                .Subscribe(shortcuts => QuickSelectShortcuts = shortcuts)
                .DisposedBy(oldVariablesDisposeBag);

            DayHeadersObservable
                .Subscribe(dayHeaders => DayHeaders = dayHeaders)
                .DisposedBy(oldVariablesDisposeBag);

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

                changeDateRange(weeklyQuickSelectShortcut.DateRange);

                return shortcuts;
            }
        }

        private void quickSelect(QuickSelectShortcut quickSelectShortCut)
        {
            intentDonationService.DonateShowReport(quickSelectShortCut.Period);
            changeDateRange(quickSelectShortCut.DateRange);
            currentPageSubject.OnNext(MonthsToShow - 1 - quickSelectShortCut.PageOffset);
        }

        private async Task calendarDayTapped(ReportsCalendarDayViewModel tappedDay)
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

        public void OnToggleCalendar() => selectStartOfSelectionIfNeeded();

        public void OnHideCalendar() => selectStartOfSelectionIfNeeded();

        [Obsolete("Use the DayHeaders observable instead")]
        public string DayHeaderFor(int index)
            => DayHeaders[index];

        public void SelectPeriod(ReportPeriod period)
        {
            reportPeriod = period;
            var initialShortcut = QuickSelectShortcuts.Single(shortcut => shortcut.Period == period);
            changeDateRange(initialShortcut.DateRange.WithSource(ReportsSource.Initial));
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
            highlightedDateRangeSubject.OnNext(newDateRange);
            selectedDateRangeSubject.OnNext(newDateRange);
        }
    }
}
