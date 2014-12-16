namespace Ops.Cms.Extend.SSO
{
    public class Variables
    {
        /// <summary>
        /// 会话Cookie名称
        /// </summary>
        public static string SessionCookieName = "_ssokey";

        /// <summary>
        /// 会话键长度
        /// </summary>
        public static int SeesionKeyLength = 5;

        /// <summary>
        /// 通信密钥
        /// </summary>
        public static string CommunicateToken = "123";

        /// <summary>
        /// 单点登陆服务器
        /// </summary>
        public static string SSO_SERVER = "http://xxx.com/sso_login";
    }
}
