/*
 * Copyright 2012 OPS,All rights reseved!
 * name     : SqlConst
 * author   : newmin
 * date     : 2012/12/22    19:56
 * 
 */

namespace JR.Cms.Sql
{
    /// <summary>
    /// SQL语句常量
    /// </summary>
    internal class SqlConst
    {
        public const string Archive_NotSystemAndHidden = " flags LIKE '%st:''0''%' AND flags LIKE '%v:''1''%'";
        
        /// <summary>
        /// 特殊文档
        /// </summary>
        public const string Archive_Special = " flags LIKE '%st:''0''%' AND flags LIKE '%v:''1''%' AND flags LIKE '%sc:''1''%'";
    }
}
