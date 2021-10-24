using System;

namespace JR.Cms.Domain.Interface.Site.Category
{
    /// <summary>
    /// 栏目标记
    /// </summary>
    [Flags]
    public enum CategoryFlag : int
    {
        /// <summary>
        /// 启用的栏目
        /// </summary>
        Enabled = 1 << 0,

        /// <summary>
        /// 重定向栏目
        /// </summary>
        Redirect = 1 << 1,

        /// <summary>
        /// 锁定的栏目
        /// </summary>
        Locked = 1 << 2
    }
}