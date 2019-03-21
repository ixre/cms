using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Cms.Infrastructure.KV
{
    enum TypeFlag
    {
       Int = 1 << 0,
       String = 1 << 1,
       Bytes = 1 << 2,
    }

    internal class KvKey
    {
        public String Prefix;
        public String Key;
    }

    internal class Pair
    {
        private TypeFlag flag;
        private int _vInt;
        private string _vStr;
        private byte[] _vBytes;

        public Pair(TypeFlag flag,Object v)
        {
            this.flag = flag;
            if((flag & TypeFlag.Int) == TypeFlag.Int)
            {
                this._vInt = (int)v;
            }else if((flag & TypeFlag.String) == TypeFlag.String)
            {
                this._vStr = (string)v;
            }else if((flag & TypeFlag.Bytes) == TypeFlag.Bytes)
            {
                this._vBytes = (byte[])v;
            }
        }
        
        internal TypeFlag Type()
        {
            return this.flag;
        }

        public int TryInt()
        {
            return this._vInt;
        }
        public string TryStr()
        {
            return this._vStr;
        }
        public byte[] TryBytes()
        {
            return this._vBytes;
        }
    }

    internal class KvStorageBlock
    {
        private string prefix;
        private Dictionary<string, Pair> data;

        public KvStorageBlock(string prefix)
        {
            this.prefix = prefix;
            this.data = new Dictionary<string, Pair>();
        }
        public Dictionary<string,Pair> Pairs()
        {
            return this.data;
        }

        internal void Set(string key, TypeFlag flag, object value)
        {
            var p = new Pair(flag, value);
            if (this.data.ContainsKey(key))
            {
                this.data[key] = p;
            }
            else
            {
                this.data.Add(key, p);
            }
        }

        internal void Delete(string key)
        {
            if (this.data.ContainsKey(key))
            {
                this.data.Remove(key);
            }
        }

        internal Pair GetValue(string key)
        {
            if (this.data.ContainsKey(key))
            {
                return this.data[key];
            }
            return null;
        }
    }

    internal class KvDbImpl
    {
        private Dictionary<string, KvStorageBlock> data;

        public KvDbImpl()
        {
            this.data = new Dictionary<string, KvStorageBlock>();
        }

        internal void Clean()
        {
            foreach(var i in this.data)
            {
                var pairs = i.Value.Pairs();
                foreach(string k in pairs.Keys)
                {
                    pairs.Remove(k);
                }
                this.data.Remove(i.Key);
            }
            this.data = new Dictionary<string, KvStorageBlock>();
        }
           
        public KvStorageBlock GetBlock(string prefix)
        {
            var contain = this.data.Keys.Contains(prefix);
            if (!contain)
            {
                this.data[prefix] = new KvStorageBlock(prefix);
            }
            return this.data[prefix];
        }

    }


    public class MicroKvStorage
    {
        private KvDbImpl db;
        public MicroKvStorage()
        {
            this.db = new KvDbImpl();
        }

        public void Clean()
        {
            this.db.Clean();
        }

        private KvKey GetKey(string key)
        {
            int i = key.IndexOf(":");
            if (i == -1)
            {
                throw new ArgumentException("key not right:"+key);
            }
            KvKey k = new KvKey { };
            k.Prefix = key.Substring(0, i);
            k.Key = key.Substring(i + 1);
            return k;
        }

        private KvStorageBlock GetBlock(string prefix)
        {
            return this.db.GetBlock(prefix);
        }

        public string Put(string key, string value)
        {
            var k = this.GetKey(key);
            var block = this.GetBlock(k.Prefix);
            block.Set(k.Key, TypeFlag.String, value);
            return value;
        }
        internal void PutInt(string key, int value)
        {
            var k = this.GetKey(key);
            var block = this.GetBlock(k.Prefix);
            block.Set(k.Key, TypeFlag.Int, value);
        }

        public void Delete(string key)
        {
            var k = this.GetKey(key);
            var block = this.GetBlock(k.Prefix);
            block.Delete(k.Key);
        }

        public string Get(string key)
        {
            var k = this.GetKey(key);
            var block = this.GetBlock(k.Prefix);
            var p = block.GetValue(k.Key);
            if(p != null){
                return p.TryStr();
            }
            return null;
        }

        internal int GetInt(string key)
        {
            var k = this.GetKey(key);
            var block = this.GetBlock(k.Prefix);
            var p = block.GetValue(k.Key);
            if(p != null){
                return p.TryInt();
            }
            return 0;
        }

       
    }
}
