/**
 * Copyright (C) 2009-2022 56X.NET, All rights reserved.
 *
 * name : CmsJobRepositoryImpl.cs
 * author : jarrysix
 * date : 2022/02/23 23:16:48
 * description :
 * history :
 */

using System.Collections.Generic;
using System.Data;
using Dapper;
using JR.Cms.Domain.Interface.Models;
using JR.Stand.Core.Data.Provider;

namespace JR.Cms.Dao.Impl{
    /** 定时任务仓储接口 */
    public class CmsJobRepositoryImpl : ICmsJobRepository{
        private readonly IDbProvider _provider;
    
        /// <summary>
        /// 创建仓储对象
        /// </summary>
        public CmsJobRepositoryImpl(IDbProvider provider)
        {
            this._provider = provider;
        }
            
        /// <summary>
        /// 获取所有定时任务
        /// </summary>
        /// <returns></returns>
        public IList<CmsJobEntity> FindAll()
        {
           using (IDbConnection db = _provider.GetConnection())
           {
               return db.Query<CmsJobEntity>(_provider.FormatQuery("SELECT * FROM $PREFIX_job")).AsList();
           } 
        }
        
        /// <summary>
        /// 保存定时任务
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public long Save(CmsJobEntity e)
        {
            using (IDbConnection db = _provider.GetConnection())
            {
                if (e.Id == 0)
                {
                    int i = db.Execute(_provider.FormatQuery(
                        @"INSERT INTO $PREFIX_job(
                           job_name,job_class,cron_exp,job_describe,enabled
                        ) VALUES(
                          @JobName,@JobClass,@CronExp,@JobDescribe,@Enabled
                        )"),
                    e);
                    return e.Id;
                }

                db.Execute(
                    _provider.FormatQuery(
                    @"UPDATE $PREFIX_job SET 
                     job_name=@JobName, job_class=@JobClass, cron_exp=@CronExp, job_describe=@JobDescribe, enabled=@Enabled  
                     WHERE id=@Id"),
                    e);
                return e.Id;
            }
        }
        
         /// <summary>
         /// 根据ID获取定时任务
         /// </summary>
         /// <param name="id"></param>
         /// <returns></returns>
         public CmsJobEntity FindById(long id)
         {
            using (IDbConnection db = _provider.GetConnection())
            {
                return db.QueryFirst<CmsJobEntity>(_provider.FormatQuery("SELECT * FROM $PREFIX_job WHERE id = @Id"),
                    new CmsJobEntity{
                      Id = id, 
                    });
            }  
         }
         
         /// <summary>
         /// 删除定时任务
         /// </summary>
         public int DeleteById(long id)
         {
            using (IDbConnection db = _provider.GetConnection())
            {
                 return db.Execute(
                     _provider.FormatQuery(
                         "DELETE FROM $PREFIX_job WHERE id = @Id"),
                     new CmsJobEntity{
                       Id = id, 
                     });
            }
         }
    }
}