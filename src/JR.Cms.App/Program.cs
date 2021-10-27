using System;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JR.Cms.App
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} - [ JRCms][ Log]: cms is starting..");
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, logging) =>
                {
                    var devMode = context.HostingEnvironment.IsDevelopment();
                    if (!devMode)
                    {
                        logging.ClearProviders();
                    }
                    else
                    {
                        Cms.OfficialEnvironment = false;
                    }
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}