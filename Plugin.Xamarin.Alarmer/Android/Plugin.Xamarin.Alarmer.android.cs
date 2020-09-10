using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Java.Lang;
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
        private string notificationId;
        const string channelName = "AlarmChannel";
        readonly string channelId = "default";
        int messageId = -1;

        public event EventHandler NotificationReceived;

        private int _largeIcon;
        private int _smallIcon;

        /// <summary>
        /// set your application icon names which are already defined in Resources folder.
        /// </summary>
        /// <param name="largeIcon"></param>
        /// <param name="smallIcon"></param>
        public void Initialize(int largeIcon, int smallIcon)
        {
            notificationId = Guid.NewGuid().ToString();
            Essential.Preferences.Set("largeIcon", largeIcon);
            Essential.Preferences.Set("smallIcon", smallIcon);
            _largeIcon = largeIcon;
            _smallIcon = smallIcon;
        }

        private Intent CreateAlarmIntent(string title, string message, bool enableSound, bool enableVibrate)
        {
            Intent intent = new Intent(Essential.Platform.CurrentActivity, typeof(AlarmNotificationReceiver));
            intent.SetAction(GetActionName(notificationId));
            intent.PutExtra(Consts.TitleKey, title);
            intent.PutExtra(Consts.MessageKey, message);
            intent.PutExtra(Consts.SoundKey, enableSound);
            intent.PutExtra(Consts.VibrateKey, enableVibrate);

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
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Low)
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

        public string Schedule(string title, string message, DateTime StartTime, Enums.AlarmSequence alarmSequence, bool enableSound, bool enableVibrate, int interval = 1)
        {
            var alarmIntent = CreateAlarmIntent(title, message, enableSound, enableVibrate);
            var pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Application.Context, 1, alarmIntent, PendingIntentFlags.UpdateCurrent);

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

            var alarmManager = AndroidApp.Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            if (alarmSequence == Enums.AlarmSequence.OneTime)
                alarmManager?.Set(AlarmType.RtcWakeup, totalMilliSeconds, pendingIntent);
            else
                alarmManager?.SetRepeating(AlarmType.RtcWakeup, totalMilliSeconds, intervalMilis, pendingIntent);

            return notificationId;
        }

        public int Notify(string title, string message, bool enableSound, bool enableVibrate)
        {
            global::Android.Net.Uri alert = null;
            if (enableSound)
            {
                alert = RingtoneManager.GetDefaultUri(RingtoneType.Alarm);
                if (alert == null)
                    alert = RingtoneManager.GetDefaultUri(RingtoneType.Ringtone);
            }

            messageId++;

            NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Application.Context, channelId)
               .SetContentTitle(title)
               .SetContentText(message);

            addButtons(ref builder);

            _smallIcon = Essential.Preferences.Get("smallIcon", 0);
            _largeIcon = Essential.Preferences.Get("largeIcon", 0);

            Console.WriteLine("small icon : " + _smallIcon);
            Console.WriteLine("large icon : " + _largeIcon);

            Log.WriteLine(LogPriority.Debug, "", "Log small icon : " + _smallIcon);
            Log.WriteLine(LogPriority.Debug, "", "Log large icon : " + _largeIcon);


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

            CreateNotificationChannel(alert, enableVibrate);
            Notification notification = builder.Build();
            manager.Notify(messageId, notification);

            return messageId;
        }

        private void addButtons(ref NotificationCompat.Builder builder)
        {
            Intent yesReceive = new Intent();
            yesReceive.SetAction("YES_ACTION");
            PendingIntent pendingIntentYes = PendingIntent.GetBroadcast(Essential.Platform.CurrentActivity,12345, yesReceive, PendingIntentFlags.UpdateCurrent);
            builder.AddAction(_smallIcon, "Yes", pendingIntentYes);

            //Maybe intent
            Intent maybeReceive = new Intent();
            maybeReceive.SetAction("MAYBE_ACTION");
            PendingIntent pendingIntentMaybe = PendingIntent.GetBroadcast(Essential.Platform.CurrentActivity, 12345, maybeReceive, PendingIntentFlags.UpdateCurrent);
            builder.AddAction(_smallIcon, "Partly", pendingIntentMaybe);

            //No intent
            Intent noReceive = new Intent();
            noReceive.SetAction("NO_ACTION");
            PendingIntent pendingIntentNo = PendingIntent.GetBroadcast(Essential.Platform.CurrentActivity, 12345, noReceive, PendingIntentFlags.UpdateCurrent);
            builder.AddAction(_smallIcon, "No", pendingIntentNo);
        }

        public void ReceiveNotification(string title, string message)
        {
            var args = new LocalNotificationEventArgs()
            {
                Title = title,
                Message = message,
            };
            NotificationReceived?.Invoke(null, args);
        }

    }
}
