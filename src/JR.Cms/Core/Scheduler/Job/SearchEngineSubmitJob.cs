using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Framework.Net;
using JR.Stand.Core.Framework.Scheduler;
using Quartz;

namespace JR.Cms.Core.Scheduler.Job
{
    /// <summary>
    /// 提交搜索引擎任务
    /// </summary>
    public class SearchEngineSubmitJob : IJob, ICronJob
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
            this.ExecuteByJobId(jobId);
           return Task.CompletedTask;
        }
        /// <summary>
        /// .NET45运行
        /// </summary>
        /// <param name="context"></param>

        public void ExecuteJob(object context)
        {
            CmsJobEntity job = context as CmsJobEntity;
            this.ExecuteByJobId(job.Id);
        }

        private void ExecuteByJobId(long jobId)
        {
            CmsJobEntity job = LocalService.Instance.JobService.FindJobById(jobId);
            if (job == null)
            {
                _logger.Error("JOB不存在,建议重新启动应用");
                return;

                if (job.Enabled != 1)
                {
                    _logger.Info($"任务{job.JobName}未启动");
                    return;
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
                        urls.Add(se.SiteUrl + "/" + archive.Path + ".html");
                    }
                    if (urls.Count > 0)
                    {
                        this.SubmitUrlToSearchEngine(site, se, urls);
                    }
                }
            }
        }

        private void SubmitUrlToSearchEngine(SiteDto siteDto, CmsSearchEngineEntity se, IList<string> urls)
        {
            String[] urlArray = urls.ToArray();
            _logger.Info($"[ Job][ Baidu]: 推送的URL为:{String.Join(",",urlArray)}");
            this.SubmitUrlToBaidu(siteDto,se, urlArray);
        }

        private void SubmitUrlToBaidu(SiteDto site, CmsSearchEngineEntity se, string[] urls)
        {
            if (se == null) return;
            // {
            //     se = new CmsSearchEngineEntity
            //     {
            //         SiteUrl = "https://fze.net",
            //         BaiduSiteToken = "44aehEoPIs7aBdef"
            //     };
            // }

            String ret = HttpClient.Request(
                $"http://data.zz.baidu.com/urls?site={se.SiteUrl}&token={se.BaiduSiteToken}", "POST",
                new HttpRequestParam
                {
                    Body = String.Join("\n", urls),
                });
            var rs = JsonSerializer.DeserializeObject<Dictionary<string, Object>>(ret);
            if (rs.TryGetValue("error", out _))
            {
                Object errorMessage = rs["message"];
                _logger.Error($"[ Job][ Baidu]: 百度URL推送失败:{errorMessage}");
            }
            else
            {
                _logger.Error($"[ Job][ Baidu]: 百度URL推送成功");
            }
        }

       
    }
}