using System;

namespace JR.Cms.Domain.Interface.Common
{
    /// <summary>
    /// 链接关联
    /// </summary>
    public interface ILink : IValueObject
    {
        /// <summary>
        /// 
        /// </summary>
        int LinkId { get; set; }

        /// <summary>
        /// 链接名称,如:原文出处
        /// </summary>
        string LinkName { get; set; }

        /// <summary>
        /// 链接标题,如：新浪网
        /// </summary>
        string LinkTitle { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        string LinkUri { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// 转换为Html字符串
        /// </summary>
        /// <returns></returns>
        string ToHtml();
    }
}