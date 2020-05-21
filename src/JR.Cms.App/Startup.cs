using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JR.Cms.Conf;
using JR.Cms.Web.Portal;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Framework.Web;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace JR.Cms.App
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // 使用Session
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = "_cms_session";
                options.IdleTimeout = TimeSpan.FromSeconds(1200);
                //options.Cookie.Path = "/" + Settings.SYS_ADMIN_TAG;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            // 启动异步IO
            services.Configure<IISServerOptions>(options => options.AllowSynchronousIO = true);
            services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);
            // 使用MVC并添加Session支持
            services.AddControllers().AddSessionStateTempDataProvider();
            services.AddResponseCompression();
            HttpContextNetCoreExtension.AddHttpContextAccessor(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 注册编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // 使用Session
            app.UseSession();
            app.UseStaticHttpContext();
            app.UseRouting();
            app.UseResponseCompression(); // 启用gzip
            //app.UseStaticFiles();        // 开启静态资源访问

            var root = Directory.GetCurrentDirectory();
            // 使用root目录作为根目录
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(root, "root")),
            });
            //　添加根目录作为静态资源文件
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(root)
            });
            // 处理异常
            app.UseExceptionHandler(builder => builder.Run(async context => await ErrorEvent(context)));
            // 捕获404错误
            app.UseStatusCodePagesWithReExecute("/software/errors/{0}");
            // Cms初始化, 注册中间件，应在路由注册之前注册
            app.UserCmsInitializer();
            app.UseCmsRoutes();
        }

        private static Task ErrorEvent(HttpContext context)
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;
            if (error != null)
            {
                Console.WriteLine("[ cms][ error]: {0} {1}\n", error.Message, error.StackTrace);
                CmsLogger.Println(LoggerLevel.Error, error.Message + "; stack =" + error.StackTrace);
            }
            context.Response.ContentType = "text/html;charset=utf-8";
            return context.Response.WriteAsync("系统未知异常，请联系管理员");
        }
    }

}