using CodeNameTwang.Services.Interfaces;
using CodeNameTwang.ViewModels.DataStructures;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CodeNameTwang.Views.CalenderRendering
{
    public class CalenderBooking : ISelectable, IDrawable
    {

        public RoomBooking ThisBooking { get; private set; }
        double xScale,yScale,xpos,ypos,h,w;

        public CalenderBooking(RoomBooking booking) {
            ThisBooking = booking;
        }

        public bool Inside(float x, float y){
            return x>xpos&&x<xpos+w&&y>ypos&&y<ypos+h;
        }
        public void Draw(SKCanvas canvas, SKImageInfo info)
        {
            double LONG = info.Width > info.Height ? info.Width : info.Height;
            double SHORT = info.Width < info.Height ? info.Width : info.Height;

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                Color = Color.LightBlue.ToSKColor(),
                StrokeWidth = 2
            };

            if (Application.Current.RequestedTheme==OSAppTheme.Dark) {
                paint.Color = Color.DarkBlue.ToSKColor();
            }

            var pos = DependencyService.Get<IUtilities>().GetMousePosition();
            if (Inside((float)pos.Item1,(float) pos.Item2)) {
                paint.Style = SKPaintStyle.Fill;
            }


            //landscape
            if (info.Width > info.Height)
            {
                xScale = (LONG / (AvailabilityPage.max - AvailabilityPage.min));
                yScale = (SHORT);
                xpos = ((ThisBooking.start.TimeOfDay.TotalMinutes / 60.0f) - AvailabilityPage.min) * xScale;
                h = yScale * 0.25f;
                w = ((ThisBooking.duration / 60.0f) + (ThisBooking.duration == 60 ? 1 : 0)) * xScale;
                ypos = (info.Height - h);
                canvas.DrawRoundRect(new SKRoundRect(new SKRect((float)xpos, (float)ypos, (float)xpos +(float)w, (float)ypos+(float)h),10,10), paint);
            }
            //portrait
            else {
                xScale = (LONG / (AvailabilityPage.max - AvailabilityPage.min));
                yScale = (SHORT);
                ypos = ((ThisBooking.start.TimeOfDay.TotalMinutes / 60.0f) - AvailabilityPage.min) * xScale;
                xpos = yScale * 0;
                w = yScale * 0.25f;
                h = ((ThisBooking.duration / 60.0f) + (ThisBooking.duration == 60 ? 1 : 0)) * xScale;
                canvas.DrawRoundRect(new SKRoundRect(new SKRect((float)xpos, (float)ypos, (float)xpos +(float)w, (float)ypos+(float)h),10,10), paint);
            }
        }
    }
}
