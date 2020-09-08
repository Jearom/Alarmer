using Android.App;
using Android.Content;
using Android.OS;
using Java.Lang;
using Plugin.Plugin.Xamarin.Alarmer.Android.Receivers;
using Plugin.Plugin.Xamarin.Alarmer.Shared;
using Plugin.Plugin.Xamarin.Alarmer.Shared.Models;
using System;

namespace Plugin.Plugin.Xamarin.Alarmer.Android
{
    /// <summary>
    /// Interface for Plugin.Xamarin.Alarmer
    /// </summary>
    public class AlarmerImplementation : IAlarmer
    {
        public const string TitleKey = "title";
        public const string MessageKey = "message";

        NotificationManager manager;
        private bool isChannelInitialized;
        const string channelName = "AlarmChannel";
        readonly string channelId;

        public event EventHandler NotificationReceived;

        public AlarmerImplementation()
        {
            channelId = Guid.NewGuid().ToString();
            CreateNotificationChannel();
        }


        private string GetActionName(string channelId)
        {
            return $"{Application.Context.PackageName}.notify.{channelId}";
        }


        private void Schedule(string title, string message, Enums.AlarmSequence alarmSequence, DateTime StartTime, int interval, int badge, AlarmType alarmType = AlarmType.RtcWakeup)
        {
            var alarmIntent = CreateAlarmIntent(title, message);
            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 1, alarmIntent, PendingIntentFlags.UpdateCurrent);

            DateTime _jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            long repeatForMinute = 60000; // In milliseconds  
            long repeatForHour = 3600000; // In milliseconds  
            long repeatForDay = 86400000; // In milliseconds  
            long repeatForWeek = 604800000; // In milliseconds  
            

            long totalMilliSeconds = (long)(StartTime.ToUniversalTime() - _jan1st1970).TotalMilliseconds;
            long intervalMilis = 0;
            if (totalMilliSeconds < JavaSystem.CurrentTimeMillis())
            {
                switch (alarmSequence)
                {
                    case Enums.AlarmSequence.Minute:
                        totalMilliSeconds = totalMilliSeconds + repeatForMinute;
                        intervalMilis = repeatForMinute;
                        break;
                    case Enums.AlarmSequence.Hourly:
                        totalMilliSeconds = totalMilliSeconds + repeatForHour;
                        intervalMilis = repeatForHour;
                        break;
                    case Enums.AlarmSequence.Daily:
                        totalMilliSeconds = totalMilliSeconds + repeatForDay;
                        intervalMilis = repeatForDay;
                        break;
                    case Enums.AlarmSequence.Weekly:
                        totalMilliSeconds = totalMilliSeconds + repeatForWeek;
                        intervalMilis = repeatForWeek;
                        break;
                    case Enums.AlarmSequence.Monthly:
                        intervalMilis = repeatForWeek;
                        break;
                    case Enums.AlarmSequence.Yearly:
                        break;

                    default:
                        totalMilliSeconds = totalMilliSeconds + repeatForDay;
                        break;
                }

            }

            var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            if (alarmSequence == Enums.AlarmSequence.OneTime)
                alarmManager?.Set(alarmType, totalMilliSeconds, pendingIntent);
            else
                alarmManager?.SetRepeating(alarmType, totalMilliSeconds, intervalMilis, pendingIntent);
        }




        private Intent CreateAlarmIntent(string title, string message)
        {
            Intent intent = new Intent(Application.Context, typeof(AlarmNotificationReceiver));
            intent.SetAction(GetActionName(channelId));
            intent.PutExtra(TitleKey, title);
            intent.PutExtra(MessageKey, message);

            return intent;
        }

        private void CreateNotificationChannel()
        {
            manager = (NotificationManager)Application.Context.GetSystemService(Application.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Low)
                {
                    Description = "Alarm Channel"
                };
                manager.CreateNotificationChannel(channel);
            }

            isChannelInitialized = true;
        }


        public void ReceiveNotification(string title, string message, int badge = 0)
        {
            var args = new LocalNotificationEventArgs()
            {
                Title = title,
                Message = message,
                Badge = badge
            };
            NotificationReceived?.Invoke(null, args);
        }

    }
}
