using System;
using System.Web;
using Ops.Cms.Extend.SSO.Server;

namespace Ops.Cms.Extend.SSO.Demo
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
            SessionSington = new SessionServer(handler);
        }

        public void ProcessRequest(HttpContext context)
        {

            #region 测试数据
            string isTest = context.Request["test"];

            if (!String.IsNullOrEmpty(isTest))
            {
                string sessionKey = SessionSington.SessionManager.GetCurrentSessionKey();


                //如果未包含登陆信息，则登陆
                if (String.IsNullOrEmpty(sessionKey))
                {
                    bool result = SessionSington.Login("new.min@msn.com", "123456", (user, pwd) =>
                    {
                        return 1;
                    });
                    context.Response.Write("用户登陆" + (result ? "成功" : "失败") + "!");
                    sessionKey = SessionSington.SessionManager.GetCurrentSessionKey();
                }


                //输出登陆信息
                context.Response.Write("sessionKey = " + sessionKey + " & sessionSecret = "
                    + SessionSington.SessionManager.GetSession(sessionKey));

                //退出登陆
                //SessionSington.LoginOut();

                return;
            }

            #endregion

            string resultStr = SessionSington.Response.Process(context);
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