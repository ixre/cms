using System;
using System.Data.Common;
using JR.Stand.Core;
using JR.Stand.Core.Data;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Utils;

namespace JR.Cms.Library.DataAccess.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class DbAccess
    {
        private readonly DataBaseType _dbType;
        private readonly string _connectionString;
        private readonly bool _sqlTrace;
        private readonly FileLogger _fileLogger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="connectionString"></param>
        /// <param name="sqlTrace"></param>
        public DbAccess(DataBaseType dbType, string connectionString, bool sqlTrace)
        {
            _dbType = dbType;
            _connectionString = connectionString;
            _sqlTrace = sqlTrace;
            var dbAccess = new DataBaseAccess(_dbType, _connectionString);
            if (sqlTrace)
            {
                _fileLogger = new FileLogger(EnvUtil.GetBaseDirectory() + "/tmp/sql_profile.txt");
                _fileLogger.Truncate();
                dbAccess.Use(ProfileTrace);
            }
        }

        private bool ProfileTrace(string action, string sql, DbParameter[] sqlParams, Exception exc)
        {
            var dt = DateTime.Now;
            if (exc == null)
            {
                _fileLogger.Printf(" | {0:yyyy-MM-dd HH:mm:ss}:  {1} [ OK]; SQL={2};Data={3} \r\n\r\n", dt, action,
                    sql, DataUtil.ParamsToString(sqlParams), Environment.NewLine);
                return true;
            }

            _fileLogger.Printf(" | {0:yyyy-MM-dd HH:mm:ss}: {1} [ Fail]; SQL={2};Data={3}; Exception:{4} | \r\n\r\n", dt,
                action,
                sql, DataUtil.ParamsToString(sqlParams), exc.Message);
            return false;
        }

        /// <summary>
        /// 表前缀
        /// </summary>
        public string TablePrefix { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataBaseAccess CreateInstance()
        {
            var db = new DataBaseAccess(_dbType, _connectionString);
            if (_sqlTrace) db.Use(ProfileTrace);
            return db;
            //return this.dbAccess;
        }
    }
}