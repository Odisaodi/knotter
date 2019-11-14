using System;
using System.Collections.Generic;
using System.Text;
using QuickType;
using System.Threading.Tasks;
using System.Net.Http;

namespace Knotter
{

    public static class UserActions
    {
        public static bool _isloggedin ;
        public static bool Isloggedin() {
            return (Settings.ApiKey.Length > 0);
        }

        static private bool TryParse<T>(string json, out T result)
        {
            try
            {
                result = Deserialize.FromJson<T>(json);
                return true;
            }
            catch (Newtonsoft.Json.JsonSerializationException)
            {
                result = default;
                return false;
            }
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

            if (TryParse(json, out LoginSuccess UserInfo))
            {
                //UserActions.Identity = UserInfo;
                Settings.Username = UserInfo.Name;
                Settings.ApiKey = UserInfo.PasswordHash;
                //
                return _isloggedin = true;
            }

            if (TryParse(json, out LoginFailure loginFailure))
            {
                //to do: do something with loginFailure
                return _isloggedin = false;
            }
            return false;
        }

        static public async Task<bool> VoteAsync(long post_id, int score = 0)
        {
            var vote = new Dictionary<string, string>
            {
                //identitify yourself
                { "name", Settings.Username },
                { "password_hash", Settings.ApiKey },
                //action to perform
                { "id" , post_id.ToString() },//"2051732"
                { "score" , score.ToString() },

            };

            var Success_type = await Connection.FetchResults<string>("/post/vote.json", vote);
            return true;
            //if it worked it worked?
        }


    }
}
