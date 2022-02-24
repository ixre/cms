/**
 * Copyright (C) 2009-2022 56X.NET, All rights reserved.
 *
 * name : ICmsSearchEngineRepository.cs
 * author : jarrysix
 * date : 2022/02/24 12:31:22
 * description :
 * history :
 */

using System.Collections.Generic;
using JR.Cms.Domain.Interface.Models;

namespace JR.Cms.Dao{
    /** 搜索引擎设置仓储接口 */
    public interface ICmsSearchEngineRepository{
        /// <summary>
        /// 获取所有搜索引擎设置
        /// </summary>
        /// <returns></returns>
        IList<CmsSearchEngineEntity> FindAll(); 
        
        /// <summary>
        /// 保存搜索引擎设置
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        long Save(CmsSearchEngineEntity e);
        
         /// <summary>
         /// 根据ID获取搜索引擎设置
         /// </summary>
         /// <param name="id"></param>
         /// <returns></returns>
         CmsSearchEngineEntity FindById(long id);
         
         /// <summary>
         /// 删除搜索引擎设置
         /// </summary>
         int DeleteById(long id);
    }
}