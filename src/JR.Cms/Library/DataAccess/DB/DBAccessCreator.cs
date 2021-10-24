using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JR.Stand.Core;
using JR.Stand.Core.Data;

namespace JR.Cms.Library.DataAccess.DB
{
    internal static class DbAccessCreator
    {
        /// <summary>
        /// 初始化数据源
        /// </summary>
        /// <param name="connectionString"></param>
        public static DataBaseAccess GetDbAccess(string connectionString, ref DataBaseType type)
        {
            // ======= 初始化 ========
            var dbType = default(DataBaseType); //数据库类型

            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connString", "连接字符串不允许为空");

            IDictionary<DataBaseType, string> dbPatternDictionary = new Dictionary<DataBaseType, string>(4);
            dbPatternDictionary.Add(DataBaseType.SQLite,
                "^sqlite://\\s*Data\\s+Source=(\\$ROOT)*.+$"); //SQLite数据库链接字符串正则匹配模式
            dbPatternDictionary.Add(DataBaseType.MonoSQLite,
                "^monosqlite://\\s*Data\\s+Source=(\\$ROOT)*.+$"); //Mono SQLite数据库链接字符串正则匹配模式
            dbPatternDictionary.Add(DataBaseType.OLEDB,
                "oledb://\\s*Provider\\s*=\\s*Microsoft.JET.OLEDB"); //OLEDB类型数据库链接字符串正则匹配模式
            dbPatternDictionary.Add(DataBaseType.SQLServer, "mssql://\\s*Server=[^;]+;"); //SQL SERVER
            dbPatternDictionary.Add(DataBaseType.MySQL, "mysql://\\s*Server=[^;]+;"); //MySql

            var hasMatched = false;

            foreach (var db in dbPatternDictionary)
                //如果与字典中相匹配则设置数据库类型
                if (Regex.IsMatch(connectionString, db.Value, RegexOptions.IgnoreCase))
                {
                    dbType = db.Key;
                    hasMatched = true;
                    break;
                }

            if (!hasMatched)
                throw new ArgumentException(connectionString + @"

                        连接字符串格式无法被系统识别!请参阅以下格式修改：

                        SQLite数据库：sqlite://Data Source=C:\data\#db.db3
                        MS SQLServer：mssql://Server=.;DataBase=db;uid=sa;pwd=123456
                        Oracle MySQL：mysql://Server=.;DataBase=db;uid=sa;pwd=123456;port=3306
                        Access数据库：oledb://Provider=Microsoft.JET.OLEDB.4.0;Data Source=C:\data\#db.mdb

                    ");

            //---------- 初始化DbHelper对象属性 ------------------//

            //替换字符串的$ROOT为网站根目录
            var reg = new Regex("\\$ROOT", RegexOptions.IgnoreCase);
            if (reg.IsMatch(connectionString))
            {
                var baseDir = EnvUtil.GetBaseDirectory();
                connectionString = reg.Replace(connectionString, baseDir);
            }


            //去掉协议
            connectionString = connectionString.Substring(connectionString.IndexOf("://") + 3);

            type = dbType;

            return new DataBaseAccess(dbType, connectionString);
        }


        /// <summary>
        /// 创建数据访问
        /// </summary>
        /// <param name="type"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static DataBaseAccess GetDbAccess(DataBaseType type, string connectionString)
        {
            return new DataBaseAccess(type, connectionString);
        }
    }
}