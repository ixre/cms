using System;

namespace AtNet.Cms.Domain.Interface.User
{
    /// <summary>
    /// 用户凭据
    /// </summary>
    public class UserCredential : IValueObject
    {
        /// <summary>
        ///用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否使用 
        /// </summary>
        public int Enabled { get; set; }

        public bool Equal(IValueObject that)
        {
            return false;
        }
    }
}
