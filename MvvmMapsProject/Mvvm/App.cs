namespace MvvmMapsProject.Mvvm
{
    using GalaSoft.MvvmLight.Ioc;
    using GalaSoft.MvvmLight.Threading;
    using GalaSoft.MvvmLight.Views;

    public static class App
    {
        #region Fields

        private static ViewModelLocator _locator;
        private static bool _isInitailized;

        #endregion

        #region Methods

        public static void Initialize()
        {
            if (_isInitailized)
            {
                return;
            }

            _isInitailized = true;
            
            // Initialize the MVVM Light DispatcherHelper.
            // This needs to be called on the UI thread.
            DispatcherHelper.Initialize();

            Registration();

            SetNavigation((NavigationService)SimpleIoc.Default.GetInstance<INavigationService>());
        }
        
        private static void Registration()
        {
            // Configure and register the MVVM Light NavigationService
            var navigationService = new NavigationService();

            SimpleIoc.Default.Register<INavigationService>(() => navigationService);
            // Register the MVVM Light DialogService
            
            var dialogService = new DialogService();
            
            SimpleIoc.Default.Register<IDialogService>(() => dialogService);
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
                    Initialize();

                    _locator = new ViewModelLocator();
                }

                return _locator;
            }
        }
    }
}
