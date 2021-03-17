using System;
using System.Collections.Generic;
using System.Text;
using UniCore.Mobile.Contracts.Base;

namespace Plugin.Xamarin.Alarmer.Shared.Entities
{
    public class AlarmOptionEntity : IModel
    {

        public int AlarmId { get; set; }
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
