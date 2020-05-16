using System;

namespace JR.Stand.Core.PluginKernel
{
    /// <summary>
    /// 插件信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PluginHostAttribute : Attribute
    {
        public PluginHostAttribute()
        {
        }

        public PluginHostAttribute(string name, string descript)
        {
            Name = name;
            Descript = descript;
        }

        /// <summary>
        /// 插件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 插件应用描述
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// 插件类名格式（正则表达式）
        /// 默认为空，不限制
        /// </summary>
        public string TypePattern { get; set; }

        /// <summary>
        /// 记录日志
        /// </summary>
        private bool WriteLog { get; set; }
    }
}