namespace JR.Cms.Domain.Interface.Site
{
    public enum SiteRunType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 独立域名运行
        /// </summary>
        Stand = 1,

        /// <summary>
        /// 虚拟目录运行
        /// </summary>
        VirtualDirectory = 2
    }
}