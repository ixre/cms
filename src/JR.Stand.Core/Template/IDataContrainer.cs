//
// Copyright 2011 @ S1N1.COM,All right reserved.
// Name:TemplateUtility.cs
// Author:newmin
// Create:2013/09/05
//

using System.Collections.Generic;

namespace JR.Stand.Core.Template
{
    /// <summary>
    /// 数据适配器
    /// </summary>
    public interface IDataAdapter
    {
        object GetItem(string key);
        void SetItem(string key, object value);
        object GetCache(string key);

        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="duration">时间间隔(单位：秒)</param>
        /// <param name="dependFileName">缓存依赖文件路径</param>
        void InsertCache(string key, object value, int duration, string dependFileName);

        /// <summary>
        ///  获取查询参数
        /// </summary>
        /// <param name="varKey"></param>
        /// <returns></returns>
        string GetQueryParam(string varKey);

        /// <summary>
        /// 获取表单参数
        /// </summary>
        /// <param name="varKey"></param>
        /// <returns></returns>
        string GetFormParam(string varKey);
    }

    /// <summary>
    /// IDataContainer接口
    /// </summary>
    public interface IDataContainer
    {
        /// <summary>
        /// 获取数据适配器
        /// </summary>
        /// <returns></returns>
        IDataAdapter GetAdapter();
        /// <summary>
        /// 获取模板页缓存内容
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns></returns>
        string GetTemplatePageCacheContent(string templateId);

        /// <summary>
        /// 设置模板页缓存内容
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="content"></param>
        /// <param name="dependFileName"></param>
        void SetTemplatePageCacheContent(string templateID, string content, string dependFileName);

        /// <summary>
        /// 设置变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void DefineVariable<T>(string key, T variable);

        /// <summary>
        /// 定义变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="variable"></param>
        void DefineVariable(string key, Variable variable);

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetVariable(string key);

        /// <summary>
        /// 获取自定义变量
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> GetDefineVariable();
    }
}