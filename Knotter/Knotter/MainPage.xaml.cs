using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Essentials;
//using Booru;

namespace Knotter
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer

    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

            TitleText.Text = Settings.NameValue;

            SearchBox.Completed += Search_Clicked;
            SettingsButton.Clicked += Settings_Clicked;
        }

        public async void Settings_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void Search_Clicked(object sender, EventArgs e)
        {
            //search clicked
            debug.Text = "submitted query";
            await Navigation.PushAsync(new ResultsPage(SearchBox.Text));
        }

    }
}
