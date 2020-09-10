using System;

namespace Plugin.Xamarin.Alarmer.Shared
{
    public partial interface IAlarmer
    {
        string Schedule(string title, string message, DateTime StartTime, Enums.AlarmSequence alarmSequence, bool enableSound, bool enableVibrate, int interval = 1);
        int Notify(string title, string message, bool enableSound, bool enableVibrate);
        void ReceiveNotification(string title, string message);
    }
}
