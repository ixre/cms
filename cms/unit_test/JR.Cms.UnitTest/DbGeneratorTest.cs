using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JR.DevFw.Data;
using System.IO;

namespace JR.Cms.UnitTest
{
    [TestClass]
    public class DbGeneratorTest
    {
        [TestMethod]
        public void TestGenSqliteDB()
        {
            String targetFile = "../../files/sqlite.sql";
            string connStr = "Data Source=$ROOT/sqlite.db";
            Exception ex = ExtraDB(DataBaseType.SQLite, connStr, "cms_",targetFile);
            if(ex != null)
            {
                Assert.Fail(ex.Message);
            }
        }


        private Exception ExtraDB(DataBaseType dbType, string connStr, string dbPrefix,string sqlScript)
        {
            DataBaseAccess db = new DataBaseAccess(dbType, connStr.Replace("$ROOT", "."));

            string sql = null;
            if (dbType == DataBaseType.SQLite)
            {
                //sqlScript = String.Concat(Cms.PyhicPath, MYSQL_INSTALL_SCRIPT);
                return execDbScript(db,dbPrefix, ref sql, sqlScript);
            }
            return null;
        }

        /// <summary>
        /// 执行数据脚本
        /// </summary>
        /// <param name="dbPrefix"></param>
        /// <param name="sql"></param>
        /// <param name="sqlScript"></param>
        /// <returns></returns>
        private Exception execDbScript(DataBaseAccess db,string dbPrefix, ref string sql, string sqlScript)
        {
            if (sqlScript != null)
            {
                //从脚本中读取出SQL语句
                TextReader tr = new StreamReader(sqlScript);
                sql = tr.ReadToEnd().Replace("cms_", dbPrefix)
                    .Replace(",False", ",0")
                    .Replace(",True", ",1")
                    .Replace(",index", ",`index`");
                tr.Dispose();
            }

            if (sql != null)
            {
                try
                {
                    db.ExecuteScript(sql, null);
                }
                catch (Exception exc)
                {
                    return exc;
                }
            }
            return null;
        }
    }
}
