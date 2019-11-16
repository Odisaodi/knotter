using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin;

namespace Knotter
{
    class Tagging
    {
    }

    public class PageTypeGroup : List<PageModel>
    {
        public string Title { get; set; }
        public string ShortName { get; set; } //will be used for jump lists
        public string Subtitle { get; set; }
        private PageTypeGroup(string title, string shortName)
        {
            Title = title;
            ShortName = shortName;
        }

        public static IList<PageTypeGroup> All { private set; get; }

        static PageTypeGroup()
        {
            List<PageTypeGroup> Groups = new List<PageTypeGroup> {
            new PageTypeGroup ("Alpha", "A"){
                new PageModel("Amelia", "Cedar", new switchCellPage(),""),
                new PageModel("Alfie", "Spruce", new switchCellPage(), "grapefruit.jpg"),
                new PageModel("Ava", "Pine", new switchCellPage(), "grapefruit.jpg"),
                new PageModel("Archie", "Maple", new switchCellPage(), "grapefruit.jpg")
            },
            new PageTypeGroup ("Bravo", "B"){
                new PageModel("Brooke", "Lumia", new switchCellPage(),""),
                new PageModel("Bobby", "Xperia", new switchCellPage(), "grapefruit.jpg"),
                new PageModel("Bella", "Desire", new switchCellPage(), "grapefruit.jpg"),
                new PageModel("Ben", "Chocolate", new switchCellPage(), "grapefruit.jpg")
            }
        };
            All = Groups; //set the publicly accessible list
        }
    }

}

//<? xml version="1.0" encoding="UTF-8"?>
//<ContentPage xmlns = "http://xamarin.com/schemas/2014/forms"
//             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
//             x:Class="DemoListView.GroupingViewPage"
//    <ContentPage.Content>
//        <ListView x:Name="GroupedView"
//                   GroupDisplayBinding="{Binding Title}"
//                   GroupShortNameBinding="{Binding ShortName}"
//                   IsGroupingEnabled="true">
//            <ListView.ItemTemplate>
//                <DataTemplate>
//                    <TextCell Text = "{Binding Title}"
//                              Detail="{Binding Subtitle}" />
//                </DataTemplate>
//            </ListView.ItemTemplate>
//        </ListView>
//    </ContentPage.Content>
//</ContentPage>