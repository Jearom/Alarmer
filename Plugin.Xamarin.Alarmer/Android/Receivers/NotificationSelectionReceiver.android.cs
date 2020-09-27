using Android.Content;
using Android.Util;
using Newtonsoft.Json;
using Plugin.Xamarin.Alarmer.Shared.Constants;
using Plugin.Xamarin.Alarmer.Shared.Models;
using System;

namespace Plugin.Xamarin.Alarmer.Android.Receivers
{
    [BroadcastReceiver(Enabled = true)]
    public class NotificationSelectionReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            string selectedAction = intent.Action;
            var notificationId = intent.GetIntExtra(Consts.NotificationIdKey, 0);

            var message = intent.GetStringExtra(Consts.MessageKey);
            var title = intent.GetStringExtra(Consts.TitleKey);
            int alarmRunCounter = intent.GetIntExtra(Consts.AlarmCounterKey, 0);

            Log.Debug("Alarm", "AlarmNotificationReceiver Started : " + notificationId.ToString());

            NotificationOptions options = JsonConvert.DeserializeObject<NotificationOptions>(intent.GetStringExtra(Consts.OptionsKey));
           
            if (!string.IsNullOrEmpty(selectedAction))
            {
                if (selectedAction == "Snooze" || selectedAction == "Ertele")
                {
                    var alarmer = new AlarmerImplementation();
                    alarmer.AlarmCounter = alarmRunCounter - 1;
                    Log.Debug("Alarm", "AlarmNotificationReceiver alarmRunCounter : " + alarmer.AlarmCounter.ToString());
                    DateTime date = DateTime.Now.AddMinutes(15);
                    alarmer.Schedule(title, message, date, new AlarmOptions { AlarmSequence = Shared.Enums.AlarmSequence.OneTime }, options);
                }


                Alarmer.Current.ReceiveSelectedNotification(string.Empty, string.Empty, notificationId, selectedAction);
            }
        }


    }
}
