/**
 * Copyright (C) 2009-2022 56X.NET, All rights reserved.
 *
 * name : CmsJobEntity.cs
 * author : jarrysix
 * date : 2022/02/23 21:08:37
 * description :
 * history :
 */


using System.Collections.Generic;

namespace JR.Cms.Domain.Interface.Models
{
/// <summary>
/// 定时任务(cms_job)
/// </summary>
public class CmsJobEntity 
{
    /// <summary>
    /// 编号
    /// </summary>
    public long Id{get;set;}
    
    /// <summary>
    /// 任务名称
    /// </summary>
    public string JobName{get;set;}
    
    /// <summary>
    /// 任务类
    /// </summary>
    public string JobClass{get;set;}
    
    /// <summary>
    /// CRON表达式
    /// </summary>
    public string CronExp{get;set;}
    
    /// <summary>
    /// 任务描述
    /// </summary>
    public string JobDescribe{get;set;}
    
    /// <summary>
    /// 是否启用
    /// </summary>
    public int Enabled{get;set;}
    
    /// <summary>
    /// 创建深拷贝
    /// </summary>
    /// <returns></returns>
    public CmsJobEntity Copy()
    {
        return new CmsJobEntity
        {
            Id = this.Id,
            JobName = this.JobName,
            JobClass = this.JobClass,
            CronExp = this.CronExp,
            JobDescribe = this.JobDescribe,
            Enabled = this.Enabled,
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
            {"JobName",this.JobName},
            {"JobClass",this.JobClass},
            {"CronExp",this.CronExp},
            {"JobDescribe",this.JobDescribe},
            {"Enabled",this.Enabled},
        };
    }

    /// <summary>
    /// 使用默认值创建实例 
    /// </summary>
    /// <returns></returns>
    public static CmsJobEntity CreateDefault(){
        return new CmsJobEntity{
            Id = 0L,
            JobName = "",
            JobClass = "",
            CronExp = "",
            JobDescribe = "",
            Enabled = 0,
        };
    }
}
}