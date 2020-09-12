using System;

namespace Plugin.Xamarin.Alarmer.Shared.Models
{
    public class LocalNotificationEventArgs : EventArgs
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string SelectedAction { get; set; }
    }
}
