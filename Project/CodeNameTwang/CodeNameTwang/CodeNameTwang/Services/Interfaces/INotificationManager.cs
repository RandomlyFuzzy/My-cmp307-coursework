using System;
using System.Collections.Generic;
using System.Text;

namespace CodeNameTwang.Services.Interfaces
{
    public interface INotificationManager
    {
        int ScheduleNotification(string title, string message, DateTime scheduledTime);
    }
}
