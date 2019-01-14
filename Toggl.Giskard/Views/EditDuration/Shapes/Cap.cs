using System;
using Android.Graphics;

namespace Toggl.Giskard.Views.EditDuration.Shapes
{
    public sealed class Cap
    {
        private readonly Paint capPaint = new Paint(PaintFlags.AntiAlias);
        private readonly Paint capBorderPaint = new Paint(PaintFlags.AntiAlias);
        private readonly Paint iconPaint = new Paint(PaintFlags.AntiAlias);
        private readonly Paint arcPaint = new Paint(PaintFlags.AntiAlias);
        private readonly float radius;
        private readonly float capInnerSquareSide;
        private readonly float capBorderStrokeWidth;
        private readonly float arcRadius;
        private readonly float shadowWidth;
        private readonly Bitmap iconBitmap;
        private readonly Bitmap shadowBitmap;

        private readonly Paint shadowPaint = new Paint(0)
        {
            Color = Color.ParseColor("#66000000")
        };


        public Color ForegroundColor
        {
            set => arcPaint.Color = value;
        }

        public PointF Position { get; set; }
        public bool ShowOnlyBackground { get; set; }

        public Cap(float radius,
            float arcWidth,
            Color capColor,
            Color capBorderColor,
            Color foregroundColor,
            float capBorderStrokeWidth,
            Bitmap icon,
            Color iconColor,
            float shadowWidth)
        {
            this.capBorderStrokeWidth = capBorderStrokeWidth;
            this.shadowWidth = shadowWidth;
            this.radius  = radius;
            capInnerSquareSide = (float) Math.Sqrt((radius - shadowWidth) * (radius - shadowWidth) * 2) * 0.5f;
            arcRadius = arcWidth / 2f;
            arcPaint.Color = foregroundColor;
            capPaint.Color = capColor;
            capBorderPaint.SetStyle(Paint.Style.Stroke);
            capBorderPaint.StrokeWidth = capBorderStrokeWidth;
            capBorderPaint.Color = capBorderColor;
            iconBitmap = icon;
            iconPaint.SetColorFilter(new PorterDuffColorFilter(iconColor, PorterDuff.Mode.SrcIn));
            shadowPaint.SetMaskFilter(new BlurMaskFilter(shadowWidth, BlurMaskFilter.Blur.Normal));
            shadowPaint.SetStyle(Paint.Style.Fill);
            shadowBitmap = Bitmap.CreateBitmap((int) (radius * 2f), (int) (radius * 2f), Bitmap.Config.Argb8888);
            var shadowCanvas = new Canvas(shadowBitmap);
            shadowCanvas.DrawCircle(radius, radius, radius - shadowWidth, shadowPaint);
        }

        public void OnDraw(Canvas canvas)
        {
            if (ShowOnlyBackground)
            {
                canvas?.DrawCircle(Position.X, Position.Y, arcRadius, arcPaint);
                return;
            }

            var innerSquareLeft = Position.X - capInnerSquareSide;
            var innerSquareTop = Position.Y - capInnerSquareSide;
            canvas?.DrawBitmap(shadowBitmap, Position.X - radius, Position.Y - radius, shadowPaint);
            canvas?.DrawCircle(Position.X, Position.Y, radius - shadowWidth - capBorderStrokeWidth / 4f, capPaint);
            canvas?.DrawCircle(Position.X, Position.Y, radius - shadowWidth, capBorderPaint);
            canvas?.DrawBitmap(iconBitmap,
                innerSquareLeft + (capInnerSquareSide * 2f - iconBitmap.Width) / 2f,
                innerSquareTop + (capInnerSquareSide * 2f - iconBitmap.Height) / 2f, iconPaint);
        }
    }
}
