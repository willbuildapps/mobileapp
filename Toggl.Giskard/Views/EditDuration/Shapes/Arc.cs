using Android.Graphics;
using Toggl.Multivac;

namespace Toggl.Giskard.Views.EditDuration.Shapes
{
    public class Arc
    {
        private readonly Paint paint = new Paint(PaintFlags.AntiAlias);
        private readonly RectF bounds;

        private float startAngle;
        private float endAngle;
        private float endStroke;

        public Color FillColor
        {
            set => paint.Color = value;
        }

        public Arc(RectF bounds, float strokeWidth, Color fillColor)
        {
            this.bounds = bounds;
            FillColor = fillColor;
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = strokeWidth;
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
