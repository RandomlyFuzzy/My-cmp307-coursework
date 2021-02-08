using CodeNameTwang.Model;
using CodeNameTwang.ViewModels.DataStructures;
using CodeNameTwang.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CodeNameTwang.Services;

namespace CodeNameTwang.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddBookingPage : ContentPage
    {
        public RoomBooking Booking { get; set; } = new RoomBooking();

        public ObjectViewModel<User> Items { get; set; } = new ObjectViewModel<User>();

        public List<User> participants = new List<User>();

        public Room CurrentRoom = new Room {rname="", capcaity = 0 };

        bool AVACheck = false;


        public AddBookingPage()
        {
            InitializeComponent();
            Booking = new RoomBooking{
                duration = 0,
                start = DateTime.MinValue,
                title = ""
            };
            participants = new List<User>();
            CurrentRoom = new Room { rname = "", capcaity = 1 };


            Items = new ObjectViewModel<User>();
            Items.Items = new System.Collections.ObjectModel.ObservableCollection<User>();
            entries.BindingContext = Booking;
            List<string> r = new List<string>();
            var v = DependencyService.Get<InfoDataStore>().GetObjectOfType<Room>().GetAwaiter().GetResult().AsEnumerable().ToList();
            v.Sort((a, b) => a.id < b.id ? -1 : a.id > b.id ? 1 : 0);
            v.All((a) =>
            {
                if (!r.Contains(a.rname)) { 
                    r.Add(a.rname);
                }
                return true;
            });
            Rooms.ItemsSource = r;


            List<string> durations = new List<string>();
            for (int i = 5; i <= 60; i++) {
                durations.Add("" + i);
            }
            duration.ItemsSource = durations;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();


            if (AVACheck) {
                AVACheck = false;
                return;
            }

            Booking = new RoomBooking
            {
                duration = 0,
                start = DateTime.MinValue,
                title = ""
            };
            participants = new List<User>();
            CurrentRoom = new Room { rname = "", capcaity = 1 };
            entries.BindingContext = Booking;

            duration.SelectedIndex = -1;
            Rooms.SelectedIndex = -1;


            Items.loadItems.Execute(null);
            Items.IsBusy = true;
            Items.Items = new System.Collections.ObjectModel.ObservableCollection<User>();
            Items.Items.Add(CurrentUser.GetCurrentUser().GetAwaiter().GetResult());
            participants.Add(CurrentUser.GetCurrentUser().GetAwaiter().GetResult());
            ItemsListView.ItemsSource = Items.Items;
            BindingContext = Items;
            Items.IsBusy = false;
            cap.Text = ""+participants.Count+" / " + CurrentRoom.capcaity+" \t\t";
        }





        /// <summary>
        /// Adds user to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Pressed_1(object sender, EventArgs e)
        {
            List<string> s = new List<string>();
            var v = DependencyService.Get<InfoDataStore>().GetObjectOfType<User>();
            v.Wait();
            v.Result.ToList().ForEach((a) => {
                if (!Items.Items.Any((b) => a.id == b.id)&&a.id!=CurrentUser.GetCurrentUser().GetAwaiter().GetResult().id&&!s.Contains(a.name)) { 
                    s.Add(a.name);
                }
            });

            s.Sort();
            string ret = await DisplayActionSheet("Select Employee", "Cancel", null, s.ToArray());
            if (ret == "Cancel" || ret == null) return;

            User us = v.Result.Where((a) =>
            {
                if (a.name == ret)
                {
                    return true;
                }
                return false;
            }).ToList()[0];
            Items.Items.Add (us);
            participants.Add(us);


            cap.Text = "" + participants.Count + " / " + CurrentRoom.capcaity + " \t\t";

            Items.IsBusy = true;
            BindingContext = Items;
            Items.IsBusy = false;
        }

        private async void Rooms_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (Rooms.SelectedItem == null) return;

            //UtilsPage.Notifiation("hello ",Rooms.SelectedItem.ToString());

            Booking.rid = Rooms.SelectedIndex;
            CurrentRoom = (await new Room().store.GetObjects<Room>("rname", Rooms.SelectedItem, true)).First();
            Booking.rid = CurrentRoom.id;
            cap.Text = ""+participants.Count+" / " + CurrentRoom.capcaity+" \t\t";
            var v = 0;
        }

        private void duration_SelectedIndexChanged(object sender, EventArgs e)
        {
            //make room booking 
            if (duration.SelectedItem == null) return;

            int i = duration.SelectedIndex + 5;
            Booking.duration = i;


        }

        private async void ItemsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as User;
            if (item == null)
            {
                return;
            }

            string s = await UtilsPage.ActionSheet($"What do you want to do to {item.name}", new string[] { "remove" });
            if (s == "Cancel" || s == null)
            {
                ItemsListView.SelectedItem = null;
                return;
            }

            Items.IsBusy = true;
            Items.Items.Remove(item);
            participants.Remove(item);

            ItemsListView.SelectedItem = null;
            ItemsListView.ItemsSource = ItemsListView.ItemsSource;
            Items.IsBusy = false;

            cap.Text = "" + participants.Count + " / " + CurrentRoom.capcaity + " \t\t";

            Items.IsBusy = true;
            BindingContext = Items;
            Items.IsBusy = false;

        }

        private void TimePicker_Unfocused(object sender, FocusEventArgs e)
        {
            Booking.CurrentSpan = TimePick.Time;
        }





        /// <summary>
        /// Add Booking to db
        /// after some validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Pressed(object sender, EventArgs e)
        {
            Random r = new Random();

            //check Room set
            //check Time set
            //check duration set
            //check title set
            //check 0< cap <capacity
            List<View> views = new List<View>();
            bool err = false;
            if (Booking.start.Ticks - DateTime.Now.Ticks < 0) {
                err = true;
                views.Add(TimePick);

            }
            if (Booking.duration == 0) {
                err = true;
                views.Add(duration);

            }
            if (Booking.title == "") {
                err = true;
                views.Add(title);

            }
            if (CurrentRoom.rname == "")
            {
                err = true;
                views.Add(Rooms);

            }
            if (Items.Items.Count > CurrentRoom.capcaity || CurrentRoom.capcaity <= 1) {
                err = true;
                views.Add(Add);
            }

            if (err)
            {
                await aniamtionErrors(views);
                return;
            }


            var v = (await DependencyService.Get<InfoDataStore>().GetObjects<RoomBooking>("rid", CurrentRoom.id)).Where((a) => {
                return a.start > DateTime.Now;
            }).ToList();

            bool inside = InsideTimeFrame(v.ToArray(), Booking.start) || InsideTimeFrame(v.ToArray(), Booking.start.AddMilliseconds(Booking.duration));


            //needs to be redone :/
            /*
            { 
                RoomBooking infront = null;
                RoomBooking behind = null;
                if (inside)
                {
                    //suggest valid time
                    //get 1 min after anybooking check if avaliable until end
                    foreach (var item in v)
                    {
                        List<RoomBooking> temp = new List<RoomBooking>();
                        temp.AddRange(v);
                        temp.Remove(item);
                        inside = InsideTimeFrame(v.ToArray(), item.start.AddMilliseconds(item.duration + 1)) || InsideTimeFrame(v.ToArray(), item.start.AddMilliseconds(item.duration + 1).AddMilliseconds(Booking.duration));

                        if (!inside) {
                            infront = item;
                            break;
                        }
                    }

                    if (!inside) {
                        if (await UtilsPage.Alert($"would you like to change your meeting to {infront.start.AddMinutes(1 + infront.duration).ToShortTimeString()}", "we could not book your session at that time", "no thanks", "yes"))
                        {
                            Booking.start = infront.start.AddMinutes(1 + infront.duration);
                        }
                        else
                        {
                            return;
                        }
                    }

                }


                //find before
                if (inside)
                {
                    foreach (var item in v)
                    {
                        List<RoomBooking> temp = new List<RoomBooking>();
                        temp.AddRange(v);
                        temp.Remove(item);
                        inside = InsideTimeFrame(v.ToArray(), item.start.AddMilliseconds(-(1 + Booking.duration))) || InsideTimeFrame(v.ToArray(), item.start.AddMilliseconds(-1));

                        if (!inside)
                        {
                            behind = item;
                            break;
                        }
                    }

                    if (!inside) {
                        if (await UtilsPage.Alert($"would you like to change your meeting to {behind.start.AddMinutes(1 + infront.duration).ToShortTimeString()}", "we could not book your session at that time", "no thanks", "yes"))
                        {
                            Booking.start = behind.start.AddMilliseconds(-(1 + Booking.duration));
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            */
            if (inside)
            {
                await UtilsPage.Alert("Sorry the room you booked is occupied today please try a different room ", "Sorry we could not accomidate you");
                return;
            }






            Booking.oid = CurrentUser.GetCurrentUser().GetAwaiter().GetResult().id;
            Booking.id = r.Next();
            Booking.rid = CurrentRoom.id;
            await Booking.Commit();


            List<Task<int>> tsk = new List<Task<int>>();

            foreach (var item in participants)
            {
                var ub = new UserBooking{id=r.Next(), bid = Booking.id,uid=item.id};
                tsk.Add(ub.Commit());
            }
            Task.WaitAll(tsk.ToArray());


            SocketServices.BroadcastMessage("B+" + Booking.id, participants.ToArray());

            //await DependencyService.Get<InfoDataStore>().AddItem<RoomBooking>(Booking,true);
            SideBar.AddBooking(Booking);

            OnAppearing();

        }



        private bool InsideTimeFrame(RoomBooking[] bookings, DateTime time) {
            bool inside = false;

            foreach (var item in bookings)
            {
                inside = (item.start.Ticks <= Booking.start.Ticks && item.start.AddMinutes(item.duration).Ticks >= Booking.start.Ticks);


                if (inside)
                {
                    return true;
                }
            }
            return false;
        }










        bool run = false;
        public async  Task aniamtionErrors(List<View> views) {
            if (run) return;
            List<View> viws = views;
            double f = 1;
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 40), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    foreach (var item in views)
                    {
                        item.BackgroundColor = new Color((f -= 0.025f), 0, 0,1);
                    }
                });

                return run = f > 0;
            });
            return;
        }

        private async void Button_Pressed_2(object sender, EventArgs e)
        {
            AVACheck = true;
            var v = (await DependencyService.Get<InfoDataStore>().GetObjects<RoomBooking>("rid", CurrentRoom.id,true)).Where((a) => a.start.DayOfYear == DateTime.Now.DayOfYear);
            await Navigation.PushAsync(new AvailabilityPage(CurrentRoom.rname+" Bookings for today", v.ToList()));
        }
    }
}