using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Knotter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultsPage : ContentPage
    {
        private Booru CurrentSearch;

        public ResultsPage(string Tags)
        {
            InitializeComponent();

            //event handlers
            SearchTerms.Completed += Search_Completed;
            ScrollContent.Scrolled += OnScrolledAsync;
            //
            CurrentSearch = new Booru(Settings.HostValue);
            if (CurrentSearch.ColCount != Results.ColumnDefinitions.Count)
            {
                AddColumn(CurrentSearch.ColCount);
            }
            Results.VerticalOptions = LayoutOptions.StartAndExpand;
            Results.HorizontalOptions = LayoutOptions.CenterAndExpand;
            Results.Padding = 2;
            //initial search results
            SearchTerms.Text = Tags;
            SubmitSearchAsync(Tags);
        }

        private void Search_Completed(object sender, EventArgs e)
        {
            SubmitSearchAsync(((Entry)sender).Text);
        }

        private async void SubmitSearchAsync(string tags)
        {
            //notify the user we are thinking
            IsLoading();

            //clear the screen
            Results.Children.Clear();

            //Clear the memory
            CurrentSearch.tiles.Clear();
            CurrentSearch.posts.Clear();

            string last_id = null;
            if (CurrentSearch.posts.Count < 0)
                last_id = CurrentSearch.posts[CurrentSearch.posts.Count - 1].Id.ToString();

            Connection.Arguments = new Dictionary<string, string>
            {
                ["typed_tags"] = "true",
                ["limit"] = CurrentSearch.ResultsPerRequest.ToString(),
                ["tags"] = tags,//"rating:s ", 
            };
            if (last_id != null)
                Connection.Arguments["last_id"] = last_id;

            int count = await CurrentSearch.UpdateCacheAsync();//.ConfigureAwait(false);
            if(count > 0)
                AddTiles();
                //Add a page worth of Tiles to the UI
                //note 0 results may be caused by an invalid struct cast (aka tryParse() fails)
            //removie the activity indicator
            IsLoading(false);
        }

        private async void OnScrolledAsync(object sender, ScrolledEventArgs e)
        {
            ScrollView scroller = (ScrollView)sender;

            double threashold = (e.ScrollY + scroller.Height) + Booru.preview_height * 2;

            //if we touch the threshhold...
            if (threashold >= scroller.ContentSize.Height)
            {//threshhold == bottom of scrollveiw + height of one image (aka just before it's visible)
                debug.Text = "Fetching";

                if (CurrentSearch.posts.Count > 0)
                    Connection.Arguments["before_id"] = CurrentSearch.posts[CurrentSearch.posts.Count - 1].Id.ToString();

                //notify the user something is happening
                IsLoading();

                //return control to the parent thread (UI) until this await has completed
                await CurrentSearch.UpdateCacheAsync();//.ConfigureAwait(false);

                AddTiles();

                //removie the activity indicator
                IsLoading(false);

                //finished / resume
                debug.Text = "Fetch complete";
            }
        }
    }
}