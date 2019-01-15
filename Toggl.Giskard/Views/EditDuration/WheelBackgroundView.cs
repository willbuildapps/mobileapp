using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Toggl.Giskard.Extensions;
using Toggl.Giskard.Views.EditDuration.Shapes;

namespace Toggl.Giskard.Views.EditDuration
{
    [Register("toggl.giskard.views.WheelBackgroundView")]
    public class WheelBackgroundView : View
    {
        private readonly Color wheelColor = Color.ParseColor("#f3f3f3");

        private PointF center;
        private RectF bounds;

        private float radius;
        private float arcWidth;
        private float margins;
        private Wheel wheel;

        #region Constructors

        protected WheelBackgroundView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public WheelBackgroundView(Context context) : base(context)
        {
            init();
        }

        public WheelBackgroundView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            init();
        }

        public WheelBackgroundView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            init();
        }

        public WheelBackgroundView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            init();
        }

        private void init()
        {
            arcWidth = 8.DpToPixels(Context);
            margins = 28.DpToPixels(Context);
        }

        #endregion

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            radius = Width * 0.5f;
            center = new PointF(radius, radius);
            bounds = new RectF(margins, margins, Width - margins, Width - margins);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            setupDrawingDelegates();
            wheel.OnDraw(canvas);
        }

        private void setupDrawingDelegates()
        {
            if (wheel == null)
            {
                wheel = new Wheel(bounds, arcWidth, wheelColor);
            }
        }
    }
}
