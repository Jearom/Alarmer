using Android.App;
using Android.Content;
using Android.Util;
using Newtonsoft.Json;
using Plugin.Xamarin.Alarmer.Shared.Constants;
using Plugin.Xamarin.Alarmer.Shared.Models;
using Plugin.Xamarin.Alarmer.Shared.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Xamarin.Alarmer.Android.Receivers
{
    [BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class AlarmNotificationReceiver : BroadcastReceiver
    {
        public AlarmNotificationReceiver()
        {
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Log.Debug("Alarm", "AlarmNotificationReceiver Started : " + DateTime.Now.ToString());
            var alarmer = new AlarmerImplementation();

            if (intent.Action != null && intent.Action == Intent.ActionBootCompleted)
            {
                Log.Debug("Alarm", "AlarmNotificationReceiver Reboot Started : " + DateTime.Now.ToString());
                StartAfterReboot(alarmer);
            }
            else
            {
                try
                {
                    var notificationId = intent.GetIntExtra(Consts.NotificationIdKey, 0);

                    var message = intent.GetStringExtra(Consts.MessageKey);
                    var title = intent.GetStringExtra(Consts.TitleKey);
                    int alarmRunCounter = intent.GetIntExtra(Consts.AlarmCounterKey, 0);

                    Log.Debug("Alarm", "AlarmNotificationReceiver Started : " + notificationId.ToString());

                    NotificationOptions options = JsonConvert.DeserializeObject<NotificationOptions>(intent.GetStringExtra(Consts.OptionsKey));
                    AlarmOptions alarmOptions = JsonConvert.DeserializeObject<AlarmOptions>(intent.GetStringExtra(Consts.AlarmOptionsKey));
                    DateTime dateTime = JsonConvert.DeserializeObject<DateTime>(intent.GetStringExtra(Consts.StartDateKey));


                    if (alarmOptions?.AlarmSequence != Shared.Enums.AlarmSequence.OneTime)
                    {

                        Log.Debug("Alarm", "AlarmNotificationReceiver alarmRunCounter : " + alarmRunCounter.ToString());
                        alarmer.AlarmCounter = alarmRunCounter;
                        Log.Debug("Alarm", "AlarmNotificationReceiver alarmRunCounter : " + alarmer.AlarmCounter.ToString());
                        alarmer.Schedule(notificationId, title, message, dateTime, alarmOptions, options);
                    }

                    Log.Debug("Alarm", "AlarmNotificationReceiver Started : " + options.ToString());

                    alarmer.Notify(title, message, notificationId, options);
                    Log.Debug("Alarm", "AlarmNotificationReceiver finished : " + DateTime.Now.ToString());
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
                    var message = string.Join(" - ", messages);

                    Console.WriteLine("AlarmNotificationReceiver : " + message);
                    Log.Error("Alarm", "AlarmNotificationReceiver : " + ex.Message);
                }
            }
        }


        private async Task StartAfterReboot(AlarmerImplementation alarmer)
        {
            AlarmRepository _alarmRepo = new AlarmRepository();
            TimingRepository _timingRepository = new TimingRepository();
            CustomActionRepository _customActionRepository = new CustomActionRepository();

            try
            {
                var alarmList = await _alarmRepo.GetListAsync();

                if (alarmList != null && alarmList.Count > 0)
                    foreach (var alarm in alarmList)
                    {
                        var timings = await _timingRepository.QueryAsync().Where(w => w.AlarmId == alarm.Id).ToArrayAsync();
                        var customs = await _customActionRepository.QueryAsync().Where(w => w.AlarmId == alarm.Id).ToArrayAsync();

                        var alarmOption = new AlarmOptions
                        {
                            EndDate = alarm.EndDate,
                            AdditionalTimes = timings.Select(s => s.Time).ToArray(),
                            AlarmSequence = alarm.AlarmSequence,
                            DaysOfWeek = alarm.DaysOfWeek,
                            Interval = alarm.Interval,
                            TotalAlarmCount = alarm.TotalAlarmCount
                        };

                        var notification = new NotificationOptions
                        {
                            EnableSound = alarm.EnableSound,
                            EnableVibration = alarm.EnableVibration,
                            LargeIcon = alarm.LargeIcon,
                            SmallIcon = alarm.SmallIcon,
                            CustomActions = customs.Select(s => { return new CustomAction { Icon = s.Icon, Name = s.Name }; }).ToArray()
                        };

                        alarmer.Schedule(alarm.Id, alarm.Title, alarm.Message, alarm.StartDate, alarmOption, notification, isNew: false);
                    }
            }
            catch (Exception)
            {

            }
        }
    }
}
