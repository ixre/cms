namespace JR.Cms.Extend.SSO
{
    /// <summary>
    /// 登陆处理
    /// </summary>
    /// <param name="usr"></param>
    /// <param name="pwd"></param>
    /// <returns>返回personId</returns>
   public delegate int SSOLoginHandler(string usr,string pwd);
}
