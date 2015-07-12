using System;
using System.Text.RegularExpressions;
using J6.Cms.DB;
using J6.DevFw.Data;

namespace J6.Cms.DAL
{
    public class SqlQueryHelper
    {
        /// <summary>
        /// 优化SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        //private static Regex signReg=new Regex("\\$([^\\$]+)\\$");
        private static readonly Regex signReg = new Regex("\\$([^(_|\\s)]+_)");

        public static string OptimizeSQL(string sql)
        {
            if (signReg.IsMatch(sql))
            {
                return signReg.Replace(sql, m =>
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
            return new SqlQuery(OptimizeSQL(sql), objects);
        }
        
        public static SqlQuery Create(string sql,object[,] data)
        {
            return new SqlQuery(OptimizeSQL(sql), data);
        }

        public static SqlQuery Format(string sql,params string[] formatValues)
        {
            return new SqlQuery(String.Format(OptimizeSQL(sql), formatValues));
        }

        public static SqlQuery Format(string sql, object[,] data, params string[] formatValues)
        {
            string _sql = OptimizeSQL(sql);
            _sql = formatValues.Length == 0 ? _sql : String.Format(_sql, formatValues);
            return new SqlQuery(_sql, data);
        }
    }
}
