using Plugin.Xamarin.Alarmer.Shared.Models;
using SQLite;
using System;
using UniCore.Mobile.Contracts.Base;

namespace Plugin.Xamarin.Alarmer.Shared.Entities
{
    public class AlarmEntity : IModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime StartDate { get; set; }

        public Enums.AlarmSequence AlarmSequence { get; set; } = Enums.AlarmSequence.OneTime;
        public DateTime? EndDate { get; set; }
        public int? TotalAlarmCount { get; set; }
        public int Interval { get; set; }
        public Enums.DaysOfWeek DaysOfWeek { get; set; }

        public bool EnableSound { get; set; }
        public bool EnableVibration { get; set; }
        public string SmallIcon { get; set; }
        public string LargeIcon { get; set; }
    }
}
