/*
* Copyright(C) 2010-2013 Z3Q.NET
* 
* File Name	: SiteState
* publisher_id	: Newmin (new.min@msn.com)
* Create	: 2013/05/21 19:59:54
* Description	:
*
*/

namespace JR.Cms.Domain.Interface.Site
{

    /// <summary>
    /// 站点状态
    /// </summary>
    public enum SiteState:int
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal=1,

        /// <summary>
        /// 暂停访问
        /// </summary>
        Paused=2,

        /// <summary>
        /// 关闭
        /// </summary>
        Closed=3
    }
}
