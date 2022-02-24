/**
 * Copyright (C) 2009-2022 56X.NET, All rights reserved.
 *
 * name : ICmsJobService.cs
 * author : jarrysix
 * date : 2022/02/24 09:00:31
 * description :
 * history :
 */

using System;
using System.Collections.Generic;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Infrastructure;

namespace JR.Cms.Dao
{
    /// <summary>
/// 定时任务服务
/// </summary>
public class CmsJobServiceImpl {
    private ICmsJobRepository _repo;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="repo"></param>
    public CmsJobServiceImpl(ICmsJobRepository repo)
    {
        this._repo = repo;
    }
   
    /// <summary>
    /// 查找
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public CmsJobEntity FindJobById(long id){
        return this._repo.FindById(id);
    }
    
    /// <summary>
    /// 查找全部
    /// </summary>
    /// <returns></returns>
    public IList<CmsJobEntity> FindAll() {
        return this._repo.FindAll();
    }

    /** 保存定时任务 */
    public Error saveJob(CmsJobEntity e){
            CmsJobEntity dst;
            if (e.Id > 0) {
                dst = this._repo.FindById(e.Id);
                if(dst == null)throw new Exception("no such data");
            } else {
                dst = CmsJobEntity.CreateDefault();
            }
            dst.setJobName(e.getJobName());
            dst.setJobClass(e.getJobClass());
            dst.setCronExp(e.getCronExp());
            dst.setJobDescribe(e.getJobDescribe());
            dst.setEnabled(e.getEnabled());
            
            this.repo.save(dst);
            return null;
        }).error();
    }


    /** 批量保存定时任务 */
    public Iterable<CmsJobEntity> saveAllJob(Iterable<CmsJobEntity> entities){
        return this.repo.saveAll(entities);
    }

    /** 删除定时任务 */
    public Error deleteJobById(Long id) {
         return Standard.std.tryCatch(()-> {
             this.repo.deleteById(id);
             return null;
         }).error();
    }
}
}