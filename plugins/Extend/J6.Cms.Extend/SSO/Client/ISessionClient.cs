using System;
using System.Web;

namespace T2.Cms.Extend.SSO.Client
{
    public interface ISessionClient
    {
        /// <summary>
        /// 处理SSO请求
        /// </summary>
        /// <param name="context"></param>
        void HandleSsoRequest(HttpContext context);

        /// <summary>
        /// 获取会话
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        SessionResult GetSession(String sessionKey);

        /// <summary>
        /// 获取会话Key
        /// </summary>
        /// <returns></returns>
        String GetSessionKey();

        /// <summary>
        /// 同步登陆
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        SsoResult Login(String user, String pwd);

        /// <summary>
        /// 同步登出
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        SsoResult Logout(String sessionKey);
    }
}
