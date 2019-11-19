using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Knotter
{
    public partial class Application : Xamarin.Forms.Application
    {

        public Application()
        {
            InitializeComponent();

            //this initializes Booru
            VersionTracking.Track();
            //Settings.LoadUserSettings();

            //this allows us to use page navigation
            MainPage = new NavigationPage(new MainPage()); //MainPage = new MainPage();
        }

        void OnRotate(Object sender, DisplayInfoChangedEventArgs e)
        {
            //Booru.ColCount = Booru.ScreenWidth / Booru.preview_height;
            //MainPage.ColCount = ScreenWidth / (preview_size);
            //change column count ??
        }

        protected override void OnStart()
        {
            // Handle when your app starts

        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
