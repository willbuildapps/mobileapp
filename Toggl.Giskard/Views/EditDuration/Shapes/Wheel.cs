using Android.Graphics;

namespace Toggl.Giskard.Views.EditDuration.Shapes
{
    public sealed class Wheel
    {
        private Paint paint = new Paint();
        private readonly RectF bounds;

        public bool Hidden { get; set; }

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

        public Wheel(RectF bounds, float strokeWidth, Color fillColor)
        {
            this.bounds = bounds;
            StrokeWidth = strokeWidth;
            FillColor = fillColor;
            paint.SetStyle(Paint.Style.Stroke);
            Hidden = false;
        }


        public void OnDraw(Canvas canvas)
        {
            if (Hidden) return;

            canvas?.DrawArc(bounds, 0f, 360f, false, paint);
        }
    }
}
