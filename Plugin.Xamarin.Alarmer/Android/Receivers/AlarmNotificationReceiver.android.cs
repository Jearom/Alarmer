using Android.Content;
using Newtonsoft.Json;
using Plugin.Xamarin.Alarmer.Shared.Constants;
using Plugin.Xamarin.Alarmer.Shared.Models;

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

            NotificationOptions options = JsonConvert.DeserializeObject<NotificationOptions>(intent.GetStringExtra(Consts.OptionsKey));

            Alarmer.Current.Notify(title, message, notificationId, options);

        }
    }
}
