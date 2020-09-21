using System;

namespace Plugin.Xamarin.Alarmer.Shared
{
    public class Enums
    {

        public enum AlarmSequence : int
        {
            OneTime = 0,
            Minute = 1,
            Hourly = 2,
            Daily = 3,
            Weekly = 4,
            Monthly = 5,
            Yearly = 6
        }


        [Flags]
        public enum DaysOfWeek
        {
            Sunday = 1,
            Monday = 2,
            Tuesday = 4,
            Wednesday = 8,
            Thursday = 16,
            Friday = 32,
            Saturday = 64,

            None = 0,
            All = Weekdays | Weekend,
            Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
            Weekend = Sunday | Saturday,
        }

    }
}
