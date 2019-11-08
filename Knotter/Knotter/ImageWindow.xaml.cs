using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
//using Xamarians.MediaPlayer;
using Octane.Xamarin.Forms.VideoPlayer;
using System.Linq;
using System.Collections.Generic;


using QuickType;
using System.Threading.Tasks;

namespace Knotter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageWindow : ContentPage
    {
        //public MediaPlayer mp;
        public static int PostIndex = 0;
        public static CPost SelectedPost;

        //public ActivityIndicator Activity;

        public static Grid MediaContent = new Grid
        {
            RowDefinitions = {
                new RowDefinition {
                    Height = GridLength.Star
                }
            },
            ColumnDefinitions = {
                new ColumnDefinition {
                    Width = GridLength.Star
                }
            },
            VerticalOptions = LayoutOptions.FillAndExpand,
            Children = { /* media */},
        };

        public ImageWindow(int index)//List<object> posts, int index
        {
            InitializeComponent();
            PostIndex = index;
            SelectedPost = Booru.Results[PostIndex];

            //add gesture to the MediaWindow
            PanGestureRecognizer Panning = new PanGestureRecognizer();
            Panning.PanUpdated += OnImagePanUpdated;
            MediaContainer.GestureRecognizers.Add(Panning);

            //Add Gesture to the PullUp
            PanGestureRecognizer PullUp = new PanGestureRecognizer();
            PullUp.PanUpdated += OnPullUpUpdated;
            PullUpLayout.GestureRecognizers.Add(PullUp);

            //Indicate(true);

            DisplayContent(SelectedPost);

            UpdateTagList(SelectedPost);

            PullUpLayout.TranslationY = Booru.ScreenHeight - 150;
        }

        private void UpdateTagList(CPost post)
        {
            TagContent.Children.Clear();

            var taglist = post.Tags.Split(' ').ToList();

            foreach (var tag in taglist)
            {
                var term = new Frame
                {
                    Padding = 5,
                    CornerRadius = 10,
                    BackgroundColor = Color.FromHex("#BF808080"),
                    Content = new Label
                    {
                        Text = tag.ToString(),
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Color.Fuchsia,
                    }
                };

                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) => {
                    Navigation.PushAsync(
                        new ResultsPage(tag.ToString())
                    );
                };

                term.GestureRecognizers.Add(tap);

                TagContent.Children.Add(term);
            }
           // PullUpLayout.TranslationY = MainLayout.Height - 50;
        }

        ActivityIndicator activityIndicator;
        public void Indicate(bool state = true)
        {
            if (activityIndicator != null)
                MediaContainer.Children.Remove(activityIndicator);

            if (state)
            {
                activityIndicator = new ActivityIndicator { 
                    IsRunning = true, 
                    VerticalOptions = LayoutOptions.CenterAndExpand, 
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                };
                MediaContainer.Children.Add(activityIndicator);
            }
        }

        public void DisplayContent(CPost post)
        {
            MediaContainer.Children.Clear();

            Indicate(true);

            //add media
            MediaContent.Children.Add( GetMedia(post) );
            //Update Title Bar
            TitleBar.Text = $"Score: {(int)post.Score}; Favs: {(int)post.FavCount}; type {post.FileExt};";

            Indicate(false);

            MediaContainer.Children.Add(MediaContent);
        }

        private void OnPullUpUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    PullUpLayout.TranslationY = e.TotalY;
                    break;

                case GestureStatus.Completed:
                    debug.Text = PullUpLayout.TranslationY.ToString();
                    break;
            }
        }

        private void OnImagePanUpdated(object sender, PanUpdatedEventArgs e)
        {
            //throw new NotImplementedException();

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    break;

                case GestureStatus.Running:
                    
                    MediaContent.TranslationX = e.TotalX;
                    MediaContent.TranslationY = e.TotalY / 4;
                    break;

                case GestureStatus.Completed:

                    double x = MediaContent.TranslationX;
                    //double y = MediaContainer.TranslationY;

                    double XMagnitude = Math.Abs(x);
                    //double YMagnitude = Math.Abs(y);

                    double Threshhold = (Booru.ScreenWidth / 2); //aka half of the screen
                    if (XMagnitude > Threshhold)
                    {
                        //A new Image Will Load, Clear the Current one
                        MediaContent.Children.Clear();

                        //place on oppsite side (so the next one slides in)
                        //[last] <- [current] <- [next]
                        //   v----------------------^

                        bool Direction = (x < 0) ? true : false;

                        switch (Direction)
                        {
                            case true:// swipe right ->
                                MediaContent.TranslationX = Booru.ScreenWidth;
                                GetNextPost();
                                break;

                            case false://swipe left <-
                                MediaContent.TranslationX = -Booru.ScreenWidth;
                                GetLastPost();
                                break;
                        }
                    }
                    MediaContent.TranslateTo(0, 0);

                    //if (YMagnitude > Threshhold)
                    //{
                    //    bool Direction = (x < 0) ? true : false;

                    //    switch (Direction)
                    //    {
                    //        case true:// swipe up ->
                    //            break;

                    //        case false://swipe down <-
                    //            break;
                    //    }
                    //}


                    //MediaContainer.TranslationY = 0;
                    break;
            }
        }

        private async void GetNextPost()
        {
            //if Next_post touches the threshhold we have to update the prefetch
            if ((Booru.Results.Count - PostIndex) < Booru.ResultsPerPage)
            {
                await Booru.UpdateCacheAsync(Booru.Arguments);
            }            
            PostIndex++;
            SelectedPost = Booru.Results[PostIndex];
            DisplayContent(SelectedPost);
        }

        private void GetLastPost()
        {
            if(PostIndex < 0)
                return;

            PostIndex--;
            SelectedPost = Booru.Results[PostIndex];
            DisplayContent(SelectedPost);
        }

        private bool StrCmp(string s1, string s2)
        {
            return (String.Compare(s1, s2) == 0);
        }
        private dynamic GetMedia(CPost post)
        {

            dynamic media;
            if (StrCmp(post.FileExt, "gif"))
            {
                media = WebVeiwTemplate(post.FileUrl.AbsoluteUri); ;
            }

            else if (StrCmp(post.FileExt, "webm") | StrCmp(post.FileExt, "swf"))
            {
                media = new VideoPlayer
                {
                    Source = post.FileUrl.AbsoluteUri,
                    WidthRequest = Booru.ScreenWidth,
                    Volume = 0,//no volume
                };
            }

            else
            {
                media = new Image
                {
                    Source = ImageSource.FromUri(post.SampleUrl),
                    WidthRequest = Booru.ScreenWidth,
                    HeightRequest = Booru.ScreenHeight,
                };
            }
            //return
            return media;
        }

        public static Grid WebVeiwTemplate(string imageurl) //post.FileUrl.AbsoluteUri
        {
            var webveiw = new WebView
            {
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
                //WidthRequest = Booru.ScreenWidth,
                //HeightRequest = Booru.ScreenHeight,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            /* 
             * note: 
             * WebView's dont accept touch gestures (for some reason?)
             * so a transparent top layer (frame) is required so we can still swipe
             */
            var frame = new Frame
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                Content = webveiw,
            };

            return new Grid
            {
                VerticalOptions=LayoutOptions.FillAndExpand,
                BackgroundColor=Color.Accent,
                Children =
                {
                    //webveiw,//webview blocks gestures
                    frame,//frame overlays webview and accepts Gestures
                },
            };
        }
    }
}
