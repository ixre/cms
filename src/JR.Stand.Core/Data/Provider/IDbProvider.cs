using System;
using System.Data;

namespace JR.Stand.Core.Data.Provider
{
    /// <summary>
    /// 数据库提供者,用于向对象提供注入
    /// </summary>
    public interface IDbProvider
    {
        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();
        /// <summary>
        /// 格式化查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        String FormatQuery(String query);
    }
}