using Plugin.Xamarin.Alarmer;
using Plugin.Xamarin.Alarmer.Shared;
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
        public void Initialize(string largeIcon, string smallIcon)
        {
            throw new NotImplementedException();
        }

        public int Notify(string title, string message, bool enableSound, bool enableVibrate)
        {
            throw new NotImplementedException();
        }

        public void ReceiveNotification(string title, string message)
        {
            throw new NotImplementedException();
        }

        public string Schedule(string title, string message, DateTime StartTime, Enums.AlarmSequence alarmSequence, bool enableSound, bool enableVibrate, int interval = 1)
        {
            throw new NotImplementedException();
        }
    }
}
