/*
 * Copyright 2010 OPS,INC.All rights reseved!
 * Create by newmin at 2010.11.05 07:54
 */
namespace Ops.Cms.DAL
{
    using System;
    using System.Web;
    using System.Data.Common;
    using Ops.Data;

    public static class DbHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private static string connectionString;

        /// <summary>
        /// 数据库类型
        /// </summary>
        //（*）ArchiveUtility类需要使用数据库,所以设为公共
        internal static DataBaseType DbType;


        /// <summary>
        /// 数据库访问对象
        /// </summary>
        internal static DataBaseAccess db
        {
            get
            {
                if (String.IsNullOrEmpty(connectionString))
                {
                    throw new NullReferenceException("请检查系统是否被授权或使用ApplicationManager.InitializeDataSource初始化数据库连接");
                }

                return new DataBaseAccess(DbType,connectionString);
            }
        }



        /// <summary>
        /// 初始化数据库
        /// </summary>
        public static void Initialize(DataBaseType type,string _connectionString)
        {
            DbHelper.connectionString = _connectionString;
            DbType = type;

            //初始化DALBase对象属性
            DALBase.Initialize(type, _connectionString);
            
            //清除数据
            //WatchServiceRegister.Register();
        }
    }
}
