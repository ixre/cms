using J6.Cms.old;

namespace J6.Cms.Web
{
    using J6.Cms;
    using System;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// 重定向模块
    /// </summary>
    [Obsolete]
    public class RedirectHttpModule:HttpModuleBase
    {
        public override void On_BeginRequest(object o, EventArgs e)
        {
            
            base.On_BeginRequest(o, e);
            HttpContext context = HttpContext.Current;

            HttpContext c = HttpContext.Current;
            const string pattern = "([^\\.]*)\\.*([^\\.]+)\\.([^/]+)";
            if (Regex.IsMatch(c.Request.Url.Host, pattern))
            {
                Match m = Regex.Match(c.Request.Url.Host, pattern);

                if (m.Groups[1].Value != "www")
                {
                    c.Response.Status = "301 Moved Permanently";
                    string path;
                    if (c.Request.ApplicationPath == "/")
                    {
                        path = c.Request.Url.PathAndQuery;
                    }
                    {
                        path = c.Request.Url.PathAndQuery.Substring(c.Request.ApplicationPath.Length);
                    }

                    c.Response.Headers.Add("Location", String.Format("http://www.{0}.{1}/{2}",
                        m.Groups[2].Value,
                        m.Groups[3].Value,
                        path));
                }
            }
        }
    }
}