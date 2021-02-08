using System;
using CodeNameTwang.Services.RestAPI;
using CodeNameTwang.ViewModels.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace UnitTestProject1
{
    [TestClass]
    public class TestWebServicIntegration_DataStructures
    {

        /**
         *  accesslog 
         *    get most recent
         *  User
         *    IsOnline
         *    GetLastAccesIP
         *    GetLastOnline
         *  UserBooking
         *    GetUser
         *    GetBooking
         *  RoomBooking
         *    GetRoom
         *    GetUsers
         *    GetOrganiser
         */

        public TestWebServicIntegration_DataStructures() {
        }

        public void CreateTokenConnection()
        {
            HTTPRequester.APILocation = "http://127.0.0.1/";
            Token t = RequestFactory.Get<Token>("login", "uname", "Elsa Duncan", "id", "11630816").GetAwaiter().GetResult()[0];
            Assert.IsNotNull(t.token);
            CurrentUser.token = t.token;
        }



        [TestMethod]
        public void AccessLog_GetMostRecent()
        {
            HTTPRequester.APILocation = "http://127.0.0.1/";
            CreateTokenConnection();
            AccessLog al = new AccessLog();
            al.id = 11630816;
            AccessLog al2 = new AccessLog(11630816);

            Assert.IsTrue(al.PromoteToMostRecent());

            Assert.Equals(al2, al2);
        }

        [TestMethod]
        public void User_IsOnline()
        {
            HTTPRequester.APILocation = "http://127.0.0.1/";
            CreateTokenConnection();
            User usr = new User {id=11630816,name="Elsa Duncan" };
            Assert.IsFalse(usr.IsOnline());
        }
        [TestMethod]
        public void User_GetLastAccesIP()
        {
            HTTPRequester.APILocation = "http://127.0.0.1/";
            User usr = new User { id = 11630816, name = "Elsa Duncan" };
            Assert.AreNotEqual(usr.GetLastAccesIP(),"NONE") ;
        }
        [TestMethod]
        public void User_GetLastOnline()
        {
            HTTPRequester.APILocation = "http://127.0.0.1/";
            CreateTokenConnection();
            User usr = new User { id = 11630816, name = "Elsa Duncan" };
            Assert.AreNotEqual(usr.GetLastOnline(), new DateTime(1753, 0, 1));
        }
        [TestMethod]
        public void UserBooking_GetUser()
        {
            HTTPRequester.APILocation = "http://127.0.0.1/";
            CreateTokenConnection();
            UserBooking ub = new UserBooking();
            ub.bid = 1;
            ub.uid = 116730816;
            User us = ub.GetUser();
            Assert.AreEqual(us, new User { id = 11630816, name = "Elsa Duncan", pkey = "randomkey" });
        }
        [TestMethod]
        public void UserBooking_GetRoomBooking()
        {
            HTTPRequester.APILocation = "http://127.0.0.1/";
            CreateTokenConnection();
            UserBooking ub = new UserBooking();
            ub.bid = 1;
            ub.uid = 116730816;
            RoomBooking rb = ub.GetRoomBooking();
            DateTime dt = new DateTime();
            rb.start = dt;
            rb.SetProperty<DateTime>("BookingTime", dt);
            RoomBooking rb2 = new RoomBooking
            {
                id = 11630816,
                rid = 1,
                title = "Random title",
                start = dt,
                day = dt,
                oid = 11630816
            };
            rb2.SetProperty<DateTime>("BookingTime", dt);

            Assert.AreEqual(rb,rb2);
        }

        [TestMethod]
        public void RoomBooking_GetRoom()
        {

        }
        [TestMethod]
        public void RoomBooking_GetUsers()
        {

        }
        [TestMethod]
        public void RoomBooking_GetOrganiser()
        {

        }

    }
}
