using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using JR.Stand.Core.Framework;
using Microsoft.CodeAnalysis;
using Quartz;

namespace JR.Cms.Core.Scheduler.Job
{
    /// <summary>
    /// 提交搜索引擎任务
    /// </summary>
    public class SearchEngineSubmitJob:IJob
    {
        private readonly Logger _logger = new Logger(typeof(SearchEngineSubmitJob));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
           long jobId = context.JobDetail.JobDataMap.GetLongValue("job_id");
           CmsJobEntity job = LocalService.Instance.JobService.FindJobById(jobId);
           if (job == null)
           {
               _logger.Error("JOB不存在,建议重新启动应用");
               return Task.CompletedTask;
           }

           if (job.Enabled != 1)
           {
               _logger.Info("任务未启动");
               return Task.CompletedTask;
           }

           long unix = TimeUtils.Unix() - 2 * 3600;
           IList<SiteDto> sites = LocalService.Instance.SiteService.GetSites();
           foreach (SiteDto site in sites)
           {
               var se = LocalService.Instance.SeoService.FindSearchEngineBySiteId(site.SiteId);
               IList<String> urls = new List<string>();
               IEnumerable<ArchiveDto> archives =
                   LocalService.Instance.ArchiveService.GetArchiveByTimeAgo(site.SiteId, unix, int.MaxValue);
               foreach (var archive in archives)
               {
                   urls.Add(archive.Path + ".html");
               }
               
           }
           return Task.CompletedTask;
        }
    }
}