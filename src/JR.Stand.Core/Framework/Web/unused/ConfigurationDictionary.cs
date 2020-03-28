using System.Configuration;

namespace JR.DevFw.Framework.Web.unused
{
    public static class ConfigurationDictionary
    {
        /// <summary>
        /// 是否记录错误
        /// </summary>
        internal static bool RecordError = ConfigurationManager.AppSettings["recordError"] == "true";

        /// <summary>
        /// 报告错误的地址
        /// </summary>
        internal static string ReportErrorUri = System.Configuration.ConfigurationManager.AppSettings["errorReportUri"];

        /// <summary>
        /// 是否启用压缩
        /// </summary>
        internal static bool EnableCompression =
            System.Configuration.ConfigurationManager.AppSettings["enableCompression"] == "True";
    }
}