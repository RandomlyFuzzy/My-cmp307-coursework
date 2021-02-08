using CodeNameTwang.Model;
using CodeNameTwang.ViewModels;
using CodeNameTwang.ViewModels.DataStructures;
using CodeNameTwang.Views;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CodeNameTwang.Services.RestAPI
{
    public class RequestFactory
    {
        private static HTTPRequester reqs = new HTTPRequester();

        private static bool Validate(string[] args) {
            return args.Length % 2 == 0;
        }

        private static bool CheckForErrors(HttpStatusCode code) {

            if ((int)code < 300) {
                return true;
            }

            switch (code) {
                case HttpStatusCode.Unauthorized:
                    Application.Current.MainPage = new HomePage();
                    UtilsPage.Alert("You have been signed out please login again");
                    break;
                default:
                    UtilsPage.Alert("Error with code " + code.ToString());
                    break;
            }
            return false;
        }


        public static async Task<List<T>> Get<T>(string url, params string[] args) where T:JsonObject {
            string body = "";
            for (int i = 0; i < args.Length; i += 2)
            {
                body += args[i] + "=" + args[i + 1] + "&";
            }
            url = url.Replace("[token]", CurrentUser.token);

            string resp = await reqs.Get(url, body);
            if (resp[0] == '#') {
                //check for 401 if so login
                return new List<T>();
            }
            return JsonParser.Parse<T>(resp);
        }
        public static async Task<bool> Put(string url, params string[] args)
        {
            string body = "";
            for (int i = 0; i < args.Length; i += 2)
            {
                body += args[i] + "=" + args[i + 1] + "&";
            }

            url = url.Replace("[token]", CurrentUser.token);
            HttpStatusCode ret = await reqs.Put(url, body);

            return CheckForErrors(ret);
        }
        public static async Task<bool> Post(string url, params string[] args)
        {
            string body = "";
            for (int i = 0; i < args.Length; i += 2)
            {
                body += args[i] + "=" + args[i + 1] + "&";
            }

            url = url.Replace("[token]", CurrentUser.token);
            HttpStatusCode ret = await reqs.Post(url, body);

            return CheckForErrors(ret);
        }
        public static async Task<bool> Delete(string url)
        {
            url = url.Replace("[token]", CurrentUser.token);
            HttpStatusCode ret = await reqs.Delete(url);

            return CheckForErrors(ret);
        }
    }
}
