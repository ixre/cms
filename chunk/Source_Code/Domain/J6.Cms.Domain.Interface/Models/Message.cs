//
// Message  消息模型
// Copryright 2011 @ K3F.NET,All rights reseved !
// Create by newmin @ 2011/04/06
//

using System;

namespace J6.Cms.Domain.Interface.Models
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 发送
        /// </summary>
        Send,
        /// <summary>
        /// 接收
        /// </summary>
        Receive
    }
    /// <summary>
    /// 会员
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 消息编号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 发送的用户ID
        /// </summary>
        public int SendUID { get; set; }
        /// <summary>
        /// 接收的用户ID
        /// </summary>
        public int ReceiveUID { get; set; }
        /// <summary>
        /// 消息主题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 是否已读
        /// </summary>
        public bool HasRead { get; set; }
        /// <summary>
        /// 是否回收,指收件人删除
        /// </summary>
        public bool Recycle { get; set; }
        /// <summary>
        /// 发送日期
        /// </summary>
        public DateTime SendDate { get; set; }
    }
}