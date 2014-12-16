using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LevelDB;
using System.IO;

namespace Ops.Cms.Infrastructure.KV
{
    public class LevelDb
    {
        private DB db;

        internal LevelDb(string path)
        {
            Cache c = new Cache(100 * 1024 * 1024);
            var options = new Options()
            {
                BlockCache = c,
                CreateIfMissing = true,
            };

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path).Create();
            }
            this.db = new DB(options, path);
        }

        public string Put(string key, string value)
        {
            string oldValue = this.db.Get(key);


            if (value != null && !Platform.RunAtLinux() &&
                value.Length > Platform.LimitCharLength)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                value = Convert.ToBase64String(bytes);
            }

            this.db.Put(key, value);
            return oldValue;
        }

        public void Delete(string key)
        {
            this.db.Delete(key);
        }

        public string Get(string key)
        {
            string value = this.db.Get(key);
            
            if ( value!= null  && !Platform.RunAtLinux()&& value.Length > Platform.LimitCharLength)
            {
                byte[] bytes = Convert.FromBase64String(value);
                return Encoding.UTF8.GetString(bytes);
            }
            

            return value;
        }

        internal int GetInt(string key)
        {
            int i;
            if (int.TryParse(this.Get(key), out i))
            {
                return i;
            }
            return -1;
        }
    }
}
