//
//
//  Copryright 2011 @ S1N1.COM.All rights reserved.
//
//  Project : OPS.Data
//  File Name : DataBaseType.cs
//  Date : 8/19/2011
//  Author : 刘铭
//
//

namespace JR.Stand.Core.Data
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DataBaseType
    {
        SQLServer,

        /// <summary>
        /// SQLite数据库
        /// </summary>
        SQLite,

        /// <summary>
        ///Mono SQLite 
        /// </summary>
        MonoSQLite,

        /// <summary>
        /// mysql数据库
        /// </summary>
        MySQL,

        /// <summary>
        /// Access等数据库
        /// </summary>
        OLEDB
    }
}