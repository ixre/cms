using System;
using System.Web;
using J6.Cms.Extend.SSO.Server;

namespace J6.Cms.Extend.SSO.Demo
{
    /// <summary>
    /// server 的摘要说明
    /// </summary>
    public class server : IHttpHandler,System.Web.SessionState.IRequiresSessionState
    {
        /// <summary>
        /// SSO单例
        /// </summary>
        private static SessionServer SessionSington;

        static server()
        {
            PersonFetchHandler handler = personId =>{
                return new Person{
                    Id = 1,
                    Enabled = true,
                    Name = "刘铭",
                    Sex = 1,
                    Username = "new.min@msn.com"
                };
            };

            SSOLoginHandler ssoHandler = (user, pwd) =>
            {
                return 1;
            };

            SessionSington = new SessionServer(handler, ssoHandler,"123456",null);

        }

        public void ProcessRequest(HttpContext context)
        {

            #region 测试数据
            string isTest = context.Request["test"];

            if (!String.IsNullOrEmpty(isTest))
            {
                String sessionKey = null;
                //如果未包含登陆信息，则登陆
//                if (String.IsNullOrEmpty(sessionKey))
//                {
//                    SsoResult result = SessionSington.Login("new.min@msn.com", "123456");
//                    context.Response.Write("用户登陆" + (result.Result ? "成功" : "失败") + "!");
//                    sessionKey = result.SessionKey;
//                }
//
//
//                //输出登陆信息
//                context.Response.Write("sessionKey = " + sessionKey + " & sessionSecret = "
//                    + SessionSington.SessionManager.GetSession(sessionKey));

                //退出登陆
                //SessionSington.LoginOut();

                return;
            }

            #endregion

            string resultStr = SessionSington.Process(context);
            context.Response.Write(resultStr); 
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}