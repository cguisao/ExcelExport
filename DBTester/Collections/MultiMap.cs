using DBTester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelModifier
{
    public class MultiMap<V>
    {
        Dictionary<string, List<V>> _dictionary =
            new Dictionary<string, List<V>>();
        
        public void Add(string key, V value)
        {
            List<V> list;
            if (this._dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<V>();
                list.Add(value);
                this._dictionary[key] = list;
            }
        }
        
        public IEnumerable<string> Keys
        {
            get
            {
                return this._dictionary.Keys;
            }
        }
        
        public List<V> this[string key]
        {
            get
            {
                List<V> list;
                if (!this._dictionary.TryGetValue(key, out list))
                {
                    list = new List<V>();
                    this._dictionary[key] = list;
                }
                return list;
            }
        }
    }

    public class MultiMapFrog<FrogList>
    {
        Dictionary<string, List<FrogList>> _dictionary =
            new Dictionary<string, List<FrogList>>();

        public void Add(string key, FrogList value)
        {
            List<FrogList> list;
            if (this._dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<FrogList>();
                list.Add(value);
                this._dictionary[key] = list;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return this._dictionary.Keys;
            }
        }

        public List<FrogList> this[string key]
        {
            get
            {
                List<FrogList> list;
                if (!this._dictionary.TryGetValue(key, out list))
                {
                    list = new List<FrogList>();
                    this._dictionary[key] = list;
                }
                return list;
            }
        }
    }

    public class MultiMapShopify<ShopifyList>
    {
        Dictionary<string, List<ShopifyList>> _dictionary =
            new Dictionary<string, List<ShopifyList>>();

        public void Add(string key, ShopifyList value)
        {
            List<ShopifyList> list;
            if (this._dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<ShopifyList>();
                list.Add(value);
                this._dictionary[key] = list;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return this._dictionary.Keys;
            }
        }

        public List<ShopifyList> this[string key]
        {
            get
            {
                List<ShopifyList> list;
                if (!this._dictionary.TryGetValue(key, out list))
                {
                    list = new List<ShopifyList>();
                    this._dictionary[key] = list;
                }
                return list;
            }
        }
    }
}
