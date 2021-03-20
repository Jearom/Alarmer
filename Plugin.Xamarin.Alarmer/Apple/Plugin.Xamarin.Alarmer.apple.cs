using Plugin.Xamarin.Alarmer;
using Plugin.Xamarin.Alarmer.Shared;
using Plugin.Xamarin.Alarmer.Shared.Models;
using System;
using System.Threading.Tasks;
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

        public void Cancel(int notificationId)
        {
            throw new NotImplementedException();
        }

        public void CancelAll()
        {
            throw new NotImplementedException();
        }

        public DateTime GetNextAlarm()
        {
            throw new NotImplementedException();
        }

        public int Notify(string title, string message, int notificationId, NotificationOptions options = null)
        {
            throw new NotImplementedException();
        }

        public void ReceiveSelectedNotification(string title, string message, int notificationId, string selectedAction)
        {
            throw new NotImplementedException();
        }


        public Task<int> Schedule(int? id, string title, string message, DateTime startTime, AlarmOptions alarmOptions, NotificationOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
