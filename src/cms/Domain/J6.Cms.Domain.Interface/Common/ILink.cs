using System;

namespace J6.Cms.Domain.Interface.Common
{
    /// <summary>
    /// 链接关联
    /// </summary>
    public interface ILink:IValueObject
    {
        /// <summary>
        /// 
        /// </summary>
        int LinkId { get; set; }

        /// <summary>
        /// 链接名称,如:原文出处
        /// </summary>
        String LinkName { get; set; }

        /// <summary>
        /// 链接标题,如：新浪网
        /// </summary>
        String LinkTitle { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        String LinkUri { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// 转换为Html字符串
        /// </summary>
        /// <returns></returns>
        String ToHtml();
    }
}
