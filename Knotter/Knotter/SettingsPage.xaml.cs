using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace Knotter
{

    public static class Settings
    {
        //maybe use a dictionary?
        public static string HostValue;
        public static string NameValue;
        public static void LoadUserSettings()
        {
            HostValue = Preferences.Get("Host", "https://e926.net");
            NameValue = Preferences.Get("Name", "e926");
            //todo: username/apikey 

            Booru.Initialize(Settings.HostValue);
        }
        public static void UpdateUserSettings(string Host, string Name)
        {
            Preferences.Set("Host", HostValue = Host);
            Preferences.Set("Name", NameValue = Name);

            Booru.Initialize(HostValue);
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            Settings.LoadUserSettings();

            //Display Settings
            UIHostValue.Text = Settings.HostValue;
            UINameValue.Text = Settings.NameValue;

            Submit.Clicked += UpdateSettings;
        }

        private async void UpdateSettings(object sender, EventArgs e)
        {
            Settings.UpdateUserSettings(UIHostValue.Text, UINameValue.Text);

            //find the root page
            var root = Navigation.NavigationStack.First();

            // insert the new page at the beginning of the stack
            Navigation.InsertPageBefore(new MainPage(), root);

            //pop to the new root page
            await Navigation.PopToRootAsync();
        }

        public void DisplaySettings()
        {
            //in case it hasnt already been called for some reason
            Settings.LoadUserSettings();

            //Display Settings
            UIHostValue.Text = Settings.HostValue;
            UINameValue.Text = Settings.NameValue;
        }

    }
}