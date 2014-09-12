using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ops.Cms.Domain.Interface.User
{

    /// <summary>
    /// 用户凭据
    /// </summary>
    public interface IUserCredential : IValueObject
    {
        /// <summary>
        ///用户名
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// 是否使用 
        /// </summary>
        bool Enabled { get; set; }
    }
}
