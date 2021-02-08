using CodeNameTwang.Model;
using CodeNameTwang.Services;
using CodeNameTwang.ViewModels.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace CodeNameTwang.Views
{
    public class HomePage : ContentPage
    {
        public HomePage()
        {

            Button Pressed = new Button();
            Pressed.Text = "Click here to login";
            Pressed.Clicked += async (o,e) => {
                if (!SocketServices.CheckForInternetConnection())
                {
                    await UtilsPage.Alert("You must be connected to th internet to use this application");
                    return;
                }


                if (null != (await CurrentUser.GetCurrentUser())) {
                   

                    Device.BeginInvokeOnMainThread(() => {

                        new Room().store.GetObjectOfType<Room>(true);
                        new User().store.GetObjectOfType<User>(true);
                        Application.Current.MainPage = new MainPage();
                        ((MasterDetailPage)Application.Current.MainPage).Detail = new NavigationPage(new WelcomePage());

                    });
                }
                string s = "12345";// Guid.NewGuid().ToString().Substring(1, 4);
                //UtilsPage.SendEmail(/*v.GetEmail()*/"1902540@uad.ac.uk","Your verification code",s);//todobackend
                //await Navigation.PushAsync(new ValidateUserLogin(v, s));
            };






            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Press button bellow to log in" },
                    Pressed,
                }
            };
        }
    }
}