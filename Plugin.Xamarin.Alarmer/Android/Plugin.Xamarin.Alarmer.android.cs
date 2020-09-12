using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Java.Lang;
using Newtonsoft.Json;
using Plugin.Xamarin.Alarmer;
using Plugin.Xamarin.Alarmer.Android.Receivers;
using Plugin.Xamarin.Alarmer.Shared;
using Plugin.Xamarin.Alarmer.Shared.Constants;
using Plugin.Xamarin.Alarmer.Shared.Models;
using System;
using Xamarin.Forms;
using AndroidApp = Android.App;
using Essential = Xamarin.Essentials;

[assembly: Dependency(typeof(AlarmerImplementation))]
namespace Plugin.Xamarin.Alarmer
{
    /// <summary>
    /// Interface for Plugin.Xamarin.Alarmer
    /// </summary>
    /// 
    public class AlarmerImplementation : IAlarmer
    {
        NotificationManager manager;
        const string channelName = "AlarmChannel";
        readonly string channelId = "default";
        int messageId = -1;

        public event EventHandler<LocalNotificationEventArgs> NotificationReceived;
        public event EventHandler<LocalNotificationEventArgs> NotificationSelectionReceived;

        private Intent CreateAlarmIntent(string title, string message, string notificationId, NotificationOptions options)
        {
            Intent intent = new Intent(Essential.Platform.CurrentActivity, typeof(AlarmNotificationReceiver));
            intent.SetAction(GetActionName(notificationId));
            intent.PutExtra(Consts.NotificationIdKey, notificationId);
            intent.PutExtra(Consts.TitleKey, title);
            intent.PutExtra(Consts.MessageKey, message);
            if (options != null)
                intent.PutExtra(Consts.OptionsKey, JsonConvert.SerializeObject(options));

            return intent;
        }

        private string GetActionName(string channelId)
        {
            return $"{Essential.Platform.AppContext.PackageName}.notify.{channelId}";
        }

        private void CreateNotificationChannel(global::Android.Net.Uri alertUri, bool enableVirate)
        {
            manager = (NotificationManager)AndroidApp.Application.Context.GetSystemService(AndroidApp.Application.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.High)
                {
                    Description = "Alarm Channel"
                };

                if (alertUri != null)
                    channel.SetSound(alertUri, new AudioAttributes.Builder()
                        .SetContentType(AudioContentType.Sonification)
                        .SetUsage(AudioUsageKind.Alarm)
                        .Build()
                        );
                channel.EnableVibration(enableVirate);
                manager.CreateNotificationChannel(channel);
            }
        }

        private global::Android.Net.Uri GetAlarm(NotificationOptions options)
        {
            global::Android.Net.Uri alert = null;
            if (options != null && options.EnableSound)
            {
                alert = RingtoneManager.GetDefaultUri(RingtoneType.Alarm);
                if (alert == null)
                    alert = RingtoneManager.GetDefaultUri(RingtoneType.Ringtone);
            }
            return alert;
        }

        private int GetIconId(string name)
        {
            return Essential.Platform.CurrentActivity.Resources.GetIdentifier(name, "drawable", Essential.AppInfo.PackageName);
        }

        private void AddButtons(ref NotificationCompat.Builder builder,string notificationId, CustomAction[] actions)
        {
            if (actions != null)
                foreach (var item in actions)
                {
                    Intent intent = new Intent(Essential.Platform.CurrentActivity, typeof(NotificationSelectionReceiver));
                    intent.SetAction(item.Name);
                    intent.PutExtra(Consts.NotificationIdKey, notificationId);
                    PendingIntent pendingIntent = PendingIntent.GetBroadcast(Essential.Platform.CurrentActivity, 12345, intent, PendingIntentFlags.UpdateCurrent);
                    builder.AddAction(GetIconId(item.Icon), item.Name, pendingIntent);
                }
        }

        public string Notify(string title, string message, string notificationId = null, NotificationOptions options = null)
        {
            messageId++;

            if (string.IsNullOrEmpty(notificationId))
                notificationId = Guid.NewGuid().ToString();

            NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Application.Context, channelId)
               .SetContentTitle(title)
               .SetContentText(message);
            builder.SetCategory(Notification.CategoryAlarm);

            Intent mainIntent = new Intent(AndroidApp.Application.Context, typeof(AndroidApp.Activity));
            PendingIntent pendingMainIntent = PendingIntent.GetBroadcast(Essential.Platform.CurrentActivity, 12345, mainIntent, PendingIntentFlags.OneShot);
            builder.SetContentIntent(pendingMainIntent);

            global::Android.Net.Uri alert = GetAlarm(options);
            AddButtons(ref builder, notificationId, options?.CustomActions);

            int _smallIcon = string.IsNullOrEmpty(options?.SmallIcon) ? Essential.Platform.AppContext.ApplicationInfo.Icon : GetIconId(options.SmallIcon);
            int _largeIcon = string.IsNullOrEmpty(options?.LargeIcon) ? 0 : GetIconId(options.LargeIcon);

            Console.WriteLine("small icon : " + _smallIcon);
            Console.WriteLine("large icon : " + _largeIcon);

            if (_smallIcon != 0)
                builder.SetSmallIcon(_smallIcon);
            if (_largeIcon != 0)
                builder.SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Application.Context.Resources, _largeIcon));
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                builder.SetSound(alert)
                    .SetVibrate(new long[] { 1000, 1000, 1000 });
#pragma warning restore CS0618 // Type or member is obsolete
            }

            CreateNotificationChannel(alert, options != null && options.EnableVibration);
            Notification notification = builder.Build();
            manager.Notify(messageId, notification);

            var args = new LocalNotificationEventArgs()
            {
                Id = notificationId,
                Title = title,
                Message = message,

            };

            NotificationReceived?.Invoke(null, args);

            return notificationId;
        }

        public string Schedule(string title, string message, DateTime StartTime, Enums.AlarmSequence alarmSequence, int interval, NotificationOptions options)
        {
            string notificationId = Guid.NewGuid().ToString();

            var alarmIntent = CreateAlarmIntent(title, message, notificationId, options);
            var pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Application.Context, 1, alarmIntent, PendingIntentFlags.UpdateCurrent);

            DateTime _jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            long repeatForMinute = 60000; // In milliseconds  
            long repeatForHour = AlarmManager.IntervalHour; // In milliseconds  
            long repeatForDay = AlarmManager.IntervalDay; // In milliseconds  
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

            var alarmManager = AndroidApp.Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            if (alarmSequence == Enums.AlarmSequence.OneTime)
                alarmManager?.Set(AlarmType.RtcWakeup, totalMilliSeconds, pendingIntent);
            else
                alarmManager?.SetRepeating(AlarmType.RtcWakeup, totalMilliSeconds, intervalMilis, pendingIntent);

            return notificationId;
        }

        public void ReceiveSelectedNotification(string title, string message,string notificationId, string selectedAction)
        {
            var args = new LocalNotificationEventArgs()
            {
                Id = notificationId,
                Title = title,
                Message = message,
                SelectedAction = selectedAction
            };

            NotificationSelectionReceived?.Invoke(null, args);

        }

        public DateTime GetNextAlarm()
        {
            var alarmManager = AndroidApp.Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            var triggerTime = alarmManager.NextAlarmClock?.TriggerTime;
            if (triggerTime != null)
                return new DateTime((long)triggerTime);
            else
                return new DateTime();
        }
    }
}
