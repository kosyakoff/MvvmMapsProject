namespace MvvmMapsProject.Model
{
    using System;

    public class MarkerInfo
    {
        #region Properties

        public string Address { get; set; }
        public DateTime LastModData { get; set; }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
        public string Snippet { get; set; }
        public string Title { get; set; }

        #endregion
    }
}
