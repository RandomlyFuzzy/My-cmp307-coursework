using CodeNameTwang.Model;
using System;
using System.Collections.Generic;
//using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;
using CodeNameTwang.Services;
using System.IO;
using Xamarin.Essentials;
using CodeNameTwang.Services.RestAPI;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Diagnostics;
using CodeNameTwang.Model;

namespace CodeNameTwang.ViewModels.DataStructures
{
    public class CurrentUser : User
    {
        private static CurrentUser CUser = null;
        public string stringDecodeString(string message) {
            return message;
        }

        public static string token = "_";

        public CurrentUser() : base() { }

        public async static Task<CurrentUser> GetCurrentUser(bool forced = false) {
            string savefile = Path.Combine(FileSystem.CacheDirectory, @"./UserData.txt");
            if ((CUser == null||token == "_")||forced)
            {
                if (File.Exists(savefile))
                {
                    var args = File.ReadAllText(savefile).Split('\n');
                    try
                    {
                        List<Token> t = (await RequestFactory.Get<Token>("login", "id", args[0], "uname", args[1]));
                        if (t.Count != 0) { 
                            token = t[0].token;

                            User usr = (await new User().store.GetObjects<User>("id",int.Parse(args[0]), true)).ToList()[0];//checks for a single entry compaired to datastore that gets everything
                            CUser = new CurrentUser();
                            CUser.SetJSONOBJ<User>(usr);
                        }
                    }
                    catch(Exception ex) {
                        Debug.WriteLine(ex);
                    }//really should do something here
                }

                if (CUser == null)
                {
                    //Authenticate current user here
                    string s = await CodeNameTwang.Services.UtilsPage.AlertTextBox("Please enter your name", "Authentication", "Elsa Duncan");
                    string id = await CodeNameTwang.Services.UtilsPage.AlertTextBox("Please enter Employee number", "Authentication", "11630816");
                    try
                    {

                        List<Token> t = (await RequestFactory.Get<Token>("login", "id", id, "uname", s));
                        if (t.Count != 0)
                        {
                            token = t[0].token;
                            List<User> usr = (await new User().store.GetObjects<User>("id", int.Parse(id), true)).ToList();//checks for a single entry compaired to datastore that gets everything

                            User cu = usr[0];
                            CUser = new CurrentUser();
                            CUser.SetJSONOBJ<User>(cu);

                            File.WriteAllText(savefile, "" + id + "\n" + s);
                            UtilsPage.Alert($"Welcome {CUser.name}", "Welcome");
                        }
                        else { 
                            UtilsPage.Alert("Their was an error authenticating please try again");
                        }
                    }
                    catch (Exception ex) {
                        UtilsPage.Alert("Their was an error authenticating please try again");
                        return null;
                    }
                }
                //therwise will send N request where N is the amount of employees
                //this information would be bundled in with the users to save bandwidth but that is for later revisions.
                await CUser.store.GetObjectOfType<AccessLog>(true);
                SocketServices.Logon();
            }
            return CUser;
        }

        public static async void SetCurrentUser(User usr) {
            //CUser = new CurrentUser();
            if (CUser == null) {
                await GetCurrentUser();
            }
            CUser.SetJSONOBJ(usr);
        }

        public static void LogOut() {
            CUser = null;
            Application.Current.MainPage = new CodeNameTwang.Views.HomePage();
        }

        /// <summary>
        /// needs to be redone TODO
        /// </summary>
        public async Task GetAnyBookings()
        {
            System.Collections.Generic.Dictionary<string, Tuple<object, System.Data.SqlDbType>> dict = new System.Collections.Generic.Dictionary<string, Tuple<object, System.Data.SqlDbType>>();

            dict.Add("id", new Tuple<object, System.Data.SqlDbType>(CurrentUser.GetCurrentUser().GetAwaiter().GetResult().id, System.Data.SqlDbType.Int));



            // need to find a better method could be massive request
            var RoomBookings = await RequestFactory.Get<UserBooking>("DATA/{0}/Twang.UserBooking", "uid", ""+id);

            foreach (var item in RoomBookings)
            {
                var v = await RequestFactory.Get<RoomBooking>("DATA/{0}/Twang.UserBooking","bid",""+item.bid, "bookingDay", DateTime.Now.ToShortDateString());

                if (v.Count() != 0) {
                    foreach (var item2 in v)
                    {
                        CodeNameTwang.Views.SideBar.AddBooking(item2);
                    }
                }
            }

        }
    }
    [Serializable]
    public class Token:JsonObject{
        [JsonPropertyName("token")]
        public string token { get { return GetProperty<string>("token"); } set { SetProperty("token", value); } }
        public Token() { }//for the Activator
    }
}
