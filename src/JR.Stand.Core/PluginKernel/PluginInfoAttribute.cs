using System;

namespace JR.Stand.Core.PluginKernel
{
    /// <summary>
    /// 插件信息
    /// </summary>
    [Obsolete]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PluginInfoAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public PluginInfoAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="author"></param>
        /// <param name="webpage"></param>
        /// <param name="describe"></param>
        [Obsolete]
        public PluginInfoAttribute(string name, string author, string webpage, string descript)
        {
        }

        private string id;

        /// <summary>
        /// 插件编号
        /// </summary>
        public string ID
        {
            get
            {
                if (id == null)
                {
                    id = String.Format("{0}{1}", GetType().Assembly.GetName().Name,
                        IndexNum == 0 ? string.Empty : "_" + IndexNum);
                }
                return id;
            }
        }

        /// <summary>
        /// 插件序号,如果插件文件包含多个插件，则需设置
        /// </summary>
        public int IndexNum { get; set; }

        /// <summary>
        /// 插件其他信息
        /// </summary>
        public object Tag { get; set; }
    }
}