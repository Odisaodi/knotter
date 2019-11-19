using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

using QuickType;
using Octane.Xamarin.Forms.VideoPlayer;

namespace Knotter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaPage : ContentPage
    {
        private Booru Search;
        private CPost Post;
        private int Index = 0;
        private static readonly double upperBounds = Booru.ScreenHeight / 3;
        private static readonly double LowerBounds = Booru.ScreenHeight - upperBounds;

        public MediaPage(object sender, int index)
        {
            InitializeComponent();

            Search = (Booru)sender;
            Index = index;
            Post = Search.posts[Index];

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

            UIFavoriteClick.Clicked +=
                (s, e) => UIVoteClicked();

            UpdateMedia();
        }

        public void UpdateMedia()
        {
            Indicate(true);
            FetchMediaContent();
            UpdateSlidingPane();
            Indicate(false);
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

        //swipe action
        private async void GetNextPost()
        {
            //if Next_post touches the threshhold we have to update the prefetch
            if ((Search.posts.Count - Index) < Search.ResultsPerPage)
            {
                await Search.UpdateCacheAsync();
            }
            Index++;
            Post = Search.posts[Index];

            UpdateMedia();
        }

        private void FetchMediaContent()
        {
            ExternalButton.Clicked += (s, e) =>
            {
                Launcher.OpenAsync(new Uri(Settings.HostValue + "/post/show/" + Post.Id));
            };
            ExternalButton.Text = $"View at {Settings.NameValue}";

            UIMediaContent.Children.Clear();
            UIMediaContent.Children.Add( MediaTemplate() );
        }

        //swipe action
        private void GetLastPost()
        {
            if (Index <= 0)
            {
                DisplayAlert("owo no", "End of List", "ok");
                return;
            }

            Index--;
            Post = Search.posts[Index];

            UpdateMedia();
        }

        //voting
        public bool state;
        private async void UIVoteClicked()//(object sender, EventArgs e)
        {//note: upvoting is different than favorating

            state = !state;
            var vote = (state) ? 1 : -1;
            int ret = await UserActions.VoteAsync(Post.Id, vote);

            switch (ret)
            {
                case -1:
                    //notify user of success
                    UIFavoriteClick.Source = "downvote.png";
                    break;

                case 0:
                    //failure types
                    //"already voted You have already voted for this post."
                    //"invalid score You have supplied an invalid score."

                    //neutral icon
                    UIFavoriteClick.Source = "vote.png";

                    //turn notification on
                    UINotification.IsVisible = true;

                    //turn notification off after 2 seconds
                    Device.StartTimer(TimeSpan.FromSeconds(2),
                        () => { return UINotification.IsVisible = false; }
                    );

                    break;

                case 1:
                    //notify user of success
                    UIFavoriteClick.Source = "upvote.png";
                    break;
            }
        }

        //swipe action
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

        //templates
        public ContentView MediaTemplate()
        {
            dynamic media;
            switch (Post.FileExt)
            {
                case "gif":
                    media = WebVeiwTemplate(Post.FileUrl.AbsoluteUri);
                    break;

                case "webm"://Webm's used to work but dont anymore, hmm
                    media = new VideoPlayer
                    {
                        Source = VideoSource.FromUri(Post.FileUrl),
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
                        Source = ImageSource.FromUri(Post.SampleUrl),
                        //WidthRequest = Booru.ScreenWidth,
                    };
                    break;

                default:
                    media = new Label
                    {
                        Text = $"Media format not supported: {Post.FileExt}",
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


    }

}
