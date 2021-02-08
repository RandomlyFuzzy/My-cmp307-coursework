using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CodeNameTwang.Model;

namespace CodeNameTwang.ViewModels.DataStructures
{
    public class AccessLog:JsonObject
    {
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
        public string accessip
        {
            get { return GetProperty<string>("accessip"); }
            set { SetProperty<string>("accessip", value); }
        }
        public DateTime LogonTime
        {
            get { return GetProperty<DateTime>("addtime"); }
            set { SetProperty<DateTime>("addtime", value); }
        }
        public DateTime LogoffTime
        {
            get { return GetProperty<DateTime>("Logofftime"); }
            set { SetProperty<DateTime>("Logofftime", value); }
        }

        public AccessLog()
        {
            Settable("Twang.UserAccessStore");
            SetPK("id");
        }
        public AccessLog(int id):this()
        {
            this.id = id;
            PromoteToMostRecent();
        }


        public bool PromoteToMostRecent()
        {
            var v = store.GetObjects<AccessLog>("uid",uid).GetAwaiter().GetResult();//must redo
            
            List<AccessLog> logs = new List<AccessLog>();
            logs.AddRange(v.Where((a) =>  a.LogoffTime.Year == 1753 ));

            if (logs.Count == 0) {
                return false;
            }

            logs.Sort((a, b) => {
                if (a.LogonTime.Ticks > b.LogonTime.Ticks) return -1;
                if (a.LogonTime.Ticks < b.LogonTime.Ticks) return 1;

                return 0;

            });

            AccessLog al = logs[0];
            id          = al.id;
            uid         = al.uid;
            accessip    = al.accessip;
            LogonTime   = al.LogonTime;
            LogoffTime  = al.LogoffTime;
            return true;
        }
    }
}
