
namespace JR.Cms.Domain.Interface.Models
{
    /// <summary>
    /// 站内标签
    /// </summary>
    public class SiteTag
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Tag { get; set; }
        
        /// <summary>
        /// 链接地址
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}