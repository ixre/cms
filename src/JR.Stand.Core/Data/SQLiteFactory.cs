//
//
//  Copryright 2011 @ S1N1.COM.All rights reseved.
//
//  Project : OPS.Data
//  File Name : SQLiteFactory.cs
//  Date : 8/19/2011
//  Author : 刘铭
//
//

// https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki
// 稳定版：
// http://system.data.sqlite.org/downloads/1.0.95.0/sqlite-netFx40-binary-bundle-Win32-2010-1.0.95.0.zip
// http://system.data.sqlite.org/downloads/1.0.95.0/sqlite-netFx40-binary-bundle-x64-2010-1.0.95.0.zip

using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;

namespace JR.Stand.Core.Data
{
    public class SQLiteFactory : IDbDialect
    {
        private string connectionString;

        public SQLiteFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public  DbConnection GetConnection()
        {
            return new SQLiteConnection(this.connectionString);
        }

        public DbParameter CreateParameter(string name, object value)
        {
            return new SQLiteParameter(name, value);
        }

        public DbCommand CreateCommand(string sql)
        {
            return new SQLiteCommand(sql);
        }

        public DbDataAdapter CreateDataAdapter(DbConnection connection, string sql)
        {
            return new SQLiteDataAdapter(sql, (SQLiteConnection) connection);
        }

        public int ExecuteScript(DbConnection conn, RowAffect r, string sql, string delimiter)
        {
            int result = 0;
            string[] array = sql.Split(';');
            foreach (string s in array)
            {
                result += r(s);
            }
            return result;
        }

        public string GetConnectionString()
        {
            return this.connectionString;
        }

        public DbParameter[] ParseParameters(IDictionary<string, object> paramMap)
        {
            return DataUtil.ParameterMapToArray(this, paramMap);
        }
    }
}