using Plugin.Xamarin.Alarmer;
using Plugin.Xamarin.Alarmer.Shared;
using Plugin.Xamarin.Alarmer.Shared.Models;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(AlarmerImplementation))]
namespace Plugin.Xamarin.Alarmer
{
    /// <summary>
    /// Interface for Plugin.Xamarin.Alarmer
    /// </summary>
    public class AlarmerImplementation : IAlarmer
    {
        public int AlarmCounter => throw new NotImplementedException();

        public event EventHandler<LocalNotificationEventArgs> NotificationReceived;
        public event EventHandler<LocalNotificationEventArgs> NotificationSelectionReceived;

        public DateTime GetNextAlarm()
        {
            throw new NotImplementedException();
        }

        public string Notify(string title, string message, string notificationId = null, NotificationOptions options = null)
        {
            throw new NotImplementedException();
        }

        public void ReceiveSelectedNotification(string title, string message, string notificationId, string selectedAction)
        {
            throw new NotImplementedException();
        }

        public string Schedule(string title, string message, DateTime StartTime, Enums.AlarmSequence alarmSequence, int interval, NotificationOptions options)
        {
            throw new NotImplementedException();
        }

        public string Schedule(string title, string message, DateTime startTime, AlarmOptions alarmOptions, NotificationOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
