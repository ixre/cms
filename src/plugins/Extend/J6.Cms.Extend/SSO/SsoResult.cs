
namespace JR.Cms.Extend.SSO
{
    public struct SsoResult
    {
        /// <summary>
        /// 登陆结果
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 登陆消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 会话键
        /// </summary>
        public string SessionKey { get; set; }
    }
}
