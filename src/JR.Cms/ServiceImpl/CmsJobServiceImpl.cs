/**
 * Copyright (C) 2009-2022 56X.NET, All rights reserved.
 *
 * name : CmsJobServiceImpl.cs
 * author : jarrysix
 * date : 2022/02/24 12:31:22
 * description :
 * history :
 */

using System;
using System.Collections.Generic;
using JR.Cms.Dao;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.ServiceContract;
using JR.Stand.Core.Extensions;

namespace JR.Cms.ServiceImpl
{
    /// <summary>
    /// 定时任务服务
    /// </summary>
    public class CmsJobServiceImpl : ICmsJobService
    {
        private readonly ICmsJobRepository _repo;

        /// <summary>
        /// 创建服务实例
        /// </summary>
        /// <param name="repo">仓储</param>
        public CmsJobServiceImpl(ICmsJobRepository repo)
        {
            this._repo = repo;
        }

        /// <summary>
        /// 查找定时任务
        /// </summary>
        /// <returns></returns>
        public CmsJobEntity FindJobById(long id)
        {
            return this._repo.FindById(id);
        }

        /// <summary>
        /// 查找全部定时任务
        /// </summary>
        /// <returns></returns>
        public IList<CmsJobEntity> FindAllJob()
        {
            return this._repo.FindAll();
        }

        /// <summary>
        /// 保存定时任务
        /// </summary>
        public Error SaveJob(CmsJobEntity e)
        {
            try
            {
                CmsJobEntity dst;
                if (e.Id > 0)
                {
                    dst = this._repo.FindById(e.Id);
                    if (dst == null) throw new Exception("no such data");
                }
                else
                {
                    dst = CmsJobEntity.CreateDefault();
                }

                dst.JobName = e.JobName;
                dst.JobClass = e.JobClass;
                dst.CronExp = e.CronExp;
                dst.JobDescribe = e.JobDescribe;
                dst.Enabled = e.Enabled;
                this._repo.Save(dst);
            }
            catch (Exception ex)
            {
                return new Error((ex.InnerException ?? ex).Message);
            }

            return null;
        }

        /// <summary>
        /// 删除定时任务
        /// </summary>
        public int DeleteJobById(long id)
        {
            return this._repo.DeleteById(id);
        }
    }
}