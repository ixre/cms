
namespace OPSite.WebHandler
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.IO;
    using System.Text;
    using Ops.Cms;
    using Ops.Web;


    [WebExecuteable]
    public class Log
    {
        public string GetErrorLog()
        {
            string log=null;
            if (File.Exists(AppContext.ErrorFilePath))
            {
                using (StreamReader sr = new StreamReader(AppContext.ErrorFilePath, Encoding.UTF8))
                {
                    log = sr.ReadToEnd();
                }
            }
            return String.IsNullOrEmpty(log) ? "非常棒!还没有产生错误日志!" : log;
        }
        public void ClearErrorLog()
        {
            if (File.Exists(AppContext.ErrorFilePath))
            {
                using (FileStream file = new FileStream(AppContext.ErrorFilePath, FileMode.Truncate, FileAccess.Write))
                {
                }
            }
        }
    }
}
