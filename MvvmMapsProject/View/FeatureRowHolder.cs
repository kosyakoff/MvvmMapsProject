namespace MvvmMapsProject.Mvvm
{
    using Android.Content;
    using Android.Views;
    using Android.Widget;

    using Model;

    internal class FeatureRowHolder : FrameLayout
    {
        #region Fields

        private readonly TextView _description;
        private readonly TextView _title;

        #endregion

        #region Constructors

        public FeatureRowHolder(Context context)
            : base(context)
        {
            var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.Feature, this);
            _title = view.FindViewById<TextView>(Resource.Id.title);
            _description = view.FindViewById<TextView>(Resource.Id.description);
        }

        #endregion

        #region Methods

        public void UpdateFrom(SampleActivityMetaData sample)
        {
            _title.SetText(sample.TitleResource);
            _description.SetText(sample.DescriptionResource);
        }

        #endregion
    }
}
