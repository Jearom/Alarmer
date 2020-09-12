namespace Plugin.Xamarin.Alarmer.Shared.Models
{
    public class NotificationOptions
    {
        public bool EnableSound { get; set; }
        public bool EnableVibration { get; set; }
        public string SmallIcon { get; set; }
        public string LargeIcon { get; set; }
        public CustomAction[] CustomActions { get; set; } 

    }
}
