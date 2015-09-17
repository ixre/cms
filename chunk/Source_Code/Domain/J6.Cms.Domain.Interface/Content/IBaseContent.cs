using System;
using J6.Cms.Domain.Interface.Common;
using J6.Cms.Domain.Interface.Site.Category;

namespace J6.Cms.Domain.Interface.Content
{
    public interface IBaseContent:IAggregateroot
    {
        /// <summary>
        /// 标题
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 小标题
        /// </summary>
        String SmallTitle { get; set; }

        /// <summary>
        /// 资源地址
        /// </summary>
        String Uri { get; set; }

        /// <summary>
        /// 重定向URL
        /// </summary>
        string Location { get; set; }

        /// <summary>
        /// 栏目分类
        /// </summary>
        ICategory Category { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        int PublisherId { get; set; }

        /// <summary>
        /// 文档内容
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// 标签（关键词）
        /// </summary>
        string Tags { get; set; }

        /// <summary>
        /// 显示次数
        /// </summary>
        int ViewCount { get; set; }

        /// <summary>
        /// 排序序号
        /// </summary>
        int SortNumber { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateDate { get; set; }

        /// <summary>
        ///最后修改时间
        /// </summary>
        DateTime LastModifyDate { get; set; }

        /// <summary>
        /// 链接管理
        /// </summary>
        IContentLinkManager LinkManager {get;}

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        int Save();

        /// <summary>
        /// 下移排序
        /// </summary>
        void SortLower();

        /// <summary>
        /// 上移排序
        /// </summary>
        void SortUpper();
    }
}
