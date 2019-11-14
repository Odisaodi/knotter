using Octane.Xamarin.Forms.VideoPlayer;
using QuickType;
using System;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Knotter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaPage : ContentPage
    {

        private int SelectedPostIndex = 0;
        private CPost SelectedPost;
        private static double LowerBounds;

        public MediaPage(int index)//List<object> posts, int index
        {
            InitializeComponent();

            LowerBounds = Booru.ScreenHeight - (Booru.ScreenHeight / 3);

            SelectedPostIndex = index;
            SelectedPost = Booru.Results[SelectedPostIndex];

            //add gesture to the MediaWindow
            var MediaWindowPan = new PanGestureRecognizer();
            MediaWindowPan.PanUpdated += OnMediaPanUpdated;
            UIContentLayers.GestureRecognizers.Add(MediaWindowPan);

            //Add Gesture to the SlidingMenu
            PanGestureRecognizer SlidingMenu = new PanGestureRecognizer();
            SlidingMenu.PanUpdated += OnSlidingMenuPanUpdated;
            UIContentLayers.GestureRecognizers.Add(SlidingMenu);

            //button event
            UIButtonCollapse.Clicked +=
                (s, e) => Collapse_clicked();

            Indicate(true);
            UpdateMediaContent();
            UpdateSlidingPane();
            Indicate(false);
        }

        private void Collapse_clicked()
        {
            UIButtonCollapse.IsVisible = false;

            UISlidingMenu.TranslateTo(0, LowerBounds);
        }

        private async void GetNextPost()
        {
            //if Next_post touches the threshhold we have to update the prefetch
            if ((Booru.Results.Count - SelectedPostIndex) < Booru.ResultsPerPage)
            {
                await Booru.UpdateCacheAsync();
            }
            SelectedPostIndex++;
            SelectedPost = Booru.Results[SelectedPostIndex];

            Indicate(true);
            UpdateMediaContent();
            UpdateSlidingPane();
            Indicate(false);
        }

        private void GetLastPost()
        {
            if (SelectedPostIndex <= 0)
            {
                DisplayAlert("owo nowo", "End of List", "ok");
                return;
            }

            SelectedPostIndex--;
            SelectedPost = Booru.Results[SelectedPostIndex];

            Indicate(true);
            UpdateMediaContent();
            UpdateSlidingPane();
            Indicate(false);
        }

        private void UpdateMediaContent()
        {
            ExternalButton.Clicked += (s, e) =>
            {
                Launcher.OpenAsync(new Uri(Settings.HostValue + "/post/show/" + SelectedPost.Id));
            };
            ExternalButton.Text = $"View at {Settings.NameValue}";

            UIMediaContent.Children.Clear();

            var media = GetMediaContent();

            UIMediaContent.Children.Add(media);
        }

        private void UpdateSlidingPane()
        {
            UISlidingMenu.Children.Clear();

            //UISliderCaption
            var UISliderCaption = new Label
            {
                Text = "↑ Tags ↑", //↑↓;
                HeightRequest = 50,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                BackgroundColor = Color.Accent,
            };
            UISlidingMenu.Children.Add(UISliderCaption);


            //
            var TagStack = new StackLayout
            {
                Padding = 10,
            };

            foreach (var tag in SelectedPost.Tags.Split(' ').ToList())
            {

                Button button = new Button
                {
                    Text = tag.ToString(),
                    HorizontalOptions = LayoutOptions.Start,
                };

                button.Clicked += (s, e) =>
                {
                    Navigation.PushAsync(
                        new ResultsPage(tag.ToString())
                    );
                };

                TagStack.Children.Add(button);
            }
            //Add tagstack to pane
            UISlidingMenu.Children.Add(TagStack);

            //the lower 1/4th of the screen
            UISlidingMenu.TranslateTo(0, LowerBounds);
        }

        ActivityIndicator activityIndicator;
        public void Indicate(bool state = true)
        {
            if (activityIndicator == null)
            {
                activityIndicator = new ActivityIndicator
                {
                    IsRunning = true,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                };
            }
            activityIndicator.IsRunning = state;
        }

        private double StartDragY;
        public void OnSlidingMenuPanUpdated(object sender, PanUpdatedEventArgs e)
        {

            double delta = (SlidingWindow.Height - UISlidingMenu.Height);

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    StartDragY = UISlidingMenu.TranslationY;
                    break;

                case GestureStatus.Running:
                    UISlidingMenu.TranslationY = StartDragY + e.TotalY;

                    //display a "drop/close" button if we scroll the content
                    if (UISlidingMenu.TranslationY < LowerBounds)
                        UIButtonCollapse.IsVisible = true;
                    else
                        UIButtonCollapse.IsVisible = false;
                    break;

                case GestureStatus.Completed:

                    if (UISlidingMenu.TranslationY > LowerBounds)
                    {
                        UISlidingMenu.TranslateTo(0, LowerBounds);
                        break;
                    }

                    if (UISlidingMenu.TranslationY < delta)
                    {
                        UISlidingMenu.TranslateTo(0, delta);
                        break;
                    }
                    break;
            }
            //debug.Text = UISlidingPane.TranslationY.ToString();
        }

        private void OnMediaPanUpdated(object sender, PanUpdatedEventArgs e)
        {

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    break;

                case GestureStatus.Running:

                    UIMediaContent.TranslationX = e.TotalX;
                    break;

                case GestureStatus.Completed:

                    double x = UIMediaContent.TranslationX;

                    double Threshhold = (Booru.ScreenWidth / 3); //aka 1/3 of the screen
                    if (Threshhold < Math.Abs(x))
                    {
                        //A new Image Will Load, Clear the Current one
                        UIMediaContent.Children.Clear();

                        //place on oppsite side (so the next one slides in)
                        //[last] <- [current] <- [next]
                        //   ^----------------------^

                        bool Direction = (x < 0) ? true : false;

                        switch (Direction)
                        {
                            case true:// swipe right ->
                                UIMediaContent.TranslationX = Booru.ScreenWidth;
                                GetNextPost();
                                break;

                            case false://swipe left <-
                                UIMediaContent.TranslationX = -Booru.ScreenWidth;
                                GetLastPost();
                                break;
                        }
                    }
                    UIMediaContent.TranslateTo(0, 0);
                    break;
            }
        }

        public ContentView GetMediaContent()
        {
            dynamic media;
            switch (SelectedPost.FileExt)
            {
                case "gif":
                    media = WebVeiwTemplate(SelectedPost.FileUrl.AbsoluteUri);
                    break;

                case "webm"://Webm's used to work but dont anymore, hmm
                    media = new VideoPlayer
                    {
                        Source = VideoSource.FromUri(SelectedPost.FileUrl),
                        Volume = 0,//Default Mute
                    };
                    break;

                case "png":
                case "jpg":
                case "bmp":
                    media = new Image
                    {
                        //preview, although smaller, loads much faster than absurd_res and saves data
                        //user can still view full image on web
                        Source = ImageSource.FromUri(SelectedPost.SampleUrl),
                        //WidthRequest = Booru.ScreenWidth,
                    };
                    break;

                default:
                    media = new Label
                    {
                        Text = $"Media format not supported: {SelectedPost.FileExt}",
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = Color.Chartreuse,
                    };
                    break;
            }

            return new ContentView
            {
                Content = media,
                HeightRequest = Booru.ScreenHeight,
                WidthRequest = Booru.ScreenWidth,
            };
        }

        public static View WebVeiwTemplate(string imageurl) //post.FileUrl.AbsoluteUri
        {
            var webveiw = new WebView
            {
                WidthRequest = Booru.ScreenWidth,
                HeightRequest = Booru.ScreenHeight,

                Source = new HtmlWebViewSource
                {
                    Html =
                    "<!DOCTYPE html>" +
                    "<html>" +
                        "<head>" +
                        "<meta name='viewport' content='target-densityDpi=device-dpi'/>" +
                        "<style> " +
                            "img {text-align: center; position: absolute; margin: auto; top: 0; right: 0; bottom: 0; left: 0;}" +
                            "body {background-color: #262626;}" +
                        " </style>" +
                        "</head>" +
                        $"<body><img src=\"{imageurl}\" width=\"{Booru.ScreenWidth}\" ></body>" +
                    "</html>"
                },
            };

            /* 
            * note: 
            * WebView's dont accept touch gestures (for some reason?)
            * so a transparent top layer (frame) is required so we can still swipe
            */

            return new Frame
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                Content = webveiw,
            };
        }

        public bool state;
        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            state = !state;
            if (state)
                UIFavoriteClick.Source = "heart_small.png";
            else
                UIFavoriteClick.Source = "heart_large.png";
        }
    }

}
