using JR.DevFw.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T2.Cms.CacheService;

namespace T2.Cms.UnitTest
{
    internal class TestBase
    {
        public DataBaseAccess GetDb()
        {
            DataBaseAccess db = new DataBaseAccess(DataBaseType.SQLite, "Data Source=\"../../../../UIApp/T2.Cms.WebUI/data/db.db\"");
            db.Use(this.profileTrace);
            return db;
        }

        private bool profileTrace(string action, string sql, DbParameter[] sqlParams, Exception exc)
        {
            DateTime dt = DateTime.Now;
            if (exc == null)
            {
                Console.Write(" | {0:yyyy-MM-dd HH:mm:ss}:  {1} [ OK]; SQL={2};Data={3} \r\n\r\n", dt, action,
                    sql, DataUtil.ParamsToString(sqlParams), System.Environment.NewLine);
                return true;
            }
            Console.Write(" | {0:yyyy-MM-dd HH:mm:ss}: {1} [ Fail]; SQL={2};Data={3}; Exception:{4} | \r\n\r\n", dt, action,
                  sql, DataUtil.ParamsToString(sqlParams), exc.Message);
            return false;
        }
        public void Boot()
        {
            // Cms.Init(BootFlag.UnitTest,"../../../../UIApp/T2.Cms.WebUI/config/cms.conf");
            Cms.Init(BootFlag.UnitTest, "./config/cms.conf");

            //读取站点
            if (Cms.Installed)
            {
                Cms.RegSites(SiteCacheManager.GetAllSites().ToArray());
            }
        }
    }
}
