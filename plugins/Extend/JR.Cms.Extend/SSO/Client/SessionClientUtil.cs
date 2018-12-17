using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace JR.Cms.Extend.SSO.Client
{
    /// <summary>
    /// 会话客户端
    /// </summary>
    public class ClientUtil
    {
        public static SessionResult RequestSession(string server_url,string token, string sessionKey)
        {
            //http://localhost:4617/server.ashx?action=getSession&token=123
            //&session.key=hxeke&session.secret=2014081532992

            string url = String.Format("{0}{1}action=getSession&token={2}&session.key={3}",
               server_url,
                server_url.Contains("?") ? "&" : "?",
                token,
                sessionKey
                );
            return JsonConvert.DeserializeObject<SessionResult>(get(url));
        }

        private static string get(string url)
        {
            string result;
            WebRequest request = WebRequest.Create(url);

            Stream stream = request.GetResponse().GetResponseStream();
            StreamReader sr = new StreamReader(stream);

            result = sr.ReadToEnd();
            sr.Dispose();
            stream.Dispose();
            return result;
        }


        internal static SsoResult LoginRequest(string server_url, string token, string usr, string pwd)
        {
            string url = String.Format("{0}{1}action=Login&token={2}&session.key=&usr={3}&pwd={4}",
              server_url,
               server_url.Contains("?") ? "&" : "?",
               token,
               usr,
               pwd
               );
            return JsonConvert.DeserializeObject<SsoResult>(get(url));
        }

        internal static SsoResult LogoutRequest(string server_url, string token, string sessionKey)
        {
            string url = String.Format("{0}{1}action=Logout&token={2}&session.key={3}",
                 server_url,
                  server_url.Contains("?") ? "&" : "?",
                  token,
                  sessionKey
                  );
            return JsonConvert.DeserializeObject<SsoResult>(get(url));
        }
    }
}
