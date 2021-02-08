using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CodeNameTwang.Views;
using CodeNameTwang.Model;
using CodeNameTwang.ViewModels.DataStructures;
using CodeNameTwang.Services;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;
using System.Net.NetworkInformation;

namespace CodeNameTwang
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //UtilsPage.Notifiation("hello", "world");

            //UtilsPage.SendEmail("1902540@abertay.ac.uk", "hello", "world");

            DependencyService.Register<InfoDataStore>();

            MainPage = new HomePage();
            
            //MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            SocketServices.Start();
        }

        protected override void OnSleep()
        {
            SocketServices.Stop();
            SocketServices.Logoff();
        }

        protected override void OnResume()
        {
            SocketServices.Start();
            CurrentUser.GetCurrentUser().GetAwaiter().GetResult().GetAnyBookings().GetAwaiter().GetResult();
            SocketServices.Logon();
        }
    }
}
