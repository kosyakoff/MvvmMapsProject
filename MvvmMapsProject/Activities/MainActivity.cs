namespace MvvmMapsProject
{
    using System;
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

    using Utility;

    using Environment = Android.OS.Environment;
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

        protected static readonly string _defaultFilename = "markers.json";

        // Keep track of bindings to avoid premature garbage collection
        private readonly List<Binding> _bindings = new List<Binding>();
        private IGenerateNameOfFile _filenameGenerator;

        private bool _isGooglePlayServicesInstalled;

        private List<MarkerInfo> _markers = new List<MarkerInfo>();
        private Button _searchMarkerButton;

        private TextView _searchMarkerEditText;

        private GoogleMap _googleMap;

        private static readonly string[] PERMISSIONS_TO_REQUEST =
        {
            Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage
        };

        private static readonly int REQUEST_PERMISSIONS_LOCATION = 1000;

        #endregion

        #region Methods

        public void OnMapReady(GoogleMap map)
        {
            _googleMap = map;

            foreach (MarkerInfo markerInfo in _markers)
                using (var markerOption = new MarkerOptions())
                {
                    markerOption.SetPosition(new LatLng(markerInfo.Latitude, markerInfo.Longtitude));
                    markerOption.SetTitle(markerInfo.Title);
                    markerOption.SetSnippet(markerInfo.Address);

                    // save the "marker" variable returned if you need move, delete, update it, etc...
                    Marker marker = _googleMap.AddMarker(markerOption);
                }

            if (this.PerformRuntimePermissionCheckForLocation(REQUEST_PERMISSIONS_LOCATION))
            {
                InitializeUiSettingsOnMap();
            }

            _googleMap.MapClick += HandleOnGoogleMapClick;
            _googleMap.MarkerClick += HandleOnGoogleMapMarkerClick;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (RC_INSTALL_GOOGLE_PLAY_SERVICES == requestCode && resultCode == Result.Ok)
            {
                _isGooglePlayServicesInstalled = true;
            }
            else
            {
                ShowErrorAsync($"Don't know how to handle resultCode {resultCode} for request {requestCode}.");
            }
              
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
            {
                _filenameGenerator = new ExternalStorageFilenameGenerator(this);
            }
            else
            {
                _filenameGenerator = new InternalCacheFilenameGenerator(this);
            }
                
            if (RequestExternalStoragePermissionIfNecessary(RC_READ_EXTERNAL_STORAGE_PERMISSION))
            {
                string json = FileHelper.ReadFile(_filenameGenerator, _defaultFilename);

                if (!string.IsNullOrWhiteSpace(json))
                    _markers = JsonConvert.DeserializeObject<List<MarkerInfo>>(json);
            }

            _isGooglePlayServicesInstalled = TestIfGooglePlayServicesIsInstalled();

            if (!_isGooglePlayServicesInstalled)
            {
                Log.Error(Resources.GetString(Resource.String.app_name), Resources.GetString(Resource.String.googleServiceNotInstalled));
                return;
            }

            Messenger.Default.Register<MarkerMessage>(this, ReceiveMarkerInfo);

            var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);
        }

        private async void HandleOnGoogleMapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            if (!RequestExternalStoragePermissionIfNecessary(RC_WRITE_EXTERNAL_STORAGE_PERMISSION))
            {
                return; 
            }

            Address address = await GeocodingHelper.ReverseGeocodeCurrentLocation(e.Point,this);

            var marker = new MarkerInfo
            {
                Longtitude = e.Point.Longitude,
                Latitude = e.Point.Latitude,
                Address = address == null ? string.Empty : $"{address.CountryName} {address.AdminArea}",
                LastModData = DateTime.Now
            };

            string markerString = JsonConvert.SerializeObject(marker);

            App.Locator.MainViewModel.CreateNewMarkerCommand.Execute(markerString);
        }

        private void HandleOnGoogleMapMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            e.Handled = true;

            if (!RequestExternalStoragePermissionIfNecessary(RC_WRITE_EXTERNAL_STORAGE_PERMISSION))
                return;

            MarkerInfo markerInfo =
                _markers.FirstOrDefault(x => x.Latitude == e.Marker.Position.Latitude && x.Longtitude == e.Marker.Position.Longitude);

            if (markerInfo == null)
                return;

            string markerString = JsonConvert.SerializeObject(markerInfo);

            App.Locator.MainViewModel.CreateNewMarkerCommand.Execute(markerString);
        }

        private async void HandleOnSearchMarkerButtonClick(object sender, EventArgs e)
        {
            string searchMarkerName = _searchMarkerEditText.Text;

            MarkerInfo foundElement = _markers.FirstOrDefault(x => x.Title.Contains(searchMarkerName));

            if (foundElement != null)
            {
                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(new LatLng(foundElement.Latitude, foundElement.Longtitude));
                builder.Zoom(10);
                CameraPosition cameraPosition = builder.Build();

                // AnimateCamera provides a smooth, animation effect while moving
                // the camera to the the position.
                _googleMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
            }
            else
            {
                await ShowErrorAsync(Resources.GetString(Resource.String.couldnt_find_any_marker));
            }
        }

        private async Task ShowErrorAsync(string errorString)
        {
            var dialogService = (DialogService)SimpleIoc.Default.GetInstance<IDialogService>();
            await dialogService.ShowError(
                errorString,
                Resources.GetString(Resource.String.error),
                Resources.GetString(Resource.String.ok),
                null);
        }

        private void InitializeUiSettingsOnMap()
        {
            _googleMap.UiSettings.MyLocationButtonEnabled = true;
            _googleMap.UiSettings.CompassEnabled = true;
            _googleMap.UiSettings.ZoomControlsEnabled = true;
            _googleMap.MyLocationEnabled = true;
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
                        Marker marker = _googleMap.AddMarker(markerOption);

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

                    FileHelper.WriteFile(_filenameGenerator,_defaultFilename, json);
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

            Log.Warn(Resources.GetString(Resource.String.app_name), "External storage is not mounted; cannot request permission");
            return false;
        }



        private bool TestIfGooglePlayServicesIsInstalled()
        {
            int queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info(Resources.GetString(Resource.String.app_name), "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                string errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error(Resources.GetString(Resource.String.app_name), "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);
            }

            return false;
        }



        #endregion
    }
}
