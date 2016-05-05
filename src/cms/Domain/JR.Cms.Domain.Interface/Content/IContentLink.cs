

namespace JR.Cms.Domain.Interface.Content
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContentLink:IValueObject
    {
        int Id { get; set; }

        /// <summary>
        /// 关联内容编号
        /// </summary>
        int ContentId { get; set; }


        /// <summary>
        /// 链接的内容类型
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// 关联类型
        /// </summary>
        int RelatedIndent { get; set; }

        int RelatedSiteId { get; set; }

        int RelatedContentId { get; set; }

        bool Enabled { get; set; }

        string ToHtml();
    }
}
