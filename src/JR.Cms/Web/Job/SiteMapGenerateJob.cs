using System.Threading.Tasks;
using JR.Cms.Web.Util;
using Quartz;

namespace JR.Cms.Web.Job
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteMapGenerateJob:IJob
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            return SiteMapUtils.Generate();
        }
    }
}