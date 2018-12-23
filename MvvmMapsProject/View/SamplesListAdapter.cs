namespace MvvmMapsProject.Mvvm
{
    using System.Collections.Generic;
    using System.Linq;

    using Android.Content;
    using Android.Views;
    using Android.Widget;

    using Model;

    internal class SamplesListAdapter : BaseAdapter<SampleActivityMetaData>
    {
        #region Fields

        private readonly List<SampleActivityMetaData> activities;
        private readonly Context context;

        #endregion

        #region Indexers

        public override SampleActivityMetaData this[int position]
        {
            get
            {
                return activities[position];
            }
        }

        #endregion

        #region Constructors

        public SamplesListAdapter(Context context, IEnumerable<SampleActivityMetaData> sampleActivities)
        {
            this.context = context;
            activities = sampleActivities == null ? new List<SampleActivityMetaData>(0) : sampleActivities.ToList();
        }

        #endregion

        #region Methods

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            FeatureRowHolder row = convertView as FeatureRowHolder ?? new FeatureRowHolder(context);
            SampleActivityMetaData sample = activities[position];

            row.UpdateFrom(sample);
            return row;
        }

        #endregion

        public override int Count
        {
            get
            {
                return activities.Count;
            }
        }
    }
}
