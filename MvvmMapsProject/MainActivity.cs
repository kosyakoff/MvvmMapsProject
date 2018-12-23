namespace MvvmMapsProject
{
    using System.Collections.Generic;

    using Android.App;
    using Android.Content;
    using Android.Gms.Common;
    using Android.OS;
    using Android.Util;
    using Android.Widget;

    using GalaSoft.MvvmLight.Helpers;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Views;

    using Model;

    using Mvvm;

    using AndroidUri = Android.Net.Uri;
    using Messenger = GalaSoft.MvvmLight.Messaging.Messenger;

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : ActivityBase
    {
        #region Fields

        public static readonly int RC_INSTALL_GOOGLE_PLAY_SERVICES = 1000;

        public static readonly string TAG = "XamarinMapDemo";

        // Keep track of bindings to avoid premature garbage collection
        private readonly List<Binding> _bindings = new List<Binding>();

        private bool isGooglePlayServicesInstalled;
        private SamplesListAdapter listAdapter;
        private ListView listView;

        #endregion

        #region Methods

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (RC_INSTALL_GOOGLE_PLAY_SERVICES == requestCode && resultCode == Result.Ok)
                isGooglePlayServicesInstalled = true;
            else
                Log.Warn(TAG, $"Don't know how to handle resultCode {resultCode} for request {requestCode}.");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Illustrates how to use the Messenger by receiving a message
            // and sending a message back.
            Messenger.Default.Register<NotificationMessageAction<string>>(this, HandleNotificationMessage);

            isGooglePlayServicesInstalled = TestIfGooglePlayServicesIsInstalled();

            InitializeListView();
        }

        protected override void OnResume()
        {
            base.OnResume();
            listView.ItemClick += SampleSelected;
        }

        private void HandleNotificationMessage(NotificationMessageAction<string> message)
        {
            // Execute a callback to send a reply to the sender.
            message.Execute("Success! (from MainActivity.cs)");
        }

        private void InitializeListView()
        {
            listView = FindViewById<ListView>(Resource.Id.listView);
            if (isGooglePlayServicesInstalled)
            {
                listAdapter = new SamplesListAdapter(this, App.Locator.Main.SampleMetaDataList);
            }
            else
            {
                Log.Error(TAG, "Google Play Services is not installed");
                listAdapter = new SamplesListAdapter(this, null);
            }

            listView.Adapter = listAdapter;
        }

        private void SampleSelected(object sender, AdapterView.ItemClickEventArgs e)
        {
            int position = e.Position;
            //if (position == 0)
            //{
            //    AndroidUri geoUri = AndroidUri.Parse("geo:42.374260,-71.120824");
            //    var mapIntent = new Intent(Intent.ActionView, geoUri);
            //    StartActivity(mapIntent);
            //    return;
            //}

            SampleActivityMetaData sampleToStart = App.Locator.Main.SampleMetaDataList[position];
            sampleToStart.Start(this);
        }

        private bool TestIfGooglePlayServicesIsInstalled()
        {
            int queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info(TAG, "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                string errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error(TAG, "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);
                Dialog errorDialog = GoogleApiAvailability.Instance.GetErrorDialog(this, queryResult, RC_INSTALL_GOOGLE_PLAY_SERVICES);
                //var dialogFrag = new ErrorDialogFragment();
                //dialogFrag.dial = errorDialog;

                //dialogFrag.Show(FragmentManager, "GooglePlayServicesDialog");
            }

            return false;
        }

        #endregion
    }
}
