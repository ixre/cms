using System;
using System.Web;
using Newtonsoft.Json;

namespace Ops.Cms.Extend.SSO.Server
{
    public class SessionServerResponse
    {
        private SessionServer _server;

        internal SessionServerResponse(SessionServer server)
        {
            this._server = server;
        }

        /// <summary>
        /// 处理输出
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Process(HttpContext context)
        {
            string _action = context.Request["action"];
            string token = context.Request["token"];

            if (_action != null)
            {
                SessionServerAction action = (SessionServerAction)Enum.Parse(typeof(SessionServerAction), _action, true);

                //登出
                if (action == SessionServerAction.Logout)
                {
                    this._server.LoginOut();
                    return JsonConvert.SerializeObject(new SessionResult { Result = true, Message = "已经退出！" });
                }

                //除登出外均需要验证
                if (token != Variables.CommunicateToken)
                {
                    return JsonConvert.SerializeObject(new SessionResult { Message = "通信错误！" });
                }

                if (action == SessionServerAction.GetSession)
                {
                    return ProcessClientGetSession(context);
                }
            }


            return JsonConvert.SerializeObject(new SessionResult { Message = "非法请求！" });
        }


        private string ProcessClientGetSession(HttpContext context)
        {

            string sessionKey = context.Request["session.Key"];
            string sessionSecret = context.Request["session.Secret"];

            if (String.IsNullOrEmpty(sessionSecret) || String.IsNullOrEmpty(sessionKey))
            {
                return JsonConvert.SerializeObject(new SessionResult { Message = "非法请求！" });
            }

            string serverSessionSecret = this._server.SessionManager.GetSession(sessionKey);
            if (String.IsNullOrEmpty(serverSessionSecret))
            {
                return JsonConvert.SerializeObject(new SessionResult { Message = "无效登陆信息！" });
            }

            bool verifyResult = this._server.SessionManager.VerifySessionSecret(sessionKey, sessionSecret);


            if (!verifyResult)
            {
                return JsonConvert.SerializeObject(new SessionResult { Message = "无效登陆信息！" });
            }

            Person person = this._server.SessionManager.GetPerson(sessionKey);
            if (person == null)
            {
                return JsonConvert.SerializeObject(new SessionResult { Message = "验证未通过！" });
            }

            return JsonConvert.SerializeObject(new SessionResult { Result = true, Person = person });
        }
    }
}
