using System;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;

namespace Knotter
{
    public static class Settings
    {
        //maybe use a dictionary?
        private static string _host = Preferences.Get("HostValue", "https://e926.net");
        private static string _name = Preferences.Get("HostName", "e926");
        private static string _user = Preferences.Get("Username", "");
        private static string _pass = Preferences.Get("ApiKey", "");
        public static string HostValue { 
            get { return _host; } 
            set {
                _host = value;
                Preferences.Set("HostValue", value);
            } 
        }

        public static string HostTitle { 
            get { return _name; }
            set {
                _name  = value;
                Preferences.Set("HostName", value); 
            }
        }

        public static string Username {
            get { return _user; }
            set {
                _user = value;
                Preferences.Set("Username", value); }
        }

        public static string ApiKey {
            get { return _pass; }
            set {
                _pass = value;
                Preferences.Set("ApiKey", value); } 
        }

        public static void UpdateUserSettings(string Host, string Name)
        {
            Settings.HostValue = Host;
            Settings.HostTitle = Name;
            //Booru.Initialize(HostValue);
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : TabbedPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            //Display Settings
            UIEntryTitle.Text = Settings.HostTitle;
            UIEntryURL.Text = Settings.HostValue;
            UIEntryUserame.Text = Settings.Username;
            //
            UIButtonUpdateHost.Clicked += UpdateSettings;
            UIButtonLogin.Clicked += LoginAsync;
            UICheckVersion.Clicked += CheckForUpdates;

            if (UserActions.Isloggedin())
            {
                UILabelLoggedIn.Text = $"Welcome back, {Settings.Username}";
                UILabelLoggedIn.BackgroundColor = Color.LightGreen;
            }
            else
            {
                UILabelLoggedIn.Text = $"Not logged in";
                UILabelLoggedIn.BackgroundColor = Color.IndianRed;
            } 
            //Adverts
            GroundZer0s.Clicked += async (s, e) => { await Launcher.OpenAsync(new Uri("https://groundzer0s-art.tumblr.com/")); };
        }

        public void UIGetWelcomeText()
        {
            if (UserActions.Isloggedin())
            {
                UILabelLoggedIn.Text = $"Welcome back, {Settings.Username}";
                UILabelLoggedIn.BackgroundColor = Color.LightGreen;
            }
            else
            {
                UILabelLoggedIn.Text = $"Not logged in";
                UILabelLoggedIn.BackgroundColor = Color.IndianRed;
            }
        }

        private async void LoginAsync(object sender, EventArgs e)
        {
            bool status = await UserActions.Login(UIEntryUserame.Text, UIEntryPassword.Text);
            if (status)
                UIGetWelcomeText();
        }

        private async void CheckForUpdates(object sender, EventArgs e)
        {
            var check = await Connection.CheckVersion();
            if (check)
            {
                bool reply = await DisplayAlert("wuw whats this", "An Update is availible!", "Download", "Cancel");
                if (reply)
                {
                    await Launcher.OpenAsync(new Uri("https://github.com/keihoag/knotter/raw/master/apk/com.lolsoft.Knotter.apk"));//https://github.com/keihoag/knotter
                }
            }
            else
            {
                UIVersionLabel.IsVisible = true;
                UIVersionLabel.Text = "No Updates. :(";
                Device.StartTimer(TimeSpan.FromSeconds(2), () => {
                   return UIVersionLabel.IsVisible = false;//no update
                });
            }
        }

        private async void UpdateSettings(object sender, EventArgs e)
        {
            Settings.UpdateUserSettings(UIEntryURL.Text, UIEntryTitle.Text);
            
            UIUpdateSettings();

            
            var root = Navigation.NavigationStack.First();

            // insert the new page at the beginning of the stack
            Navigation.InsertPageBefore(new MainPage(), root);

            //pop to the new root page
            await Navigation.PopToRootAsync();
            
        }

        public void UIUpdateSettings()
        {
            UIEntryURL.Text = Settings.HostValue;
            UIEntryTitle.Text = Settings.HostTitle;
        }

    }
}