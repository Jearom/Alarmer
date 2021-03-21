using System;

namespace Plugin.Xamarin.Alarmer.Shared.Models
{
    public class AlarmModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime StartDate { get; set; }
        public AlarmOptions AlarmOptions { get; set; }
        public NotificationOptions NotificationOptions { get; set; }
    }
}
