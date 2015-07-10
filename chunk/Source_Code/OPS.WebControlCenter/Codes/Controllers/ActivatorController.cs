
namespace OPSoft.WebControlCenter.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using OPSoft.XML;
    using OPSoft.Json;

    public class ActivatorController:Controller
    {
        /// <summary>
        /// 激活数据文件
        /// </summary>
        private static SettingFile activatorFile = new SettingFile(AppDomain.CurrentDomain.BaseDirectory + "/data/activator.db");

        /// <summary>
        /// 激活结果处理
        /// </summary>
        private static SettingFile handleConfigFile = new SettingFile(AppDomain.CurrentDomain.BaseDirectory + "/data/handler.conf");



        //[AcceptVerbs("POST")]
        public string Verify(string domain,string key,string token)
        {
            //activatorFile.Append("YmIyNDAwMGI3YmEyZGMwZTgxZWI2OGQxYzk3MWU4NWI=", "{domain:'temp.j6.cc',start:'*',end='*'}");
            try
            {

                if (activatorFile.Contains(key))
                {
                    
                    JsonAnalyzer ja=new JsonAnalyzer(activatorFile[key]);

                    string endDate = ja.GetValue("end");
                    //如果结束时间为不限
                    if (endDate == "*") return GethandleJson("ok");
                    //判断结束时间是否已过,未过则返回"ok";
                    DateTime _endDate;
                    DateTime.TryParse(endDate, out _endDate);
                    if (_endDate > DateTime.Now) return GethandleJson("ok");
                }
            }
            catch
            {
                //如果控制端出现任何异常，则默认全部通过
                return GethandleJson("ok");
            }


            //
            // 不存在Key或者已过有效期
            // 返回值
            // go:返回数据但不调用Response.End()
            // end:返回数据调用Response.End()
            // 其他数据则默认通过且存入缓存
            return GethandleJson("end");
        }


        /// <summary>
        /// 获取处理JSON数据
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private string GethandleJson(string action)
        {
            if (handleConfigFile.Contains(action))
            {
                return handleConfigFile[action];
            }
            return String.Empty;
        }


    }
}