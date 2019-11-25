
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;


namespace Knotter
{
    public static partial class Connection 
    {
        static public Dictionary<string, string> Arguments { get; set; }
        //-----------
        static private HttpClient Client;

        static public bool Connect(string host = "https://e926.net")
        {
            try
            {
                Client = new HttpClient
                {
                    BaseAddress = new Uri(host),
                };
                Client.DefaultRequestHeaders.UserAgent.ParseAdd("Knotter/1.0 (user do6kids9)");//MyProject/1.0 (by username on Knotter)

                Arguments = new Dictionary<string, string>
                {
                    ["typed_tags"] = "true",
                    ["limit"] = "1",
                };
            }
            catch //what
            {
                return false;
            }
            return true;
        }

        static public async Task<HttpResponseMessage> GetResponse(string page, Dictionary<string, string> arguments, bool IsPost = false)
        {
            HttpMethod Method = IsPost ? HttpMethod.Post : HttpMethod.Get;//default GET

            var request = new HttpRequestMessage(Method, page)
            {
                Content = new FormUrlEncodedContent(arguments)
            };

            return await Client.SendAsync(request);//.ConfigureAwait(false);
        }

        static public async Task<bool> CheckVersion()
        {
            Client = new HttpClient
            {
                BaseAddress = new Uri("https://raw.githubusercontent.com"),
            };
            Client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            Client.DefaultRequestHeaders.Add("UserAgent", "Knotter /1.0 (on github)");

            var VersionURL = "/keihoag/knotter/master/apk/version.txt";
            var response = await Client.GetAsync(VersionURL);
            if (response.IsSuccessStatusCode)
            {
                string latest = await response.Content.ReadAsStringAsync();
                //aka ("1.0")
                if (latest != null)
                {
                    var currentVersion = VersionTracking.CurrentVersion;
                    string ThisVersion = currentVersion;
                    string ThatVersion = latest;
                    //bool UpdateAvailible = (ThisVersion < ThatVersion) ? true : false;
                    bool UpdateAvailible = string.Compare(ThisVersion, ThatVersion, StringComparison.Ordinal) <= 0;
                    if (UpdateAvailible)
                    {
                        return true;
                    }
                }
            }
            return false;
            //return false;
        }
    }
}
