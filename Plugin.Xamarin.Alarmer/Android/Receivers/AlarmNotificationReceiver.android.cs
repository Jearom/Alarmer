using Android.Content;
using Android.Util;
using Newtonsoft.Json;
using Plugin.Xamarin.Alarmer.Shared.Constants;
using Plugin.Xamarin.Alarmer.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Xamarin.Alarmer.Android.Receivers
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class AlarmNotificationReceiver : BroadcastReceiver
    {
        public AlarmNotificationReceiver()
        {
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Log.Debug("Alarm", "AlarmNotificationReceiver Started : " + DateTime.Now.ToString());

            try
            {
                var notificationId = intent.GetIntExtra(Consts.NotificationIdKey,0);

                var message = intent.GetStringExtra(Consts.MessageKey);
                var title = intent.GetStringExtra(Consts.TitleKey);
                int alarmRunCounter = intent.GetIntExtra(Consts.AlarmCounterKey, 0);

                Log.Debug("Alarm", "AlarmNotificationReceiver Started : " + notificationId.ToString());

                NotificationOptions options = JsonConvert.DeserializeObject<NotificationOptions>(intent.GetStringExtra(Consts.OptionsKey));
                AlarmOptions alarmOptions = JsonConvert.DeserializeObject<AlarmOptions>(intent.GetStringExtra(Consts.AlarmOptionsKey));
                DateTime dateTime = JsonConvert.DeserializeObject<DateTime>(intent.GetStringExtra(Consts.StartDateKey));

                var alarmer = new AlarmerImplementation();

                if (alarmOptions?.AlarmSequence != Shared.Enums.AlarmSequence.OneTime)
                {

                    Log.Debug("Alarm", "AlarmNotificationReceiver alarmRunCounter : " + alarmRunCounter.ToString());
                    alarmer.AlarmCounter = alarmRunCounter;
                    Log.Debug("Alarm", "AlarmNotificationReceiver alarmRunCounter : " + alarmer.AlarmCounter.ToString());
                    alarmer.Schedule(title, message, dateTime, alarmOptions, options);
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
}
