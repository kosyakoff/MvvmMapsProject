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
        private TextView addressText;
        private TextView lastModifiedText;
        private TextView latitudeText;
        private TextView longtitudeText;
        private EditText snippetEditText;
        private EditText titleEditText;

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

                longtitudeText = FindViewById<TextView>(Resource.Id.longitudeText);
                longtitudeText.Text = marker.Longtitude.ToString();

                latitudeText = FindViewById<TextView>(Resource.Id.latitudeText);
                latitudeText.Text = marker.Latitude.ToString();

                addressText = FindViewById<TextView>(Resource.Id.addressText);

                addressText.Text = marker.Address;

                lastModifiedText = FindViewById<TextView>(Resource.Id.lastModifiedText);

                lastModifiedText.Text = marker.LastModData.ToString();

                titleEditText = FindViewById<EditText>(Resource.Id.titleEditText);

                titleEditText.Text = marker.Title;

                snippetEditText = FindViewById<EditText>(Resource.Id.snippetEditText);

                snippetEditText.Text = marker.Snippet;
            }

            // Create your application here
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            var markerInfo = new MarkerInfo
            {
                Address = addressText.Text,
                Latitude = double.Parse(latitudeText.Text),
                Longtitude = double.Parse(longtitudeText.Text),
                Title = titleEditText.Text,
                Snippet = snippetEditText.Text,
                LastModData = DateTime.Parse(lastModifiedText.Text)
            };
            var markerMessage = new MarkerMessage
            {
                IsSuccess = true,
                MarkerInfo = markerInfo
            };

            Messenger.Default.Send<MarkerMessage>(markerMessage);
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
