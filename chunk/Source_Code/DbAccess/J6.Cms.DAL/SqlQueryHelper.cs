using System;
using System.Text.RegularExpressions;
using J6.Cms.DB;
using J6.DevFw.Data;

namespace J6.Cms.Dal
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
                        /*
                        case "SITE":
                            if(j6.MultSiteVersion)
                            {
                                Site site = Cms.Context.CurrentSite;
                                return " siteid=" + site.SiteId.ToString();
                            }
                            return null;

                        case "ANDSITE":
                            if(j6.MultSiteVersion)
                            {
                                Site site = Cms.Context.CurrentSite;
                                return " AND siteid=" + site.SiteId.ToString();
                            }
                            return null;
                        */
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
            return new SqlQuery(String.Format(OptimizeSql(sql), formatValues));
        }

        public static SqlQuery Format(string sql, object[,] data, params string[] formatValues)
        {
            string _sql = OptimizeSql(sql);
            _sql = formatValues.Length == 0 ? _sql : String.Format(_sql, formatValues);
            return new SqlQuery(_sql, data);
        }
    }
}
