/**
 * Copyright (C) 2007-2015 S1N1.COM,All rights reseved.
 * Get more infromation of this software,please visit site http://z3q.net/cms
 * 
 * name : LocaledExcelExportProvider.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using J6.DevFw.Toolkit.Data.Export;
using J6.DevFw.Toolkit.Data.Export.ExportProvider;
using J6.DevFw.Toolkit.Data.Export.UI;
using Newtonsoft.Json;

namespace com.plugin.sso.Core.Utils
{

    public class ExportHandle
    {
        public static string Setup(string portal)
        {
            IDataExportPortal dp = ExportItemManager.GetPortal(portal);
            return WebExportOptionUIBuilder.BuildColumnCheckHtml(dp);
        }

        public static string GetExportData(HttpContext context)
        {
            IDataExportPortal portal = ExportItemManager.GetPortal(context.Request.QueryString["portal"]);
            ExportParams parameter = DataExportDirector.GetExportParams(
                 context.Request["params"], null);
            int pageIndex, pageSize;

            if (context.Request["page"] != null)
            {
                int.TryParse(context.Request["page"], out pageIndex);
                parameter.Parameters.Add("pageIndex", pageIndex);
            }

            if (context.Request["rows"] != null)
            {
                int.TryParse(context.Request["rows"], out pageSize);
                parameter.Parameters.Add("pageSize", pageSize);
            }

            int totalCount;
            DataTable dt = portal.GetShemalAndData(parameter.Parameters, out totalCount);
            return JsonConvert.SerializeObject(new { total = totalCount, rows = dt });
        }

        public static void ProcessExport(HttpContext context)
        {
            IList<string> columnFields;
            IDataExportProvider provider;
            string extension;
            string portal = context.Request["portal"];
            string columns = context.Request["columns"];

            //获取导出提供者
            switch (context.Request["exportFormat"] ?? "excel")
            {
                default:
                case "excel":
                    provider = new LocaledExcelExportProvider();
                    extension = "xls";
                    break;
                case "csv":
                    provider = new CsvExportProvider();
                    extension = "csv";
                    break;
                case "txt":
                    provider = new TextExportProvider();
                    extension = "txt";
                    break;
            }


            //获取列名
           // Regex reg = new Regex("^export_fields\\[\\d+\\]$", RegexOptions.IgnoreCase);
            columnFields = new List<string>();

//            foreach (string key in context.Request.QueryString.Keys)
//            {
//                if (reg.IsMatch(key))
//                {
//                    columnFields.Add(context.Request.QueryString[key]);
//                }
//            }


            IDataExportPortal _portal = ExportItemManager.GetPortal(context.Request.QueryString["portal"]);
            if (_portal == null) throw new ArgumentNullException("_portal");

            ExportParams parameter = DataExportDirector.GetExportParams(context.Request["params"], columnFields.ToArray());


            byte[] bytes = DataExportDirector.Export(_portal, parameter, provider);

            context.Response.BinaryWrite(bytes);
            context.Response.ContentType = "application/octet-stream";
            context.Response.AppendHeader("Content-Disposition", String.Format("attachment;filename=\"{0:yyyyMMdd-hhssfff}.{1}\"",
                DateTime.Now,
                extension));

        }
    }
}
