namespace MvvmMapsProject
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Android;
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Gms.Common;
    using Android.Gms.Maps;
    using Android.Gms.Maps.Model;
    using Android.Locations;
    using Android.OS;
    using Android.Support.Design.Widget;
    using Android.Util;
    using Android.Widget;

    using GalaSoft.MvvmLight.Helpers;
    using GalaSoft.MvvmLight.Ioc;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Views;

    using Model;

    using Mvvm;

    using Newtonsoft.Json;

    using DateTime = System.DateTime;
    using Messenger = GalaSoft.MvvmLight.Messaging.Messenger;

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : ActivityBase, IOnMapReadyCallback
    {
        #region Constants

        private const int RC_READ_EXTERNAL_STORAGE_PERMISSION = 1100;
        private const int RC_WRITE_EXTERNAL_STORAGE_PERMISSION = 1000;

        #endregion

        #region Fields

        public static readonly int RC_INSTALL_GOOGLE_PLAY_SERVICES = 1000;

        public static readonly string TAG = "XamarinMapDemo";
        protected static readonly string _defaultFilename = "markers.json";

        // Keep track of bindings to avoid premature garbage collection
        private readonly List<Binding> _bindings = new List<Binding>();
        private IGenerateNameOfFile _filenameGenerator;

        private bool _isGooglePlayServicesInstalled;

        private List<MarkerInfo> _markers = new List<MarkerInfo>();
        private Button _searchMarkerButton;

        private TextView _searchMarkerEditText;

        private GoogleMap googleMap;

        private static readonly string[] PERMISSIONS_TO_REQUEST =
        {
            Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage
        };

        private static readonly int REQUEST_PERMISSIONS_LOCATION = 1000;

        #endregion

        #region Methods

        public void OnMapReady(GoogleMap map)
        {
            googleMap = map;

            foreach (MarkerInfo markerInfo in _markers)
                using (var markerOption = new MarkerOptions())
                {
                    markerOption.SetPosition(new LatLng(markerInfo.Latitude, markerInfo.Longtitude));
                    markerOption.SetTitle(markerInfo.Title);
                    markerOption.SetSnippet(markerInfo.Address);

                    // save the "marker" variable returned if you need move, delete, update it, etc...
                    Marker marker = googleMap.AddMarker(markerOption);
                }

            if (this.PerformRuntimePermissionCheckForLocation(REQUEST_PERMISSIONS_LOCATION))
                InitializeUiSettingsOnMap();

            googleMap.MapClick += HandleOnGoogleMapClick;
            googleMap.MarkerClick += HandleOnGoogleMapMarkerClick;
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
            App.Initialize();
            
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MapLayout);

            _searchMarkerEditText = FindViewById<TextView>(Resource.Id.searchMarkerEditText);
            _searchMarkerButton = FindViewById<Button>(Resource.Id.searchMarkerButton);
            _searchMarkerButton.Click += HandleOnSearchMarkerButtonClick;

            if (Environment.MediaMounted.Equals(Environment.ExternalStorageState))
                _filenameGenerator = new ExternalStorageFilenameGenerator(this);
            else
                _filenameGenerator = new InternalCacheFilenameGenerator(this);

            string json = ReadFile(_filenameGenerator);

            if (!string.IsNullOrWhiteSpace(json))
                _markers = JsonConvert.DeserializeObject<List<MarkerInfo>>(json);

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

        private async void HandleOnSearchMarkerButtonClick(object sender, System.EventArgs e)
        {
            var searchMarkerName = _searchMarkerEditText.Text;

            var foundElement = _markers.FirstOrDefault(x => x.Title.Contains(searchMarkerName));

            if (foundElement != null)
            {
                var builder = CameraPosition.InvokeBuilder();
                builder.Target(new LatLng(foundElement.Latitude,foundElement.Longtitude));
                builder.Zoom(10);
                var cameraPosition = builder.Build();

                // AnimateCamera provides a smooth, animation effect while moving
                // the camera to the the position.
                googleMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
            }
            else
            {
                var dialogService = (DialogService)SimpleIoc.Default.GetInstance<IDialogService>();
               await dialogService.ShowError(Resources.GetString(Resource.String.couldnt_find_any_marker), Resources.GetString(Resource.String.error), Resources.GetString(Resource.String.ok), null);
            }

        }

        private void HandleNotificationMessage(NotificationMessageAction<string> message)
        {
            // Execute a callback to send a reply to the sender.
            message.Execute("Success! (from MainActivity.cs)");
        }

        private async void HandleOnGoogleMapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            if (!RequestExternalStoragePermissionIfNecessary(RC_WRITE_EXTERNAL_STORAGE_PERMISSION))
                return;

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

        private void HandleOnGoogleMapMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            e.Handled = true;

            if (!RequestExternalStoragePermissionIfNecessary(RC_WRITE_EXTERNAL_STORAGE_PERMISSION))
                return;

            MarkerInfo markerInfo =
                _markers.FirstOrDefault(x => x.Latitude == e.Marker.Position.Latitude && x.Longtitude == e.Marker.Position.Longitude);

            if (markerInfo == null)
            {
                return;
            }

            string markerString = JsonConvert.SerializeObject(markerInfo);

            App.Locator.MainVm.CreateNewMarkerCommand.Execute(markerString);
        }

        private void InitializeUiSettingsOnMap()
        {
            googleMap.UiSettings.MyLocationButtonEnabled = true;
            googleMap.UiSettings.CompassEnabled = true;
            googleMap.UiSettings.ZoomControlsEnabled = true;
            googleMap.MyLocationEnabled = true;
        }

        private string ReadFile(IGenerateNameOfFile fileName)
        {
            string backingFile = fileName.GetAbsolutePathToFile(_defaultFilename);

            if (backingFile == null)
                return string.Empty;

            return File.ReadAllText(backingFile);
        }

        private void ReceiveMarkerInfo(MarkerMessage message)
        {
            if (message.IsSuccess && message.MarkerInfo != null)
                using (var markerOption = new MarkerOptions())
                {
                    bool inArray = _markers.Any(x => x.Latitude == message.MarkerInfo.Latitude && x.Longtitude == message.MarkerInfo.Longtitude);

                    if (!inArray)
                    {
                        MarkerInfo markerInfo = message.MarkerInfo;
                        markerOption.SetPosition(new LatLng(markerInfo.Latitude, markerInfo.Longtitude));
                        markerOption.SetTitle(markerInfo.Title);
                        markerOption.SetSnippet(markerInfo.Address);

                        // save the "marker" variable returned if you need move, delete, update it, etc...
                        Marker marker = googleMap.AddMarker(markerOption);

                        _markers.Add(markerInfo);
                    }
                    else
                    {
                        MarkerInfo markerToRemove = _markers.First(
                            x => x.Latitude == message.MarkerInfo.Latitude && x.Longtitude == message.MarkerInfo.Longtitude);
                        _markers.Remove(markerToRemove);
                        _markers.Add(message.MarkerInfo);
                    }

                    string json = JsonConvert.SerializeObject(_markers, Formatting.Indented);

                    WriteFile(_filenameGenerator, json);
                }
        }

        private bool RequestExternalStoragePermissionIfNecessary(int requestCode)
        {
            if (Environment.MediaMounted.Equals(Environment.ExternalStorageState))
            {
                if (CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                    return true;

                if (ShouldShowRequestPermissionRationale(Manifest.Permission.WriteExternalStorage))
                {
                    Snackbar.Make(
                        FindViewById(Android.Resource.Id.Content),
                        Resource.String.write_external_permissions_rationale,
                        Snackbar.LengthIndefinite).SetAction(
                        Resource.String.ok,
                        delegate
                        {
                            RequestPermissions(PERMISSIONS_TO_REQUEST, requestCode);
                        });
                }
                else
                {
                    RequestPermissions(PERMISSIONS_TO_REQUEST, requestCode);
                    return false;
                }

                return false;
            }

            Log.Warn(TAG, "External storage is not mounted; cannot request permission");
            return false;
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

        private void WriteFile(IGenerateNameOfFile fileName, string json)
        {
            string backingFile = fileName.GetAbsolutePathToFile(_defaultFilename);

            if (backingFile == null)
                return;
            File.WriteAllText(backingFile, json);

            string sub = File.ReadAllText(backingFile);
        }

        #endregion
    }
}
