using CodeNameTwang.Model;
using CodeNameTwang.ViewModels.DataStructures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CodeNameTwang.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SideBar : ContentPage
    {
        public static SideBar mp;

        public ListView ListView;
        private SideBarViewModel vm;

        List<RoomBooking> Bookings = new List<RoomBooking>();

        static int cnt = 1;

        public SideBar()
        {
            InitializeComponent();
            Appearing += MainPageMaster_Appearing;
        }

        private void MainPageMaster_Appearing(object sender, EventArgs e)
        {
            var v = DependencyService.Get<InfoDataStore>().GetObjects<UserBooking>("uid", CurrentUser.GetCurrentUser().GetAwaiter().GetResult().id, true).GetAwaiter().GetResult();

            BindingContext = vm = new SideBarViewModel();
            ListView = MenuItemsListView;
            ListView.ItemSelected += ListView_ItemSelected;
            mp = this;
            foreach (var item in v)
            {
                RoomBooking rb = DependencyService.Get<InfoDataStore>().GetObjects<RoomBooking>("id", item.bid, true).GetAwaiter().GetResult().ToArray()[0];
                if (rb.start.DayOfYear == DateTime.Now.DayOfYear) { 
                    AddBooking(rb);
                }
            }

            BindingContext = vm;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MenuItem;
            if (item == null)
                return;

            Page page = null;
            try
            {
                if (e.SelectedItemIndex == 0)
                {
                    ((MenuItemAdvanced)item).Ctorsetvalue = new object[2] {"Calender", (object)Bookings };
                    page = ((MenuItemAdvanced)item).OnLoad();
                }
                else { 
                    var page2 = ((MenuItemAdvanced )item).OnLoad();
                    page = page2;
                    ((BookingPage)page).booking = (RoomBooking)(((MenuItemAdvanced)item).Ctorsetvalue)[0];
                }
            }
            catch { 
                page = item.OnLoad();

            }
            page.Title = item.Title;

            ((MasterDetailPage)Application.Current.MainPage).Detail = new NavigationPage(page);
            ((MasterDetailPage)Application.Current.MainPage).IsPresented = false;

            ListView.SelectedItem = null;
        }
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="rb"></param>
        protected void _AddBooking(RoomBooking rb) {
            if (Bookings.Contains(rb)) return;

            Bookings.Add(rb);
            IsBusy = true;
            string title = rb.title+", "+rb.GetRoom().rname + " @ " + rb.start.ToShortTimeString();//WIP
            mp.vm.MenuItems.Insert(mp.vm.MenuItems.Count-1, new MenuItemAdvanced { Title = title, TargetType = typeof(BookingPage), Ctorsetvalue  = new object[1] { rb } });
            var v = mp.vm.MenuItems;
            v[0].notification = "" + cnt++;
            mp.vm.MenuItems = v;
            IsBusy = false;
        }
        public static void AddBooking(RoomBooking rb) {
            if (rb == null) {
                return;
            }
            mp._AddBooking(rb);
        }

        class SideBarViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MenuItem> MenuItems { get; set; }

            public SideBarViewModel()
            {
                MenuItems = new ObservableCollection<MenuItem>(new[]
                {
                    new MenuItemAdvanced { Title = "Calender",TargetType=typeof(AvailabilityPage) },
                    new MenuItem { Title = "Add Booking",TargetType=typeof(AddBookingPage) },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}