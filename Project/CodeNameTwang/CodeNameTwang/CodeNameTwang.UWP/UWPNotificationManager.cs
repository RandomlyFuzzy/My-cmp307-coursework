using CodeNameTwang.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace CodeNameTwang.UWP
{
    public class UWPNotificationManager: INotificationManager 
    {

        int messageId = -1;

        public int ScheduleNotification(string title, string message, DateTime scheduledTime)
        {

            messageId++;

            Microsoft.Toolkit.Uwp.Notifications.ToastContent toastContent = new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                    //.AddToastActivationInfo("action=viewConversation&conversationId=5", Microsoft.Toolkit.Uwp.Notifications.ToastActivationType.Foreground)
                    .AddText(title,Microsoft.Toolkit.Uwp.Notifications.AdaptiveTextStyle.Header)
                    .AddText(message)
                    .GetToastContent();

            var toast = new ScheduledToastNotification(toastContent.GetXml(), scheduledTime);
            var toast2 = new ToastNotification(toastContent.GetXml());


            ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast);
            ToastNotificationManager.CreateToastNotifier().Show(toast2);



            return messageId;
        }

    }

}
