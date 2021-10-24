/*
* Copyright(C) 2010-2013 OPS.CC
* 
* File Name	: Languages.cs
* author_id	: Newmin (new.min@msn.com)
* Create	: 2013/05/21 19:59:54
* Description	:
*
*/

namespace JR.Cms.Domain.Interface.Common.Language
{
    /// <summary>
    /// 语言
    /// </summary>
    public enum Languages : int
    {
        /// <summary>
        /// 未知语言
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 英语(Unit States)
        /// </summary>
        en_US = 1,

        /// <summary>
        /// 中文简体(Chinese Simplified)
        /// </summary>
        zh_CN = 2,

        /// <summary>
        /// 中文繁体(Chinese Traditional)
        /// </summary>
        zh_TW = 3,

        /// <summary>
        /// 西班牙语
        /// </summary>
        Es = 4,

        /// <summary>
        /// 法语France
        /// </summary>
        Fr = 5,

        /// <summary>
        /// 泰语(ภาษาไทย)
        /// </summary>
        Th = 6,

        /// <summary>
        /// 俄语
        /// </summary>
        Ru = 7,

        /// <summary>
        /// 阿拉伯语Arabic
        /// </summary>
        Ar = 8,
    }
}