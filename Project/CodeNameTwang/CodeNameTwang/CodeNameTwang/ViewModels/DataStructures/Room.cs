using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.Json.Serialization;
using CodeNameTwang.Model;

namespace CodeNameTwang.ViewModels.DataStructures
{
    public class Room:JsonObject
    {
        public int id { get { return this.GetProperty<int>("id"); } set { SetProperty<int>("id", value); } }
        public string rname { get { return this.GetProperty<string>("rname"); } set { SetProperty<string>("rname", value); } } 
        public int capcaity { get { return this.GetProperty<int>("capacity"); } set { SetProperty<int>("capacity", value); } }

        public Room() {
            Settable("Twang.Room");
            SetPK("id");
        }


        public List<RoomBooking> GetTodaysBookings() {
            List<RoomBooking> ret = new List<RoomBooking>();
            var v = this.store.GetObjectOfType<RoomBooking>(true);
            v.Wait();
            ret.AddRange(v.Result);
            ret.RemoveAll((a) => {
                if (a.rid != id) return true;

                return (a.start.Date.DayOfYear == DateTime.Now.Date.DayOfYear);
            
            });
            return ret;
        }

    }
}
