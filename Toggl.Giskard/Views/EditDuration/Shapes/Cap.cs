using Android.Graphics;

namespace Toggl.Giskard.Views.EditDuration.Shapes
{
    public sealed class Cap
    {
        private Paint paint = new Paint();

        public Color FillColor
        {
            get => paint.Color;
            set => paint.Color = value;
        }

        public float Radius { get; set; }

        public PointF Position { get; set; }
        public bool ShowOnlyBackground { get; set; }

        public Cap(float radius, Color color)
        {
            Radius = radius;
            FillColor = color;
        }

        public void OnDraw(Canvas canvas)
        {
            canvas?.DrawCircle(Position.X, Position.Y, Radius, paint);
            if (!ShowOnlyBackground)
            {
                //draw bitmap
            }
        }
    }
}
