using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.Extensions;
using Toggl.Giskard.Extensions.Reactive;
using Toggl.Multivac.Extensions;

namespace Toggl.Giskard.Activities
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme.BlueStatusBar",
        WindowSoftInputMode = SoftInput.AdjustResize,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class EditDurationActivity : ReactiveActivity<EditDurationViewModel>
    {
        private readonly Subject<Unit> saveSubject = new Subject<Unit>();
        private EditMode editMode;
        private Dialog editDialog;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.EditDurationActivity);
            InitializeViews();
            setupToolbar();

            ViewModel.StartTimeString
                .Subscribe(startTimeText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.StartDateString
                .Subscribe(startDateText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.StopTimeString
                .Subscribe(stopTimeText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.StopDateString
                .Subscribe(stopDateText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.IsRunning
                .Subscribe(running =>
                {
                    var stopDateTimeViewsVisibility = (!running).ToVisibility(useGone: false);
                    stopTimerLabel.Visibility = running.ToVisibility();
                    stopTimeText.Visibility = stopDateTimeViewsVisibility;
                    stopDateText.Visibility = stopDateTimeViewsVisibility;
                    stopDotSeparator.Visibility = stopDateTimeViewsVisibility;
                })
                .DisposedBy(DisposeBag);

            stopTimerLabel.Rx()
                .BindAction(ViewModel.EditStopTime)
                .DisposedBy(DisposeBag);

            stopTimerLabel.Rx().Tap()
                .Subscribe(_ => { editMode = EditMode.Time; })
                .DisposedBy(DisposeBag);

            startTimeText.Rx().Tap()
                .Subscribe(_ => { editMode = EditMode.Time; })
                .DisposedBy(DisposeBag);

            startDateText.Rx().Tap()
                .Subscribe(_ => { editMode = EditMode.Date; })
                .DisposedBy(DisposeBag);

            startTimeText.Rx()
                .BindAction(ViewModel.EditStartTime)
                .DisposedBy(DisposeBag);

            startDateText.Rx()
                .BindAction(ViewModel.EditStartTime)
                .DisposedBy(DisposeBag);

            stopTimeText.Rx().Tap()
                .Subscribe(_ => { editMode = EditMode.Time; })
                .DisposedBy(DisposeBag);

            stopDateText.Rx().Tap()
                .Subscribe(_ => { editMode = EditMode.Date; })
                .DisposedBy(DisposeBag);

            stopTimeText.Rx()
                .BindAction(ViewModel.EditStopTime)
                .DisposedBy(DisposeBag);

            stopDateText.Rx()
                .BindAction(ViewModel.EditStopTime)
                .DisposedBy(DisposeBag);


            stopTimeText.Rx().Tap()
                .Subscribe()
                .DisposedBy(DisposeBag);

            stopDateText.Rx().Tap()
                .Subscribe()
                .DisposedBy(DisposeBag);

            var activeTimeSubject = new Subject<DateTimeOffset>();
            var editEndedSubject = new Subject<Unit>();

            ViewModel.IsEditingStartTime
                .Where(CommonFunctions.Identity)
                .SelectMany(_ => ViewModel.StartTime)
                .Subscribe(time =>
                {
                    if (editMode == EditMode.Time)
                    {
                        editTime(time.ToLocalTime(), activeTimeSubject, editEndedSubject);
                    }
                    else
                    {
                        editDate(time.ToLocalTime(), activeTimeSubject, editEndedSubject);
                    }
                })
                .DisposedBy(DisposeBag);

            ViewModel.IsEditingStopTime
                .Where(CommonFunctions.Identity)
                .SelectMany(_ => ViewModel.StopTime)
                .Subscribe(time =>
                {
                    if (editMode == EditMode.Time)
                    {
                        editTime(time.ToLocalTime(), activeTimeSubject, editEndedSubject);
                    }
                    else
                    {
                        editDate(time.ToLocalTime(), activeTimeSubject, editEndedSubject);
                    }
                })
                .DisposedBy(DisposeBag);

            activeTimeSubject
                .Subscribe(ViewModel.ChangeActiveTime.Inputs)
                .DisposedBy(DisposeBag);

            editEndedSubject
                .Subscribe(ViewModel.StopEditingTime.Inputs)
                .DisposedBy(DisposeBag);
            saveSubject
                .Subscribe(ViewModel.Save.Inputs)
                .DisposedBy(DisposeBag);
        }

        private void editTime(DateTimeOffset currentTime, Subject<DateTimeOffset> timeChangedSubject, Subject<Unit> editEndedSubject)
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.GenericSaveMenu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                editDialog = new TimePickerDialog(this, new TimePickerListener(currentTime, timeChangedSubject),
                    currentTime.Hour, currentTime.Minute, false);
                editDialog.DismissEvent += (_, __) =>
                {
                    editDialog = null;
                    editEndedSubject.OnNext(Unit.Default);
                };
                editDialog.Show();
                case Resource.Id.SaveMenuItem:
                    saveSubject.OnNext(Unit.Default);
                    return true;

                case Android.Resource.Id.Home:
                    navigateBack();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void setupToolbar()
        {
           toolbar.Title = Foundation.Resources.StartAndStopTime;

            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        public override void OnBackPressed()
        {
            navigateBack();
            base.OnBackPressed();
        }

        private void navigateBack()
        {
            ViewModel.Close.Execute();
        }
            }
        }

        private void editDate(DateTimeOffset currentDate, Subject<DateTimeOffset> dateChangedSubject, Subject<Unit> editEndedSubject)
        {
            if (editDialog == null)
            {
                editDialog = new DatePickerDialog(this, new DatePickerListener(currentDate, dateChangedSubject),
                    currentDate.Year, currentDate.Month, currentDate.Day);
                editDialog.DismissEvent += (_, __) =>
                {
                    editDialog = null;
                    editEndedSubject.OnNext(Unit.Default);
                };
                editDialog.Show();
            }
        }

        private sealed class TimePickerListener : Java.Lang.Object, TimePickerDialog.IOnTimeSetListener
        {
            private readonly DateTimeOffset currentTime;
            private readonly Subject<DateTimeOffset> editTimeSubject;

            public TimePickerListener(DateTimeOffset currentTime, Subject<DateTimeOffset> editTimeSubject)
            {
                this.currentTime = currentTime;
                this.editTimeSubject = editTimeSubject;
            }

            public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
            {
                var pickedTime = new DateTimeOffset(currentTime.Year, currentTime.Month, currentTime.Day, hourOfDay, minute, currentTime.Minute, currentTime.Millisecond, currentTime.Offset);
                editTimeSubject.OnNext(pickedTime);
            }
        }

        private sealed class DatePickerListener : Java.Lang.Object, DatePickerDialog.IOnDateSetListener
        {
            private readonly DateTimeOffset currentDate;
            private readonly Subject<DateTimeOffset> editDateSubject;

            public DatePickerListener(DateTimeOffset currentDate, Subject<DateTimeOffset> editDateSubject)
            {
                this.currentDate = currentDate;
                this.editDateSubject = editDateSubject;
            }

            public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
            {
                var pickedDate = new DateTimeOffset(year, month, dayOfMonth, currentDate.Hour, currentDate.Minute, currentDate.Minute, currentDate.Millisecond, currentDate.Offset);
                editDateSubject.OnNext(pickedDate);
            }
        }

        private enum EditMode
        {
            Time,
            Date
        }
    }
}
