using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JR.Cms.Library.DataAccess.DB;
using JR.Stand.Core.Data;

namespace JR.Cms.Library.DataAccess.DAL
{
    public class SqlQueryHelper
    {
        /// <summary>
        /// 优化SQL语句
        /// </summary>
        /// <returns></returns>
        //private static Regex signReg=new Regex("\\$([^\\$]+)\\$");
        private static readonly Regex SignReg = new Regex("\\$([^(_|\\s)]+_)");

        public static string OptimizeSql(string sql)
        {
            if (SignReg.IsMatch(sql))
                return SignReg.Replace(sql, m =>
                {
                    switch (m.Groups[1].Value.ToUpper())
                    {
                        case "PREFIX_": return CmsDataBase.TablePrefix;
                    }

                    return string.Empty;
                });
            return sql;
        }

        public static SqlQuery FormaJR(string sql, params object[] data)
        {
            var objects = new object[data.Length, 2];
            var tmpInt = 0;
            foreach (object[] d in data)
            {
                objects[tmpInt, 0] = d[0];
                objects[tmpInt, 1] = d[1];
                tmpInt++;
            }

            return new SqlQuery(OptimizeSql(sql), objects);
        }

        public static SqlQuery Create(string sql, object[,] data)
        {
            return new SqlQuery(OptimizeSql(sql), data);
        }

        public static SqlQuery Create(string sql, IDictionary<string, object> data)
        {
            return new SqlQuery(OptimizeSql(sql), data);
        }


        public static SqlQuery Format(string sql, params string[] formatValues)
        {
            return new SqlQuery(OptimizeSql(string.Format(sql, formatValues)));
        }

        public static SqlQuery Format(string sql, object[,] data, params string[] formatValues)
        {
            var _sql = formatValues.Length == 0 ? sql : string.Format(sql, formatValues);
            _sql = OptimizeSql(_sql);
            return new SqlQuery(_sql, data);
        }
    }
}