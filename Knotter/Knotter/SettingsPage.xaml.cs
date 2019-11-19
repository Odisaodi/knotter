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
        public static string HostValue { 
            get { return Preferences.Get("HostValue", "https://e926.net"); } 
            set { HostValue = Preferences.Get("HostValue", value); } 
        }
        public static string NameValue { 
            get { return Preferences.Get("NameValue", "e926"); }
            set { Preferences.Set("NameValue", value); }
        }
        public static string Username {
            get { return Preferences.Get("Username", ""); }
            set { Preferences.Set("Username", value); }
        }
        public static string ApiKey {
            get { return Preferences.Get("ApiKey", ""); }
            set { Preferences.Set("ApiKey", value); } 
        }

        public static void LoadUserSettings()
        {
            //todo: username/apikey 
            Booru.Initialize(Settings.HostValue);
        }
        public static void UpdateUserSettings(string Host, string Name)
        {
            Settings.HostValue = Host;
            Settings.NameValue = Name;
            Booru.Initialize(HostValue);
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : TabbedPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            Settings.LoadUserSettings();

            //Display Settings
            UIEntryTitle.Text = Settings.NameValue;
            UIEntryURL.Text = Settings.HostValue;
            UIEntryUserame.Text = Settings.Username;
            //
            UIButtonUpdateHost.Clicked += UpdateSettings;
            UIButtonLogin.Clicked += LoginAsync;

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
            bool success = await UserActions.Login(UIEntryUserame.Text, UIEntryPassword.Text);
        }

        private async void CheckForUpdates(object sender, EventArgs e)
        {
            var check = await Connection.CheckVersion();
            if (check)
            {
                bool reply = await DisplayAlert("wuw whats this", "An Update is availible!", "Download", "Cancel");
                if (reply)
                {
                    await Launcher.OpenAsync(new Uri("https://github.com/keihoag/knotter"));
                }
            }
        }

        private async void UpdateSettings(object sender, EventArgs e)
        {
            Settings.UpdateUserSettings(UIEntryURL.Text, UIEntryTitle.Text);

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
            UIEntryURL.Text = Settings.HostValue;
            UIEntryTitle.Text = Settings.NameValue;
        }

    }
}