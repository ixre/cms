using System;
using System.Threading.Tasks;
using JR.Cms.Conf;
using JR.Cms.WebImpl.Editor;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;

namespace JR.Cms.WebImpl.WebManager.Handle
{
    /// <summary>
    /// 编辑器
    /// </summary>
    public class EditorHandler: BasePage
    {

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task KindEditorFileUpload_POST(HttpContext context)
        {
            string siteId = Logic.CurrentSite.SiteId.ToString();
            //根目录路径，相对路径
            String rootPath = $"{CmsVariables.RESOURCE_PATH}{siteId}/";
            //根目录URL，可以指定绝对路径，比如 http://www.yoursite.com/attached/
            string appPath = Cms.Context.ApplicationPath;
            var u = new KindEditor(appPath,rootPath);
            return u.UploadRequest(HttpHosting.Context);
        }

        /// <summary>
        /// 浏览文件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task KindEditorFileManager(HttpContext context)
        {
            string siteId = Logic.CurrentSite.SiteId.ToString();
            //根目录路径，相对路径
            String rootPath = $"{CmsVariables.RESOURCE_PATH}{siteId}/";
            //根目录URL，可以指定绝对路径，比如 http://www.yoursite.com/attached/
            string appPath = Cms.Context.ApplicationPath;
            var u = new KindEditor(appPath,rootPath);
            return u.FileManagerRequest(context);
        }
    }
}