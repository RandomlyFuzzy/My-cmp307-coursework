using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CodeNameTwang.Views
{

    public class MenuItem:INotifyPropertyChanged
    {
        public MenuItem()
        {
            TargetType = typeof(MenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public Type TargetType { get; set; }

        private string myVar;

        public string notification
        {
            get { return myVar; }
            set { myVar = value; OnPropertyChanged(); }
        }


        public Page OnLoad() {
            return (Page)Activator.CreateInstance(TargetType);
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