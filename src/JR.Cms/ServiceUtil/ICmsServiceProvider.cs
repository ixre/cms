﻿using JR.Cms.ServiceContract;

namespace JR.Cms.ServiceUtil
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICmsServiceProvider
    {
        /// <summary>
        /// 站点服务
        /// </summary>
        ISiteServiceContract SiteService { get; }

        /// <summary>
        /// 内容服务 
        /// </summary>
        IContentServiceContract ContentService { get; }

        /// <summary>
        /// 文档服务
        /// </summary>
        IArchiveServiceContract ArchiveService { get; }

        /// <summary>
        /// 用户服务
        /// </summary>
        IUserServiceContract UserService { get; }
        
        /// <summary>
        /// 定时任务服务
        /// </summary>
        ICmsJobService JobService { get; }
        
        /// <summary>
        /// SEO服务
        /// </summary>
        ICmsSearchEngineService SeoService { get; }
    }
}