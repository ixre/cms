using System;
using System.Threading.Tasks;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceContract;
using JR.Stand.Core.Extensions;
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
            long now = TimeUtils.Unix();
            var list = LocalService.Instance.ArchiveService.GetArchivesByScheduleTime(now, 10);
            if (list.Count > 0)
            {
                _logger.Info("定时发布文章, 总数:" + list.Count);
                foreach (var item in list)
                {
                    Error err = LocalService.Instance.ArchiveService.PublishArchive(item.SiteId, item.Id);
                    if (err != null)
                    {
                        _logger.Error(String.Format("定时发布文章失败, ID={0},Err={1}", item.Id, err.Message));
                    }
                }
            }
        }
    }
}