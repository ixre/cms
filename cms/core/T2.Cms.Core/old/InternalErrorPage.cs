/*
* Copyright(C) 2010-2013 OPSoft Inc
* 
* File Name	: InternalErrorPage
* Author	: Newmin (new.min@msn.com)
* Create	: 2013/04/04 15:59:54
* Description	:
*
*/

using System.Web;

namespace J6.Cms.old
{
    /// <summary>
    /// 内部错误页面
    /// </summary>
   public class InternalErrorPage
    {
       public static void Show(string html)
       {
           HttpResponse rsp = HttpContext.Current.Response;
           rsp.Write(html);
       }
    }
}
