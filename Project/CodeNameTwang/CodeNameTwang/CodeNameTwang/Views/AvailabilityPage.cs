using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeNameTwang.Model;
using CodeNameTwang.Services.Interfaces;
using CodeNameTwang.ViewModels.DataStructures;
using CodeNameTwang.Views.CalenderRendering;
using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.Forms;

namespace CodeNameTwang.Views
{
    public class AvailabilityPage : ContentPage
    {
        public static float min = 25;
        public static float max = 0;

        IUtilities utils = DependencyService.Get<IUtilities>();
        InfoDataStore store = DependencyService.Get<InfoDataStore>();
        List<CalenderBooking> drawables = new List<CalenderBooking>();
        DetailCalender dc = null;
        SKPoint MousePoint = new SKPoint(0, 0);

        public AvailabilityPage(string title,List<RoomBooking> item)
        {
            Title = title;
            max = 0;
            min = 24;
            init(item);
        }

        private async void init(List<RoomBooking> items)
        {
            foreach (var rb in items)
            {
                if (rb.start.DayOfYear == DateTime.Now.DayOfYear) {
                    drawables.Add(new CalenderBooking(rb));
                    if (min > rb.start.TimeOfDay.Hours+ (rb.start.TimeOfDay.Minutes/60.0f)) {
                        min = rb.start.TimeOfDay.Hours + (rb.start.TimeOfDay.Minutes / 60.0f);
                    }
                    if (max < rb.start.TimeOfDay.Hours + ((rb.start.TimeOfDay.Minutes+rb.duration) / 60.0f)){
                        max = rb.start.TimeOfDay.Hours + ((rb.start.TimeOfDay.Minutes + rb.duration) / 60.0f);
                    }
                }
            }


            SKCanvasView canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            canvasView.Touch += CanvasTouched;
            Content = canvasView;

            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 32), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    try{
                        var pos = utils.GetMousePosition();
                        MousePoint = new SKPoint((float)pos.Item1, (float)pos.Item2);
                    }catch{}
                    canvasView.InvalidateSurface();
                });
                return true;
            });
        }

        private void CanvasTouched(object sender, SKTouchEventArgs e)
        {
            MousePoint = e.Location;
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.LightBlue.ToSKColor(),
                StrokeWidth = 2
            };
            SKPaint Text = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Cyan.ToSKColor(),
                StrokeWidth = 1,
                TextSize = 20
            };

            if (Application.Current.RequestedTheme == OSAppTheme.Dark)
            {
                paint.Color = Color.DarkBlue.ToSKColor();
                Text.Color = Color.DarkCyan.ToSKColor();
                Text.StrokeWidth = 2;
            }

            //canvas.DrawText(""+ MousePoint.ToFormsPoint().ToString(),0,20, Text);
            //canvas.DrawCircle(MousePoint, 5, paint);

            bool found = false;

            foreach (var item in drawables)
            {
                item.Draw(canvas, info);
                if (found) continue;

                if (item.Inside(MousePoint.X,MousePoint.Y)&&utils.MouserPressed()) {
                    found = true;
                    dc = new DetailCalender(item.ThisBooking);
                }
            }
            if (dc != null) {
                dc.Inside(MousePoint.X, MousePoint.Y);
                dc.Draw(canvas, info);
            }

            string minText = "" + (int)min + ((min % 1.0f != 0) ? ":" + (min % 1.0f * 60).ToString("##") : "") + (min < 12 ? " AM" : " PM");
            string maxText = "" + (int)(max%24) + ((max % 1.0f != 0) ? ":" + (max % 1.0f * 60).ToString("##") : "") + (max < 12 ? " AM" : max < 24 ? " PM" : " AM");
            SKPoint left = new SKPoint(0, 0), right = new SKPoint(0, 0);
            if (info.Width > info.Height)
            {
                left = new SKPoint(5, info.Height * 0.75f - 10);
                right = new SKPoint(info.Width-Text.MeasureText(maxText)-10, info.Height * 0.75f-10);
            }
            else {
                left = new SKPoint(info.Width * 0.3f, 22);
                right = new SKPoint(info.Width * 0.3f, info.Height-4);
            }


           
            canvas.DrawText(""+ minText, left, Text);
            canvas.DrawText(""+ maxText, right, Text);

        }
    }
}