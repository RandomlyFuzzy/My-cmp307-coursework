using CodeNameTwang.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace CodeNameTwang.Services
{
    public class UtilsPage : ContentPage
    {

        public static async Task<bool> Alert(string msg, string title, string cncl, string acpt)
        {
            return await Application.Current.MainPage.DisplayAlert(title, msg, acpt, cncl);
        }

        public static async Task Alert(string msg, string title = "ALERT")
        {
            await Application.Current.MainPage.DisplayAlert(title, msg, "ok");
            return;
        }

        public static async Task Alert(string msg, Action deleg, string title = "ALERT"
        )
        {
            await Application.Current.MainPage.DisplayAlert(title, msg, "ok");
            deleg.Invoke();
            return;
        }

        public static async Task<string> AlertTextBox(string msg, string Title = "ALERT", string placeholder = "")
        {
            string s = "";
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                s = await Application.Current.MainPage.DisplayPromptAsync(Title, msg, "OK", "Cancel", placeholder);
            });

            return s;
        }


        public static async Task Notifiation(string title, string msg) => Notifiation(title, msg, DateTime.Now.AddSeconds(5));//cannot add compile time dynamic constants as overload defaults
        public static async Task Notifiation(string title,string msg , DateTime activate ) {
            DependencyService.Get<INotificationManager>().ScheduleNotification(title, msg, activate);
        }

        public static async Task<string> ActionSheet(string title,string[] items,string cancel = "Cancel",string destr=null) {
            return await Application.Current.MainPage.DisplayActionSheet(title, cancel,destr,items);
        }


        public static async Task<bool> SendEmail(string toEmail,string header,string body) {

            try
            {

                await Email.ComposeAsync(header, body, toEmail);
                return true;
            }
            catch {
                return false;
            }
        }


    }
}