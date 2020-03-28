using System;
using JR.Cms.Conf;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Framework.Extensions;
using Microsoft.AspNetCore.Http;

namespace JR.Cms.WebImpl.Util
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
        /// <returns></returns>
        public static string GetUploadDirPath(int siteId, string fileType, bool joinDate)
        {
            if (!joinDate)
                return string.Format("/{0}{1}/{2}/", CmsVariables.RESOURCE_PATH,
                    siteId.ToString(), fileType);
            return string.Format("/{0}{1}/{2}/{3:yyyyMM}/", CmsVariables.RESOURCE_PATH,
                siteId.ToString(), fileType, DateTime.Now);
        }

        /// <summary>
        /// 获取上传文件名称
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uploadfor"></param>
        /// <returns></returns>
        public static string GetUploadFileName(ICompatiblePostedFile file, string uploadfor)
        {
            // 使用原始名称
            if (Settings.SYS_USE_UPLOAD_RAW_NAME) return GetUploadFileRawName(file);
            // 使用自动化名称
            var dt = DateTime.Now;
            return
                $"{(string.IsNullOrEmpty(uploadfor) ? "" : uploadfor + "_")}{dt:ddHHss}_{string.Empty.RandomLetters(4)}";
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