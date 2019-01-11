using Android.Graphics;
using Toggl.Multivac;

namespace Toggl.Giskard.Views.EditDuration.Shapes
{
    public class Arc
    {
        private Paint paint = new Paint();
        private readonly RectF bounds;

        private float startAngle;
        private float endAngle;
        private float endStroke;

        public Color FillColor
        {
            get => paint.Color;
            set => paint.Color = value;
        }

        public float StrokeWidth
        {
            get => paint.StrokeWidth;
            set => paint.StrokeWidth = value;
        }

        public Arc(RectF bounds, float strokeWidth, Color fillColor)
        {
            this.bounds = bounds;
            StrokeWidth = strokeWidth;
            FillColor = fillColor;
            paint.SetStyle(Paint.Style.Stroke);
        }

        public void OnDraw(Canvas canvas)
        {
            var startAngleInDegrees = Java.Lang.Math.ToDegrees(startAngle);
            var endStrokeInDegrees = Java.Lang.Math.ToDegrees(endStroke);
            canvas?.DrawArc(bounds, (float)startAngleInDegrees, (float)endStrokeInDegrees, false, paint);
        }

        public void Update(double startTimeAngle, double endTimeAngle)
        {
            startAngle = (float)startTimeAngle;
            endAngle = (float)endTimeAngle;
            var diffAngle = endAngle - startAngle + (endAngle < startAngle ? Math.FullCircle : 0);
            endStroke = (float) diffAngle;
        }
    }
}
