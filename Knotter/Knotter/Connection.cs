using QuickType;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Essentials;

public static class Connection
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

    static public async Task<T> FetchResults<T>(string page, Dictionary<String, String> arguments)
    {//Get Response as String

        string response = await FetchResultStringAsync(page, arguments);//.ConfigureAwait(false);
        //Deserialize response to Type
        return (T)Deserialize.FromJson<T>(response);
    }

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
            if (latest!=null)
            {
                var currentVersion = VersionTracking.CurrentVersion;

                // (1.1) -> (1.0)
                bool UpdateAvailible = string.Compare(latest, currentVersion, StringComparison.Ordinal) <= 0;
                if(UpdateAvailible)
                {
                    return true;
                }
            }
        }
        return false;
    }
    //public static void OpenConnection(string URL = "https://e926.net")
    //{
    //    Client = new HttpClient
    //    {
    //        BaseAddress = new Uri(URL)
    //    };
    //    Client.DefaultRequestHeaders.UserAgent.ParseAdd("Knotter/1.0 (user do6kids9)");//MyProject/1.0 (by username on Knotter)
    //}
    //public static async Task<string> GetResponse(string page, Dictionary<String, String> arguments, bool IsPost = false)
    //{
    //    HttpMethod Method = IsPost ? HttpMethod.Post : HttpMethod.Get;

    //    var request = new HttpRequestMessage(Method, page)
    //    {
    //        Content = new FormUrlEncodedContent(arguments)
    //    };

    //    HttpResponseMessage httpResponse = await Client.SendAsync(request);//.ConfigureAwait(false);
    //    if (httpResponse.IsSuccessStatusCode)
    //    {
    //        return await httpResponse.Content.ReadAsStringAsync();//.ConfigureAwait(false);
    //    }
    //    return null;
    //}
}
