/*
* Copyright(C) 2010-2013 OPSoft Inc
* 
* File Name	: OperateResult
* Author	: Newmin (new.min@msn.com)
* Create	: 2012-01-06 16:35:33
* Description	:
*
*/

namespace Ops.Cms.Infrastructure
{

    public enum OperateResult
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        Success=1,

        /// <summary>
        /// 操作失败
        /// </summary>
        Fail = 2,

        /// <summary>
        /// 产生异常
        /// </summary>
        Except=3,

        /// <summary>
        /// 系统
        /// </summary>
        IsSystem=4,

        /// <summary>
        /// 不允许操作
        /// </summary>
        Disallow=5,

        /// <summary>
        /// 受关联的，操作不成功
        /// </summary>
        Related=6,

        /// <summary>
        /// 已存在
        /// </summary>
        Exists=7
    }
}
