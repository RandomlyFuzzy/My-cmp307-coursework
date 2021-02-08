using CodeNameTwang.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CodeNameTwang.ViewModels
{
    public class ObjectViewModel<T> where T:JsonObject
    {
        public InfoDataStore DataStore => DependencyService.Get<InfoDataStore>();
        public bool IsBusy { get; set; } = false;

        public ObservableCollection<T> Items { get; set; } = new ObservableCollection<T>();
        public Command loadItems { get; set; } = null;

        public ObjectViewModel()
        {
            Items = new ObservableCollection<T>();
            loadItems = new Command(()=>LoadItemsAsync());
        }


        public async Task LoadItemsAsync() {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetObjectOfType<T>();
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                //UtilsPage.Alert("[+-]:" + ex.Message, "ERROR");//todo
            }
            IsBusy = false;

        }

    }
}
