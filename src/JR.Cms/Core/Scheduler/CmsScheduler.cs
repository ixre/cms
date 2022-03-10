using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Library.CacheService;
using JR.Stand.Core.Framework;
using Quartz;
using Quartz.Impl;
using JR.Stand.Core.Framework.Scheduler;

namespace JR.Cms.Core.Scheduler
{
    /// <summary>
    /// CMS定时任务
    /// </summary>
    public static class CmsScheduler
    {

        private static readonly Logger Logger = new Logger(typeof(CmsScheduler));
        private static bool initialized = false;
        private static IScheduler sc;
        private static CronDaemon daemon;
        /// <summary>
        /// 初始化定时任务
        /// </summary>
        public static void Init()
        {
            if (initialized) return;
            CheckJob();
            var jobs = LocalService.Instance.JobService.FindAllJob();
            initialized = true;
            if (Environment.Version.Major <= 4)
            {
                StartSchedulerWithCronNet(jobs);
                return;
            }
            StartSchedulerWithQuartz(jobs);
        }

        private static void StartSchedulerWithCronNet(IList<CmsJobEntity> jobs)
        {
            daemon = new CronDaemon();
            foreach(CmsJobEntity je in jobs)
            {
                try
                {
                    Type classType = Assembly.GetExecutingAssembly().GetType(je.JobClass);
                    ICronJob job = Activator.CreateInstance(classType) as ICronJob;
                    if (job == null)throw new NotImplementedException($"{je.JobClass} not implementation ICronJob");
                    daemon.AddJob(ParseCronExp(je.CronExp), () =>
                    {
                        job.ExecuteJob(je);
                    });
                }
                catch (Exception ex)
                {
                    Logger.Error($"任务注册失败, {je.JobName}, 异常:" + (ex.InnerException ?? ex).Message + "\n" + (ex.InnerException ?? ex).StackTrace);
                }
                initialized = true;
                Logger.Info($"定时任务{je.JobName}注册成功, 启动规则为:{je.CronExp}");
            }
            daemon.Start();
        }

        /// <summary>
        /// 将6位长度的cron表达式转为5位
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static string ParseCronExp(string exp)
        {
            if (exp.Split(' ').Length == 6)
            {
                int i = exp.IndexOf(' ');
                return exp.Substring(i + 1);
            }

            return exp;
        }

        /// <summary>
        /// 使用Quartz运行定时任务
        /// </summary>
        /// <returns></returns>
        private static void StartSchedulerWithQuartz(IList<CmsJobEntity> jobs)
        {
            // 构造一个调度器工厂
            NameValueCollection props = new NameValueCollection
            {
                {"quartz.serializer.type", "binary"},
                                  {"quartz.threadPool.type","Quartz.Simpl.SimpleThreadPool, Quartz" },
                {"quartz.threadPool.threadCount","1" },
                {"quartz.jobStore.misfireThreshold","60000" },

            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            // 得到一个调度器
            sc = factory.GetScheduler().Result;
            sc.Start();

            foreach (CmsJobEntity je in jobs)
            {
                try
                {
                    // 定义作业并将其绑定到HelloJob类
                    Type classType = Assembly.GetExecutingAssembly().GetType(je.JobClass);
                    IJobDetail job = JobBuilder.Create()
                        .OfType(classType)
                        .UsingJobData("job_id", je.Id)
                        .WithIdentity(je.JobName, "group1")
                        .Build();
                    // 触发作业现在运行，然后每40秒运行一次
                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity("myTrigger" + je.Id, "group1")
                        .StartNow()
                        .WithCronSchedule(je.CronExp)
                        .Build();
                    sc.ScheduleJob(job, trigger).Start();
                }
                catch (Exception ex)
                {
                    Logger.Error($"任务注册失败, {je.JobName}, 异常:" + (ex.InnerException ?? ex).Message + "\n" + (ex.InnerException ?? ex).StackTrace);
                }
                initialized = true;
                Logger.Info($"定时任务{je.JobName}注册成功, 启动规则为:{je.CronExp}");
            }
        }

        /// <summary>
        /// 关闭定时任务
        /// </summary>
        public static async void Shutdown()
        {
            if (sc != null && !sc.IsShutdown) await sc.Shutdown();
            if (daemon != null) daemon.Stop();
        }
        private static void CheckJob()
        {
            var jobs = LocalService.Instance.JobService.FindAllJob();
            // 初始化默认Job
            IList<CmsJobEntity> initial = new List<CmsJobEntity>
            {
                new CmsJobEntity
                {
                    JobName = "搜索引擎提交任务",
                    CronExp = "0 0 */2 * * ?",
                    Enabled = 1,
                    JobClass = "JR.Cms.Core.Scheduler.Job.SearchEngineSubmitJob",
                    JobDescribe = "每日将新的文档提交到百度等搜索引擎, 每2小时执行一次"
                }
            };
            foreach (var it in initial)
            {
                if (jobs.FirstOrDefault(a => a.JobName == it.JobName) == null)
                {
                    LocalService.Instance.JobService.SaveJob(it);
                }
            }
        }
    }
}