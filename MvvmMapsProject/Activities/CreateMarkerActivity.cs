namespace MvvmMapsProject
{
    using System;

    using Android.App;
    using Android.OS;
    using Android.Widget;

    using CommonServiceLocator;

    using GalaSoft.MvvmLight.Views;

    using Model;

    using Newtonsoft.Json;

    using Messenger = GalaSoft.MvvmLight.Messaging.Messenger;

    [Activity(Label = "CreateMarkerActivity")]
    public class CreateMarkerActivity : ActivityBase
    {
        #region Fields

        private NavigationService _navigationService;
        private TextView _addressText;
        private TextView _lastModifiedText;
        private TextView _latitudeText;
        private TextView _longtitudeText;
        private EditText _snippetEditText;
        private EditText _titleEditText;

        #endregion

        #region Methods

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CreateMarkerActivity);

            _navigationService = (NavigationService)ServiceLocator.Current.GetInstance<INavigationService>();
            var parameter = _navigationService.GetAndRemoveParameter<string>(Intent);

            var cancelButton = FindViewById<Button>(Resource.Id.cancelButton);
            cancelButton.Click += CancelButton_Click;

            var applyButton = FindViewById<Button>(Resource.Id.applyButton);
            applyButton.Click += ApplyButton_Click;

            if (parameter != null)
            {
                var marker = JsonConvert.DeserializeObject<MarkerInfo>(parameter);

                _longtitudeText = FindViewById<TextView>(Resource.Id.longitudeText);
                _longtitudeText.Text = marker.Longtitude.ToString();

                _latitudeText = FindViewById<TextView>(Resource.Id.latitudeText);
                _latitudeText.Text = marker.Latitude.ToString();

                _addressText = FindViewById<TextView>(Resource.Id.addressText);

                _addressText.Text = marker.Address;

                _lastModifiedText = FindViewById<TextView>(Resource.Id.lastModifiedText);

                _lastModifiedText.Text = marker.LastModData.ToString();

                _titleEditText = FindViewById<EditText>(Resource.Id.titleEditText);

                _titleEditText.Text = marker.Title;

                _snippetEditText = FindViewById<EditText>(Resource.Id.snippetEditText);

                _snippetEditText.Text = marker.Snippet;
            }
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            var markerInfo = new MarkerInfo
            {
                Address = _addressText.Text,
                Latitude = double.Parse(_latitudeText.Text),
                Longtitude = double.Parse(_longtitudeText.Text),
                Title = _titleEditText.Text,
                Snippet = _snippetEditText.Text,
                LastModData = DateTime.Now
            };
            var markerMessage = new MarkerMessage
            {
                IsSuccess = true,
                MarkerInfo = markerInfo
            };

            Messenger.Default.Send(markerMessage);
            _navigationService.GoBack();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Messenger.Default.Send(
                new MarkerMessage
                {
                    IsSuccess = true,
                    MarkerInfo = null
                });
            _navigationService.GoBack();
        }

        #endregion
    }
}
