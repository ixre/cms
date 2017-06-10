using System;
using System.Text.RegularExpressions;
using JR.Cms.DB;
using JR.DevFw.Data;

namespace JR.Cms.Dal
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
            {
                return SignReg.Replace(sql, m =>
                {
                    switch (m.Groups[1].Value.ToUpper())
                    {
                        case "PREFIX_": return CmsDataBase.TablePrefix;
                    }
                    return string.Empty;
                });
            }
            return sql;
        }

        public static SqlQuery Format2(string sql,params object[] data)
        {
            Object[,] objects = new Object[data.Length, 2];
            int tmpInt=0;
            foreach (Object[] d in data)
            {
                objects[tmpInt, 0] = d[0];
                objects[tmpInt, 1] = d[1];
                tmpInt++;
            }
            return new SqlQuery(OptimizeSql(sql), objects);
        }
        
        public static SqlQuery Create(string sql,object[,] data)
        {
            return new SqlQuery(OptimizeSql(sql), data);
        }

        public static SqlQuery Format(string sql,params string[] formatValues)
        {
            return new SqlQuery(OptimizeSql(String.Format(sql, formatValues)));
        }

        public static SqlQuery Format(string sql, object[,] data, params string[] formatValues)
        {
            string _sql = formatValues.Length == 0 ? sql : String.Format(sql, formatValues);
            _sql = OptimizeSql(_sql);
            return new SqlQuery(_sql, data);
        }
    }
}
