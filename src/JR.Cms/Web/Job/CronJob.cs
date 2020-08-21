using Quartz;
using Quartz.Impl;

namespace JR.Cms.Web.Job
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public static class CronJob
    {
        /// <summary>
        /// 初始化JOB
        /// </summary>
        public static async void Initialize()
        {
            // Grab the Scheduler instance from the Factory
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();
            RegisterSiteMapJob(scheduler);
            // and start it off
            await scheduler.Start();
        }

        /// <summary>
        /// 注册站点地图JOB
        /// </summary>
        /// <param name="scheduler"></param>
        private static void RegisterSiteMapJob(IScheduler scheduler)
        {
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("site_map_trigger1", "group1")
                //.StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(12)
                    //.WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();

            IJobDetail job = JobBuilder.Create<SiteMapGenerateJob>()
                .WithIdentity("site_map_job", "group1")
                .Build();
            scheduler.ScheduleJob(job, trigger);
        }
    }
}