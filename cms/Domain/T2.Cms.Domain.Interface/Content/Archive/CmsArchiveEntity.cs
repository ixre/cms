// this file is auto generated!

using JR.DevFw.Framework;
using JR.DevFw.Data.Orm.Mapping;


/*
 
     
     dst.ID = src.ID;
     dst.StrId = src.StrId;
     dst.SiteId = src.SiteId;
     dst.Alias = src.Alias;
     dst.CatId = src.CatId;
     dst.Path = src.Path;
     dst.Flag = src.Flag;
     dst.PublisherId = src.PublisherId;
     dst.Title = src.Title;
     dst.SmallTitle = src.SmallTitle;
     dst.Location = src.Location;
     dst.SortNumber = src.SortNumber;
     dst.Source = src.Source;
     dst.Tags = src.Tags;
     dst.Outline = src.Outline;
     dst.Content = src.Content;
     dst.ViewCount = src.ViewCount;
     dst.Agree = src.Agree;
     dst.Disagree = src.Disagree;
     dst.Createdate = src.Createdate;
     dst.Lastmodifydate = src.Lastmodifydate;
     dst.Flags = src.Flags;
     dst.Thumbnail = src.Thumbnail;

 */

namespace T2.Cms.Models{

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
        /// 
        /// </summary>
        [Alias("publisher_id")]
        public string PublisherId { get; set; }
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
        [Alias("createdate")]
        public int Createdate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Alias("lastmodifydate")]
        public int Lastmodifydate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Alias("flags")]
        public string Flags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Alias("thumbnail")]
        public string Thumbnail { get; set; }
    }
}
