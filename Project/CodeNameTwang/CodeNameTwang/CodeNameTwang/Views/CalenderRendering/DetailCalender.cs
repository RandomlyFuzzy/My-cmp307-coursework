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
    public class DetailCalender : IDrawable, ISelectable
    {
        public RoomBooking ThisBooking {get; private set; }

        SKPoint Buttonpos = new SKPoint(0, 0);
        SKPoint btnwh = new SKPoint(0, 0);

        bool inside = false;


        public DetailCalender(RoomBooking booking)
        {
            ThisBooking = booking;
        }

        public void Draw(SKCanvas canvas, SKImageInfo info)
        {
            double LONG = info.Width > info.Height ? info.Width : info.Height;
            double SHORT = info.Width < info.Height ? info.Width : info.Height;

            SKPaint Title = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.White.ToSKColor(),
                StrokeWidth = 2,
                TextSize = 30,
                TextAlign = SKTextAlign.Center
            };

            SKPaint text = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.White.ToSKColor(),
                StrokeWidth = 2,
                //TextSize = 20,
                TextAlign = SKTextAlign.Center

            };

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                Color = Color.LightBlue.ToSKColor(),
                StrokeWidth = 2
            };

            if (inside) {
                paint.Style = SKPaintStyle.Fill;
                if (DependencyService.Get<IUtilities>().MouserPressed()) {
                    ((MasterDetailPage)Application.Current.MainPage).Detail = new NavigationPage(new MenuItemAdvanced { TargetType = typeof(BookingPage), Ctorsetvalue = new object[] { ThisBooking } }.OnLoad());
                }
            }

            ThisBooking.TimeLeft = "" + new TimeSpan(ThisBooking.start.TimeOfDay.Ticks - DateTime.Now.TimeOfDay.Ticks).ToString();

            SKPoint titlePos        = new SKPoint(info.Width * 0.66f, 70);
            SKPoint RoomPos         = new SKPoint(info.Width * 0.66f, 105);
            SKPoint TimeLeftPos     = new SKPoint(info.Width * 0.66f, 140);
            SKPoint DurPos          = new SKPoint(info.Width * 0.66f, 175);
            SKPoint OrgPos          = new SKPoint(info.Width * 0.66f, 210);
            Buttonpos               = new SKPoint(info.Width * 0.46f, info.Height*0.85f);
            btnwh                   = new SKPoint(info.Width * 0.40f, info.Height * 0.1f);
            if (info.Width > info.Height)
            {
                text.TextAlign = SKTextAlign.Left;
                Title.TextAlign = SKTextAlign.Left;
                titlePos        = new SKPoint(info.Width * 0.05f, 40);
                RoomPos         = new SKPoint(info.Width * 0.05f, 70);
                DurPos          = new SKPoint(info.Width * 0.05f, 100);
                OrgPos          = new SKPoint(info.Width * 0.05f, 130);
                TimeLeftPos     = new SKPoint(info.Width * 0.05f, 160);
                Buttonpos       = new SKPoint(info.Width * 0.55f, 130);
                btnwh           = new SKPoint(info.Width * 0.40f, info.Height * 0.1f);
            }

            canvas.DrawText(ThisBooking.title, titlePos.X, titlePos.Y, Title);
            canvas.DrawText(ThisBooking.TimeLeft+" until Event starts" , TimeLeftPos.X, TimeLeftPos.Y, text);
            canvas.DrawText("the event will take place in "+ThisBooking.GetRoom().rname, RoomPos.X, RoomPos.Y, text);
            canvas.DrawText("this event was organised by "+ThisBooking.GetOrganiser().name, OrgPos.X, OrgPos.Y, text);
            canvas.DrawText("Starts at "+ThisBooking.start.ToShortTimeString()+ " last for " + ThisBooking.duration + " minutes", DurPos.X, DurPos.Y, text);

            text.TextAlign = SKTextAlign.Center;
            if (inside)
            {
                text.Color = Color.Red.ToSKColor();
            }
            canvas.DrawText("View Booking", (float)Buttonpos.X + ((float)btnwh.X/2.0f), (float)Buttonpos.Y + ((float)btnwh.Y/2.0f), text);

            canvas.DrawRoundRect(new SKRoundRect(new SKRect((float)Buttonpos.X, (float)Buttonpos.Y, (float)Buttonpos.X + (float)btnwh.X, (float)Buttonpos.Y + (float)btnwh.Y),10,10), paint);

        }

        public bool Insidepos(float x, float y)
        {
            return x > Buttonpos.X && x < Buttonpos.X + btnwh.X && y > Buttonpos.Y && y < Buttonpos.Y + btnwh.Y;
        }

        public bool Inside(float x, float y)
        {
            return inside = Insidepos(x, y);
        }
    }
}
