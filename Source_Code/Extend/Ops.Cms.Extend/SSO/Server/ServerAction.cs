namespace AtNet.Cms.Extend.SSO.Server
{
    /// <summary>
    /// 会话服务动作
    /// </summary>
    internal enum ServerAction
    {
        Test = 0,
        /// <summary>
        /// 获取用户Session
        /// </summary>
        GetSession = 1,

        /// <summary>
        /// 登陆
        /// </summary>
        Login = 2,

        /// <summary>
        /// 退出
        /// </summary>
        Logout =3,
    }
}
