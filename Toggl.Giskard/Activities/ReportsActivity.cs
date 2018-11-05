﻿using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Toggl.Foundation.MvvmCross.Extensions;
using Toggl.Foundation.MvvmCross.ViewModels.Reports;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.Adapters;
using Toggl.Giskard.Extensions.Reactive;
using Toggl.Giskard.Views;
using Toggl.Multivac.Extensions;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Toggl.Giskard.Activities
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class ReportsActivity : ReactiveActivity<ReportsViewModel>
    {
        private static readonly TimeSpan toggleCalendarThrottleDuration = TimeSpan.FromMilliseconds(300);

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ReportsActivity);
            OverridePendingTransition(Resource.Animation.abc_slide_in_right, Resource.Animation.abc_fade_out);

            InitializeViews();

            selectWorkspaceFAB.Rx().Tap()
                .Subscribe(ViewModel.SelectWorkspace)
                .DisposedBy(DisposeBag);

            ViewModel.WorkspaceNameObservable
                .Subscribe(workspaceName.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            calendarView.SetupWith(ViewModel.CalendarViewModel);

            setupReportsRecyclerView();
            setupToolbar();

            ViewModel.CurrentDateRangeStringObservable
                .Subscribe(toolbarCurrentDateRangeText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            toolbarCurrentDateRangeText.Rx().Tap()
                .Throttle(toggleCalendarThrottleDuration)
                .VoidSubscribe(ViewModel.ToggleCalendar)
                .DisposedBy(DisposeBag);
        }

        private void setupReportsRecyclerView()
        {
            var reportsAdapter = new ReportsRecyclerAdapter(this, ViewModel);
            reportsRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
            reportsRecyclerView.SetAdapter(reportsAdapter);
        }

        public override void OnEnterAnimationComplete()
        {
            base.OnEnterAnimationComplete();
            ViewModel.StopNavigationFromMainLogStopwatch();
        }

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_slide_out_right);
        }

        private void setupToolbar()
        {
            toolbar.Title = "";
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            toolbar.NavigationClick += onNavigateBack;
        }

        private void onNavigateBack(object sender, Toolbar.NavigationClickEventArgs e)
        {
            Finish();
        }

        internal void ToggleCalendarState(bool forceHide)
        {
            reportsMainContainer.ToggleCalendar(forceHide);
        }
    }
}
