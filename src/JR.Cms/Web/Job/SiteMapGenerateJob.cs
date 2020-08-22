using System;
using System.Threading.Tasks;
using JR.Cms.Conf;
using JR.Cms.Web.Util;
using JR.Stand.Abstracts.Safety;
using Quartz;

namespace JR.Cms.Web.Job
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteMapGenerateJob : IJob
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            if (String.IsNullOrEmpty(Settings.SYS_SITE_MAP_PATH))
            {
                return SafetyTask.CompletedTask;
            }

            if (Settings.SYS_SITE_MAP_PATH.IndexOf("localhost", StringComparison.Ordinal) != -1)
            {
                return SafetyTask.CompletedTask;
            }

            return SiteMapUtils.Generate();
        }
    }
}