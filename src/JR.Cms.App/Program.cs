using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JR.Cms.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"{DateTime.Now.ToString()} - [ JRCms][ Log]: cms is starting..");
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