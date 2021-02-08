using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using CodeNameTwang.ViewModels.DataStructures;
using Xamarin.Forms;
using CodeNameTwang.Model;
using CodeNameTwang.Views;
using System.Net.NetworkInformation;
using CodeNameTwang.ViewModels;
using CodeNameTwang.Services.RestAPI;

namespace CodeNameTwang.Services
{

    public class SocketServices
    {
        private static SocketServices service = null;
        private Thread userThread;
        private CancellationToken token = CancellationToken.None;
        private TcpListener listener;

        string ipadd = "localhost";
        int port = 4444;

        bool loggedon = false;



        private SocketServices() {
            service = this;
            ipadd = Getip();

            //UtilsPage.Alert(ipadd);

            listener = new TcpListener(IPAddress.Parse(ipadd), 4444);

            userThread = new Thread(async () =>
            {
                listener.Start();

                while (!token.IsCancellationRequested)
                {
                    string str = "";
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();//yes this does discard the object after use because all the connection are not likely to send/recieve infromation frequently
                        while (client.Client.Available == 0) { Thread.Sleep(1); }
                        byte[] data = new byte[client.Client.Available];
                        client.GetStream().Read(data, 0, data.Length);
                        str = DecodePacket(Encoding.ASCII.GetString(data));


                        switch (str[0]) {
                            case'L'://log...
                                switch (str[1]) {
                                    case 'N'://on
                                    case 'F'://off
                                        DependencyService.Get<InfoDataStore>().GetObjects<AccessLog>("uid", int.Parse(str.Substring(2)), true).GetAwaiter().GetResult();
                                        break;
                                    default:
                                        UtilsPage.Alert(str, "unknown message recieve");
                                        break;
                                }
                                break;
                            case'B':
                                switch (str[1]){
                                    case '+'://added booking
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            RoomBooking rb = DependencyService.Get<InfoDataStore>().GetObjects<RoomBooking>("id", int.Parse(str.Substring(2)), true).GetAwaiter().GetResult().FirstOrDefault();
                                            if (rb != null)
                                            {
                                                SideBar.AddBooking(rb);
                                                UtilsPage.Notifiation(rb.title, rb.start.ToShortTimeString(), rb.start);
                                            }
                                            else { 
                                                UtilsPage.Alert(str, "unknown message recieve");
                                            }
                                        });
                                        break;
                                    default:
                                        UtilsPage.Alert(str, "unknown message recieve");
                                        break;
                                }
                                break;
                            default:
                                UtilsPage.Alert(str, "unknown message recieve");
                                break;
                        }


                        //UtilsPage.Notifiation("Socket service", str);



                    }
                    catch (Exception ex) {
                        await UtilsPage.Alert("error");
                        await UtilsPage.Alert(ex.Message);
                        await UtilsPage.Alert(ex.InnerException.ToString());
                    }



                }
                listener.Stop();
                service = null;
            });
            userThread.Start();

        }

        private string Getip() {

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("you are not connected to anydevice :/");
        }

        //recieve 
        public string DecodePacket(string pkt)
        {
            return CurrentUser.GetCurrentUser().GetAwaiter().GetResult().stringDecodeString(pkt);
        }

        private void _BroadcastMessage(string message, User[] users)
        {
            foreach (var item in users)
            {
                string name = item.name;
                if (item.id != CurrentUser.GetCurrentUser().GetAwaiter().GetResult().id)
                {
                    if (item.IsOnline())
                    {
                        try
                        {
                            TcpClient client = new TcpClient(item.GetLastAccesIP(), port);
                            if (client.Connected)
                            {
                                byte[] data = Encoding.ASCII.GetBytes(message);
                                client.GetStream().Write(data, 0, data.Length);
                            }

                        }
                        catch (Exception ex) { 
                        
                        }
                       
                    }
                }
            }
        }
        private void _Logon()
        {
            loggedon = true;
            _BroadcastMessage("LN" + CurrentUser.GetCurrentUser().Id, DependencyService.Get<CodeNameTwang.Model.InfoDataStore>().GetObjectOfType<User>().GetAwaiter().GetResult().ToArray());
        }
        private void _Logoff()
        {
            if (!loggedon) return;


            RequestFactory.Get<JsonObject>("logout?token=[token]").GetAwaiter().GetResult();
            _BroadcastMessage("LF" + CurrentUser.GetCurrentUser().Id, DependencyService.Get<CodeNameTwang.Model.InfoDataStore>().GetObjectOfType<User>().GetAwaiter().GetResult().ToArray());
        }


        //send
        public static void BroadcastMessage(string message, User[] users) => service._BroadcastMessage(message, users);


        public static void Logon() => service._Logon();
        public static void Logoff() => service._Logoff();


        public static void Start()
        {
            if (service == null) {
                new SocketServices();
            }
        }

        public static void Stop()
        {
            CancellationTokenSource src = new CancellationTokenSource();
            src.Cancel();
            service.token = src.Token;
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
