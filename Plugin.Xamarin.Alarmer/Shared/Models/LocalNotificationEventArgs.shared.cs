using System;

namespace Plugin.Xamarin.Alarmer.Shared.Models
{
    public class LocalNotificationEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public int Badge { get; set; }
        public string ScreenKey { get; set; }
    }
}
