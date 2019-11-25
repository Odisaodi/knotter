using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Knotter
{
    public partial class MediaPage : ContentPage
    {
        
        private double StartDragY;
        public void OnFloatingMenuPan(object sender, PanUpdatedEventArgs e)
        {
            UIContentLayers.RaiseChild(UISlidingPane);

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    StartDragY = UISlidingPane.TranslationY;
                    break;

                case GestureStatus.Running:
                    UISlidingPane.TranslationY = StartDragY + e.TotalY;
                    break;

                case GestureStatus.Completed:
                    double final = UISlidingPane.TranslationY;
                    double dist = final - StartDragY; 

                    if (Math.Abs(dist) < 50)//minimum drag distance 
                        return;//ignore smaller movenents

                    switch (dist > 0) //direction
                    {
                        case true:
                            UISlidingPane.TranslateTo(0, LowerBounds);//down
                            UIButtonCollapse.IsVisible = false;
                            break;

                        case false:
                            UISlidingPane.TranslateTo(0, 0);//top
                            break;
                    }
                    break;//GestureStatus.Completed:
            }
        }

        public void UpdateActionBar()
        {
            //UIContentLayers.RaiseChild(UIActionBar);
            //reset the buttons state
            //to do: load state when swiping back to a previous image (already voted)
            UIFavoriteClicked.Source = "stargrey.png";
            UIVoteDown.Source = "votedowngrey.png";
            UIVoteUp.Source = "voteupgrey.png";

            ExternalButton.Text = $"View on {Settings.HostTitle}";
            //
            var ExternamLink = Settings.HostValue + $"/post/show/{Post.Id}";
            ExternalButton.Clicked += async (s, e) => { await Launcher.OpenAsync(ExternamLink); };

            UIFavoriteClicked.Clicked += (s, e) => { HeartClicked(); };
            UIVoteDown.Clicked += (s, e) => { VoteClicked(-1); };
            UIVoteUp.Clicked += (s, e) => { VoteClicked(1); };
        }

        private View CreateActionBar()
        {
            var UIExternalLink = new Button
            {
                Text = "View on web",
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            UIExternalLink.Clicked += (s, e) => { };

            //
            var UIFavoriteClicked = new ImageButton
            {
                Source = "stargrey.png",
                BackgroundColor = Color.Transparent,
            };
            UIFavoriteClicked.Clicked += (s, e) => { HeartClicked(); };

            //
            var UIVoteDown = new ImageButton
            {
                Source = "votedowngrey.png",
                BackgroundColor = Color.Transparent,
            };
            UIVoteDown.Clicked += (s, e) => { VoteClicked(-1); };

            var UIVoteUp = new ImageButton
            {
                Source = "voteupgrey.png",
                BackgroundColor = Color.Transparent,
            };
            UIVoteUp.Clicked += (s, e) => { VoteClicked(1); };

            //
            var UIActionBar = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = Color.FromHex("7F000000"),
                Children =
                {
                    UIExternalLink, UIFavoriteClicked, UIVoteDown, UIVoteUp, 
                }
            };
            
            return UIActionBar;
            //throw new NotImplementedException();
        }

        public View UpdateTagLibrary(QuickType.Tags tags)
        {
            var root = new TableRoot();
            foreach (System.Reflection.PropertyInfo category in tags.GetType().GetProperties())
            {
                var tableSection = new TableSection(category.Name);

                foreach (var tag in (List<string>)category.GetValue(tags))
                {
                    ViewCell cell = new ViewCell {
                        View = new StackLayout {
                            Children = {
                                new Button
                                {
                                    Text = tag.ToString(),
                                    Command = new Command(
                                        execute: () =>
                                        {
                                            Navigation.PushAsync(
                                                    new ResultsPage(tag.ToString())
                                            );
                                        }
                                    ),
                                },
                            },
                        },
                    };
                    tableSection.Add(cell);
                }
                root.Add(tableSection);
            }

            var content = new TableView {
                Root = root,
                BackgroundColor = Color.FromHex("#EE000000") 
            };
            return content;
        }

        private void Collapse_clicked()
        {
            UIButtonCollapse.IsVisible = false;
            UISlidingPane.TranslateTo(0, LowerBounds);
        }
    }
}

//return new TableView
//{
//    Root = new TableRoot
//    {
//        new TableSection("one")
//        {
//          // TableSection constructor takes title as an optional parameter
//          new SwitchCell { Text = "New Voice Mail" },
//          new SwitchCell { Text = "New Mail", On = true }
//        },
//        new TableSection("two")
//        {
//          // TableSection constructor takes title as an optional parameter
//          new SwitchCell { Text = "New Voice Mail" },
//          new SwitchCell { Text = "New Mail", On = true }
//        }
//    },
//    Intent = TableIntent.Settings
//};
