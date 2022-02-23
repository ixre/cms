/**
 * Copyright (C) 2009-2022 56X.NET, All rights reserved.
 *
 * name : CmsSearchEngineEntity.cs
 * author : jarrysix
 * date : 2022/02/23 21:08:37
 * description :
 * history :
 */


using System.Collections.Generic;

namespace JR.Cms.Domain.Interface.Models
{
/// <summary>
/// 搜索引擎设置(cms_search_engine)
/// </summary>
public class CmsSearchEngineEntity 
{
    /// <summary>
    /// 编号
    /// </summary>
    public long Id{get;set;}
    
    /// <summary>
    /// 站点编号
    /// </summary>
    public long SiteId{get;set;}
    
    /// <summary>
    /// 百度推送Token
    /// </summary>
    public string BaiduSiteToken{get;set;}
    
    /// <summary>
    /// 创建深拷贝
    /// </summary>
    /// <returns></returns>
    public CmsSearchEngineEntity Copy()
    {
        return new CmsSearchEngineEntity
        {
            Id = this.Id,
            SiteId = this.SiteId,
            BaiduSiteToken = this.BaiduSiteToken,
        };
    }

    /// <summary>
    /// 转换为MAP
    /// </summary>
    /// <returns></returns>
    public IDictionary<string,object> ToMap()
    {
        return new Dictionary<string,object>
        {
            {"Id",this.Id},
            {"SiteId",this.SiteId},
            {"BaiduSiteToken",this.BaiduSiteToken},
        };
    }

    /// <summary>
    /// 使用默认值创建实例 
    /// </summary>
    /// <returns></returns>
    public static CmsSearchEngineEntity CreateDefault(){
        return new CmsSearchEngineEntity{
            Id = 0L,
            SiteId = 0L,
            BaiduSiteToken = "",
        };
    }
}
}