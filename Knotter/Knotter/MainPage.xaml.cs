using System;
using System.ComponentModel;
using Xamarin.Forms;
//using Booru;

namespace Knotter
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer

    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

            TitleText.Text = Settings.NameValue;

            SearchBox.Completed += SearchClicked;
            SettingsButton.Clicked += SettingsClicked;
        }

        public async void SettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage()).ConfigureAwait(false);//.ConfigureAwait(false);
        }

        private async void SearchClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ResultsPage(SearchBox.Text));//.ConfigureAwait(false);
        }

    }
}
