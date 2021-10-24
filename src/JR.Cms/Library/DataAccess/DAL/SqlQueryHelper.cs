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

        public static SqlQuery Create(string sql, object[,] data)
        {
            return new SqlQuery(OptimizeSql(sql), data);
        }

        public static SqlQuery CreateQuery(string sql)
        {
            return new SqlQuery(OptimizeSql(sql));
        }
        
        public static SqlQuery Create(string sql, IDictionary<string, object> data)
        {
            return new SqlQuery(OptimizeSql(sql), data);
        }


        public static SqlQuery Format(string sql, object[,] data)
        {
            return new SqlQuery(OptimizeSql(sql), data);
        }
        
        // .net45及以下不支持,会报错: 找不到方法:“!!0[] System.Array.Empty()”。 
        //public static SqlQuery Format(string sql, object[,] data, params string[] formatValues)
    }
}