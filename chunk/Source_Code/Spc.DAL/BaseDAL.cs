using Ops.Cms.DB;
using Ops.Data;
using Spc.Sql;
using System.Data;
using System.Text.RegularExpressions;

namespace Ops.Cms.DAL
{
    public abstract class DALBase
    {
        private DataBaseAccess _db;

        private static string _table_prefix;

        /// <summary>
        /// 用于生成参数的数据库访问对象
        /// </summary>
        public static IDataBase DbFact;

        
        /// <summary>
        /// SQL脚本包对象
        /// </summary>
        protected static SqlPack SP;

        protected  static DataBaseType type;

        static DALBase()
        {
            DataBaseAccess _db = CmsDataBase.Main;
            DbFact = _db.DataBaseAdapter;
            _table_prefix = CmsDataBase.TablePrefix;
            //SQLPack对象
            SP = SqlPack.Factory(_db.DbType);
        }

        /// <summary>
        /// 用于执行操作的数据库访问对象
        /// </summary>
        public DataBaseAccess db
        {
            get
            {
                return CmsDataBase.Main;
            }
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DbType { get { return type; } }



        /// <summary>
        /// 优化SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        //private static Regex signReg=new Regex("\\$([^\\$]+)\\$");
        private static Regex signReg=new Regex("\\$([^(_|\\s)]+_)");

        internal string OptimizeSQL(string sql)
        {
            
        	if(signReg.IsMatch(sql))
            {
                return signReg.Replace(sql, m =>
                {
                    switch (m.Groups[1].Value.ToUpper())
                    {
                        case "PREFIX_": return _table_prefix;
                        /*
                        case "SITE":
                            if(Cms.MultSiteVersion)
                            {
                                Site site = Cms.Context.CurrentSite;
                                return " siteid=" + site.SiteId.ToString();
                            }
                            return null;

                        case "ANDSITE":
                            if(Cms.MultSiteVersion)
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

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal int ExecuteNonQuery(params SqlQuery[] sql)
        {
            int result=db.ExecuteNonQuery(sql);
            //db.CloseConn();
            return result;
        }

        internal object ExecuteScalar(SqlQuery sqlEnt)
        {
            object result = db.ExecuteScalar(sqlEnt);
            //db.CloseConn();
            return result;
        }


        internal void ExecuteReader(SqlQuery sql, DataReaderFunc func)
        {
            db.ExecuteReader(sql, func);
           // db.CloseConn();
        }

        public DataSet GetDataSet(SqlQuery sqlEnt)
        {
            DataSet ds = db.GetDataSet(sqlEnt);
            //db.CloseConn();
            return ds;

        }
    }
}