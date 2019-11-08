using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.ComponentModel;
using QuickType;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace Knotter
{
    static partial class Booru
    { 
        static public Dictionary<string, string> Arguments { get; set; }
        //-----------
        static private HttpClient Client;
        
        static public void Connect(string host = "https://e926.net")// / Knotter
        {
            Client = new HttpClient
            {
                BaseAddress = new Uri(host),
            };
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Knot/1.0 (by do6kids9 on Knotter)");//MyProject/1.0 (by username on Knotter)
        }
        
        static private async Task<string> FetchResultStringAsync(string page, Dictionary<string, string> arguments, bool IsPost = false)
        {
            HttpMethod Method = IsPost ? HttpMethod.Post : HttpMethod.Get;

            var request = new HttpRequestMessage(Method, page)
            {
                Content = new FormUrlEncodedContent(arguments)
            };

            HttpResponseMessage httpResponse = await Client.SendAsync(request);//.ConfigureAwait(false);

            if (httpResponse.IsSuccessStatusCode)
                return await httpResponse.Content.ReadAsStringAsync();//.ConfigureAwait(false);

            return null;
        }

        static private async Task<T> FetchResults<T>(string page, Dictionary<String, String> arguments)
        {//Get Response as String

            //Booru.PageID = { {POST, "/post/create"}, {GET, "/Post/search"} }
            string response = await FetchResultStringAsync(page, arguments);
            //Deserialize response to Type
            return (T)Deserialize.FromJson<T>(response);
        }

        //-------------
        static public int preview_width = 110;
        static public int preview_height = 150;
        static private DisplayInfo DisplayInfo;
        
        static public int ColCount;
        static public int RowCount;
        static public int ResultsPerPage;
        static public int ResultsPerRequest;

        static public int ScreenWidth;
        static public int ScreenHeight;
        static public List<CPost> Results;//List<Post_Item>
        static public List<Grid> Tiles;
        public static bool _isGettingNewItems;

        static public void Initialize(string host)
        {
            Connect(host);//assume success

            //results downloaded
            Results = new List<CPost>();

            //results displayed
            Tiles = new List<Grid>();

            //calculate screen size;
            DisplayInfo = DeviceDisplay.MainDisplayInfo;
            ScreenWidth = (int)(DisplayInfo.Width / DisplayInfo.Density);
            ScreenHeight = (int)(DisplayInfo.Height / DisplayInfo.Density);

            //set col/row/etc
            ColCount = ColsPerPage();
            RowCount = RowsPerPage();

            ResultsPerPage = ColCount * RowCount;
            ResultsPerRequest = ResultsPerPage * 2;//aka always have two pages in memory.
        }

        static public int ColsPerPage()
        {
            return (int)ScreenWidth / (preview_width);// / (int)App.ScreenInfo.Density);
        }

        static public int RowsPerPage()
        {
            return (int)ScreenHeight / (preview_height);// / (int)App.ScreenInfo.Density);
        }

        static public async Task<int> UpdateCacheAsync(Dictionary<string, string> arguments)
        {
            int remainder = (Booru.Results.Count - Booru.Tiles.Count);

            //do we even need to update? 
            if (remainder > Booru.ResultsPerPage)
                return remainder;

            //if its already updating?
            if (_isGettingNewItems)
                return remainder;

            //else update
            _isGettingNewItems = true;

            var value = await FetchResults<List<CPost>>("/post/index.json", arguments);

            if (value != null)
                Booru.Results.AddRange(value);
            else
                remainder = 0;

            _isGettingNewItems = false;
         
            return remainder;
        }


        //static public async Task FetchAndAppendResults()
        //{
        //    int remaining = (Results.Count - Tiles.Count);

        //    //if we have less than 1 page of results in memory, add more.
        //    if (remaining < ResultsPerPage)
        //    {
        //        List<CPost> Results = await FetchResults<List<CPost>>("/post/index.json", Arguments).ConfigureAwait(false);
        //        Booru.Results.AddRange(Results);
        //    }

        //}
        //
    }
}