using CodeNameTwang.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CodeNameTwang.ViewModels.DataStructures
{
    public class UserBooking:JsonObject
    {

        public UserBooking() {
            Settable("Twang.UserBooking");
            SetPK("id");
        }
        public int id
        {
            get { return GetProperty<int>("id"); }
            set { SetProperty<int>("id", value); }
        }
        public int uid
        {
            get { return GetProperty<int>("uid"); }
            set { SetProperty<int>("uid", value); }
        }
        public int bid
        {
            get { return GetProperty<int>("bid"); }
            set { SetProperty<int>("bid", value); }
        }

        public User GetUser() {
            var v = ((InfoDataStore)Activator.CreateInstance(typeof(InfoDataStore))).GetItem<User>((object)uid);// DependencyService.Get<InfoDataStore>().GetItem<User>((object)uid);
            v.Wait();
            return v.Result;
        }
        public RoomBooking GetRoomBooking() {
            var v = ((InfoDataStore)Activator.CreateInstance(typeof(InfoDataStore))).GetItem<RoomBooking>((object)bid);//DependencyService.Get<InfoDataStore>().GetItem<RoomBooking>((object)bid);
            v.Wait();
            return v.Result;
        }


    }
}