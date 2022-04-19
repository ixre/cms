using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using JR.Cms.Conf;
using JR.Cms.ServiceDto;
using JR.Cms.Web.Portal.Controllers;
using JR.Stand.Abstracts;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Mvc;

namespace JR.Cms.Web.Portal.Comm
{
    /// <summary>
    /// WebApiHandler
    /// </summary>
    public class WebApiHandler
    {
        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="formId">表单编号</param>
        /// <param name="formSubject">表单主题</param>
        /// <param name="forms">表单数据</param>
        /// <returns></returns>
        public static Result PostForm(string formId, string formSubject, Dictionary<String, String> forms)
        {
            String fromAddress = "support@fze.net";
            SiteDto site = Cms.Context.CurrentSite;
            String toAddress = site.ProEmail.Trim().Replace("#", "@");
            String subject = site.Name + "-" + formSubject;
            if (toAddress.Length == 0)
            {
                toAddress = fromAddress;
            }

            String ip = WebCtx.HttpCtx.RemoteAddress();
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style='border:solid 1px #DDDD;border-radius:8px'>");
            sb.Append("<div style='background:#34B334;color:#FFF;font-size:16px;font-weight:500;padding:15px;border-radius:8px 8px 0 0'>")
                .Append(site.Name+formSubject).Append("(#"+formId+")").Append("</div>");
            sb.Append("<div style='padding:30px;'>");
            sb.Append("<div style='color:#00aa00; font-weight:400;'>")
                .Append("用户:&nbsp;").Append(ip)
                .Append("&nbsp;&nbsp;&nbsp;&nbsp;提交时间:&nbsp;").Append($"{DateTime.Now:yyyy:MM:dd HH:mm}")
                .Append("</div>");
            sb.Append("<ul style='margin:30px 0 0 0;padding:0'>");
            foreach (var p in forms)
            {
                sb.Append("<li style='list-style:none'><label>")
                    .Append(p.Key).Append(":</label>&nbsp;&nbsp;<span>")
                    .Append(p.Value)
                    .Append("</span></li>");
            }

            sb.Append("</ul>");
            sb.Append("<br /><br /><br />")
                .Append("<div style='color:#999;font-size:12px;'>")
                .Append("本邮件为系统<a style='color:#999;' href='https://fze.net/cms' target='_blank'>JR.Cms v")
                .Append(CmsVariables.VERSION)
                .Append("</a>自动发布,请您定期查看邮箱,以免错过重要信息！</div>");
                sb.Append("</div></div>");
            SendMailClient client = new SendMailClient(
                Settings.SMTP_USERNAME, Settings.SMTP_PASSWORD, Settings.SMTP_HOST, Settings.SMTP_PORT, Settings.SMTP_SSL);
            new Thread(() =>
            {
                client.Send(fromAddress,
                    toAddress,
                    "站点机器人",
                    subject,
                    sb.ToString(), true);
            }).Start();
            return Result.Success("");
        }
    }
}