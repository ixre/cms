using System.IO;
using LevelDB;

namespace J6.Cms.Extend.SSO
{
    internal class LevelDbSessionProvider : ISessionSet
    {
        static LevelDB.DB Database { get; set; }

        internal void Initilize()
        {
            string tempPath = Path.GetTempPath();
            string randName = Path.GetRandomFileName();
            LevelDB.Cache c = new LevelDB.Cache(100 * 1024 * 1024);
            var options = new Options()
            {
                BlockCache = c,
                CreateIfMissing = true,
            };
            Database = new LevelDB.DB(options, "tmp/data/.sso/");
        }
        public string Put(string key, string value)
        {
            string oldValue = Database.Get(key);
            Database.Put(key, value);
            return oldValue;
        }

        public void Delete(string key)
        {
            Database.Delete(key);
        }

        public string Get(string key)
        {
            return Database.Get(key);
        }
    }
}
