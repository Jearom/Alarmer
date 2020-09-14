using Android.Content;
using Newtonsoft.Json;
using Plugin.Xamarin.Alarmer.Shared.Constants;
using Plugin.Xamarin.Alarmer.Shared.Models;
using System;

namespace Plugin.Xamarin.Alarmer.Android.Receivers
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class AlarmNotificationReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {

            var notificationId = intent.GetStringExtra(Consts.NotificationIdKey);

            var message = intent.GetStringExtra(Consts.MessageKey);
            var title = intent.GetStringExtra(Consts.TitleKey);
            int  alarmRunCounter = intent.GetIntExtra(Consts.AlarmCounterKey,0);

            NotificationOptions options = JsonConvert.DeserializeObject<NotificationOptions>(intent.GetStringExtra(Consts.OptionsKey));
            AlarmOptions alarmOptions = JsonConvert.DeserializeObject<AlarmOptions>(intent.GetStringExtra(Consts.AlarmOptionsKey));
            DateTime dateTime = JsonConvert.DeserializeObject<DateTime>(intent.GetStringExtra(Consts.StartDateKey));

            var alarmer = new AlarmerImplementation();

            if (alarmOptions?.AlarmSequence != Shared.Enums.AlarmSequence.OneTime)
            {
                alarmer.AlarmCounter = alarmRunCounter;
                alarmer.Schedule(title, message, dateTime, alarmOptions, options);
            }
               

            alarmer.Notify(title, message, notificationId, options);

        }
    }
}
