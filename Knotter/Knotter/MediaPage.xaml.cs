using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

using QuickType;
using Octane.Xamarin.Forms.VideoPlayer;
using System.Threading.Tasks;

namespace Knotter
{
    internal static class PageTemplate
    {

        public static readonly StackLayout UIActionBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            BackgroundColor = Color.FromHex("#7F000000"),
            HorizontalOptions = LayoutOptions.FillAndExpand,
            Children = {

            }
        };

        public static readonly View UIContentLayers = new Grid
        {
            Children = {
                UIActionBar,
            }
        };

    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaPage : ContentPage
    {
        private Booru Search;
        private CPost Post;
        private int Index = 0;
        private static readonly double upperBounds = Booru.ScreenHeight / 4;
        private static readonly double LowerBounds = Booru.ScreenHeight - upperBounds;

        public MediaPage(object sender, int index)
        {
            InitializeComponent();

            Search = (Booru)sender;
            Index = index;
            Post = Search.posts[Index];

            //Add Gesture
            var floatingMenuPan = new PanGestureRecognizer();
            floatingMenuPan.PanUpdated += OnFloatingMenuPan;
            UIContentLayers.GestureRecognizers.Add(floatingMenuPan);

            //Add Gesture
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnMediaPan;
            UIContentLayers.GestureRecognizers.Add(panGesture);

            ReloadMediaPage();
        }

        public void ReloadMediaPage()
        {
            //remember it's on a grid
            Indicate();

            //layer 0 (bottom)
            UpdateMediaLayer();

            //layer 1 (center)
            UpdateActionBar();

            //layer 2 (center)
            UpdateNotification("loading", Color.LightGreen, 2);

            UpdateFloatingMenu();

            //UIContentLayers.Children.Add(UISlidingPane);
            Indicate(false);
        }

        private void UpdateFloatingMenu()
        {
            UISlidingPane.Children.Clear();

            //Caption
            var UISliderCaption = new Label
            {
                Text = "↑ Tags ↓", //↑↓;
                HeightRequest = 50,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                BackgroundColor = Color.Accent,
            };

            UISlidingPane.Children.Add( UISliderCaption );
            //
                TagLibrary.Children.Clear();
                TagLibrary.Children.Add( UpdateTagLibrary(Post.Tags) );
                UISlidingPane.Children.Add(TagLibrary);
            //
            UISlidingPane.TranslateTo(0, LowerBounds);
        }

        private void UpdateNotification(string text, Color color, double seconds)//BackgroundColor="IndianRed"
        {
            var UINotification = new Label
            {
                Text = "Progress...",
                BackgroundColor = Color.IndianRed,
                VerticalOptions = LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Center,
            };

            UIContentLayers.Children.Add(UINotification);

            Task.Run(() => { 
                Device.StartTimer(TimeSpan.FromSeconds(2), () => { 
                    UINotification.IsVisible = false;
                    return true;
                });            
            });

            
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
                UIContentLayers.Children.Add(activityIndicator);
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

            ReloadMediaPage();
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

            ReloadMediaPage();
        }


        public bool _liked;
        private async void HeartClicked()//(object sender, EventArgs e)
        {//note: upvoting is different than favorating
            _liked = !_liked;
            int ret = await UserActions.Favourite(Post.Id, _liked);

            if (_liked & (ret == 1))
            {//if the action was a success and the current state is "like"
                
                UIFavoriteClicked.Source = "star.png";
                return;
            }
            else if (_liked & (ret == 0))
            {//if the action was a success and the current state is "dislike"
                //turn notification on
                UINotification.IsVisible = true;
                //turn notification off after 2 seconds
                Device.StartTimer(TimeSpan.FromSeconds(2),
                    () => { return UINotification.IsVisible = false; }
                );
            }
            else
                UIFavoriteClicked.Source = "stargrey.png";
        }

        //voting
        //public bool state;
        private async void VoteClicked(int vote)//(object sender, EventArgs e)
        {
            //int ret = await UserActions.Favourite(Post.Id, state);
            bool ret = await UserActions.VoteAsync(Post.Id, vote);
            if ( ret )
            {
                switch (vote)
                {
                    case -1:
                        UIVoteUp.Source = "voteupgrey.png";
                        UIVoteDown.Source = "votedown.png";
                        break;

                    case 0:
                        UIVoteUp.Source = "voteupgrey.png";
                        UIVoteDown.Source = "votedowngrey.png";
                        break;

                    case 1:
                        UIVoteUp.Source = "voteup.png";
                        UIVoteDown.Source = "votedowngrey.png";
                        break;
                }
            }else{
                UINotification.IsVisible = true;
                //turn notification off after 2 seconds
                Device.StartTimer(TimeSpan.FromSeconds(2),
                    () => { return UINotification.IsVisible = false; }
                );
            }
        }

        //swipe action
        private void OnMediaPan(object sender, PanUpdatedEventArgs e)
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
        public void UpdateMediaLayer()
        {
            UIMediaContent.Children.Clear();

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
                        WidthRequest = Booru.ScreenWidth,
                    };
                    break;

                case "png":
                case "jpg":
                case "bmp":
                    media = new Image{ 
                        Source = ImageSource.FromUri(Post.SampleUrl),
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        //HeightRequest = Booru.ScreenHeight,
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

            var result = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,

                Children = { media },
            };

            UIMediaContent.Children.Add(result);
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
