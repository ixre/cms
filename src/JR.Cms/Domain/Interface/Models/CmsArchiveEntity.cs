/**
 * Copyright (C) 2009-2022 56X.NET, All rights reserved.
 *
 * name : CmsArchiveEntity.cs
 * author : jarrysix
 * date : 2022/02/25 10:30:30
 * description :
 * history :
 */


using System.Collections.Generic;

namespace JR.Cms.Domain.Interface.Models
{
    /// <summary>
    /// CmsArchive(cms_archive)
    /// </summary>
    public class CmsArchiveEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// StrId
        /// </summary>
        public string StrId { get; set; }

        /// <summary>
        /// 站点编号
        /// </summary>
        public long SiteId { get; set; }

        /// <summary>
        ///  别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 栏目编号
        /// </summary>
        public long CatId { get; set; }

        /// <summary>
        /// 文档路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 文档标志
        /// </summary>
        public int Flag { get; set; }

        /// <summary>
        /// 作者编号
        /// </summary>
        public long AuthorId { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// SmallTitle
        /// </summary>
        public string SmallTitle { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// SortNumber
        /// </summary>
        public long SortNumber { get; set; }

        /// <summary>
        /// Source
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Tags
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Outline
        /// </summary>
        public string Outline { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// ViewCount
        /// </summary>
        public long ViewCount { get; set; }

        /// <summary>
        /// Agree
        /// </summary>
        public long Agree { get; set; }

        /// <summary>
        /// Disagree
        /// </summary>
        public long Disagree { get; set; }

        /// <summary>
        /// Thumbnail
        /// </summary>
        public string Thumbnail { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public long UpdateTime { get; set; }

        public long ScheduleTime { get; set; }

        /// <summary>
        /// 创建深拷贝
        /// </summary>
        /// <returns></returns>
        public CmsArchiveEntity Copy()
        {
            return new CmsArchiveEntity
            {
                Id = this.Id,
                StrId = this.StrId,
                SiteId = this.SiteId,
                Alias = this.Alias,
                CatId = this.CatId,
                Path = this.Path,
                Flag = this.Flag,
                AuthorId = this.AuthorId,
                Title = this.Title,
                SmallTitle = this.SmallTitle,
                Location = this.Location,
                SortNumber = this.SortNumber,
                Source = this.Source,
                Tags = this.Tags,
                Outline = this.Outline,
                Content = this.Content,
                ViewCount = this.ViewCount,
                Agree = this.Agree,
                Disagree = this.Disagree,
                Thumbnail = this.Thumbnail,
                CreateTime = this.CreateTime,
                UpdateTime = this.UpdateTime,
                ScheduleTime = this.ScheduleTime,
            };
        }

        /// <summary>
        /// 转换为MAP
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> ToMap()
        {
            return new Dictionary<string, object>
        {
            {"Id",this.Id},
            {"StrId",this.StrId},
            {"SiteId",this.SiteId},
            {"Alias",this.Alias},
            {"CatId",this.CatId},
            {"Path",this.Path},
            {"Flag",this.Flag},
            {"AuthorId",this.AuthorId},
            {"Title",this.Title},
            {"SmallTitle",this.SmallTitle},
            {"Location",this.Location},
            {"SortNumber",this.SortNumber},
            {"Source",this.Source},
            {"Tags",this.Tags},
            {"Outline",this.Outline},
            {"Summary",this.Outline},
            {"Content",this.Content},
            {"ViewCount",this.ViewCount},
            {"Agree",this.Agree},
            {"Disagree",this.Disagree},
            {"Thumbnail",this.Thumbnail},
            {"CreateTime",this.CreateTime},
                {"UpdateTime", this.UpdateTime},
                {"ScheduleTime", this.ScheduleTime},
            };
        }

        /// <summary>
        /// 使用默认值创建实例 
        /// </summary>
        /// <returns></returns>
        public static CmsArchiveEntity CreateDefault()
        {
            return new CmsArchiveEntity
            {
                Id = 0L,
                StrId = "",
                SiteId = 0L,
                Alias = "",
                CatId = 0L,
                Path = "",
                Flag = 0,
                AuthorId = 0L,
                Title = "",
                SmallTitle = "",
                Location = "",
                SortNumber = 0L,
                Source = "",
                Tags = "",
                Outline = "",
                Content = "",
                ViewCount = 0L,
                Agree = 0L,
                Disagree = 0L,
                Thumbnail = "",
                CreateTime = 0L,
                UpdateTime = 0L,
                ScheduleTime = 0L,
            };
        }
    }
}