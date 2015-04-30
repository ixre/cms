
namespace AtNet.Cms.Domain.Interface.Site.Link
{
    /// <summary>
    /// 链接
    /// </summary>
    public interface ISiteLink:IDomain<int>
    {
        /// <summary>
        /// 父级链接
        /// </summary>
        int Pid { get; set; }

        /// <summary>
        /// 链接类型
        /// </summary>
        SiteLinkType Type { get; set; }

        /// <summary>
        /// 链接文本
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        string Uri { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        string ImgUrl { get; set; }

        /// <summary>
        /// 链接目标
        /// </summary>
        string Target { get; set; }

        /// <summary>
        /// 链接顺序
        /// </summary>
        int OrderIndex { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// 绑定信息
        /// </summary>
        string Bind { get; set; }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        int Save();
    }
}
