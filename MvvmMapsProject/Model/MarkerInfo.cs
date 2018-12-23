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

namespace MvvmMapsProject.Model
{
    public class MarkerInfo
    {
        public double Longtitude { get; set; }
        public double Latitude { get; set; }
        public string Address { get; set; }
        public string Title { get; set; }
        public string Snippet { get; set; }
        public DateTime LastModData { get; set; }
    }
}