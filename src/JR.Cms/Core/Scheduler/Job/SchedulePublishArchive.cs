using System;
using System.Threading.Tasks;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Framework.Scheduler;
using Quartz;

namespace JR.Cms.Core.Scheduler.Job
{

    /// <summary>
    /// 定时发布文章
    /// </summary>
    public class SchedulePublishArchive : IJob, ICronJob
    {
        private readonly Logger _logger = Logger.Factory(typeof(SchedulePublishArchive));

        /// <summary>
        /// .NET6.0执行发布
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            this.ProcessPublishArchives();
            return Task.CompletedTask;
        }

        /// <summary>
        /// .NET4.5执行发布
        /// </summary>
        /// <param name="context"></param>
        public void ExecuteJob(object context)
        {
            this.ProcessPublishArchives();
        }

        private void ProcessPublishArchives()
        {
            Console.WriteLine("定时发布文章");
        }

    }
}