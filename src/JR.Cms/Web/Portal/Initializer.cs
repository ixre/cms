using System;
using JR.Cms.Conf;
using JR.Cms.Web.Resource;
using JR.Stand.Core.Cache;
using JR.Stand.Core.Utils;
using Microsoft.AspNetCore.Builder;

namespace JR.Cms.Web.Portal
{
    public static class Initializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public static void UserCmsInitializer(this IApplicationBuilder app)
        {
            app.UseCmsMiddleware(); // 添加cms拦截器
            // app.UseCmsRoutes();          // 注册路由

            //Cms.OnInit += CmsEventRegister.Init;
            // 初始化资源
            SiteResourceInit.Init();
            Cms.ConfigCache(new MemoryCacheWrapper());
            Cms.Init(BootFlag.Normal, null);
            // 加载插件
            //WebCtx.Current.Plugin.Connect();


            //设置可写权限
            Cms.Utility.SetDirCanWrite(CmsVariables.RESOURCE_PATH);
            Cms.Utility.SetDirCanWrite("templates/");
            Cms.Utility.SetDirCanWrite(CmsVariables.FRAMEWORK_PATH);
            Cms.Utility.SetDirCanWrite(CmsVariables.PLUGIN_PATH);
            Cms.Utility.SetDirCanWrite(CmsVariables.TEMP_PATH + "update");
            Cms.Utility.SetDirHidden("config");

            //注册定时任务
            //CmsTask.Init();
        }
    }
}