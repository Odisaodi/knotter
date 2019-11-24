using QuickType;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin;

namespace Knotter
{
    partial class Booru
    {
        static public int ScreenWidth;
        static public int ScreenHeight;
        static private bool _isGettingNewItems;      

        static public int preview_width = 110;
        static public int preview_height = 150;
        static private DisplayInfo DisplayInfo;

        public int ColCount;
        public int RowCount;
        public int ResultsPerPage;
        public int ResultsPerRequest;

        public List<CPost> posts = new List<CPost>();
        public List<Grid> tiles = new List<Grid>();
        
        public Booru(string host)
        {
            
            Connection.Connect(host);//assume success
            
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

        public int ColsPerPage()
        {
            return (int)ScreenWidth / (preview_width);
        }

        public int RowsPerPage()
        {
            return (int)ScreenHeight / (preview_height);
        }

        public async Task<int> UpdateCacheAsync()
        {
            int remainder = this.posts.Count - tiles.Count;

            //do we even need to update? 
            if (remainder > ResultsPerPage)
                return remainder;

            //if its already updating?
            if (_isGettingNewItems)
                return remainder;

            //else update
            _isGettingNewItems = true;

            var response = await Connection.GetResponse("/post/index.json", Connection.Arguments);//.ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync();

            _isGettingNewItems = false;
            try
            {
                if (Deserialize.TryParse(json, out List<CPost> output))
                {

                    if (posts != null)
                        posts.AddRange(output);
                    else
                        remainder = 0;

                    return remainder + output.Count;
                }
            }
            catch (Newtonsoft.Json.JsonSerializationException)
            {
                return 0;
            }
            //an error occured

            if (Deserialize.TryParse(json, out ReturnStatus ret))
            {
                //to do: ReturnStatus.;
                //Device.BeginInvokeOnMainThread( ()=> DisplayAlert )
            }

            //uh oh
            return 0;
        }
    }
}