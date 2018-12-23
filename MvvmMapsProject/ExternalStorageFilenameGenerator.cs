namespace MvvmMapsProject
{
    using System;
    using System.IO;

    using Android.Content;

    using Environment = Android.OS.Environment;

    /// <summary>
    ///     Used to determine the path to the backing text file on external storage.
    ///     By default, the file will be considered <i>private</i>.
    /// </summary>
    public class ExternalStorageFilenameGenerator : IGenerateNameOfFile
    {
        #region Fields

        private readonly WeakReference<Context> contextRef;
        private readonly string directoryType;

        #endregion

        #region Properties

        public bool PublicStorage { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// </summary>
        /// <param name = "context"></param>
        /// <param name = "publicStorage"></param>
        /// <param name = "directoryType">
        ///     One of the Android directories for media storage. Can be null for private files, and will default to
        ///     <c>Android.OS.Environment.DirectoryDocuments</c> for public files.
        /// </param>
        public ExternalStorageFilenameGenerator(Context context, bool publicStorage = false, string directoryType = null)
        {
            contextRef = new WeakReference<Context>(context);
            PublicStorage = publicStorage;
            this.directoryType = string.IsNullOrWhiteSpace(directoryType) ? null : directoryType.Trim();
        }

        #endregion

        #region Methods

        public string GetAbsolutePathToFile(string fileName)
        {
            string dir;
            if (PublicStorage)
            {
                if (string.IsNullOrWhiteSpace(directoryType))
                    dir = Environment.ExternalStorageDirectory.AbsolutePath;
                else
                    dir = Environment.GetExternalStoragePublicDirectory(directoryType).AbsolutePath;
            }
            else
            {
                if (!contextRef.TryGetTarget(out Context c))
                    return null;

                dir = c.GetExternalFilesDir(directoryType).AbsolutePath;
            }

            return Path.Combine(dir, fileName);
        }

        #endregion
    }
}
