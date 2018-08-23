using System;
using System.Data.Common;
using JR.DevFw.Data;
using JR.DevFw.Framework;

namespace T2.Cms.DB
{
    public class DbAccess
    {
        private readonly DataBaseType _dbType;
        private readonly string _connectionString;
        private readonly bool _sqlTrace;
        private DataBaseAccess dbAccess;
        private LogFile logFile;

        public DbAccess(DataBaseType dbType,string connectionString,bool sqlTrace)
        {
            this._dbType = dbType;
            this._connectionString = connectionString;
            this._sqlTrace = sqlTrace;
            this.dbAccess= new DataBaseAccess(this._dbType, this._connectionString);
            if (sqlTrace)
            {
                this.logFile = new LogFile(AppDomain.CurrentDomain.BaseDirectory+"/tmp/sql_profile.txt");
                this.logFile.Truncate();
                this.dbAccess.Use(this.profileTrace);
            }
        }

        private bool profileTrace(string action, string sql, DbParameter[] sqlParams, Exception exc)
        {
            DateTime dt = DateTime.Now;
            if (exc == null)
            {
                logFile.Printf(" | {0:yyyy-MM-dd HH:mm:ss}:  {1} [ OK]; SQL={2};Data={3} \r\n\r\n", dt, action,
                    sql, DataUtil.ParamsToString(sqlParams), System.Environment.NewLine);
                return true;
            }
            logFile.Printf(" | {0:yyyy-MM-dd HH:mm:ss}: {1} [ Fail]; SQL={2};Data={3}; Exception:{4} | \r\n\r\n", dt, action,
                  sql, DataUtil.ParamsToString(sqlParams),exc.Message);
            return false;
        }

        /// <summary>
        /// 表前缀
        /// </summary>
        public string TablePrefix { get; set; }

        public DataBaseAccess CreateInstance()
        {
            DataBaseAccess db = new DataBaseAccess(this._dbType, this._connectionString);
            if (this._sqlTrace)
            {
                db.Use(this.profileTrace);
            }
            return db;
            //return this.dbAccess;
        }

    }
}
