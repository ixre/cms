/**
 * Copyright (C) 2009-2022 56X.NET, All rights reserved.
 *
 * name : ICmsJobService.cs
 * author : jarrysix
 * date : 2022/02/24 12:31:22
 * description :
 * history :
 */

using System.Collections.Generic;
using JR.Cms.Domain.Interface.Models;
using JR.Stand.Core.Extensions;

namespace JR.Cms.ServiceContract
{
    /// <summary>
    /// 定时任务服务
    /// </summary>
    public interface ICmsJobService
    {
        /// <summary>
        /// 查找定时任务
        /// </summary>
        /// <returns></returns>
        CmsJobEntity FindJobById(long id);

        /// <summary>
        /// 查找全部定时任务
        /// </summary>
        /// <returns></returns>
        IList<CmsJobEntity> FindAllJob();

        /// <summary>
        /// 保存定时任务
        /// </summary>
        Error SaveJob(CmsJobEntity e);

        /// <summary>
        /// 删除定时任务
        /// </summary>
        int DeleteJobById(long id);
    }
}