using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Newtonsoft.Json;
using Plugin.Xamarin.Alarmer;
using Plugin.Xamarin.Alarmer.Android.Receivers;
using Plugin.Xamarin.Alarmer.Shared;
using Plugin.Xamarin.Alarmer.Shared.Constants;
using Plugin.Xamarin.Alarmer.Shared.Entities;
using Plugin.Xamarin.Alarmer.Shared.Extensions;
using Plugin.Xamarin.Alarmer.Shared.Models;
using Plugin.Xamarin.Alarmer.Shared.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        AlarmRepository _alarmRepo;
        AlarmOptionRepository _optionRepository;
        TimingRepository _timingRepository;
        CustomActionRepository _customActionRepository;


        public AlarmerImplementation()
        {
            _alarmRepo = new AlarmRepository();
            _optionRepository = new AlarmOptionRepository();
            _timingRepository = new TimingRepository();
        }

        public int AlarmCounter { get; internal set; }

        public event EventHandler<LocalNotificationEventArgs> NotificationReceived;
        public event EventHandler<LocalNotificationEventArgs> NotificationSelectionReceived;

        private Intent CreateAlarmIntent(string title, string message, DateTime startTime, AlarmOptions alarmOptions, int notificationId, NotificationOptions options)
        {

            Intent intent = new Intent(AndroidApp.Application.Context, typeof(AlarmNotificationReceiver));
            intent.SetAction(GetActionName(notificationId));
            intent.PutExtra(Consts.NotificationIdKey, notificationId);
            intent.PutExtra(Consts.TitleKey, title);
            intent.PutExtra(Consts.MessageKey, message);
            intent.PutExtra(Consts.AlarmCounterKey, AlarmCounter);
            intent.PutExtra(Consts.StartDateKey, JsonConvert.SerializeObject(startTime));
            if (alarmOptions != null)
                intent.PutExtra(Consts.AlarmOptionsKey, JsonConvert.SerializeObject(alarmOptions));
            if (options != null)
                intent.PutExtra(Consts.OptionsKey, JsonConvert.SerializeObject(options));

            return intent;
        }

        private string GetActionName(int notificationId)
        {
            return $"{Essential.Platform.AppContext.PackageName}.notify.{notificationId.ToString()}";
        }

        private void CreateNotificationChannel(global::Android.Net.Uri alertUri, bool enableVirate)
        {
            manager = (NotificationManager)AndroidApp.Application.Context.GetSystemService(AndroidApp.Application.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Max)
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
            Log.Debug("Alarm", "AlarmNotificationReceiver GetAlarmSound start : " + DateTime.Now.ToString());
            global::Android.Net.Uri alert = null;
            if (options != null && options.EnableSound)
            {
                alert = RingtoneManager.GetDefaultUri(RingtoneType.Alarm);
                if (alert == null)
                    alert = RingtoneManager.GetDefaultUri(RingtoneType.Ringtone);

                Log.Debug("Alarm", "AlarmNotificationReceiver GetAlarmSound alert3 : " + alert?.ToString());
            }

            Log.Debug("Alarm", "AlarmNotificationReceiver GetAlarmSound alert ready: " + alert?.ToString());
            return alert;
        }

        private int GetIconId(string name)
        {
            Log.Debug("Alarm", "AlarmNotificationReceiver Icon: " + name.ToString());
            try
            {
                return AndroidApp.Application.Context.Resources.GetIdentifier(name, "drawable", Essential.AppInfo.PackageName);
            }
            catch (Exception ex)
            {
                var messages = new List<string>();
                do
                {
                    messages.Add(ex.Message);
                    ex = ex.InnerException;
                }
                while (ex != null);
                var errmessage = string.Join(" - ", messages);

                Log.Error("Alarm", "AlarmNotificationReceiver Icon Error: " + errmessage);
                throw;
            }

        }

        private PendingIntent GetButton(int notificationId, CustomAction action)
        {
            Log.Debug("Alarm", "AlarmNotificationReceiver AddButtons Count : " + action.ToString());

            Intent intent = new Intent(AndroidApp.Application.Context, typeof(NotificationSelectionReceiver));

            intent.SetAction(action.Name);
            intent.PutExtra(Consts.NotificationIdKey, notificationId);
            intent.PutExtra(Consts.AlarmCounterKey, AlarmCounter);


            Log.Debug("Alarm", "AlarmNotificationReceiver AddButtons intent created: " + intent.ToString());

            var pending = AndroidApp.TaskStackBuilder
                  .Create(AndroidApp.Application.Context)
                  .AddNextIntent(intent)
                  .GetPendingIntent(messageId, PendingIntentFlags.UpdateCurrent);

            Log.Debug("Alarm", "AlarmNotificationReceiver AddButtons pendingIntent created: " + pending.ToString());

            Log.Debug("Alarm", "AlarmNotificationReceiver AddButtons finished: " + DateTime.Now.ToString());

            return pending;

        }

        private DateTime GetNextWeekday(DateTime start, DayOfWeek day, int? interval)
        {
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % (7 * interval != null && interval <= 0 ? 1 : (int)interval);
            return start.AddDays(daysToAdd);
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

                        if (alarmOptions?.DaysOfWeek != Enums.DaysOfWeek.None)
                        {
                            var dayList = alarmOptions?.DaysOfWeek.GetUniqueFlags();
                            foreach (DayOfWeek dayitem in dayList)
                            {
                                days.Add(GetNextWeekday(startTime, dayitem, selectedInterval));
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

        private List<int> GetAlarmIds()
        {
            List<int> ids = new List<int>();
            try
            {
                string jsonString = Essential.Preferences.Get(Consts.MessageIdListKey, string.Empty);
                var res = JsonConvert.DeserializeObject<List<int>>(jsonString);
                if (res != null && res.Count > 0)
                    ids = res;

            }
            catch (Exception ex)
            {
                Log.Error("Alarmer", ex.Message);
            }

            return ids;
        }

        private void SaveAlarmIds(List<int> ids)
        {
            if (ids == null)
                return;

            var res = JsonConvert.SerializeObject(ids);
            Essential.Preferences.Set(Consts.MessageIdListKey, res);
        }

        private void RemoveAlarmId(int id)
        {
            List<int> list = GetAlarmIds();
            list.Remove(id);
            SaveAlarmIds(list);
        }

        private void SaveAlarmId(int id)
        {
            List<int> list = GetAlarmIds();
            list.Add(id);
            SaveAlarmIds(list);
        }

        private async Task SaveAlarm(string title, string message, DateTime startDate, AlarmOptions options, NotificationOptions notification)
        {
            try
            {
                var resp = await _alarmRepo.InsertAsync(new AlarmEntity { Message = message, StartDate = startDate, Title = title });
                await _optionRepository.InsertAsync(new AlarmOptionEntity
                {
                    AlarmId = resp,
                    AlarmSequence = options.AlarmSequence,
                    DaysOfWeek = options.DaysOfWeek,
                    EndDate = options.EndDate,
                    Interval = options.Interval,
                    TotalAlarmCount = options.TotalAlarmCount,
                    EnableSound = notification.EnableSound,
                    EnableVibration = notification.EnableVibration,
                    SmallIcon = notification.SmallIcon,
                    LargeIcon = notification.LargeIcon
                });

                foreach (var item in options.AdditionalTimes)
                {
                    await _timingRepository.InsertAsync(new TimingEntity
                    {
                        AlarmId = resp,
                        Time = item
                    });
                }
                foreach (var item in notification.CustomActions)
                {
                    await _customActionRepository.InsertAsync(new CustomActionEntity
                    {
                        AlarmId = resp,
                        Icon = item.Icon,
                        Name = item.Name
                    });
                }
            }
            catch (Exception)
            {

            }

        }

        public int Notify(string title, string message, int notificationId, NotificationOptions options = null)
        {
            messageId++;

            Log.Debug("Alarm", "AlarmNotificationReceiver Notify MessageId: " + messageId.ToString());

            NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Application.Context, channelId)
               .SetContentTitle(title)
               .SetContentText(message);
            builder.SetCategory(Notification.CategoryAlarm);
            builder.SetPriority((int)NotificationPriority.Max);

            Intent mainIntent = AndroidApp.Application.Context.PackageManager.GetLaunchIntentForPackage(AndroidApp.Application.Context.PackageName);
            builder.SetContentIntent(AndroidApp.TaskStackBuilder
                                    .Create(AndroidApp.Application.Context)
                                    .AddNextIntent(mainIntent)
                                    .GetPendingIntent(messageId, PendingIntentFlags.OneShot)
                                    );
            try
            {
                if (options.CustomActions != null)
                    foreach (var item in options.CustomActions)
                    {
                        Log.Debug("Alarm", "AlarmNotificationReceiver Action item : " + item.Name.ToString() + " notificationId : " + notificationId.ToString());
                        builder.AddAction(GetIconId(item.Icon), item.Name, GetButton(notificationId, item));
                    }

                Log.Debug("Alarm", "AlarmNotificationReceiver ButtonsAdded : " + DateTime.Now.ToString());
                int _smallIcon = string.IsNullOrEmpty(options?.SmallIcon) ? Essential.Platform.AppContext.ApplicationInfo.Icon : GetIconId(options.SmallIcon);
                int _largeIcon = string.IsNullOrEmpty(options?.LargeIcon) ? 0 : GetIconId(options.LargeIcon);

                Console.WriteLine("small icon : " + _smallIcon);
                Console.WriteLine("large icon : " + _largeIcon);

                if (_smallIcon != 0)
                    builder.SetSmallIcon(_smallIcon);
                if (_largeIcon != 0)
                    builder.SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Application.Context.Resources, _largeIcon));

                if (options.EnableSound)
                    builder.SetSound(GetAlarmSound(options));
                if (options.EnableVibration)
                    builder.SetVibrate(new long[] { 1000, 1000, 1000 });


                CreateNotificationChannel(GetAlarmSound(options), options != null && options.EnableVibration);
                Notification notification = builder.Build();

                manager.Notify(messageId, notification);

                var args = new LocalNotificationEventArgs()
                {
                    Id = notificationId,
                    Title = title,
                    Message = message,

                };

                NotificationReceived?.Invoke(null, args);

            }
            catch (Exception ex)
            {
                Log.Error("Alarm", "AlarmNotificationReceiver Notify : " + ex.Message);
                Log.Error("Alarm", "AlarmNotificationReceiver Notify : " + ex.StackTrace);
                var messages = new List<string>();
                do
                {
                    messages.Add(ex.Message);
                    ex = ex.InnerException;
                }
                while (ex != null);
                var errmessage = string.Join(" - ", messages);

                Log.Error("Alarm", "AlarmNotificationReceiver Notify : " + errmessage);
            }

            return notificationId;
        }

        public async Task<int> Schedule(string title, string message, DateTime startTime, AlarmOptions alarmOptions, NotificationOptions options)
        {
            var list = GetAlarmIds();
            int notificationId;
            if (list.Count > 0)
                notificationId = list.Max() + 1;
            else
                notificationId = 1;

            if (alarmOptions.AlarmSequence != Enums.AlarmSequence.OneTime)
                AlarmCounter++;

            if (alarmOptions?.TotalAlarmCount != null && alarmOptions?.TotalAlarmCount > 0 && AlarmCounter > alarmOptions.TotalAlarmCount)
                return notificationId;

            if (alarmOptions?.EndDate != null && alarmOptions.EndDate < DateTime.Now)
                return notificationId;
            DateTime nextTime = CalculateNextTime(startTime, alarmOptions);
            Log.Debug("Alarm", "AlarmNotificationReceiver Schedule : " + AndroidApp.Application.Context.ToString());
            var alarmIntent = CreateAlarmIntent(title, message, nextTime, alarmOptions, notificationId, options);
            var pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Application.Context, notificationId, alarmIntent, PendingIntentFlags.CancelCurrent);

            SetAlarmManager(nextTime, pendingIntent);

            SaveAlarmId(notificationId);

            await SaveAlarm(title, message, startTime, alarmOptions, options);

            return notificationId;
        }

        public void ReceiveSelectedNotification(string title, string message, int notificationId, string selectedAction)
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

        public void CancelAll()
        {
            var list = GetAlarmIds();
            foreach (var item in list)
            {
                Cancel(item);
            }
        }

        public void Cancel(int notificationId)
        {
            var alarmManager = AndroidApp.Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            Intent intent = new Intent(AndroidApp.Application.Context, typeof(AlarmNotificationReceiver));
            var pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Application.Context, notificationId, intent, PendingIntentFlags.CancelCurrent);
            alarmManager.Cancel(pendingIntent);

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

        public override string ToString()
        {
            return $"AlarmImplementation = ChannelName : {channelName} - ChannelId : {channelId}";

        }
    }
}
