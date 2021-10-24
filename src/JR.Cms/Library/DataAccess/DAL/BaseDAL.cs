using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using JR.Cms.Infrastructure;
using JR.Cms.Library.DataAccess.DB;
using JR.Cms.Library.DataAccess.SQL;
using JR.Stand.Core.Data;

namespace JR.Cms.Library.DataAccess.DAL
{
    public abstract class DalBase
    {
        public static readonly DbParameter[] EmptyParameter = null;
        private static bool _inited = false;

        /// <summary>
        /// 用于生成参数的数据库访问对象
        /// </summary>
        public static IDbDialect DbFact;

        private static void CheckAndInit()
        {
            if (!_inited)
            {
                var _db = CmsDataBase.Instance;
                if (_db == null) throw new ArgumentNullException("_db");
                DbFact = _db.GetDialect();
                //SQLPack对象
                _inited = true;
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
                    var _db = CmsDataBase.Instance;
                    if (_db == null) throw new ArgumentNullException("_db");
                    _dbType = _db.DbType;
                    _sqlPack = SqlPack.Factory(_db.DbType);
                }

                return _sqlPack;
            }
        }


        /// <summary>
        /// 用于执行操作的数据库访问对象
        /// </summary>
        public DataBaseAccess Db => CmsDataBase.Instance;

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DbType => _dbType;

        /// <summary>
        /// 优化SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        //private static Regex signReg=new Regex("\\$([^\\$]+)\\$");
        private static readonly Regex signReg = new Regex("\\$([^(_|\\s)]+_)");

        private static SqlPack _sqlPack;
        private static DataBaseType _dbType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected string OptimizeSql(string sql)
        {
            return SqlQueryHelper.OptimizeSql(sql);
        }

        /// <summary>
        /// 创建新查询
        /// </summary>
        protected SqlQuery NewQuery(string sql, DbParameter[] parameters)
        {
            return new SqlQuery(OptimizeSql(sql), parameters);
        }

        /// <summary>
        /// 创建新查询
        /// </summary>
        protected SqlQuery CreateQuery(string sql, IDictionary<string, object> parameters)
        {
            return new SqlQuery(OptimizeSql(sql), parameters);
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected int ExecuteNonQuery(SqlQuery sql)
        {
            return Db.ExecuteNonQuery(sql);
        }
        
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected int ExecuteMultiNonQuery(SqlQuery[] sql)
        {
            return Db.ExecuteNonQuery(sql);
        }


        protected object ExecuteScalar(SqlQuery sqlEnt)
        {
            return Db.ExecuteScalar(sqlEnt);
        }


        protected void ExecuteReader(SqlQuery sql, DataReaderFunc func)
        {
            Db.ExecuteReader(sql, func);
        }

        public DataSet GetDataSet(SqlQuery sqlEnt)
        {
            return Db.GetDataSet(sqlEnt);
        }

        protected void CheckSqlInject(string[] values)
        {
            foreach (var key in values)
            {
                if (DataChecker.SqlIsInject(key)) throw new ArgumentException("SQL INCORRECT");
            }
        }
    }
}