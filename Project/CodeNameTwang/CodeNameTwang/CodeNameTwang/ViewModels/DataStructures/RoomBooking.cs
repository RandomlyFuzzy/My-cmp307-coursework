using CodeNameTwang.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CodeNameTwang.Model;

namespace CodeNameTwang.ViewModels.DataStructures
{
    public class RoomBooking:JsonObject
    {
        public int id { get { return this.GetProperty<int>("id"); } set { SetProperty<int>("id", value); } }
        public int rid { get { return this.GetProperty<int>("rid"); } set { SetProperty<int>("rid", value); } }
        public string title
        {
            get { return this.GetProperty<string>("title"); } 
            set { SetProperty<string>("title", value); } }
        public DateTime start
        {
            get { return this.GetProperty<DateTime?>("starttime") ??DateTime.Now; } 
            set { SetProperty<DateTime>("starttime", value); } }
        public int duration { get { return this.GetProperty<int>("duration"); } set { SetProperty<int>("duration", value); } }
        public int oid { get { return this.GetProperty<int>("oid"); } set { SetProperty<int>("oid", value); } }


        public DateTime day
        {
            get { return this.GetProperty<DateTime?>("bookingDay") ?? DateTime.Now; }
            set { SetProperty<DateTime>("bookingDay", value); }
        }


        int i = 0;

        private string _TimeLeft = "0.0";

        public string TimeLeft
        {
            get { 
                return (_TimeLeft??".").Substring(0,_TimeLeft.LastIndexOf(".")); 
            }
            set {
                _TimeLeft = value;
                Invoke(nameof(TimeLeft));
            }
        }

        public TimeSpan CurrentSpan {
            get {
                return start.TimeOfDay;
            }
            set {
                DateTime t = new DateTime(DateTime.Today.Ticks+value.Ticks);

                start = t;
                Invoke(nameof(CurrentSpan));
            }

        }

        public string Organiser { 
            get { 
                return this.store.GetItem<User>(oid).GetAwaiter().GetResult().name; 
            } 
        }

        public bool IsBusy { get; set; } = false;

        public RoomBooking() {
            Settable("Twang.Booking");
            SetPK("id");
        }
        public Room GetRoom() {
            return store.GetItem<Room>((object)rid).Result;
        }

        public List<User> GetUsers() {
            List<User> ret = new List<User>();
            var v = store.GetObjectOfType<User>(false).GetAwaiter().GetResult();
            var v2 = store.GetObjects<UserBooking>("bid",id,true).GetAwaiter().GetResult();
            foreach (var item in v2)
            {
                ret.Add(v.First((a) => a.id == item.uid));
            }
            return ret;
        }

        public User GetOrganiser() { 
            var v = store.GetObjectOfType<User>(false).Result;
            return v.First((a) => a.id == oid);
        }

        public async new Task<int> Commit() {

            if (this.Del)
            {
                var v = await store.GetObjects<UserBooking>("bid", id,true);
                foreach (var item in v)
                {
                    item.SetDel(true);
                    item.Commit();
                    store.RemoveItem<UserBooking>(item);
                }
            }
            return await base.Commit();
        }
    }
}
