﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Knotter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForumPage : ContentPage
    {
        public ForumPage()
        {
            InitializeComponent();
        }

        public async void GetForumPosts(double? parent, int page, int limit = 25)
        {
            var args = new Dictionary<string, string>
            {
                ["parent_id"] = parent.ToString(),
                ["page"] = page.ToString(),
                ["limit "] = limit.ToString(),
            };

            var response = await Connection.GetResponse("/forum/list.json", args);

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                if (QuickType.Deserialize.TryParse(json, out List<QuickType.ForumPost> output))
                {
                    foreach (var post in output)
                    {
                        var post_template = CreateTemplate(post);
                        UITopicList.Children.Add(post_template);
                    }
                }
            }
            catch (Newtonsoft.Json.JsonSerializationException)
            {
                //todo
            }
        }

        public static string Truncate(string s, int n)
        {
            return new string(s.Take(n).ToArray());
        }
        public static View CreateTemplate(QuickType.ForumPost post)
        {
            var truncated_body = Truncate(post.Body, 255);
            //Topic
            return new StackLayout//x:Name="GeneratedTopic"
            {
                BackgroundColor = Color.FromHex("#101B2D"),
                Margin = 2,
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new StackLayout 
                    {//x:Name="none"
                        Margin = 2,
                        Children =
                        {
                            new Label //x:Name="TopicTitle"
                            {
                                Padding = 5,
                                FontSize = 15,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = Color.FromHex("#B4C7D9"),
                                BackgroundColor = Color.FromHex("#24395C"),
                            },

                            new Label //x:Name="TopicContent"
                            {
                                Text = truncated_body + " ...",
                                Padding = 10,
                                TextColor = Color.FromHex("#b4c7d9"),
                            },

                            new StackLayout{
                                Margin = 2,
                                HeightRequest = 40,
                                Orientation = StackOrientation.Horizontal,
                                BackgroundColor = Color.FromHex("#090E16"),                                
                                Children =
                                {
                                     new Button {
                                         Text="↑",
                                         WidthRequest=64,
                                     },
                                     new Button {
                                         Text="↑",
                                         WidthRequest=64,
                                     },
                                     new Button {
                                         Text="Comments",
                                         WidthRequest=64,
                                         HorizontalOptions = LayoutOptions.FillAndExpand,
                                     }
                                },
                            },
                        },
                    },
                },
            };
        }
    }

    // <StackLayout x:Name="GeneratedTopic" BackgroundColor="#101B2D" Margin="2" Orientation="Horizontal">
    //    <!-- #152f56 -->
    //    <StackLayout  Margin="2">
    //        <!-- #152f56 -->
    //        <Label
    //            x:Name="TopicTitle"
    //            TextColor="#b4c7d9"
    //            BackgroundColor="#24395C"
    //            Text="Suggestion: Sorting by video length what if this is wayyydeo length what if this is wayyydeo length what if this is wayyydeo length what if this is wayyydeo length what if this is wayyydeo length what if this is wayyyyy long?"
    //            FontAttributes="Bold"
    //            Padding="5"
    //            FontSize="15"/>

    //        <Label
    //            x:Name="TopicContent"
    //            TextColor="#b4c7d9"
    //            Padding="10"
    //            Text="This thread is for bringing old alias/implication suggestions, that are straightforward and shouldn't This thread is for bringing old alias/implication suggestions, that are straightforward and shouldn't "
    //            >
    //        </Label>

    //        <StackLayout Orientation="Horizontal" HeightRequest="40" BackgroundColor="#090E16" Margin="2">
    //            <Button Grid.Column="0" Text="75" WidthRequest="64" IsEnabled="False"/>
    //            <Button Grid.Column="1" Text="75" WidthRequest="64" IsEnabled="False"/>
    //            <Button Text="Read" HorizontalOptions="FillAndExpand"/>
    //        </StackLayout>


    //    </StackLayout>


    //</StackLayout>

}