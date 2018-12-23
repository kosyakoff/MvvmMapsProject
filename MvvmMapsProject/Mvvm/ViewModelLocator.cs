namespace MvvmMapsProject.Mvvm
{
    using System.Diagnostics.CodeAnalysis;

    using CommonServiceLocator;

    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    ///     This class contains static references to the most relevant view models in the
    ///     application and provides an entry point for the bindings.
    ///     <para>
    ///         See http://www.mvvmlight.net
    ///     </para>
    /// </summary>
    public class ViewModelLocator
    {
        #region Constructors

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }

        #endregion

        /// <summary>
        ///     Gets the Main property.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel MainVm
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

    }
}
