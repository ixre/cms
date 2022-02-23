/**
 * Copyright (C) 2009-2022 56X.NET, All rights reserved.
 *
 * name : ICmsJobRepository.cs
 * author : jarrysix
 * date : 2022/02/23 22:33:10
 * description :
 * history :
 */

using System.Collections.Generic;
using JR.Cms.Domain.Interface.Models;

namespace JR.Cms.Dao{
    /** 定时任务仓储接口 */
    public interface ICmsJobRepository{
        /// <summary>
        /// 获取所有定时任务
        /// </summary>
        /// <returns></returns>
        IList<CmsJobEntity> FindAll(); 
        
        /// <summary>
        /// 保存定时任务
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        long Save(CmsJobEntity e);
        
         /// <summary>
         /// 根据ID获取定时任务
         /// </summary>
         /// <param name="id"></param>
         /// <returns></returns>
         CmsJobEntity FindById(long id);
         
         /// <summary>
         /// 删除定时任务
         /// </summary>
         int DeleteById(long id);
    }
}