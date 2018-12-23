namespace MvvmMapsProject.Model
{
    using System;

    /// <summary>
    ///     This class holds meta-data about the various activities that are used in this application.
    /// </summary>
    public class SampleActivityMetaData
    {
        #region Properties

        public Type ActivityToLaunch { get; }
        public int DescriptionResource { get; }
        public string NavId { get; }
        public int TitleResource { get; }

        #endregion

        #region Constructors

        public SampleActivityMetaData(int titleResourceId, int descriptionId, Type activityToLaunch, string navId)
        {
            ActivityToLaunch = activityToLaunch;
            TitleResource = titleResourceId;
            DescriptionResource = descriptionId;
            NavId = navId;
        }

        #endregion
    }
}
