namespace MvvmMapsProject.Mvvm
{
    using Android.Content;
    using Android.Views;
    using Android.Widget;

    using Model;

    internal class FeatureRowHolder : FrameLayout
    {
        #region Fields

        private readonly TextView description;
        private readonly TextView title;

        #endregion

        #region Constructors

        public FeatureRowHolder(Context context)
            : base(context)
        {
            var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.Feature, this);
            title = view.FindViewById<TextView>(Resource.Id.title);
            description = view.FindViewById<TextView>(Resource.Id.description);
        }

        #endregion

        #region Methods

        public void UpdateFrom(SampleActivityMetaData sample)
        {
            title.SetText(sample.TitleResource);
            description.SetText(sample.DescriptionResource);
        }

        #endregion
    }
}
