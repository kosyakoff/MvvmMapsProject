namespace MvvmMapsProject
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Android.App;
    using Android.Gms.Maps;
    using Android.Gms.Maps.Model;
    using Android.Locations;
    using Android.OS;
    using Android.Support.V7.App;

    [Activity(Label = "@string/activity_label_mapwithmarkers")]
    public class MapWithMarkersActivity : AppCompatActivity, IOnMapReadyCallback
    {
        #region Fields

        private GoogleMap googleMap;
        //private static readonly LatLng PasschendaeleLatLng = new LatLng(50.897778, 3.013333);
        //private static readonly LatLng VimyRidgeLatLng = new LatLng(50.379444, 2.773611);
        //private Button animateToLocationButton;

        private static readonly int REQUEST_PERMISSIONS_LOCATION = 1000;

        #endregion

        #region Methods

        public void OnMapReady(GoogleMap map)
        {
            googleMap = map;

            //if (this.PerformRuntimePermissionCheckForLocation(REQUEST_PERMISSIONS_LOCATION))
            //    InitializeUiSettingsOnMap();

            googleMap.MapClick += GoogleMap_MapClick;
            googleMap.MarkerClick += GoogleMap_MarkerClick;

            //AddMarkersToMap();
            //animateToLocationButton.Click += AnimateToPasschendaele;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MapLayout);

            var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            ////////
            //animateToLocationButton = FindViewById<Button>(Resource.Id.animateButton);
            //animateToLocationButton.Click += AnimateToPasschendaele;

            //SetupZoomInButton();
            //SetupZoomOutButton();
        }

        private async void GoogleMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            Address address = await ReverseGeocodeCurrentLocation(e.Point);

            using (var markerOption = new MarkerOptions())
            {
                markerOption.SetPosition(e.Point);
                markerOption.SetTitle("StackOverflow");
                markerOption.SetSnippet(address.ToString());
                // save the "marker" variable returned if you need move, delete, update it, etc...
                Marker marker = googleMap.AddMarker(markerOption);
            }
        }

        private void GoogleMap_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            //e.Handled = true;
        }

        private void InitializeUiSettingsOnMap()
        {
            googleMap.UiSettings.MyLocationButtonEnabled = true;
            googleMap.UiSettings.CompassEnabled = true;
            googleMap.UiSettings.ZoomControlsEnabled = true;
            googleMap.MyLocationEnabled = true;
        }

        private async Task<Address> ReverseGeocodeCurrentLocation(LatLng point)
        {
            var geocoder = new Geocoder(this);
            IList<Address> addressList = await geocoder.GetFromLocationAsync(point.Latitude, point.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        #endregion

        //private void AddMarkersToMap()
        //{
        //    var vimyMarker = new MarkerOptions();
        //    vimyMarker.SetPosition(VimyRidgeLatLng).SetTitle("Vimy Ridge").
        //        SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueCyan));
        //    googleMap.AddMarker(vimyMarker);

        //    var passchendaeleMarker = new MarkerOptions();
        //    passchendaeleMarker.SetPosition(PasschendaeleLatLng).SetTitle("PasschendaeleLatLng");
        //    googleMap.AddMarker(passchendaeleMarker);

        //    // We create an instance of CameraUpdate, and move the map to it.
        //    CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(VimyRidgeLatLng, 15);
        //    googleMap.MoveCamera(cameraUpdate);
        //}

        //private void AnimateToPasschendaele(object sender, EventArgs e)
        //{
        //    // Move the camera to the PasschendaeleLatLng Memorial in Belgium.
        //    CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
        //    builder.Target(PasschendaeleLatLng);
        //    builder.Zoom(18);
        //    builder.Bearing(155);
        //    builder.Tilt(65);
        //    CameraPosition cameraPosition = builder.Build();

        //    // AnimateCamera provides a smooth, animation effect while moving
        //    // the camera to the the position.
        //    googleMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
        //}

        //private void SetupZoomInButton()
        //{
        //    var zoomInButton = FindViewById<Button>(Resource.Id.zoomInButton);
        //    zoomInButton.Click += (sender, e) =>
        //    {
        //        googleMap.AnimateCamera(CameraUpdateFactory.ZoomIn());
        //    };
        //}

        //private void SetupZoomOutButton()
        //{
        //    var zoomOutButton = FindViewById<Button>(Resource.Id.zoomOutButton);
        //    zoomOutButton.Click += (sender, e) =>
        //    {
        //        googleMap.AnimateCamera(CameraUpdateFactory.ZoomOut());
        //    };
        //}
    }
}
