using System;

namespace JR.Cms.Web.Api
{
    /// <summary>
    /// 申请令牌
    /// </summary>
    public struct RequestAccessTokenDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password;
        /// <summary>
        /// 令牌有效时间
        /// </summary>
        public int Expires;
    }

    /// <summary>
    /// 令牌
    /// </summary>
    public class AccessTokenDataDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code;
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message;
        /// <summary>
        /// 令牌
        /// </summary>
        public string AccessToken;
        /// <summary>
        /// 过期时间
        /// </summary>
        public long ExpiresTime;
    }
}