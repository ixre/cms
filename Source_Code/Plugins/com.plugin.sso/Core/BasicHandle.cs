using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using com.plugin.sso.Core.Utils;
using AtNet.Cms;
using AtNet.Cms.Conf;
using AtNet.Cms.Web;
using AtNet.DevFw.Data;
using AtNet.DevFw.Framework.Extensions;
using AtNet.DevFw.Template;
using AtNet.DevFw.Framework.Web.UI;

namespace com.plugin.sso.Core
{
    internal class BasicHandle
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Upload_post(HttpContext context)
        {
            string uploadfor = context.Request["for"];
            DateTime dt = DateTime.Now;
            string dir = string.Format(CmsVariables.RESOURCE_PATH+"weixin/{0:yyyyMMdd}/", dt);
            string name = String.Format("{0}{1:HHss}{2}",
                String.IsNullOrEmpty(uploadfor) ? "" : uploadfor + "_",
                dt, String.Empty.RandomLetters(4));

            string file = new FileUpload(dir, name).Upload();
            return "{" + String.Format("url:'{0}'", file) + "}";
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="context"></param>
        public void Download(HttpContext context)
        {
            if (!RequestProxry.VerifyLogin(context)) return;

            string url = context.Request["url"];
            string filePath = AppDomain.CurrentDomain.BaseDirectory + url;
            if (!File.Exists(filePath))
            {
                context.Response.Write("资源不存在");
                return;
            }

            string fileName = Regex.Match(url, "(\\\\|/)(([^\\\\/]+)\\.(.+))$").Groups[2].Value;
            context.Response.AppendHeader("Content-Type", "");
            context.Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);

            const int bufferSize = 100;
            byte[] buffer = new byte[bufferSize];
            int readSize = -1;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                while (readSize != 0)
                {
                    readSize = fs.Read(buffer, 0, bufferSize);
                    context.Response.BinaryWrite(buffer);
                }
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Export_Setup(HttpContext context)
        {
            if (!RequestProxry.VerifyLogin(context)) return null;

            TemplatePage page = Cms.Plugins.GetPage<Main>("mg/export_setup.html");
            page.AddVariable("page", new PageVariable());
            page.AddVariable("export", new { setup = ExportHandle.Setup(context.Request["portal"]) });
            return page.ToString();
        }

        /// <summary>
        /// 导出数据源
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Export_GetExportData_Post(HttpContext context)
        {
            return ExportHandle.GetExportData(context);
        }

        public void Export_ProcessExport(HttpContext context)
        {
            if (!RequestProxry.VerifyLogin(context)) return;
            ExportHandle.ProcessExport(context);
        }

        public string Export_Import(HttpContext context)
        {
            if (!RequestProxry.VerifyLogin(context)) return null;

            TemplatePage page = Cms.Plugins.GetPage<Main>("admin/export_import.html");
            page.AddVariable("page", new PageVariable());
            page.AddVariable("case", new { json = new object() });
            return page.ToString();
        }

        public string Export_Import_post(HttpContext context)
        {
            // try
            // {
            FileInfo file = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + context.Request["file"]);
            DataTable dt = NPOIHelper.ImportFromExcel(file.FullName).Tables[0];
            //DataView dv = dt.DefaultView;
            //dv.Sort = "财务编号 ASC";
            SqlQuery[] querys = new SqlQuery[dt.Rows.Count];
            int i = 0;
            DateTime importTime = DateTime.Now;
            foreach (DataRow dr in dt.Rows)
            {
                const string insertSql = @" ";
            }

            int rows = IocObject.GetDao().ExecuteNonQuery(querys);
            if (rows < 0) rows = 0;
            return "{result:true,message:'导入完成,共导入" + rows.ToString() + "条！'}";
            /* }
             catch (Exception exc)
             {
                 return "{result:false,message:'"+exc.Message+"！'}";
             }*/
        }
    }
}
