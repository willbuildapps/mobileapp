using Android.Graphics;
using Toggl.Giskard.Extensions;
using static Toggl.Multivac.Math;

namespace Toggl.Giskard.Views.EditDuration.Shapes
{
    public class ClockDial
    {
        private const int minuteSegmentsPerHourMark = MinutesInAnHour / HoursOnTheClock;
        private const float angleOffsetCorrection = (float) FullCircle / 4f;
        private readonly PointF dialCenter;
        private readonly float textRadius;
        private Rect textBounds = new Rect();

        private readonly Paint paint = new Paint(PaintFlags.AntiAlias)
        {
            TextAlign = Paint.Align.Left
        };

        public ClockDial(PointF dialCenter, float textSize, Color textColor, float textRadius)
        {
            this.dialCenter = dialCenter;
            this.textRadius = textRadius;
            paint.TextSize = textSize;
            paint.Color = textColor;
            paint.SetTypeface(Typeface.Create("sans-serif", TypefaceStyle.Normal));
        }

        public void OnDraw(Canvas canvas)
        {
            for (var minute = 1; minute <= MinutesInAnHour; ++minute)
            {
                var angle = (float) FullCircle * (minute / (float) MinutesInAnHour) - angleOffsetCorrection;
                var correspondsToHourMark = minute % minuteSegmentsPerHourMark == 0;
                if (correspondsToHourMark)
                {
                    drawMinuteNumber(canvas, minute, angle);
                }
            }
        }

        private void drawMinuteNumber(Canvas canvas, int number, float angle)
        {
            var minuteText = number.ToString();
            var textCenter = PointOnCircumference(dialCenter.ToPoint(), angle, textRadius).ToPointF();
            paint.GetTextBounds(minuteText, 0, minuteText.Length, textBounds);
            var centeredTextX = textCenter.X - textBounds.Width() / 2f;
            var centeredTextY = textCenter.Y + textBounds.Height() / 2f;
            canvas.DrawText(minuteText, centeredTextX, centeredTextY, paint);
        }
    }
}
