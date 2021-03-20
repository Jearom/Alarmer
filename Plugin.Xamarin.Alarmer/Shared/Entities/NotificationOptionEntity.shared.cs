using SQLite;
using UniCore.Mobile.Contracts.Base;

namespace Plugin.Xamarin.Alarmer.Shared.Entities
{
    public class NotificationOptionEntity : IModel
    {
        [PrimaryKey]
        public int AlarmId { get; set; }
        public bool EnableSound { get; set; }
        public bool EnableVibration { get; set; }
        public string SmallIcon { get; set; }
        public string LargeIcon { get; set; }
    }
}
