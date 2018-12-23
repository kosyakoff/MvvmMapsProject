namespace MvvmMapsProject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Android.App;
    using Android.Content;
    using Android.Gms.Common;
    using Android.Gms.Maps;
    using Android.Gms.Maps.Model;
    using Android.Locations;
    using Android.OS;
    using Android.Util;

    using GalaSoft.MvvmLight.Helpers;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Views;

    using Model;

    using Mvvm;

    using Newtonsoft.Json;

    using Messenger = GalaSoft.MvvmLight.Messaging.Messenger;

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : ActivityBase, IOnMapReadyCallback
    {
        #region Fields

        public static readonly int RC_INSTALL_GOOGLE_PLAY_SERVICES = 1000;

        public static readonly string TAG = "XamarinMapDemo";

        // Keep track of bindings to avoid premature garbage collection
        private readonly List<Binding> _bindings = new List<Binding>();

        private bool _isGooglePlayServicesInstalled;

        private readonly List<Marker> _markers = new List<Marker>();

        private GoogleMap googleMap;
        private static readonly int REQUEST_PERMISSIONS_LOCATION = 1000;

        #endregion

        #region Methods

        public void OnMapReady(GoogleMap map)
        {
            googleMap = map;

            if (this.PerformRuntimePermissionCheckForLocation(REQUEST_PERMISSIONS_LOCATION))
                InitializeUiSettingsOnMap();

            googleMap.MapClick += GoogleMap_MapClick;
            googleMap.MarkerClick += GoogleMap_MarkerClick;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (RC_INSTALL_GOOGLE_PLAY_SERVICES == requestCode && resultCode == Result.Ok)
                _isGooglePlayServicesInstalled = true;
            else
                Log.Warn(TAG, $"Don't know how to handle resultCode {resultCode} for request {requestCode}.");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MapLayout);

            // Illustrates how to use the Messenger by receiving a message
            // and sending a message back.
            Messenger.Default.Register<NotificationMessageAction<string>>(this, HandleNotificationMessage);

            _isGooglePlayServicesInstalled = TestIfGooglePlayServicesIsInstalled();

            if (!_isGooglePlayServicesInstalled)
            {
                Log.Error(TAG, "Google Play Services is not installed");
                return;
            }

            Messenger.Default.Register<MarkerMessage>(this, ReceiveMarkerInfo);

            var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);
        }

        private async void GoogleMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            Address address = await ReverseGeocodeCurrentLocation(e.Point);

            var marker = new MarkerInfo
            {
                Longtitude = e.Point.Longitude,
                Latitude = e.Point.Latitude,
                Address = address == null ? string.Empty : $"{address.CountryName} {address.AdminArea}",
                LastModData = DateTime.Now
            };

            string markerString = JsonConvert.SerializeObject(marker);

            App.Locator.MainVm.CreateNewMarkerCommand.Execute(markerString);
        }

        private void GoogleMap_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            //e.Handled = true;
        }

        private void HandleNotificationMessage(NotificationMessageAction<string> message)
        {
            // Execute a callback to send a reply to the sender.
            message.Execute("Success! (from MainActivity.cs)");
        }

        private void InitializeUiSettingsOnMap()
        {
            googleMap.UiSettings.MyLocationButtonEnabled = true;
            googleMap.UiSettings.CompassEnabled = true;
            googleMap.UiSettings.ZoomControlsEnabled = true;
            googleMap.MyLocationEnabled = true;
        }

        private void ReceiveMarkerInfo(MarkerMessage message)
        {
            if (message.IsSuccess && message.MarkerInfo != null)
                using (var markerOption = new MarkerOptions())
                {
                    markerOption.SetPosition(new LatLng(message.MarkerInfo.Latitude, message.MarkerInfo.Longtitude));
                    markerOption.SetTitle(message.MarkerInfo.Title);
                    markerOption.SetSnippet(message.MarkerInfo.Address);
                    // save the "marker" variable returned if you need move, delete, update it, etc...
                    Marker marker = googleMap.AddMarker(markerOption);
                    _markers.Add(marker);
                }
        }

        private async Task<Address> ReverseGeocodeCurrentLocation(LatLng point)
        {
            var geocoder = new Geocoder(this);
            IList<Address> addressList = await geocoder.GetFromLocationAsync(point.Latitude, point.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
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
            }

            return false;
        }

        #endregion
    }
}
