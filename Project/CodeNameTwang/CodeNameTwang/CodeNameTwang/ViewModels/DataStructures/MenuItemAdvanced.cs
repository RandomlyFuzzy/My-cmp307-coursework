using CodeNameTwang.ViewModels.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CodeNameTwang.Views
{
    class MenuItemAdvanced:MenuItem
    {
        public object[] Ctorsetvalue { get; set; } = null;

        public new Page OnLoad() {
            //polymorphic multivariable contructor
            Type[] arr = new Type[Ctorsetvalue.Length];
            for (int i = 0; i < arr.Length; i++){
                arr[i] = Ctorsetvalue[i].GetType();
            }

            var contructor = TargetType.GetConstructor(arr);
            return (Page)contructor.Invoke(Ctorsetvalue);
        }

    }
}
