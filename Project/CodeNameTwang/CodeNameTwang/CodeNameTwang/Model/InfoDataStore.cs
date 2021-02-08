using CodeNameTwang.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeNameTwang.Model
{
    public class InfoDataStore
    {
        private static List<JsonObject> data = new List<JsonObject>();


        public InfoDataStore() {
            if (data == null)
            {
                data = new List<JsonObject>();
            }
            else {
                var v = 1;
            }


        }

        public async Task<IEnumerable<T>> GetObjects<T>(string col, object val,bool refresh = false) where T : JsonObject
        {


            List<T> ret = new List<T>();
            T def = Activator.CreateInstance(typeof(T)) as T;

            if (refresh){
                //var v = await def.getAllOfType<T>(col,""+val);
                var v = await GetObjectOfType<T>(true);//not the best but sending too many requests is worse (should think about bundleing all things together)
                foreach (var item in v)
                {
                    if (data.Any((a) => a.getPKVal().Equals(item.getPKVal())))
                    {
                        var temp = data.Where((a) => { return a.getPKVal().Equals(item.getPKVal()); }).First();
                        data.Remove(temp);
                    }
                    data.Add(item);
                }
            }
            foreach (var item in data.Where((a)=>a.GetType()==typeof(T)).Where((a) => (a.GetProperty<object>(col) ?? null).Equals(val) ))
            {
                try
                {
                    ret.Add((T)(item));
                }
                catch { 
                
                }
            }

            return ret.AsEnumerable<T>();
        }
        public async Task<IEnumerable<T>> GetObjectOfType<T>(bool refresh = false) where T : JsonObject
        {
            T def = Activator.CreateInstance(typeof(T)) as T;
            if (refresh)
            {
                var v = await def.getAllOfType<T>();
                foreach (var item in v)
                {
                    if (data.Any((a) => a.getPKVal().Equals(item.getPKVal())))
                    {
                        var temp = data.Where((a) => { return a.getPKVal().Equals(item.getPKVal()); }).First();
                        data.Remove(temp);
                    }
                    data.Add(item);
                }
            }
            List<T> ret = new List<T>();
            foreach (var item in data.Where((a) => { return a.tableName.Equals((object)def.tableName) && a.GetType().Name == def.GetType().Name; }))
            {
                try
                {
                    ret.Add((T)(item));
                }
                catch
                {

                }
            }
            if (ret.Count == 0 && !refresh)
            {
                return await GetObjectOfType<T>(true);
            }
            return ret.AsEnumerable<T>();
        }
        public async Task<T> GetItem<T>(object pk, bool force = false) where T : JsonObject//todo
        {
            T ret = Activator.CreateInstance(typeof(T)) as T;
            ret = (await GetObjects<T>(ret.pkname, pk, force)).ToList()[0];
            return ret;
        }
        public async Task<bool> AddItem<T>(T item, bool force = false) where T:JsonObject {

            if (!data.Any((a)=> a.getPKVal().Equals(item.getPKVal())) && (force || (await item.Commit()) == 1))
            {
                data.Add(item);
                return  true;
            }
            return  (false);
        }

        public async Task<bool> RemoveItem<T>(T it) where T : JsonObject// todo reflect on sqldb
        {
            var item = data.Where(arg => arg.getPKVal().Equals(it.getPKVal())).FirstOrDefault();
            item.SetDel(true);
            if ((await item.Commit()) == 1)
            {
                return data.Remove(item);
            }
            return (false);
        }
        public async Task<bool> UpdateItem<T>(T item) where T : JsonObject//todo
        {
            var oldItem = data.Where(arg => arg.getPKVal().Equals(item.getPKVal())).FirstOrDefault();
            if ((await item.Commit()) == 1)
            {
                data[data.IndexOf(oldItem)] = (item);
                return  (true);
            }
            return  (false);
        }

    }
}
