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

namespace MvvmMapsProject.Mvvm
{
    using Model;

    class SamplesListAdapter : BaseAdapter<SampleActivityMetaData>
    {
        readonly List<SampleActivityMetaData> activities;
        readonly Context context;

        public SamplesListAdapter(Context context, IEnumerable<SampleActivityMetaData> sampleActivities)
        {
            this.context = context;
            activities = sampleActivities == null ? new List<SampleActivityMetaData>(0) : sampleActivities.ToList();
        }

        public override int Count => activities.Count;

        public override SampleActivityMetaData this[int position] => activities[position];

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var row = convertView as FeatureRowHolder ?? new FeatureRowHolder(context);
            var sample = activities[position];

            row.UpdateFrom(sample);
            return row;
        }
    }
}