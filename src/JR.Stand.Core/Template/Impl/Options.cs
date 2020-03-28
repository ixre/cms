//
// Copyright 2011 @ S1N1.COM,All right reseved.
// Name:ITemplate.cs
// Author:newmin
// Create:2011/06/05
//

namespace JR.Stand.Core.Template.Impl
{
    public delegate void TemplatePageHandler(TemplatePage page);

    /// <summary>
    /// 模板选项参数
    /// </summary>
    public class Options
    {
        /// <summary>
        /// 　版本号
        /// </summary>
        public readonly string Version = "2.1";

        /// <summary>
        /// 是否启用Html压缩
        /// </summary>
        public bool EnabledCompress = true;

        /// <summary>
        /// 是否将模板缓存
        /// </summary>
        public  bool EnabledCache = true;

        /// <summary>
        /// 是否共享URL参数值
        /// </summary>
        public  bool UrlQueryShared = true;

        /// <summary>
        /// 共享HttpItem传递的数据
        /// </summary>
        public  bool HttpItemShared = true;

        /// <summary>
        /// 模板命名规则
        /// </summary>
        public TemplateNames Names　= TemplateNames.ID;
    }
}