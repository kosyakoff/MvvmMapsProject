using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MvvmMapsProject.Utility
{
    using System.Threading.Tasks;

    using Android.Gms.Maps.Model;
    using Android.Locations;

    public static class GeocodingHelper
    {
        public static async Task<Address> ReverseGeocodeCurrentLocation(LatLng point, Context context)
        {
            var geocoder = new Geocoder(context);

            IList<Address> addressList = await geocoder.GetFromLocationAsync(point.Latitude, point.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }
    }
}