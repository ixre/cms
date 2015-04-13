using System;
using System.Web;
using Newtonsoft.Json;

namespace Ops.Cms.Extend.SSO.Server
{
    internal class SessionServerResponse
    {
        private readonly SessionServer _server;
        private readonly string _communicateToken;

        public SessionServerResponse(SessionServer server, string communicateToken)
        {
            this._server = server;
            this._communicateToken = communicateToken;
        }


        /// <summary>
        /// 处理输出
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Process(HttpContext context)
        {
            string actionStr = context.Request["action"];
            string token = context.Request["token"];
            string sessionKey = context.Request["session.key"];

            //除登出外均需要验证
            if (token != this._communicateToken)
            {
                return JsonConvert.SerializeObject(new SessionResult {Message = "通信错误！"});
            }

            if (actionStr != null)
            {
                SessionServerAction action = (SessionServerAction)Enum.Parse(
                    typeof(SessionServerAction), actionStr, true);

                //登出
                if (action == SessionServerAction.Logout)
                {
                   SsoResult result = this._server.LoginOut(sessionKey);
                   return JsonConvert.SerializeObject(result);
                }

                if (action == SessionServerAction.Login)
                {
                    SsoResult result = this._server.Login(context.Request["usr"],
                        context.Request["pwd"]);
                    return JsonConvert.SerializeObject(result);
                }

                if (action == SessionServerAction.GetSession)
                {
                    Person person = this._server.SessionManager.GetPerson(sessionKey);
                    if (person == null)
                    {
                        return JsonConvert.SerializeObject(new SessionResult { Message = "No session" });
                    }

                    return JsonConvert.SerializeObject(new SessionResult { Result = true, Person = person });

                }

                if (action == SessionServerAction.Test)
                {
                    return "Test ok!";
                }
            }


            return JsonConvert.SerializeObject(new SessionResult {Message = "Invalid request"});
        }



    }
}
