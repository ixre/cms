using JR.DevFw.Framework;
using System;
using System.Collections.Generic;
using T2.Cms.Domain.Interface.Content.Archive;
using T2.Cms.Domain.Interface.Site.Extend;
using T2.Cms.Models;

namespace T2.Cms.DataTransfer
{


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public struct ArchiveDto
    {

        /// <summary>
        /// 文档自增编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 编号,一个16位Md5字符串
        /// </summary>
        public string StrId { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public CategoryDto Category { get; set; }

        /// <summary>
        /// 文章别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 子标题
        /// </summary>
        public string SmallTitle { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Flags { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public int PublisherId { get; set; }


        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 大纲,导读
        /// </summary>
        public string Outline { get; set; }

        /// <summary>
        /// 文档内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        public string Thumbnail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FirstImageUrl { get; set; }

        /// <summary>
        /// 标签（关键词）
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 支持数
        /// </summary>
        public int Agree { get; set; }

        /// <summary>
        /// 反对数
        /// </summary>
        public int Disagree { get; set; }

        /// <summary>
        /// 显示次数
        /// </summary>
        public int ViewCount { get; set; }


        //创建时间
        public DateTime CreateDate { get; set; }

        //最后修改时间
        public DateTime LastModifyDate { get; set; }


        /// <summary>
        /// 扩展数据
        /// </summary>
        public IList<IExtendValue> ExtendValues { get; set; }

        /// <summary>
        /// 模板路径
        /// </summary>
        public string TemplatePath { get; set; }

        /// <summary>
        /// 是否为自己的模板
        /// </summary>
        public bool IsSelfTemplate { get; set; }

        /// <summary>
        /// 转换文档传输对象
        /// </summary>
        /// <param name="archive">文档</param>
        /// <param name="copyCategory"></param>
        /// <param name="copyTemplate"></param>
        /// <param name="copyExtend"></param>
        /// <returns></returns>
        public static ArchiveDto ConvertFrom(IArchive archive,bool copyCategory,bool copyTemplate,bool copyExtend)
        {
            CmsArchiveEntity a = archive.Get();
            ArchiveDto dto = new ArchiveDto
            {
                Id = archive.GetAggregaterootId(),
                StrId = archive.StrId,
                Disagree = archive.Disagree,
                Agree = archive.Agree,
                LastModifyDate = TimeUtils.UnixTime(a.UpdateTime),
                CreateDate = TimeUtils.UnixTime(a.CreateTime),
                Content = a.Content,
                Alias = archive.Alias,
                PublisherId = a.AuthorId,
                Flags = archive.Flags,
                Outline = archive.Outline,
                Source = archive.Source,
                Tags = a.Tags,
                Url = a.Path,
                Location = a.Location,
                Thumbnail = archive.Thumbnail,
                Title = a.Title,
                SmallTitle = a.SmallTitle,
                ViewCount = a.ViewCount
            };

            if(copyCategory)
            {
                CategoryDto categoryDto = CategoryDto.ConvertFrom(archive.Category);
                categoryDto.ID = archive.Category.GetDomainId();
                dto.Category = categoryDto;
            }

            if(copyExtend)
            {
                dto.ExtendValues = archive.ExtendValues;
            }

            if (copyTemplate)
            {
                if (archive.Template != null)// && archive.Template.BindRefrenceId == archive.ID)
                {
                    dto.TemplatePath = archive.Template.TplPath;
                    dto.IsSelfTemplate = archive.Template.BindRefrenceId == archive.GetAggregaterootId();
                }
            }
            return dto;
        }


        public string Url { get; set; }

        /// <summary>
        /// 重定向URL
        /// </summary>
        public string Location { get; set; }

    }
}
