using Ops.Cms.Infrastructure.KV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ops.Cms.Infrastructure
{
    public static class Kvdb
    {
        private static LevelDb db;

        public static LevelDb _currentInstance
        {
            get
            {
                chkDb();
                return db;
            }
        }

        private static void chkDb()
        {
            if (db == null)
            {
                throw new ArgumentNullException("Kvdb未初始化，请使用Kvdb.SetPath()");
            }
        }
        public static void SetPath(string path)
        {
            db = new LevelDb(path);
        }

        public static string Put(string key, string value)
        {
            return db.Put(key, value);
        }

        public static void Delete(string key)
        {
            chkDb();
            db.Delete(key);
        }

        public static string Get(string key)
        {
            chkDb();
            return db.Get(key);
        }



        public static int GetInt(string key)
        {
            return db.GetInt(key);
        }
    }
}
