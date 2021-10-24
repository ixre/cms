using System;
using System.Collections.Generic;

namespace JR.Cms.Infrastructure.KV
{
    internal enum TypeFlag
    {
        Int = 1 << 0,
        String = 1 << 1,
        Bytes = 1 << 2,
    }

    internal class KvKey
    {
        public string Prefix;
        public string Key;
    }

    internal class Pair
    {
        private TypeFlag flag;
        private int _vInt;
        private string _vStr;
        private byte[] _vBytes;

        public Pair(TypeFlag flag, object v)
        {
            this.flag = flag;
            if ((flag & TypeFlag.Int) == TypeFlag.Int)
                _vInt = (int) v;
            else if ((flag & TypeFlag.String) == TypeFlag.String)
                _vStr = (string) v;
            else if ((flag & TypeFlag.Bytes) == TypeFlag.Bytes) _vBytes = (byte[]) v;
        }

        internal TypeFlag Type()
        {
            return flag;
        }

        public int TryInt()
        {
            return _vInt;
        }

        public string TryStr()
        {
            return _vStr;
        }

        public byte[] TryBytes()
        {
            return _vBytes;
        }
    }

    internal class KvStorageBlock
    {
        private string prefix;
        private Dictionary<string, Pair> data = new Dictionary<string, Pair>();

        public KvStorageBlock(string prefix)
        {
            this.prefix = prefix;
        }

        public Dictionary<string, Pair> Pairs()
        {
            return data;
        }

        internal void Set(string key, TypeFlag flag, object value)
        {
            var p = new Pair(flag, value);
            if (data.ContainsKey(key)) data[key] = p;
            else data.Add(key, p);
        }

        internal void Delete(string key)
        {
            if (data.ContainsKey(key)) data.Remove(key);
        }

        internal Pair GetValue(string key)
        {
            if (data.ContainsKey(key)) return data[key];
            return null;
        }
    }

    internal class KvDbImpl
    {
        private Dictionary<string, KvStorageBlock> data;

        public KvDbImpl()
        {
            data = new Dictionary<string, KvStorageBlock>();
        }

        internal void Clean()
        {
            foreach (var i in data)
            {
                var pairs = i.Value.Pairs();
                foreach (var k in pairs.Keys) pairs.Remove(k);
                data.Remove(i.Key);
            }

            data = new Dictionary<string, KvStorageBlock>();
        }

        public KvStorageBlock GetBlock(string prefix)
        {
            if (!data.ContainsKey(prefix)) data[prefix] = new KvStorageBlock(prefix);
            return data[prefix];
        }
    }


    public class MicroKvStorage
    {
        private KvDbImpl db;

        public MicroKvStorage()
        {
            db = new KvDbImpl();
        }

        public void Clean()
        {
            db.Clean();
        }

        private KvKey GetKey(string key)
        {
            var i = key.IndexOf(":");
            if (i == -1) throw new ArgumentException("key not right:" + key);
            var k = new KvKey { };
            k.Prefix = key.Substring(0, i);
            k.Key = key.Substring(i + 1);
            return k;
        }

        private KvStorageBlock GetBlock(string prefix)
        {
            return db.GetBlock(prefix);
        }

        public string Put(string key, string value)
        {
            var k = GetKey(key);
            var block = GetBlock(k.Prefix);
            block.Set(k.Key, TypeFlag.String, value);
            return value;
        }

        internal void PutInt(string key, int value)
        {
            var k = GetKey(key);
            var block = GetBlock(k.Prefix);
            block.Set(k.Key, TypeFlag.Int, value);
        }

        public void Delete(string key)
        {
            var k = GetKey(key);
            var block = GetBlock(k.Prefix);
            block.Delete(k.Key);
        }

        public string Get(string key)
        {
            var k = GetKey(key);
            var block = GetBlock(k.Prefix);
            var p = block.GetValue(k.Key);
            if (p != null) return p.TryStr();
            return null;
        }

        internal int GetInt(string key)
        {
            var k = GetKey(key);
            var block = GetBlock(k.Prefix);
            var p = block.GetValue(k.Key);
            if (p != null) return p.TryInt();
            return 0;
        }
    }
}