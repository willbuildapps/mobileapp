using Android.Graphics;

namespace Toggl.Giskard.Views.EditDuration.Shapes
{
    public sealed class Wheel
    {
        private readonly Paint paint = new Paint(PaintFlags.AntiAlias);
        private readonly RectF bounds;

        public bool Hidden { private get; set; }

        public Color FillColor
        {
            set => paint.Color = value;
        }

        public Wheel(RectF bounds, float strokeWidth, Color fillColor)
        {
            this.bounds = bounds;
            FillColor = fillColor;
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = strokeWidth;
            Hidden = false;
        }


        public void OnDraw(Canvas canvas)
        {
            if (Hidden) return;

            canvas?.DrawArc(bounds, 0f, 360f, false, paint);
        }
    }
}
