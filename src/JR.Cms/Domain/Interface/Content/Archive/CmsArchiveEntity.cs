// this file is auto generated!

using JR.Stand.Core.Data.Orm.Mapping;
using JR.Stand.Core.Framework;



namespace JR.Cms.Domain.Interface.Content.Archive
{
    /// <summary>
    /// 
    /// </summary>
    [DataTable("cms_archive")]
    public class CmsArchiveEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Alias("id")]
        public int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("str_id")]
        public string StrId { get; set; }

        /// <summary>
        /// 站点编号
        /// </summary>
        [Alias("site_id")]
        public int SiteId { get; set; }

        /// <summary>
        ///  别名
        /// </summary>
        [Alias("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// 栏目编号
        /// </summary>
        [Alias("cat_id")]
        public int CatId { get; set; }

        /// <summary>
        /// 文档路径
        /// </summary>
        [Alias("path")]
        public string Path { get; set; }

        /// <summary>
        /// 文档标志
        /// </summary>
        [Alias("flag")]
        public int Flag { get; set; }

        /// <summary>
        /// 作者编号
        /// </summary>
        [Alias("author_id")]
        public int AuthorId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("title")]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("small_title")]
        public string SmallTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("location")]
        public string Location { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("sort_number")]
        public int SortNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("source")]
        public string Source { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("tags")]
        public string Tags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("outline")]
        public string Outline { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("content")]
        public string Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("view_count")]
        public int ViewCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("agree")]
        public int Agree { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("disagree")]
        public int Disagree { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Alias("thumbnail")]
        public string Thumbnail { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Alias("create_time")]
        public long CreateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Alias("schedule_time")]
        public long ScheduleTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Alias("update_time")]
        public long UpdateTime { get; set; }
    }
}