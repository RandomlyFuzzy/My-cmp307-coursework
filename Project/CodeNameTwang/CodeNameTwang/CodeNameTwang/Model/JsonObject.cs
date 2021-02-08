
using CodeNameTwang.Services;
using CodeNameTwang.Services.RestAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CodeNameTwang.Model
{
    public class JsonObject:INotifyPropertyChanged
    {
        public InfoDataStore store => DependencyService.Get<InfoDataStore>();
        private Dictionary<string, object> Override     = new Dictionary<string, object>();
        private Dictionary<string, object> Core         = new Dictionary<string, object>();
        private List<string> Ordering = new List<string>();

        public string pkname { private set; get; } = "id";
        private string table = "";

        public bool New { private set; get; } = true;
        public bool Del { private set; get; } = false;

        public string tableName { get { return table; } protected set { table = value; } }

        public delegate void valueChanged(string name, object from, object to);
        public event valueChanged OnChange;
        public event PropertyChangedEventHandler PropertyChanged;


        public void SetJSONOBJ<T>(T obj) where T : JsonObject
        {
            this.Override = obj.Override;
            this.Core = obj.Core;
            this.Ordering = obj.Ordering;

            this.pkname = obj.pkname;
            this.table = obj.table;

            this.New = obj.New;
            this.Del = obj.Del;
        }


        public JsonObject() { }

        public void SetNotNew()
        {
            New = false;
        }

        public void SetDel(bool val)
        {
            Del = val;
        }
        public T GetProperty<T>(string name)
        {
            if (Override.ContainsKey(name))
            {
                return (T)(Override[name]);
            }
            if (Core.ContainsKey(name))
            {
                return (T)(Core[name]);
            }
            return default;
        }
        public void SetProperty<T>(string name, T set, bool force = false)
        {
            Debug.WriteLine("hello");
            string temp = typeof(T).Name;
            Debug.WriteLine(typeof(T).Name);

            if (force || New)
            {
                if (Core.ContainsKey(name))
                {
                    Core[name] = set;
                    OnChange?.Invoke(name, Core[name], (object)set);
                }
                else
                {
                    Core.Add(name, set);
                    OnChange?.Invoke(name, (object)set, (object)set);
                }
                Invoke(name);
                return;
            }

            if (Core.ContainsKey(name))
            {

                if (Override.ContainsKey(name))
                {
                    Override[name] = set;
                }
                else
                {
                    Override.Add(name, set);
                }
                Invoke(name);
                OnChange?.Invoke(name, Override[name], (object)set);
            }
            else
            {
                Invoke(name);
                OnChange?.Invoke(name, (object)set, (object)set);
                Core.Add(name, set);
            }
        }


        public void Invoke(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public async Task<int> Commit()
        {
            if (Override == Core)
            {
                Debug.WriteLine("ret Prem1");
                return 0;
            }
            if (Core.Count == 0)
            {
                Debug.WriteLine("ret Prem2");
                return 0;
            }
            if (table == "")
            {
                Debug.WriteLine("ret Prem3");
                return -1;
            }

            Dictionary<string, object> temp = new Dictionary<string, object>();
            foreach (string s in Core.Keys)
            {
                if (Override.ContainsKey(s))
                {
                    if (Override[s] == (object)null) continue;

                    temp.Add(s, Override[s]);
                    continue;
                }
                if (Core[s] == (object)null) continue;

                temp.Add(s, Core[s]);
            }
            string[] arr = new string[temp.Keys.Count * 2];
            int ind = 0;
            foreach (var item in temp.Keys)
            {
                arr[ind++] = item;
                if (temp[item].GetType().Equals(typeof(DateTime)))
                {
                    DateTime t = (DateTime.Parse(temp[item].ToString()));
                    var add = 3 - t.Millisecond.ToString().Length;
                    var ms = "";
                    for (int i = 0; i < add; i++)
                    {
                        ms += "0";
                    }
                    var add2 = 2 - t.Minute.ToString().Length;
                    var min = "";
                    for (int i = 0; i < add2; i++)
                    {
                        min += "0";
                    }
                    var add3 = 2 - t.Hour.ToString().Length;
                    var hour = "";
                    for (int i = 0; i < add3; i++)
                    {
                        hour += "0";
                    }
                    var add4 = 2 - t.Second.ToString().Length;
                    var sec = "";
                    for (int i = 0; i < add4; i++)
                    {
                        sec += "0";
                    }
                    var add5 = 2 - t.Day.ToString().Length;
                    var Day = "";
                    for (int i = 0; i < add5; i++)
                    {
                        Day += "0";
                    }
                    var add6 = 2 - t.Month.ToString().Length;
                    var Month = "";
                    for (int i = 0; i < add6; i++)
                    {
                        Month += "0";
                    }

                    arr[ind++] =t.Year+"-"+Month+t.Month+"-"+Day+t.Day+"T"+ hour+t.Hour+":"+ min+t.Minute+":"+ sec+t.Second+"."+ ms+t.Millisecond+"Z";
                }
                else
                {
                    arr[ind++] = temp[item].ToString();
                }
            }


            bool ret = false;
            try
            {

                if (Del)
                {
                    //delete /{id}
                    ret = await RequestFactory.Delete("DATA/[token]/" + table + "/" + Core[pkname]);
                }
                else
                if (New)
                {
                    //insert
                    //put /
                    ret = await RequestFactory.Put("DATA/[token]/" + table + "/", arr);
                }
                else
                {
                    //post /{id}
                    ret = await RequestFactory.Post("DATA/[token]/" + table + "/" + Core[pkname], arr);
                }

                Override = new Dictionary<string, object>();
                Core = temp;
                SetNotNew();
            }
            catch (Exception ex)
            {
                UtilsPage.Alert("error "+ex.Message);
            }
            return ret?1:0;
        }
        public void SetPK(string name)
        {
            pkname = name;
        }
        public object getPKVal()
        {
            return GetProperty<object>(pkname);
        }
        public void Settable(string val)
        {
            table = val;
        }


        public async Task<List<T>> getAllOfType<T>(string col = null, string val = null) where T : JsonObject
        {

            if (col == val && col == null)
            {
                return await RequestFactory.Get<T>("DATA/[token]/" + tableName);
            }
            else
            {
                return await RequestFactory.Get<T>("DATA/[token]/" + tableName,col,val);
            }
        }
    }
}
