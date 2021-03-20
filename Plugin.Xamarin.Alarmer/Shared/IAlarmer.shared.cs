using Plugin.Xamarin.Alarmer.Shared.Models;
using System;
using System.Threading.Tasks;

namespace Plugin.Xamarin.Alarmer.Shared
{
    public partial interface IAlarmer
    {
        public event EventHandler<LocalNotificationEventArgs> NotificationReceived;
        public event EventHandler<LocalNotificationEventArgs> NotificationSelectionReceived;
        int AlarmCounter { get; }
        int Notify(string title, string message, int notificationId, NotificationOptions options = null);
        Task<int> Schedule(int? id, string title, string message, DateTime startTime, AlarmOptions alarmOptions, NotificationOptions options);
        void CancelAll();
        void Cancel(int notificationId);
        DateTime GetNextAlarm();
        void ReceiveSelectedNotification(string title, string message, int notificationId, string selectedAction);
    }
}
