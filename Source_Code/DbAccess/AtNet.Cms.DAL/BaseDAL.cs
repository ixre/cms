using System.Data;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AtNet.Cms.DB;
using AtNet.Cms.Sql;
using AtNet.DevFw.Data;

namespace AtNet.Cms.DAL
{
    public abstract class DALBase
    {
        private static bool inited = false;

        /// <summary>
        /// 用于生成参数的数据库访问对象
        /// </summary>
        public static IDataBase DbFact;

        private static void CheckAndInit()
        {
            if (!inited)
            {
                DataBaseAccess _db = CmsDataBase.Instance;
                DbFact = _db.DataBaseAdapter;
                //SQLPack对象
                inited = true;
            }
        }

        /// <summary>
        /// SQL脚本包对象
        /// </summary>
        protected SqlPack DbSql
        {
            get
            {
                if (_sqlPack == null)
                {
                    DataBaseAccess _db = CmsDataBase.Instance;
                    _dbType = _db.DbType;
                   _sqlPack=  SqlPack.Factory(_db.DbType);

                }
                return _sqlPack;
            }
        }

       
        /// <summary>
        /// 用于执行操作的数据库访问对象
        /// </summary>
        public DataBaseAccess db
        {
            get
            {
                return CmsDataBase.Instance;
            }
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DbType { get { return _dbType; } }

        /// <summary>
        /// 优化SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        //private static Regex signReg=new Regex("\\$([^\\$]+)\\$");
        private static Regex signReg=new Regex("\\$([^(_|\\s)]+_)");
        private static SqlPack _sqlPack;
        private static DataBaseType _dbType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected string OptimizeSql(string sql)
        {
        	if(signReg.IsMatch(sql))
            {
                return signReg.Replace(sql, m =>
                {
                    switch (m.Groups[1].Value.ToUpper())
                    {
                        case "PREFIX_": return CmsDataBase.TablePrefix;
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
        protected int ExecuteNonQuery(params SqlQuery[] sql)
        {
            int result=this.db.ExecuteNonQuery(sql);
            //db.CloseConn();
            return result;
        }

        protected object ExecuteScalar(SqlQuery sqlEnt)
        {
            object result = db.ExecuteScalar(sqlEnt);
            //db.CloseConn();
            return result;
        }


        protected void ExecuteReader(SqlQuery sql, DataReaderFunc func)
        {
            this.db.ExecuteReader(sql, func);
           // db.CloseConn();
        }

        public DataSet GetDataSet(SqlQuery sqlEnt)
        {
            DataSet ds = this.db.GetDataSet(sqlEnt);
            //db.CloseConn();
            return ds;

        }
    }
}