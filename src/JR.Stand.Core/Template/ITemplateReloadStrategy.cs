using System;

namespace JR.Stand.Core.Template
{
    /// <summary>
    /// 模板重载策略
    /// </summary>
    public interface ITemplateReloadStrategy
    {
        /// <summary>
        /// 检查是否符合更新条件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Boolean Check(String filePath);
    }
}

