using System;
using System.Collections.Generic;
using System.Text;
using QuickType;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace Knotter
{

    public static class UserActions
    {

        public static bool Isloggedin() {
            //use result for UI status changes
            // "log in" -> "welcome back, {user}"
            return (Settings.ApiKey.Length > 0);
        }

        static public async Task<bool> Login(string username, string password)
        {
            var login = new Dictionary<string, string>
            {
                {"name", username},
                {"password", password},
            };
            //e621.net/user/login.json?name=USERNAME_HERE&password=PASSWORD_HERE
            var response = await Connection.GetResponse("/user/login.json", login);

            var json = await response.Content.ReadAsStringAsync();

            if (Deserialize.TryParse(json, out LoginSuccess UserInfo))
            {
                //UserActions.Identity = UserInfo;
                Settings.Username = UserInfo.Name;
                Settings.ApiKey = UserInfo.PasswordHash;
                return true;
            }

            if (Deserialize.TryParse(json, out LoginFailure loginFailure))
            {
                //to do: do something with loginFailure
            }
            return false;
        }

        static public async Task<bool> VoteAsync(long post_id, int score = 0)
        {
            var vote = new Dictionary<string, string>
            {
                //identitify yourself
                { "login", Settings.Username },
                { "password_hash", Settings.ApiKey },
                //action to perform
                { "id" , post_id.ToString() },//"2051732"
                { "score" , score.ToString() },
            };
            
            var response = await Connection.GetResponse("/post/vote.json", vote, true);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                if (Deserialize.TryParse(json, out VoteSuccess Returned))
                {
                    return Returned.Success;
                }
                //If the call did not succeed, the following reasons are possible:
                //already voted You have already voted for this post.
                //invalid score You have supplied an invalid score.
            }
            //if (not 200_OK) { "we may get a non json reply (501, 404.html?)" }
            return false;
        }

        ///favorite/create.json
        static public async Task<int> Favourite(long post_id, bool state)
        {
            var favorite = new Dictionary<string, string>
            {
                //identitify yourself
                { "login", Settings.Username },
                { "password_hash", Settings.ApiKey },
                //action to perform
                { "id" , post_id.ToString() },//"2051732"
            };

            HttpResponseMessage response;
            if (state)
                response = await Connection.GetResponse("/favorite/create.json", favorite, true);
            else
                response = await Connection.GetResponse("/favorite/destroy.json", favorite, true);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                if (Deserialize.TryParse(json, out CreateFavorite Returned))
                {
                    return 1;
                }
            }
            else //if (not 200_OK) { "we may get a non json reply (501, 404.html?)" }
            {
                //to do: user not logged in?
                return (int)response.StatusCode;
            }
            return 0;//neutral
        }

    }
}
