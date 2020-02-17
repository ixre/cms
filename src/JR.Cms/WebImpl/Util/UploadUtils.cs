using System;
using System.Web;
using JR.Cms.Conf;
using JR.DevFw.Framework.Extensions;

namespace JR.Cms.WebImpl.Util
{
    /// <summary>
    /// 上传文件工具类
    /// </summary>
    public class UploadUtils
    {
        /// <summary>
        /// 获取上传文件夹路径
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static string GetUploadDirPath(int siteId, string fileType,bool joinDate)
        {
            if (!joinDate)
            {
                return String.Format("/{0}{1}/{2}/", CmsVariables.RESOURCE_PATH,
                  siteId.ToString(), fileType);
            }
            return string.Format("/{0}{1}/{2}/{3:yyyyMM}/", CmsVariables.RESOURCE_PATH,
                siteId.ToString(), fileType, DateTime.Now);
        }

        /// <summary>
        /// 获取上传文件名称
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uploadfor"></param>
        /// <returns></returns>
        public static string GetUploadFileName(HttpRequest request, string uploadfor)
        {
            // 使用原始名称
            if (Settings.SYS_USE_UPLOAD_RAW_NAME) return GetUploadFileRawName(request);
            // 使用自动化名称
            DateTime dt = DateTime.Now;
            return String.Format("{0}{1:ddHHss}_{2}",
            String.IsNullOrEmpty(uploadfor) ? "" : uploadfor + "_",
            dt, String.Empty.RandomLetters(4));
        }

        /// <summary>
        /// 获取上传文件名称
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uploadfor"></param>
        /// <returns></returns>
        public static string GetUploadFileName(HttpRequest request)
        {
            // 使用原始名称
            if (Settings.SYS_USE_UPLOAD_RAW_NAME) return GetUploadFileRawName(request);
            // 使用自动化名称
            return String.Format("{0:ddHHss}_{1}", DateTime.Now, String.Empty.RandomLetters(4));
        }

        /// <summary>
        /// 获取上传文件的原始名称
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetUploadFileRawName(HttpRequest request)
        {
            String fileName = request.Files[0].FileName;
            int i = fileName.LastIndexOf(".");
            if (i == -1) return fileName;
            return fileName.Substring(0, i);
        }
    }
}
