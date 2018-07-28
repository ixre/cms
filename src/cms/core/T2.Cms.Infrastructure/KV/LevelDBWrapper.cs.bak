using System;
using System.IO;
using System.Text;
using LevelDB;

namespace T2.Cms.Infrastructure.KV
{
    public class LevelDb
    {
        private DB _db;
        private string _path;

        internal LevelDb(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path).Create();
            }

            this._path = path;

            this.Rebuilt();
        }

        private void Rebuilt()
        {
            Cache c = new Cache(100 * 1024 * 1024);
            var options = new Options()
            {
                BlockCache = c,
                CreateIfMissing = true,
            };
            this._db = new DB(options, this._path);
        }

        public void Clean()
        {
            try
            {
                this._db.Dispose();
                Directory.Delete(this._path);
            }
            catch
            {
                //todo:
            }
            finally
            {
                this.Rebuilt();
            }
        }

        public string Put(string key, string value)
        {
            string oldValue = this._db.Get(key);


            if (value != null && !Platform.RunAtLinux() &&
                value.Length > Platform.LimitCharLength)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                value = Convert.ToBase64String(bytes);
            }

            this._db.Put(key, value);
            return oldValue;
        }

        public void Delete(string key)
        {
            this._db.Delete(key);
        }

        public string Get(string key)
        {
            string value = this._db.Get(key);

            if (value != null && !Platform.RunAtLinux() && value.Length > Platform.LimitCharLength)
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
