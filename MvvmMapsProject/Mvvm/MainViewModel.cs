namespace MvvmMapsProject.Mvvm
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
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

        private RelayCommand<string> _navigateCommand;

        private readonly INavigationService _navigationService;

        private RelayCommand _sendMessageCommand;
        private RelayCommand<string> _showDialogCommand;
        private string _welcomeTitle = "Hello MVVM";

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        #endregion

        /// <summary>
        ///     Gets the NavigateCommand.
        ///     Goes to the second page, using the navigation service.
        ///     Use the "mvvmr*" snippet group to create more such commands.
        /// </summary>
        public RelayCommand<string> NavigateCommand
        {
            get
            {
                return _navigateCommand ?? (_navigateCommand =
                                                new RelayCommand<string>(
                                                    parameter => _navigationService.NavigateTo(ValueConstants.SecondPageKey, parameter)));
            }
        }

        /// <summary>
        ///     Sets and gets the WelcomeTitle property.
        ///     Changes to this property's value raise the PropertyChanged event.
        ///     Use the "mvvminpc*" snippet group to create more such properties.
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }
            set
            {
                Set(ref _welcomeTitle, value);
            }
        }

        /// <summary>
        ///     Gets the SendMessageCommand.
        /// </summary>
        public RelayCommand SendMessageCommand
        {
            get
            {
                return _sendMessageCommand ?? (_sendMessageCommand = new RelayCommand(
                                                   () =>
                                                   {
                                                       // Any object can send messages.
                                                       // For this simple demo, the message is received by App.xaml.cs
                                                       // (see line 98).
                                                       // This message type also allows a reply to be sent.

                                                       Messenger.Default.Send(
                                                           new NotificationMessageAction<string>(
                                                               "AnyNotification",
                                                               reply =>
                                                               {
                                                                   WelcomeTitle = reply;
                                                               }));
                                                   }));
            }
        }
    }
}
