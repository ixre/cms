using System;
using System.Collections.Generic;
using J6.Cms.Domain.Interface.Content.Archive;
using J6.Cms.Domain.Interface.Site.Extend;

namespace J6.Cms.DataTransfer
{

    [Serializable]
    public delegate string LinkBehavior(SiteLinkDto link);
    public delegate string LinkGenerateGBehavior(int total,ref int current,int selected,bool child,SiteLinkDto link);

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
        /// <returns></returns>
        public static ArchiveDto ConvertFrom(IArchive archive,bool copyCategory,bool copyTemplate,bool copyExtend)
        {
            ArchiveDto dto = new ArchiveDto
            {
                Id = archive.Id,
                StrId = archive.StrId,
                Disagree = archive.Disagree,
                Agree = archive.Agree,
                LastModifyDate = archive.LastModifyDate,
                CreateDate = archive.CreateDate,
                Content = archive.Content,
                Alias = archive.Alias,
                PublisherId = archive.PublisherId,
                Flags = archive.Flags,
                Outline = archive.Outline,
                Source = archive.Source,
                Tags = archive.Tags,
                Url = archive.Uri,
                Location = archive.Location,
                Thumbnail = archive.Thumbnail,
                Title = archive.Title,
                SmallTitle = archive.SmallTitle,
                ViewCount = archive.ViewCount
            };

            if(copyCategory)
            {
                CategoryDto categoryDto = CategoryDto.ConvertFrom(archive.Category);
                categoryDto.Id = archive.Category.Id;
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
                    dto.IsSelfTemplate = archive.Template.BindRefrenceId == archive.Id;
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
