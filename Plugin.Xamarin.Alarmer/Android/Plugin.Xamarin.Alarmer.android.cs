using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Newtonsoft.Json;
using Plugin.Xamarin.Alarmer;
using Plugin.Xamarin.Alarmer.Android.Receivers;
using Plugin.Xamarin.Alarmer.Shared;
using Plugin.Xamarin.Alarmer.Shared.Constants;
using Plugin.Xamarin.Alarmer.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public int AlarmCounter { get; internal set; }

        public event EventHandler<LocalNotificationEventArgs> NotificationReceived;
        public event EventHandler<LocalNotificationEventArgs> NotificationSelectionReceived;

        private Intent CreateAlarmIntent(string title, string message, DateTime startTime, AlarmOptions alarmOptions, string notificationId, NotificationOptions options)
        {

            Intent intent = new Intent(Essential.Platform.CurrentActivity, typeof(AlarmNotificationReceiver));
            intent.SetAction(GetActionName(notificationId));
            intent.PutExtra(Consts.NotificationIdKey, notificationId);
            intent.PutExtra(Consts.TitleKey, title);
            intent.PutExtra(Consts.MessageKey, message);
            intent.PutExtra(Consts.StartDateKey, JsonConvert.SerializeObject(startTime));
            if (alarmOptions != null)
                intent.PutExtra(Consts.AlarmOptionsKey, JsonConvert.SerializeObject(alarmOptions));
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

        private global::Android.Net.Uri GetAlarmSound(NotificationOptions options)
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

        private void AddButtons(ref NotificationCompat.Builder builder, string notificationId, CustomAction[] actions)
        {
            if (actions != null)
                foreach (var item in actions)
                {
                    Intent intent = new Intent(Essential.Platform.CurrentActivity, typeof(NotificationSelectionReceiver));
                    intent.SetAction(item.Name);
                    intent.PutExtra(Consts.NotificationIdKey, notificationId);
                    intent.PutExtra(Consts.AlarmCounterKey, AlarmCounter);
                    PendingIntent pendingIntent = PendingIntent.GetBroadcast(Essential.Platform.CurrentActivity, 12345, intent, PendingIntentFlags.UpdateCurrent);
                    builder.AddAction(GetIconId(item.Icon), item.Name, pendingIntent);
                }
        }

        private DateTime GetNextWeekday(DateTime start, DayOfWeek day, int? interval)
        {
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % (7 * interval != null && interval <= 0 ? 1 : (int)interval);
            return start.AddDays(daysToAdd);
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

            global::Android.Net.Uri alert = GetAlarmSound(options);
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

        public string Schedule(string title, string message, DateTime startTime, AlarmOptions alarmOptions, NotificationOptions options)
        {
            string notificationId = Guid.NewGuid().ToString();

            if (alarmOptions.AlarmSequence != Enums.AlarmSequence.OneTime)
                AlarmCounter++;

            DateTime nextTime = CalculateNextTime(startTime, alarmOptions);

            var alarmIntent = CreateAlarmIntent(title, message, nextTime, alarmOptions, notificationId, options);
            var pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Application.Context, 1, alarmIntent, PendingIntentFlags.UpdateCurrent);

            SetAlarmManager(nextTime, pendingIntent);

            return notificationId;
        }

        private void SetAlarmManager(DateTime alarmTime, PendingIntent pendingIntent)
        {
            var alarmManager = AndroidApp.Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;

            long totalMilliSeconds = new DateTimeOffset(alarmTime).ToUnixTimeSeconds() * 1000;

            alarmManager?.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, totalMilliSeconds, pendingIntent);
        }

        private DateTime CalculateNextTime(DateTime startTime, AlarmOptions alarmOptions)
        {
            DateTime calculatedDate = startTime;
            int selectedInterval = alarmOptions.Interval <= 0 ? 1 : alarmOptions.Interval;

            if (startTime < DateTime.Now)
            {
                switch (alarmOptions.AlarmSequence)
                {
                    case Enums.AlarmSequence.Minute:
                        return startTime.AddMinutes(selectedInterval);
                    case Enums.AlarmSequence.Hourly:
                        return startTime.AddHours(selectedInterval);
                    case Enums.AlarmSequence.Daily:
                        calculatedDate = GetNextTime(startTime, alarmOptions);
                        return calculatedDate > DateTime.Now ? calculatedDate : startTime.AddDays(selectedInterval);
                    case Enums.AlarmSequence.Weekly:
                        calculatedDate = GetNextTime(startTime, alarmOptions);
                        if (calculatedDate > DateTime.Now)
                            return calculatedDate;

                        List<DateTime> days = null;

                        if (alarmOptions?.DayOfWeeks != null)
                        {
                            var dayList = alarmOptions?.DayOfWeeks.Split('|');
                            foreach (var dayitem in dayList)
                            {
                                DayOfWeek val;
                                Enum.TryParse<DayOfWeek>(dayitem, out val);
                                days.Add(GetNextWeekday(startTime, val, selectedInterval));
                            }
                            calculatedDate = days.OrderBy(o => o).FirstOrDefault(w => DateTime.Now > w);
                            if (calculatedDate > DateTime.Now)
                                return calculatedDate;
                        }

                        calculatedDate = GetNextWeekday(startTime, startTime.DayOfWeek, alarmOptions?.Interval);
                        return calculatedDate;
                    case Enums.AlarmSequence.Monthly:
                        return startTime.AddMonths(selectedInterval);
                    case Enums.AlarmSequence.Yearly:
                        return startTime.AddYears(selectedInterval);
                    default:
                        return calculatedDate;
                }

            }

            return startTime;
        }

        private DateTime GetNextTime(DateTime startTime, AlarmOptions alarmOptions)
        {
            if (alarmOptions?.AdditionalTimes != null)
            {
                var nxt = alarmOptions.AdditionalTimes.OrderBy(o => o).First(w => DateTime.Now < new DateTime(startTime.Year, startTime.Month, startTime.Day, w.Hours, w.Minutes, w.Seconds));
                if (nxt != null)
                    return new DateTime(startTime.Year, startTime.Month, startTime.Day, nxt.Hours, nxt.Minutes, nxt.Seconds);
            }

            return startTime;
        }


        public void ReceiveSelectedNotification(string title, string message, string notificationId, string selectedAction)
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
