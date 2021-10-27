using System;
using System.Collections.Generic;

namespace JR.Stand.Core.Template.Impl
{
    /// <summary>
    /// 模板变量对象,实现该接口可使用${var.data("key")}的方式动态获取数据
    /// </summary>
    public interface ITemplateVariableInstance
    {
        /// <summary>
        /// 数据字典
        /// </summary>
        IDictionary<String, String> Data { get; }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void AddData(string key, string data);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="key"></param>
        void RemoveData(string key);
    }
}