namespace JR.Cms.Domain.Interface.Common.Language
{
    /// <summary>
    /// 语言包键值
    /// </summary>
    public enum LanguagePackageKey : int
    {
        /// <summary>
        /// 没有值,可用于是否为系统语言包的键
        /// </summary>
        // NOKEY = -1,

        /// <summary>
        /// 无标签
        /// </summary>
        PAGE_NO_TAGS,

        /// <summary>
        /// 上一页文字
        /// </summary>
        PAGER_PrePageText,

        /// <summary>
        /// 下一页文字
        /// </summary>
        PAGER_NextPageText,

        /// <summary>
        /// 选择文字
        /// </summary>
        PAGER_SelectPageText,

        /// <summary>
        /// 页面分页标题,如（第几页）
        /// </summary>
        PAGE_PagerTitle,

        /// <summary>
        /// 分页统计信息
        /// </summary>
        PAGER_PagerTotal,

        /// <summary>
        /// 没有上一篇
        /// </summary>
        ARCHIVE_NoPrevious,

        /// <summary>
        /// 没有下一篇
        /// </summary>
        ARCHIVE_NoNext,

        /// <summary>
        /// 文档关键词
        /// </summary>
        ARCHIVE_Tags
    }
}