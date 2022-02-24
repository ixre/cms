/**
 * Copyright (C) 2009-2022 56X.NET, All rights reserved.
 *
 * name : ICmsSearchEngineService.cs
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
/// 搜索引擎设置服务
/// </summary>
public interface ICmsSearchEngineService {
    /// <summary>
    /// 查找搜索引擎设置
    /// </summary>
    /// <returns></returns>
    CmsSearchEngineEntity FindSearchEngineById(long id);

    /// <summary>
    /// 查找全部搜索引擎设置
    /// </summary>
    /// <returns></returns>
    IList<CmsSearchEngineEntity> FindAllSearchEngine();

    /// <summary>
    /// 保存搜索引擎设置
    /// </summary>
    Error SaveSearchEngine(CmsSearchEngineEntity e);

    /// <summary>
    /// 删除搜索引擎设置
    /// </summary>
    int DeleteSearchEngineById(long id);
}
}