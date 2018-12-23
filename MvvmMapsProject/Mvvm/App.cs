namespace MvvmMapsProject.Mvvm
{
    using GalaSoft.MvvmLight.Ioc;
    using GalaSoft.MvvmLight.Threading;
    using GalaSoft.MvvmLight.Views;

    public static class App
    {
        #region Fields

        private static ViewModelLocator _locator;

        #endregion

        #region Methods

        private static void Registration()
        {
            // Configure and register the MVVM Light NavigationService
            var nav = new NavigationService();

            SimpleIoc.Default.Register<INavigationService>(() => nav);
            // Register the MVVM Light DialogService
            SimpleIoc.Default.Register<IDialogService, DialogService>();
        }

        private static void SetNavigation(NavigationService nav)
        {
            nav.Configure(NavigationKeys.CREATE_MARKER_ACTIVITY, typeof(CreateMarkerActivity));
            nav.Configure(NavigationKeys.MAIN_ACTIVITY, typeof(MainActivity));
        }

        #endregion

        public static ViewModelLocator Locator
        {
            get
            {
                if (_locator == null)
                {
                    // Initialize the MVVM Light DispatcherHelper.
                    // This needs to be called on the UI thread.
                    DispatcherHelper.Initialize();

                    Registration();

                    SetNavigation((NavigationService)SimpleIoc.Default.GetInstance<INavigationService>());

                    _locator = new ViewModelLocator();
                }

                return _locator;
            }
        }
    }
}
