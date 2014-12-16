using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Ops.Cms.Extend.SSO.Client
{
    /// <summary>
    /// 会话客户端
    /// </summary>
    public class SessionClient
    {
        private string _serverUrl;
        private string _token;

        public SessionClient(string serverUrl,string token)
        {
            this._serverUrl = serverUrl;
            this._token = token;
        }


        public SessionResult GetSession(string sessionKey, string sessionSecret)
        {
            //http://localhost:4617/server.ashx?action=getSession&token=123&session.key=hxeke&session.secret=2014081532992

            string url = String.Format("{0}{1}action=getSession&token={2}&session.key={3}&session.secret={4}",
                this._serverUrl,
                this._serverUrl.IndexOf("?") == -1 ? "?" : "&",
                this._token,
                sessionKey,
                sessionSecret
                );


            WebRequest request = WebRequest.Create(url);

            Stream stream = request.GetResponse().GetResponseStream();
            StreamReader sr = new StreamReader(stream);

            string result = sr.ReadToEnd();
            sr.Dispose();
            stream.Dispose();

            return JsonConvert.DeserializeObject<SessionResult>(result);
        }
    }
}
