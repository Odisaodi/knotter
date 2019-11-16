using QuickType;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Essentials;

namespace Knotter
{
    public static partial class Connection
    {
        static public Dictionary<string, string> Arguments { get; set; }
        //-----------
        static private HttpClient Client;

        static public void Connect(string host = "https://e926.net")
        {
            Client = new HttpClient
            {
                BaseAddress = new Uri(host),
            };
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Knotter/1.0 (user do6kids9)");//MyProject/1.0 (by username on Knotter)
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

        //static private async Task<bool> (string page, Dictionary<string, string> arguments)
        //{
        //    HttpResponseMessage response = await GetResponse(page, arguments);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        return await response.Content.ReadAsStringAsync();//.ConfigureAwait(false);
        //    }

        //    var error = new ReturnStatus { 
        //        Status = false, 
        //        Reason = response.ReasonPhrase,
        //    };

        //    string value = Serialize.ToJson(error);
        //    return value;
        //}

        static public async Task<bool> CheckVersion()
        {
            var VersionURL = "https://raw.githubusercontent.com/keihoag/knotter/master/apk/version.txt";

            Client = new HttpClient
            {
                BaseAddress = new Uri("https://raw.githubusercontent.com")
            };
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Knotter/1.0 (user do6kids9)");//MyProject/1.0 (by username on Knotter)

            var response = await Client.GetAsync(VersionURL);
            if (response.IsSuccessStatusCode)
            {
                string latest = await response.Content.ReadAsStringAsync();
                //aka ("1.0")
                if (latest != null)
                {
                    var currentVersion = VersionTracking.CurrentVersion;

                    // (1.1) -> (1.0)
                    bool UpdateAvailible = string.Compare(latest, currentVersion, StringComparison.Ordinal) <= 0;
                    if (UpdateAvailible)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
