using System;
using Newtonsoft.Json;
using J6.Cms.Extend.SSO.Client;

namespace J6.Cms.Extend.SSO.Demo
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private static SessionClient Client = null;
        private static string server_url = "http://localhost:4617/server.ashx";

        protected void Page_Load(object sender, EventArgs e)
        {

            if(Client == null)
            {
                Client = new SessionClient(server_url, "123456");
            }

            btn_logout.Attributes.Add("href", server_url + "?action=logout");
        }

        protected void btn_Click(object sender, EventArgs e)
        {

            string sessionKey = tb_sessionKey.Text;
            string sessionSecret = tb_sessionSecret.Text;
            SessionResult result = Client.GetSession(sessionKey);

            lb_result.Text = JsonConvert.SerializeObject(result);
        }
    }
}