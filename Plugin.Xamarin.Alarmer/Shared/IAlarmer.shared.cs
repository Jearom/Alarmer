using Plugin.Xamarin.Alarmer.Shared.Models;
using System;

namespace Plugin.Xamarin.Alarmer.Shared
{
    public partial interface IAlarmer
    {
        public event EventHandler<LocalNotificationEventArgs> NotificationReceived;
        public event EventHandler<LocalNotificationEventArgs> NotificationSelectionReceived;
        int AlarmCounter { get;  }
        string Notify(string title, string message, string notificationId = null, NotificationOptions options = null);
        string Schedule(string title, string message, DateTime startTime, AlarmOptions alarmOptions, NotificationOptions options);
        DateTime GetNextAlarm();
        void ReceiveSelectedNotification(string title, string message, string notificationId, string selectedAction);
    }
}
