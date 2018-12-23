using System;
using Android.App;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace MvvmMapsProject
{
    using Android.OS;

    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Views;

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : ActivityBase
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Illustrates how to use the Messenger by receiving a message
            // and sending a message back.
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<NotificationMessageAction<string>>(
                this,
                HandleNotificationMessage);

        }

        private void HandleNotificationMessage(NotificationMessageAction<string> message)
        {
            // Execute a callback to send a reply to the sender.
            message.Execute("Success! (from MainActivity.cs)");
        }

    }
}

