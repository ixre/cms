using System;
using JR.Cms.Conf;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Framework.Extensions;

namespace JR.Cms.Web.Util
{
    /// <summary>
    /// 上传文件工具类
    /// </summary>
    public static class UploadUtils
    {
        /// <summary>
        /// 获取上传文件夹路径
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="fileType"></param>
        /// <param name="joinDate"></param>
        /// <returns></returns>
        public static string GetUploadDirPath(int siteId, string fileType, bool joinDate)
        {
            if (!joinDate)
                return $"/{CmsVariables.RESOURCE_PATH}{siteId.ToString()}/{fileType}/";
            return $"/{CmsVariables.RESOURCE_PATH}{siteId.ToString()}/{fileType}/{DateTime.Now:yyyyMM}/";
        }

        /// <summary>
        /// 获取上传文件名称
        /// </summary>
        /// <param name="file"></param>
        /// <param name="uploadFor"></param>
        /// <returns></returns>
        public static string GetUploadFileName(ICompatiblePostedFile file, string uploadFor)
        {
            // 使用原始名称
            if (Settings.SYS_USE_UPLOAD_RAW_NAME) return GetUploadFileRawName(file);
            // 使用自动化名称
            var dt = DateTime.Now;
            return $"{(string.IsNullOrEmpty(uploadFor) ? "" : uploadFor + "_")}{dt:ddHHss}{string.Empty.RandomLetters(4)}";
        }

        /// <summary>
        /// 获取上传文件名称
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetUploadFileName(ICompatiblePostedFile file)
        {
            // 使用原始名称
            if (Settings.SYS_USE_UPLOAD_RAW_NAME) return GetUploadFileRawName(file);
            // 使用自动化名称
            return $"{DateTime.Now:ddHHss}_{string.Empty.RandomLetters(4)}";
        }

        /// <summary>
        /// 获取上传文件的原始名称
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetUploadFileRawName(ICompatiblePostedFile file)
        {
            var fileName = file.GetFileName();
            var i = fileName.LastIndexOf(".", StringComparison.Ordinal);
            if (i == -1) return fileName;
            return fileName.Substring(0, i);
        }
    }
}