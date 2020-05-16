using System.Runtime.InteropServices;
using JR.Stand.Abstracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace JR.Stand.Core.Web
{
    internal class HttpHostingImpl : IServeHosting
    {
        private readonly IHttpContextAccessor _accessor;

        public HttpHostingImpl(IHttpContextAccessor accessor)
        {
            this._accessor = accessor;
        }

        public void Stop()
        {
            var app = this._accessor.HttpContext.RequestServices.GetService<IApplicationLifetime>();
            app.StopApplication();
        }

        public T GetService<T>()
        {
            return this._accessor.HttpContext.RequestServices.GetService<T>();
        }

        public string BaseDirectory()
        {
            return EnvUtil.GetBaseDirectory();
        }

        public bool IsLinuxOS()
        {
            return EnvUtil.IsOSPlatform(OSPlatform.Linux) || EnvUtil.IsOSPlatform(OSPlatform.OSX);
        }
    }
}