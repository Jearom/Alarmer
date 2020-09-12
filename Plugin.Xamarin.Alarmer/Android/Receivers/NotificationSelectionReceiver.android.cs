using Android.Content;
using Plugin.Xamarin.Alarmer.Shared.Constants;

namespace Plugin.Xamarin.Alarmer.Android.Receivers
{
    [BroadcastReceiver(Enabled = true)]
    public class NotificationSelectionReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            string selectedAction = intent.Action;
            var notificationId = intent.GetStringExtra(Consts.NotificationIdKey);
            if (!string.IsNullOrEmpty(selectedAction))
            {

                Alarmer.Current.ReceiveSelectedNotification(string.Empty, string.Empty, notificationId, selectedAction);
            }
        }
    }
}
