using System;
using JR.Cms.Library.Utility;
using MailKit.Net.Smtp;
using MimeKit;

namespace JR.Cms.Web.Portal.Controllers
{

   /// <summary>
   /// 邮件发送类
   /// </summary>
   public class SendMailClient
   {
      private readonly string _address;
      private readonly string _password;
      private readonly string _host;
      private readonly int _port;
      private readonly bool _enableSsl;


      /// <summary>
      /// 
      /// </summary>
      /// <param name="address"></param>
      /// <param name="password"></param>
      /// <param name="host"></param>
      /// <param name="port"></param>
      /// <param name="enableSsl"></param>
      public SendMailClient(string address,
       string password,
       string host,
       int port,
       bool enableSsl)
      {
         this._address = address;
         this._password = password;
         this._host = host;
         this._port = port;
         this._enableSsl = enableSsl;
      }


      /// <summary>
      /// 构造函数
      /// </summary>
      /// <param name="toAddress">收件人地址</param>
      /// <param name="fromAddress">发件人地址</param>
      /// <param name="fromName">发件人姓名</param>
      /// <param name="subject">主题</param>
      /// <param name="body">正文</param>
      /// <param name="isBodyHtml">正文是否为html格式</param>
      public void Send(string fromAddress, string toAddress, string fromName, string subject,
         string body, bool isBodyHtml)
      {
         toAddress = toAddress.Replace("#", "@");
         if (!RegexHelper.IsEmail(toAddress))
         {
            throw new ArgumentException("不是有效的邮箱地址:" + toAddress);
         }
         var message = new MimeMessage();
         message.From.Add(new MailboxAddress(fromName ?? "", fromAddress));
         message.To.Add(new MailboxAddress("", toAddress));
         message.Subject = subject;
         //html or plain
         var bodyBuilder = new BodyBuilder();
         if (isBodyHtml)
         {
            bodyBuilder.HtmlBody = body;
         }
         else
         {
            bodyBuilder.TextBody = body;
         }
         message.Body = bodyBuilder.ToMessageBody();
         using (var client = new SmtpClient())
         {
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            //smtp服务器，端口，是否开启ssl
            client.Connect(this._host, this._port, this._enableSsl);
            client.Authenticate(this._address, this._password);
            client.Send(message);
            client.Disconnect(true);
         }
      }
   }
}