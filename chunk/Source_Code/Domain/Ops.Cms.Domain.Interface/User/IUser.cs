using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ops.Cms.Domain.Interface.User
{
    public interface IUser : IAggregateroot
    {
        /// <summary>
        /// 站点编号
        /// </summary>
        int SiteID { get; set; }

        /// <summary>
        /// 用户凭据
        /// </summary>
        IUserCredential Credential { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 组ID
        /// </summary>
        int GroupID { get; set; }

        /// <summary>
        /// 账号状态
        /// </summary>
        bool Available { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateDate { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        DateTime LastLoginDate { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        UserGroups Group { get { return (UserGroups)GroupID; } }
    }
}
