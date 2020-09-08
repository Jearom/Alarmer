using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.Core.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.Plugin.Xamarin.Alarmer.Android.Receivers
{
    [BroadcastReceiver(Enabled =true)]
    public class AlarmNotificationReceiver : BroadcastReceiver
    {
       

        public override void OnReceive(Context context, Intent intent)
        {
            Notification.Builder builder = new Notification.Builder(context,channelId);

            
        }


       
    }
}
