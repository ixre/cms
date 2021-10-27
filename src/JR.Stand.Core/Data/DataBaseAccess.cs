//
//
//  Copryright 2011 @ S1N1.COM.All rights reserved.
//
//  Project : OPS.Data
//  File Name : DataBaseAccess.cs
//  Date : 8/19/2011
//  Author : 刘铭
//
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using JR.Stand.Core.Data.Extensions;

namespace JR.Stand.Core.Data
{
   

    /// <summary>
    /// DatabaseAccess
    /// </summary>
    public class DataBaseAccess
    {
        private static readonly Object locker = new object();
        private readonly IDbDialect dbDialect;
        private static readonly Regex procedureRegex = new Regex("\\s");
        private int _commandTimeout = 30000;
        private IList<Middleware> mwList = new List<Middleware>();

        /// <summary>
        /// 实例化数据库访问对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="connectionString"></param>
        public DataBaseAccess(DataBaseType type, string connectionString)
        {
            if (connectionString.IndexOf("$ROOT$") != -1)
            {
                connectionString = connectionString.Replace("$ROOT$", 
                    EnvUtil.GetBaseDirectory());
            }

            this.DbType = type;

            switch (type)
            {
                case DataBaseType.OLEDB:
                    dbDialect = new OleDbFactory(connectionString);
                    break;
                case DataBaseType.SQLite:
                    dbDialect = new SQLiteFactory(connectionString);
                    break;
                case DataBaseType.MonoSQLite:
                    throw new Exception("not implement on .net standard");
                //    dbDialect = new MonoSQLiteFactory(connectionString);
                case DataBaseType.SQLServer:
                    dbDialect = new SqlServerFactory(connectionString);
                    break;
                case DataBaseType.MySQL:
                    dbDialect = new MySqlFactory(connectionString);
                    break;
            }
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DbType { get; private set; }

        /// <summary>
        /// 执行命令超时时间，默认为30000(30秒)
        /// </summary>
        public int CommandTimeout
        {
            get { return this._commandTimeout; }
            set
            {
                if (value <= 2000)
                {
                    throw new ArgumentException("无效数值");
                }
                this._commandTimeout = value;
            }
        }

        /// <summary>
        /// Use a bew middleware and append to list of middleware.
        /// </summary>
        /// <param name="mw"></param>
        public void Use(Middleware mw)
        {
            if(mw != null) this.mwList.Add(mw);
        }

        /// <summary>
        /// call middlewares
        /// </summary>
        /// <param name="action"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="exc"></param>
        /// <returns></returns>
        private bool callMiddleware(String action,String sql,DbParameter[] parameters,Exception exc)
        {
            foreach (Middleware w in this.mwList)
            {
                if (!w(action,sql, parameters, exc)) return false;

            }
            return true;
        }

        /// <summary>
        /// create new database connection
        /// </summary>
        /// <returns></returns>
        private DbConnection createNewConnection()
        {
            DbConnection connection = dbDialect.GetConnection();
            if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
            {
                connection.Open();
            }
            return connection;
        }


        /// <summary>
        /// 数据库适配器
        /// </summary>
        public IDbDialect GetDialect()
        {
            return this.dbDialect;
        }

      

        private DbCommand CreateCommand(string sql)
        {
            DbCommand cmd = this.dbDialect.CreateCommand(sql);
            cmd.CommandTimeout = this._commandTimeout;
            return cmd;
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText)
        {
            return this.ExecuteNonQuery(new SqlQuery(commandText));
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText, params DbParameter[] parameters)
        {
            return this.ExecuteNonQuery(new SqlQuery(commandText, parameters));
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DbParameter CreateParameter(String name,object value)
        {
            return this.GetDialect().CreateParameter(name, value);
        }

        /// <summary>
        /// 将多纬数组转换为参数
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete]
        public DbParameter[] CreateParametersFromArray(object[,] parameters)
        {
            return DataUtil.ToParams(this.GetDialect(), parameters);
        }

        /// <summary>
        /// 返回查询的第一行第一列值
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText)
        {
            return this.ExecuteScalar(new SqlQuery(commandText));
        }

        /// <summary>
        /// 返回查询的第一行第一列值
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, params DbParameter[] parameters)
        {
            return this.ExecuteScalar(new SqlQuery(commandText,parameters));
        }

        /// <summary>
        /// 读取DataReader中的数据
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="func"></param>
        public void ExecuteReader(string commandText, DataReaderFunc func)
        {
            this.ExecuteReader(new SqlQuery(commandText), func);
        }

        /// <summary>
        /// 读取DataReader中的数据
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="func"></param>
        /// <param name="parameters"></param>
        public void ExecuteReader(string commandText, DataReaderFunc func, params DbParameter[] parameters)
        {
            this.ExecuteReader(new SqlQuery(commandText, parameters), func);
        }

        /// <summary>
        /// 从数据库中读取数据并保存在内存中
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string commandText)
        {
            return this.GetDataSet(new SqlQuery(commandText));
        }

        /// <summary>
        /// 从数据库中读取数据并保存在内存中
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string commandText, params DbParameter[] parameters)
        {
            return this.GetDataSet(new SqlQuery(commandText, parameters));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public T ToEntity<T>(string commandText) where T : new()
        {
            return ToEntity<T>(commandText, null);
        }

        /// <summary>
        /// 将查询结果转换为实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ToEntity<T>(string commandText, params DbParameter[] parameters) where T : new()
        {
            T t = default(T);
            ExecuteReader(new SqlQuery(commandText,parameters), (reader) =>
            {
                if (reader.HasRows)
                {
                    t = reader.ToEntity<T>();
                }
            });
            return t;
        }

        /// <summary>
        /// 以DataReader返回数据并转换成实体类集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IList<T> ToEntityList<T>(string commandText) where T : new()
        {
            return ToEntityList<T>(commandText, null);
        }

        /// <summary>
        /// 以DataReader返回数据并转换成实体类集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IList<T> ToEntityList<T>(string commandText, params DbParameter[] parameters) where T : new()
        {
            IList<T> list = null;
            ExecuteReader(new SqlQuery(commandText,parameters), (reader) =>
            {
                if (reader.HasRows)
                {
                    list = reader.ToEntityList<T>();
                }
            });
            return list ?? new List<T>();
        }

        #region  新的连结方式

        /// <summary>
        /// 执行脚本(仅Mysql)
        /// </summary>
        /// <param name="sql">sql脚本</param>
        /// <param name="delimiter">分割符，可传递空</param>
        /// <returns></returns>
        public int ExecuteScript(string sql, string delimiter)
        {
            int result = -1;
            using (DbConnection conn = this.createNewConnection())
            {
                try
                {
                    result = dbDialect.ExecuteScript(conn,this.ExecuteNonQuery, sql, delimiter);
                    this.callMiddleware("ExecuteScript", sql, null, null);
                }
                catch (Exception ex)
                {
                    this.callMiddleware("ExecuteScript", sql, null, ex);
                    throw ex;
                }
            }
            return result;
        }

        /// <summary>
        /// 执行查询操作
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private int executeNonQuery(DbConnection conn,DbTransaction trans, SqlQuery s)
        {
            //创建Command,并设置连接
            DbCommand cmd = this.CreateCommand(s.Sql);
            cmd.Connection = conn;
            //绑定事务
            cmd.Transaction = trans;
            //自动判断是T-SQL还是存储过程
            cmd.CommandType = procedureRegex.IsMatch(s.Sql)? CommandType.Text: CommandType.StoredProcedure;
            //添加参数
            cmd.Parameters.AddRange(s.Parameters);

            int result = 0;
            try {
                //SQLite不支持并发写入
                if (this.DbType == DataBaseType.SQLite)
                {
                    Monitor.Enter(locker);
                    result = cmd.ExecuteNonQuery();
                    Monitor.Exit(locker);
                }
                else
                {
                    result = cmd.ExecuteNonQuery();
                }
                this.callMiddleware("ExecuteNonQuery", s.Sql, s.Parameters, null);
            }
            catch (Exception ex)
            {
                this.callMiddleware("ExecuteNonQuery", s.Sql, s.Parameters, ex);
                throw ex;
            }
            finally
            {
                cmd.Parameters.Clear(); // 相同的参数数组只能给一个SqlCommand使用
                cmd.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(params SqlQuery[] sqls)
        {
            if (sqls.Length == 0) throw new ArgumentOutOfRangeException("sqls", "SQLEntity至少应指定一个!");
            int result = 0;
            DbConnection conn = this.createNewConnection();
          
            //使用事务
            DbTransaction trans = conn.BeginTransaction();
            try
            {
                foreach (SqlQuery sql in sqls)
                {
                    sql.Parse(this.GetDialect());
                    result += this.executeNonQuery(conn,trans,sql);
                }
                //this.callMiddleware("提交事务", "", null, null);
                //提交事务
                trans.Commit();
                //this.callMiddleware("提交事务成功,关闭CMD", "", null, null);
                //this.callMiddleware("关闭CMD成功,关闭CONN", "", null, null);
                conn.Close();
                //this.callMiddleware("关闭CONN成功", "", null, null);
            }
            catch (DbException ex)
            {
                //this.callMiddleware("回滚事务", "", null, null);
                //如果用事务执行,则回滚
                trans.Rollback();
                //this.callMiddleware("回滚关闭CMD成功,关闭CONN", "", null, null);
                conn.Close();
                //this.callMiddleware("回滚关闭CONN在功", "", null, null);
                //重新抛出异常
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 读取DataReader中的数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        public void ExecuteReader(SqlQuery sql, DataReaderFunc func)
        {
            sql.Parse(this.GetDialect());
            DbConnection conn = this.createNewConnection();
            DbCommand cmd = this.CreateCommand(sql.Sql);
            cmd.Connection = conn;
            //自动判断是T-SQL还是存储过程
            cmd.CommandType = procedureRegex.IsMatch(sql.Sql) ? CommandType.Text : CommandType.StoredProcedure;
            if (sql.Parameters != null) cmd.Parameters.AddRange(sql.Parameters);
            DbDataReader rd = null;
            try
            {
                rd = cmd.ExecuteReader();
                this.callMiddleware("ExecuteReader", sql.Sql, sql.Parameters, null);
            }
            catch (Exception ex)
            {
                this.callMiddleware("ExecuteReader", sql.Sql, sql.Parameters, ex);
                cmd.Dispose();
                conn.Close();
                throw ex;
            }
            func(rd);
            cmd.Parameters.Clear(); // 相同的参数数组只能给一个SqlCommand使用
            cmd.Dispose();
            conn.Close();
        }


        /// <summary>
        /// 从数据库中读取数据并保存在内存中
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet GetDataSet(SqlQuery sql)
        {
            sql.Parse(this.GetDialect());
            DataSet ds = new DataSet();
            using (DbConnection conn = this.createNewConnection())
            {
                DbDataAdapter adapter = dbDialect.CreateDataAdapter(conn, sql.Sql);
                if (sql.Parameters != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(sql.Parameters);
                    //自动判断是T-SQL还是存储过程
                    adapter.SelectCommand.CommandType = procedureRegex.IsMatch(sql.Sql)
                        ? CommandType.Text
                        : CommandType.StoredProcedure;
                }
                try
                {
                    adapter.Fill(ds);
                    this.callMiddleware("GetDataSet", sql.Sql, sql.Parameters, null);
                }
                catch (Exception ex)
                {
                    this.callMiddleware("GetDataSet", sql.Sql, sql.Parameters, ex);
                    throw ex;
                }
            }

            return ds;
        }


        /// <summary>
        /// 返回查询的第一行第一列值
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExecuteScalar(SqlQuery sql)
        {
            sql.Parse(this.GetDialect());
            DbConnection conn = this.createNewConnection();
            DbCommand cmd = this.CreateCommand(sql.Sql);
            cmd.Connection = conn;

            //自动判断是T-SQL还是存储过程
            cmd.CommandType = procedureRegex.IsMatch(sql.Sql)
                ? CommandType.Text
                : CommandType.StoredProcedure;

            if (sql.Parameters != null) cmd.Parameters.AddRange(sql.Parameters);
            try
            {
                Object obj = cmd.ExecuteScalar();
                cmd.Parameters.Clear(); // 相同的参数数组只能给一个SqlCommand使用
                this.callMiddleware("ExecuteScalar", sql.Sql, sql.Parameters, null);
                cmd.Dispose();
                conn.Close();
                return obj;
            }
            catch (Exception ex)
            {
                this.callMiddleware("ExecuteScalar", sql.Sql, sql.Parameters, ex);
                cmd.Dispose();
                conn.Close();
                throw ex;
            }
        }

        /// <summary>
        /// 将查询结果转换为实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">命令</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public T ToEntity<T>(SqlQuery sql) where T : new()
        {
            T t = default(T);
            this.ExecuteReader(sql, (reader) =>
            {
                if (reader.HasRows)
                {
                    t = reader.ToEntity<T>();
                }
            });
            return t;
        }

        /// <summary>
        /// 以DataReader返回数据并转换成实体类集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IList<T> ToEntityList<T>(SqlQuery sql) where T : new()
        {
            IList<T> list = null;
            this.ExecuteReader(sql, (reader) =>
            {
                if (reader.HasRows)
                {
                    list = reader.ToEntityList<T>();
                }
            });
            return list ?? new List<T>();
        }

        #endregion

        #region Hashtable获取数据

        public int ExecuteNonQuery(string commandText, Hashtable data)
        {
            var parameters = GetParametersFromHashTable(data);
            return this.ExecuteNonQuery(commandText, parameters);
        }

        public object ExecuteScalar(string commandText, Hashtable data)
        {
            return this.ExecuteScalar(commandText, this.GetParametersFromHashTable(data));
        }

        public void ExecuteReader(string commandText, Hashtable data, DataReaderFunc func)
        {
            this.ExecuteReader(commandText, func, this.GetParametersFromHashTable(data));
        }

        public DataSet GetDataSet(string commandText, Hashtable data)
        {
            return this.GetDataSet(commandText, this.GetParametersFromHashTable(data));
        }

        private DbParameter[] GetParametersFromHashTable(Hashtable data)
        {
            DbParameter[] parameters = new DbParameter[data.Keys.Count];

            int i = 0;
            foreach (DictionaryEntry d in data)
            {
                parameters[i++] = this.GetDialect().CreateParameter("@" + d.Key, d.Value);
            }
            return parameters;
        }

        #endregion

        /*
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (conn!=null && conn.State != ConnectionState.Closed)
            {
                conn.Dispose();
            }
        }

        ~DataBaseAccess()
        {
            this.Dispose();
        }
		 */
    }
}