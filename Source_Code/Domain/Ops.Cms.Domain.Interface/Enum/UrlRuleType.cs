/*
* Copyright(C) 2010-2013 S1N1.COM
* 
* File Name	: UrlRuleType
* Author	: Newmin (new.min@msn.com)
* Create	: 2013/04/04 15:59:54
* Description	:
*
*/

namespace AtNet.Cms.Domain.Interface.Enum
{
    /// <summary>
    /// 地址规则类型
    /// </summary>
    public enum  UrlRuleType:int
    {
        /// <summary>
        /// 自定义
        /// </summary>
        Custom=0,

        /// <summary>
        /// MVC框架
        /// </summary>
        Mvc=1,

        /// <summary>
        /// WebForm
        /// </summary>
        WebForm=2
    }
}
