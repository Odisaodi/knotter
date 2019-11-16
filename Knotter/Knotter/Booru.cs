using QuickType;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Knotter
{
    static partial class Booru
    {
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
            Connection.Connect(host);//assume success

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



        static public async Task<int> UpdateCacheAsync()
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

            var response = await Connection.GetResponse("/post/index.json", Connection.Arguments);//.ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync();

            if (Deserialize.TryParse(json, out List<CPost> posts))
            {
                //var value = await Connection.FetchResults<List<CPost>>("/post/index.json", Connection.Arguments);

                if (posts != null)
                    Booru.Results.AddRange(posts);
                else
                    remainder = 0;

                _isGettingNewItems = false;

                return remainder;
            }
            //an error occured

            if (Deserialize.TryParse(json, out ReturnStatus ret))
            {
                //to do: ReturnStatus.
            }

            //uh oh
            return 0;
        }
    }
}