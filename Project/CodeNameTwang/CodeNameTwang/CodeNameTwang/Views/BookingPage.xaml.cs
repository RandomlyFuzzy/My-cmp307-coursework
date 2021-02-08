using CodeNameTwang.Services;
using CodeNameTwang.ViewModels;
using CodeNameTwang.ViewModels.DataStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeNameTwang.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookingPage : ContentPage
    {
        public RoomBooking booking { get; set; }

        public ObjectViewModel<User> Items { get; set; } = new ObjectViewModel<User>();
        bool cont = true;
        public BookingPage()
        {
            InitializeComponent();
            BindingContext = booking;
            Title = booking.title;
        }
        public BookingPage(RoomBooking rb)
        {
            InitializeComponent();


            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    booking.TimeLeft = "" + new TimeSpan(booking.start.TimeOfDay.Ticks- DateTime.Now.TimeOfDay.Ticks).ToString();
                    booking.Invoke(nameof(booking.TimeLeft));
                });
                return cont;
            });

            BookingDetails.BindingContext = booking = rb;
            Title = booking.title;
            this.Disappearing += BookingPage_Disappearing;
            Items = new ObjectViewModel<User>();
            Items.Items = new System.Collections.ObjectModel.ObservableCollection<User>();
            ItemsListView.BindingContext = Items;
        }

        private void BookingPage_Disappearing(object sender, EventArgs e)
        {
            cont = false;
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as User;
            if (item == null)
            {
                return;
            }
            if (item.name == ((User)(CurrentUser.GetCurrentUser().GetAwaiter().GetResult())).name) {
                UtilsPage.Alert("you cannot do things to yourself");
                return; 
            }

            string resp = await UtilsPage.ActionSheet($"What do you want to do to {item.name}", new string[] { "Send Email" });

            if (resp == "" || resp == "cancel")
            {
                ItemsListView.SelectedItem = null;
                return;
            }

            switch (resp)
            {
                case "Send Email":
                    UtilsPage.SendEmail(item.GetEmail(), "", "");
                    break;
                default:
                    break;
            }


            ItemsListView.SelectedItem = null;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();


            Items.IsBusy = true;
            Items.loadItems.Execute(null);

            List<User> users = new List<User>();
            users.AddRange(Items.Items);

            users = booking.GetUsers() ;
            

            Items.Items = new System.Collections.ObjectModel.ObservableCollection<User>(users.AsEnumerable());
            ItemsListView.ItemsSource = Items.Items;


            BookingDetails.BindingContext = booking;

            Items.IsBusy = false;
        }


    }

}