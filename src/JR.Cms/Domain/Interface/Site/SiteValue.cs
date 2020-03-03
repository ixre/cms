// this file is auto generated!

using JR.DevFw.Data.Orm.Mapping;
using JR.Stand.Core.Framework;

namespace JR.Cms.Models{

    /// <summary>
    /// 站点
    /// </summary>
    [DataTable("cms_site")]
    public class CmsSiteEntity
    {
        /// <summary>
        /// 站点编号
        /// </summary>
        [Alias("site_id")]
        public int SiteId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Alias("name")]
        public string Name { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        [Alias("domain")]
        public string Domain { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        [Alias("app_name")]
        public string AppPath { get; set; }
        /// <summary>
        /// 重定向URL
        /// </summary>
        [Alias("location")]
        public string Location { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        [Alias("language")]
        public int Language { get; set; }
        /// <summary>
        /// 模板
        /// </summary>
        [Alias("tpl")]
        public string Tpl { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Alias("note")]
        public string Note { get; set; }
        /// <summary>
        /// SEO标题
        /// </summary>
        [Alias("seo_title")]
        public string SeoTitle { get; set; }
        /// <summary>
        /// SEO关键词
        /// </summary>
        [Alias("seo_keywords")]
        public string SeoKeywords { get; set; }
        /// <summary>
        /// SEO描述
        /// </summary>
        [Alias("seo_description")]
        public string SeoDescription { get; set; }
        /// <summary>
        /// 站点状态
        /// </summary>
        [Alias("state")]
        public int State { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        [Alias("pro_tel")]
        public string ProTel { get; set; }
        /// <summary>
        /// 移动电话
        /// </summary>
        [Alias("pro_phone")]
        public string ProPhone { get; set; }
        /// <summary>
        /// 传真
        /// </summary>
        [Alias("pro_fax")]
        public string ProFax { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [Alias("pro_address")]
        public string ProAddress { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [Alias("pro_email")]
        public string ProEmail { get; set; }
        /// <summary>
        /// 即时通讯
        /// </summary>
        [Alias("pro_im")]
        public string ProIm { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        [Alias("pro_post")]
        public string ProPost { get; set; }
        /// <summary>
        /// 公告
        /// </summary>
        [Alias("pro_notice")]
        public string ProNotice { get; set; }
        /// <summary>
        /// 标语
        /// </summary>
        [Alias("pro_slogan")]
        public string ProSlogan { get; set; }
    }
}