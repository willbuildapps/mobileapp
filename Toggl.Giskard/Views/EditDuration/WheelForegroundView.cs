using System;
using System.Reactive.Subjects;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using MvvmCross.Plugin.Color.Platforms.Android;
using Toggl.Foundation.Analytics;
using Toggl.Giskard.Extensions;
using Toggl.Giskard.Views.EditDuration.Shapes;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using static Toggl.Multivac.Math;
using FoundationColor = Toggl.Foundation.MvvmCross.Helper.Color;

namespace Toggl.Giskard.Views.EditDuration
{
    [Register("toggl.giskard.views.WheelForegroundView")]
    public class WheelForegroundView : View
    {
        private float radius;
        private float arcWidth;
        private PointF startTimePosition;
        private PointF endTimePosition;
        private PointF center;
        private RectF bounds;

        private DateTimeOffset startTime = DateTimeOffset.Now - TimeSpan.FromHours(1.88f);
        private DateTimeOffset endTime = DateTimeOffset.Now;
        private bool isRunning;

        private double startTimeAngle => startTime.LocalDateTime.TimeOfDay.ToAngleOnTheDial().ToPositiveAngle();
        private double endTimeAngle => endTime.LocalDateTime.TimeOfDay.ToAngleOnTheDial().ToPositiveAngle();

        private double endPointsRadius;
        private int numberOfFullLoops => (int) ((EndTime - StartTime).TotalMinutes / MinutesInAnHour);
        private bool isFullCircle => numberOfFullLoops >= 1;

        private Color backgroundColor
            => FoundationColor.EditDuration.Wheel.Rainbow.GetPingPongIndexedItem(numberOfFullLoops).ToNativeColor();

        private Color foregroundColor
            => FoundationColor.EditDuration.Wheel.Rainbow.GetPingPongIndexedItem(numberOfFullLoops + 1).ToNativeColor();

        private readonly Subject<EditTimeSource> timeEditedSubject = new Subject<EditTimeSource>();

        private Wheel fullWheel;
        private Arc arc;
        private Cap endCap;
        private Cap startCap;

        public DateTimeOffset MinimumStartTime { get; set; }

        public DateTimeOffset MaximumStartTime { get; set; }

        public DateTimeOffset MinimumEndTime { get; set; }

        public DateTimeOffset MaximumEndTime { get; set; }

        public DateTimeOffset StartTime
        {
            get => startTime;
            set
            {
                if (startTime == value) return;
                startTime = value.Clamp(MinimumStartTime, MaximumStartTime);
                //needs layout
            }
        }

        public DateTimeOffset EndTime
        {
            get => endTime;
            set
            {
                if (endTime == value) return;
                endTime = value.Clamp(MinimumEndTime, MaximumEndTime);
                RequestLayout();
            }
        }

        public bool IsRunning
        {
            get => isRunning;
            set
            {
                if (isRunning == value) return;
                isRunning = value;
                RequestLayout();
            }
        }

        #region Constructors

        protected WheelForegroundView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public WheelForegroundView(Context context) : base(context)
        {
        }

        public WheelForegroundView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public WheelForegroundView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public WheelForegroundView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        #endregion

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            radius = Width * 0.5f;
            arcWidth = 28.DpToPixels(Context);
            center = new PointF(radius, radius);
            bounds = new RectF(arcWidth / 2.0f, arcWidth / 2.0f, Width - arcWidth / 2.0f, Width - arcWidth / 2.0f);
            endPointsRadius = radius - arcWidth / 2.0f;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            setupDrawingDelegates();
            calculateEndPointPositions();
            updateUIElements();
            fullWheel.OnDraw(canvas);
            arc.OnDraw(canvas);
            startCap.OnDraw(canvas);
            endCap.OnDraw(canvas);
        }

        private void setupDrawingDelegates()
        {
            if (fullWheel == null)
            {
                fullWheel = new Wheel(bounds, arcWidth, backgroundColor);
                arc = new Arc(bounds, arcWidth, Color.Transparent);
                endCap = new Cap((float)(arcWidth / 2.0), foregroundColor);
                startCap = new Cap((float)(arcWidth / 2.0), foregroundColor);
            }
        }

        private void calculateEndPointPositions()
        {
            var center = this.center.ToMultivacPoint();

            startTimePosition = PointOnCircumference(center, startTimeAngle, endPointsRadius).ToPointF();
            endTimePosition = PointOnCircumference(center, endTimeAngle, endPointsRadius).ToPointF();
        }

        private void updateUIElements()
        {
            startCap.Position = startTimePosition;
            startCap.FillColor = foregroundColor;
            endCap.Position = endTimePosition;
            endCap.FillColor = foregroundColor;
            endCap.ShowOnlyBackground = IsRunning;

            fullWheel.FillColor = backgroundColor;
            fullWheel.Hidden = !isFullCircle;

            arc.FillColor = foregroundColor;
            arc.Update(startTimeAngle, endTimeAngle);
        }
    }
}
