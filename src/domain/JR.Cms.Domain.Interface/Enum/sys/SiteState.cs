/*
* Copyright(C) 2010-2013 OPSoft Inc
* 
* File Name	: SiteState
* Author	: Newmin (new.min@msn.com)
* Create	: 2013/05/21 19:59:54
* Description	:
*
*/

namespace Spc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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
