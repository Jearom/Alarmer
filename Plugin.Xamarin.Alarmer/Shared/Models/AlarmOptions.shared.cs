using System;
using System.Collections.Generic;

namespace Plugin.Xamarin.Alarmer.Shared.Models
{
    public class AlarmOptions
    {
        public Enums.AlarmSequence AlarmSequence { get; set; } = Enums.AlarmSequence.OneTime;
        public DateTime? EndDate { get; set; }
        public int? TotalAlarmCount { get; set; }
        public int Interval { get; set; }
        public Enums.DaysOfWeek DaysOfWeek { get; set; }
        public IList<TimeSpan> AdditionalTimes { get; set; }

    }
}
