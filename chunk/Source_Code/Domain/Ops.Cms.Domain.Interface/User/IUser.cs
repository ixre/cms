using System;

namespace AtNet.Cms.Domain.Interface.User
{
    public interface IUser : IAggregateroot
    {
        /// <summary>
        /// 用户凭据
        /// </summary>
        UserCredential Credential { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 角色值
        /// </summary>
        int RoleFlag { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 是否包含角色
        /// </summary>
        /// <param name="roleFlag"></param>
        /// <returns></returns>
        bool SubOf(int roleFlag);

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        int Save();
    }
}
