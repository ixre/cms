using System;
using JR.Cms.Infrastructure.KV;

namespace JR.Cms.Infrastructure
{
    public static class Kvdb
    {
        private static MicroKvStorage _db;

        public static MicroKvStorage _currentInstance
        {
            get
            {
                ChkDb();
                return _db;
            }
        }

        private static void ChkDb()
        {
            if (_db == null) throw new ArgumentNullException("Kvdb未初始化，请使用Kvdb.SetPath()");
        }

        public static void SetPath(string path)
        {
            _db = new MicroKvStorage();
        }

        public static string Put(string key, string value)
        {
            return _db.Put(key, value);
        }

        public static void PutInt(string key, int value)
        {
            _db.PutInt(key, value);
        }

        public static void Delete(string key)
        {
            ChkDb();
            _db.Delete(key);
        }

        public static string Get(string key)
        {
            ChkDb();
            return _db.Get(key);
        }

        public static int GetInt(string key)
        {
            return _db.GetInt(key);
        }

        public static void Clean()
        {
            _db.Clean();
        }
    }
}