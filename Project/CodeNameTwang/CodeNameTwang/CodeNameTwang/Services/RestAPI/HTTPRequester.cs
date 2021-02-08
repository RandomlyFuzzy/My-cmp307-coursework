using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace CodeNameTwang.Services.RestAPI
{
    public class HTTPRequester
    {
        private static HttpClient client = new HttpClient();

        public static string APILocation = "";

        private static bool ValidAddress(string url)
        {
            Uri trash = new Uri("https://www.google.com/");
            return !Uri.TryCreate(url, UriKind.Absolute, out trash);
        }

        public async Task<string> MethodRequest(string URL,string method, string body = "{}") {
            if (APILocation == "") {
                string savefile = Path.Combine(FileSystem.CacheDirectory, @"./ServerLocation.txt");
                if (File.Exists(savefile))
                {
                    APILocation = File.ReadAllText(savefile);
                }
                else
                {
                    //Authenticate current user here
                    string serverLocation = "";
                    do {
                        serverLocation = "http://"+await CodeNameTwang.Services.UtilsPage.AlertTextBox("Please enter the localip of the data server", "Server Location", "192.168.?.?");
                        serverLocation += "/";
                    }
                    while (ValidAddress(serverLocation));
                    APILocation = serverLocation;
                    File.WriteAllText(savefile,APILocation);
                }
                URL = APILocation + URL;
            }



            byte[] dataBytes = Encoding.UTF8.GetBytes(body);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            req.Method = method;
            if (method != "GET") { 
                req.ContentLength = dataBytes.Length;
                req.ContentType = "application/x-www-form-urlencoded";
                using (Stream requestBody = req.GetRequestStream())
                {
                    await requestBody.WriteAsync(dataBytes, 0, dataBytes.Length);
                }
            }

            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            using (Stream stream = resp.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                if (resp.StatusCode != HttpStatusCode.OK|| body != "{}" || method != "GET") {
                    return await Task.FromResult("#" + (int)resp.StatusCode);
                }
                return await reader.ReadToEndAsync();
            }
        }



        public async Task<string> Get(string url, string body)
        {
            if (body != "")
            {
                if (url.LastIndexOf("?") > url.LastIndexOf("\\") || url.LastIndexOf("?") > url.LastIndexOf("/"))
                {
                    url += "&";
                }
                else
                {
                    url += "?";
                }
                url += body;
            }

            string resp = await MethodRequest(APILocation + url,"GET");
            return resp;
        }
        public async Task<HttpStatusCode> Put(string url, string body)
        {
             return (HttpStatusCode)int.Parse(( await MethodRequest(APILocation + url,"PUT", body)).Substring(1));
        }
        public async Task<HttpStatusCode> Post(string url, string body)
        {
            return (HttpStatusCode)int.Parse(( await MethodRequest(APILocation + url,"POST", body)).Substring(1));
        }
        public async Task<HttpStatusCode> Delete(string url)
        {
            return (HttpStatusCode)int.Parse(( await MethodRequest(APILocation + url,"DELETE")).Substring(1));
        }
    }
}
