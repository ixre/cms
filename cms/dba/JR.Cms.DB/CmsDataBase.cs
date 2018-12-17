//
// Copyright (C) 2007-2008 TO2.NET,All rights reseved.
// 
// Project: jr.Cms
// FileName : CmsDataBase.cs
// author : PC-CWLIU (new.min@msn.com)
// Create : 2013/06/23 14:53:11
// Description :
//
// Get infromation of this software,please visit our site http://to2.net/cms
//
//

using System;
using JR.DevFw;
using JR.DevFw.Data;

namespace JR.Cms.DB
{
    /// <summary>
    /// Cms主要数据库访问
    /// </summary>
    public static class CmsDataBase
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private static DbAccess _instance;

        /// <summary>
        /// 数据库访问对象
        /// </summary>
        public static DataBaseAccess Instance
        {
            get
            {
                CheckDbAccessInstance();
                return _instance.CreateInstance();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string TablePrefix
        {
            get
            {
                CheckDbAccessInstance();
                return _instance.TablePrefix;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void CheckDbAccessInstance()
        {
            if (_instance == null)
            {
                throw new Exception("数据库连接不可用，请重新初始化！");
            }
        }


        /// <summary>
        /// 初始化数据库
        /// </summary>
        public static void Initialize(string connectionString,string dataTablePrefix, bool sqlTrace)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new NullReferenceException("请检查系统是否被授权或使用CmsDataBase.Initialize初始化数据库连接");
            }
            connectionString = connectionString.Replace("$ROOT",FwCtx.PhysicalPath);
            DataBaseType dbType=DataBaseType.MySQL;
            DataBaseAccess db= DbAccessCreator.GetDbAccess(connectionString, ref dbType);
           

            _instance = new DbAccess(dbType, db.GetDialect().GetConnectionString(),sqlTrace);
            _instance.TablePrefix = dataTablePrefix;
            //测试数据库连接
            testDbConnection(_instance);
        }

        private static void testDbConnection(DbAccess _instance)
        {
            DataBaseAccess dba = _instance.CreateInstance();
            try
            {
                dba.ExecuteScalar("SELECT 1");
            }
            catch (Exception exc)
            {
                throw new Exception("[" + dba.DbType.ToString() + "]数据库连接失败！"+exc.Message);
            }
        }



    }
}
