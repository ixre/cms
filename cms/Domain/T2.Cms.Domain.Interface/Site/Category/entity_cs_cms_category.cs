// this file is auto generated!

using JR.DevFw.Framework;
using JR.DevFw.Data.Orm.Mapping;

namespace T2.Cms.Models{

    /// <summary>
    /// 分类
    /// </summary>
    [DataTable("cms_category")]
    public class CmsCategoryEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Alias("id")]
        public int ID { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        [Alias("tag")]
        public string Tag { get; set; }
        /// <summary>
        /// 站点编号
        /// </summary>
        [Alias("site_id")]
        public int SiteId { get; set; }
        /// <summary>
        /// 上级编号
        /// </summary>
        [Alias("parent_id")]
        public int ParentId { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        [Alias("code")]
        public string Code { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        [Alias("path")]
        public string Path { get; set; }
        /// <summary>
        /// 标记
        /// </summary>
        [Alias("flag")]
        public int Flag { get; set; }
        /// <summary>
        /// 模块编号
        /// </summary>
        [Alias("module_id")]
        public int ModuleId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Alias("name")]
        public string Name { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        [Alias("icon")]
        public string Icon { get; set; }
        /// <summary>
        /// 页面标题
        /// </summary>
        [Alias("page_title")]
        public string Title { get; set; }
        /// <summary>
        /// 页面关键词
        /// </summary>
        [Alias("page_keywords")]
        public string Keywords { get; set; }
        /// <summary>
        /// 页面描述
        /// </summary>
        [Alias("page_description")]
        public string Description { get; set; }
        /// <summary>
        /// 跳转到的地址
        /// </summary>
        [Alias("location")]
        public string Location { get; set; }
        /// <summary>
        /// 排序编号
        /// </summary>
        [Alias("sort_number")]
        public int SortNumber { get; set; }
    }
}
