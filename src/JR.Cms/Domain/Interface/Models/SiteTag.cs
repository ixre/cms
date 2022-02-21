using NHibernate.Mapping.Attributes;

namespace JR.Cms.Domain.Interface.Models
{
    /// <summary>
    /// 站内标签
    /// </summary>
    [Class(Table = "cms_site_tag", Lazy = true)]
    public class SiteTag
    {
        /// <summary>
        /// ID
        /// </summary>
        [Generator(Class = "assigned")]
        [Property(Column = "id",NotNull = true)]
        public long Id { get; set; }

        /// <summary>
        /// 站点编号
        /// </summary>
        [Property(Column = "site_id",NotNull = true)]
        public long SiteId { get; set; }
        
        /// <summary>
        /// 标签名称
        /// </summary>
        [Property(Column = "tag",NotNull = true,Length = 20)]
        public string Tag { get; set; }
        
        /// <summary>
        /// 链接地址
        /// </summary>
        [Property(Column = "url",NotNull = true,Length = 180)]
        public string Url { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [Property(Column = "description",NotNull = true,Length = 120)]
        public string Description { get; set; }
    }
}