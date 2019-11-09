using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace Knotter
{
    //an extention of MainPage.xaml.cs
    public partial class ResultsPage : ContentPage
    {
        public void IsLoading(bool state = true)
        {
            if (state){
                Activity.IsVisible = true;
            }else{
                Activity.IsVisible = false;
            }
        }
        public void AddColumn(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                //GridItems = (Grid)Application.Current.MainPage.FindByName("GridItems");
                Results.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(Booru.preview_width) });
            }
        }
        public void AddTiles()
        {
            int remainder = (Booru.Results.Count - Booru.Tiles.Count);

            //return count unless remainder is less, then return remainder.       
            int count = (remainder < Booru.ResultsPerPage) ? remainder : Booru.ResultsPerPage;

            for (int i = 0; i < count; i++)
            {
                var Tile = GetNextThumbnail();

                int x = Booru.Tiles.Count % Booru.ColCount;
                int y = Booru.Tiles.Count / Booru.ColCount;
                //Add to UI
                Results.Children.Add(Tile, x, y);

                //add to List (Must happen last, as it incriments Tiles.Count)
                Booru.Tiles.Add(Tile);
            }
        }

        public Grid GetNextThumbnail() //thumbnails
        {
            if (Booru.Results.Count <= 0) //verify we have at least 1 data to use
                return null;

            //aka the "next" item
            int index = Booru.Tiles.Count;
            var post = Booru.Results[index];

            //return Task.Run(() => 
            //{
            Image media = new Image
            {
                //download/assign image: is this an Async function, does it block?
                Source = ImageSource.FromUri(post.PreviewUrl),
                //Fill the thumbnail 
                Aspect = Aspect.AspectFill,
                BackgroundColor = Color.FromHex("#7F808080"),
            };

            Label label = new Label
            {
                Text = $"[↑↓:{post.Score}] [♥:{post.FavCount}]",
                //center the text
                HorizontalTextAlignment = TextAlignment.Center,
                //semi transparent black background
                BackgroundColor = Color.FromHex("#7F000000"),//try BF
                //display at bottom of image
                VerticalOptions = LayoutOptions.EndAndExpand,
                //color
                TextColor = Color.Fuchsia,
            };

            Label isAnimated = new Label
            {
                Text = "Animated",
                IsVisible = false,
                HorizontalTextAlignment = TextAlignment.End,
                TextColor = Color.Chartreuse,
            };
  
            if (post.FileExt == "gif")
            {
                isAnimated.IsVisible = true;
            }
            
            //Create the Element to Display
            Grid Tile = new Grid
            {
                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(Booru.preview_width) }
                },
                RowDefinitions = {
                    new RowDefinition { Height = new GridLength(Booru.preview_height) }
                },

                Children = {
                    media,//note layers so adding media last covers other elements
                    isAnimated,
                    label,
                },
            };//Tile

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => Navigation.PushAsync( new MediaPage(index) );
            //
            Tile.GestureRecognizers.Add(tap);

            return Tile;  
        }
    }
}