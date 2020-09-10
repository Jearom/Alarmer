using Android.Content;
using Plugin.Xamarin.Alarmer.Shared.Constants;

namespace Plugin.Xamarin.Alarmer.Android.Receivers
{
    [BroadcastReceiver(Enabled = true)]
    public class AlarmNotificationReceiver : BroadcastReceiver
    {

        public override void OnReceive(Context context, Intent intent)
        {
            var message = intent.GetStringExtra(Consts.MessageKey);
            var title = intent.GetStringExtra(Consts.TitleKey);
            bool enableSound = intent.GetBooleanExtra(Consts.SoundKey, false);
            bool enableVibrate = intent.GetBooleanExtra(Consts.VibrateKey, false);

            Alarmer.Current.Notify(title, message, enableSound, enableVibrate);
            Alarmer.Current.ReceiveNotification(title, message);

        }

    }
}
