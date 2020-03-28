//
// Comment  评论模型
// Copryright 2011 @ TO2.NET,All rights reseved !
// Create by newmin @ 2011/03/13
//

using System;

namespace JR.Cms.Domain.Interface.Models
{
    /// <summary>
    /// 评论
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// 评论编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 文档ID,与评论相关的文档编号
        /// </summary>
        public string ArchiveID { get; set; }

        /// <summary>
        /// 会员用户名
        /// </summary>
        public int MemberID { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否回收
        /// </summary>
        public bool Recycle { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}