//
// Member  友情链接模型
// Copryright 2011 @ TO2.NET,All rights reseved !
// Create by newmin @ 2011/03/13
//

namespace JR.Cms.Domain.Interface.Models
{
    /// <summary>
    /// 会员
    /// </summary>
    public class Member
    {
        /// <summary>
        /// 会员编号,自动生成
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 用户组ID
        /// </summary>
        public int UserGroupID { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 个人说明
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string TelePhone { get; set; }
    }
}