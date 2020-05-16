using System;
using JR.Cms.Domain.Interface.Value;

namespace JR.Cms.Domain.Interface.User
{
    public interface IUser : IAggregateroot
    {
        /// <summary>
        /// 用户凭据
        /// </summary>
        Credential GetCredential();

        /// <summary>
        /// 保存用户凭据
        /// </summary>
        /// <param name="c"></param>
        int SaveCredential(Credential c);

        /// <summary>
        /// 用户名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        string Avatar { get; set; }

        /// <summary>
        /// 角色值,用于判断是否为超级管理员
        /// </summary>
        int Flag { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 校验码
        /// </summary>
        string CheckCode { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        string Phone { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        string Email { get; set; }


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

        /// <summary>
        /// Gets rolos of app
        /// </summary>
        /// <returns></returns>
        AppRoleCollection GetAppRole();

        void SetRoleFlags(int appId, int[] flags);
    }
}