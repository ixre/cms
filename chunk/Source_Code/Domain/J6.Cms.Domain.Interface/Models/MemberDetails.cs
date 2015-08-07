//
// MemberDetails  会员详细信息
// Copryright 2011 @ K3F.NET,All rights reseved !
// Create by newmin @ 2011/03/16
//

using System;

namespace J6.Cms.Domain.Interface.Models
{
    /// <summary>
    /// 会员详细信息
    /// </summary>
    public class MemberDetails
    {
        /// <summary>
        /// 会员编号,与Member.ID对应
        /// </summary>
        public int UID { get; set; }
        /// <summary>
        /// 状态,Active:等待激活,Normal:正常,Stop:停用
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 注册IP
        /// </summary>
        public string RegIP { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegTime { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }
        /// <summary>
        /// 密钥,用于激活和修改密码的密钥
        /// </summary>
        public string Token { get; set; }
    }
}