namespace MvvmMapsProject.Mvvm
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Views;

    /// <summary>
    ///     This class contains properties that the main View can data bind to.
    ///     <para>
    ///         See http://www.mvvmlight.net
    ///     </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        private readonly INavigationService _navigationService;

        #endregion

        #region Properties

        public RelayCommand<string> CreateNewMarkerCommand { get; }

        /// <summary>
        ///     Gets the NavigateCommand.
        ///     Goes to the second page, using the navigation service.
        ///     Use the "mvvmr*" snippet group to create more such commands.
        /// </summary>
        public RelayCommand<string> NavigateCommand { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateCommand = new RelayCommand<string>(NavigateTo);
            CreateNewMarkerCommand = new RelayCommand<string>(CreateNewMarker);
        }

        #endregion

        #region Methods

        private void CreateNewMarker(string parameter)
        {
            _navigationService.NavigateTo(NavigationKeys.CREATE_MARKER_ACTIVITY, parameter);
        }

        private void NavigateTo(string parameter)
        {
            _navigationService.NavigateTo(parameter, null);
        }

        #endregion
    }
}
