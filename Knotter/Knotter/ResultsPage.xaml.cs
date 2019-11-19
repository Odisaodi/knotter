using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Knotter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultsPage : ContentPage
    {
        public ResultsPage(string Tags)
        {
            InitializeComponent();

            if (Booru.ColCount != Results.ColumnDefinitions.Count)
            {
                AddColumn(Booru.ColCount);
            }

            //event handlers
            Search.Completed += Search_Completed;
            ScrollContent.Scrolled += OnScrolledAsync;

            Results.VerticalOptions = LayoutOptions.StartAndExpand;
            Results.HorizontalOptions = LayoutOptions.CenterAndExpand;
            Results.Padding = 2;
            //initial search results
            Search.Text = Tags;
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
            Booru.Tiles.Clear();
            Booru.Results.Clear();

            string last_id = null;
            if (Booru.Results.Count < 0)
                last_id = Booru.Results[Booru.Results.Count - 1].Id.ToString();

            Connection.Arguments = new Dictionary<string, string>
            {
                ["typed_tags"] = "true",
                ["limit"] = Booru.ResultsPerRequest.ToString(),
                ["before_id"] = last_id,
                ["tags"] = tags,//"rating:s ", 
            };

            await Booru.UpdateCacheAsync();//.ConfigureAwait(false);

            //Add a page worth of Tiles to the UI
            //note 0 results may be caused by an invalid struct cast (aka tryParse() fails)
            AddTiles();

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

                if (Booru.Results.Count > 0)
                    Connection.Arguments["before_id"] = Booru.Results[Booru.Results.Count - 1].Id.ToString();

                //notify the user something is happening
                IsLoading();

                //return control to the parent thread (UI) until this await has completed
                await Booru.UpdateCacheAsync();//.ConfigureAwait(false);

                AddTiles();

                //removie the activity indicator
                IsLoading(false);

                //finished / resume
                debug.Text = "Fetch complete";
            }
        }
    }
}