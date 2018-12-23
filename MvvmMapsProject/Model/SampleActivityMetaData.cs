namespace MvvmMapsProject.Model
{
    using System;

    using Android.App;
    using Android.Content;

    /// <summary>
    ///     This class holds meta-data about the various activities that are used in this application.
    /// </summary>
    public class SampleActivityMetaData
    {
        #region Properties

        public Type ActivityToLaunch { get; }
        public int DescriptionResource { get; }
        public int TitleResource { get; }

        #endregion

        #region Constructors

        public SampleActivityMetaData(int titleResourceId, int descriptionId, Type activityToLaunch)
        {
            ActivityToLaunch = activityToLaunch;
            TitleResource = titleResourceId;
            DescriptionResource = descriptionId;
        }

        #endregion

        #region Methods

        public void Start(Activity context)
        {
            var i = new Intent(context, ActivityToLaunch);
            context.StartActivity(i);
        }

        #endregion
    }
}
