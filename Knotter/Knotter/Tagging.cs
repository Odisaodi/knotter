﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;


namespace Knotter
{
    public partial class MediaPage : ContentPage
    {
        private double StartDragY;
        public void OnSlidingMenuPanUpdated(object sender, PanUpdatedEventArgs e)
        {
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
                    double distance = StartDragY - UISlidingMenu.TranslationY; //e.TotalY
                    if (Math.Abs(distance) < 50)
                        break;//ignore small movements

                    bool direction = (distance < 0) ? true : false;
                    switch (direction)
                    {
                        case true:
                            UISlidingMenu.TranslateTo(0, LowerBounds);//down
                            UIButtonCollapse.IsVisible = false;
                            break;

                        case false:
                            UISlidingMenu.TranslateTo(0, 0);//top
                            break;
                    }
                    break;
            }
        }

        private void UpdateSlidingPane()
        {
            UISlidingMenu.Children.Clear();

            //UISliderCaption
            var UISliderCaption = new Label
            {
                Text = "↑ Tags ↓", //↑↓;
                HeightRequest = 50,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                BackgroundColor = Color.Accent,
            };
            UISlidingMenu.Children.Add(UISliderCaption);

            var tagList = CreateTagList(SelectedPost.Tags);
            UISlidingMenu.Children.Add(tagList);

            UISlidingMenu.TranslateTo(0, LowerBounds);
            UIButtonCollapse.IsVisible = false;
        }

        public TableView CreateTagList(QuickType.Tags tags)
        {
            var root = new TableRoot();
            foreach (System.Reflection.PropertyInfo category in tags.GetType().GetProperties())
            {
                var tableSection = new TableSection(category.Name);
                List<string> taglist = (List<string>)category.GetValue(tags);

                foreach (var tag in taglist)
                {
                    Button button = new Button
                    {
                        Text = tag.ToString(),
                    };

                    button.Clicked += (s, e) => 
                    {
                        Navigation.PushAsync(
                            new ResultsPage( tag.ToString() )
                        );
                    };

                    ViewCell cell = new ViewCell {
                        View = new StackLayout {
                            Children = {
                                button,
                            },
                        },
                    };
                    tableSection.Add(cell);
                }
                root.Add(tableSection);
            }
            return new TableView { Root = root };
        }
        private void Collapse_clicked()
        {
            UIButtonCollapse.IsVisible = false;
            UISlidingMenu.TranslateTo(0, LowerBounds);
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