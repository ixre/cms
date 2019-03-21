 //参数code为生成的Code
//域名32位md5后base64
namespace OPSoft.WebControlCenter.Handler
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Text;
    using System.Xml;
    using OPS.Web;

    [WebExecuteable]
    public class Check
    {
        [Valid]
        [Post(AllowRefreshMillliSecond = 2000)]
        public string State(string code)
        {
            try
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(AppDomain.CurrentDomain.BaseDirectory + "config/sites.conf");
                XmlNode xn = xd.SelectSingleNode("/sites/site[@key='" + code + "']");
                //存在此Key的节点
                if (xn != null)
                {
                    string endDate = xn.Attributes["end"].Value;
                    //如果结束时间为不限
                    if (endDate == "*") return "ok";
                    //判断结束时间是否已过,未过则返回"ok";
                    DateTime _endDate;
                    DateTime.TryParse(endDate, out _endDate);
                    if (_endDate > DateTime.Now) return "ok";
                }
            }
            catch 
            {
                //如果控制端出现任何异常，则默认全部通过
                return "ok";
            }
            //
            // 不存在Key或者已过有效期
            // 返回值
            // go:返回数据但不调用Response.End()
            // end:返回数据调用Response.End()
            // 其他数据则默认通过且存入缓存
            return "go:Not Activated!";
        }

    }
}