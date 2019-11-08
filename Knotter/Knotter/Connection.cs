using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class Connection
{
    private static HttpClient Client;

    public static void OpenConnection(string URL = "https://Knotter.net")
    {
        Client = new HttpClient
        {
            BaseAddress = new Uri(URL)
        };
        Client.DefaultRequestHeaders.UserAgent.ParseAdd("uwu/1.0 (user do6kids9)");//MyProject/1.0 (by username on Knotter)
    }
    public static async Task<string> GetResponse(string page, Dictionary<String, String> arguments, bool IsPost = false)
    {
        HttpMethod Method = IsPost ? HttpMethod.Post : HttpMethod.Get;

        var request = new HttpRequestMessage(Method, page)
        {
            Content = new FormUrlEncodedContent(arguments)
        };

        HttpResponseMessage httpResponse = await Client.SendAsync(request);//.ConfigureAwait(false);
        if (httpResponse.IsSuccessStatusCode)
        {
            return await httpResponse.Content.ReadAsStringAsync();//.ConfigureAwait(false);
        }
        return null;
    }
}
