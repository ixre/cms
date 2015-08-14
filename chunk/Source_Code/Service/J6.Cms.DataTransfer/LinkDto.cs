
/*
* Copyright(C) 2010-2013 Z3Q.NET
* 
* File Name	: Site.cs
* publisher_id	: Newmin (new.min@msn.com)
* Create	: 2013/05/21 19:59:54
* Description	:
*
*/


using System;
using J6.DevFw.Framework.Automation;

namespace J6.Cms.DataTransfer
{
    /// <summary>
    /// 链接数据传输实体
    /// </summary>
    [EntityForm]
    public class LinkDto
    {
        /// <summary>
        /// 链接编号
        /// </summary> 
        [FormField("LinkId", Text = "链接编号", Hidden = true)]
        public int LinkID { get; set; }

        /// <summary>
        /// 链接名称,如:原文出处
        /// </summary>
        [FormField("LinkName", Text = "链接名称",Descript="如：原文出处, 可不填写", Length="[0,20]")]
        public String LinkName { get; set; }

        /// <summary>
        /// 链接标题,如：新浪网
        /// </summary>
        [FormField("LinkTitle", Text = "<span class=\"red\">*</span>链接标题", Descript = "如:新浪官方网站", Length = "[0,50]")]
        public String LinkTitle { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [FormField("LinkUri", Text = "<span class=\"red\">*</span>链接地址",
            Descript = "<br />如:http://www.sina.com，如果是站内链接不以'/'开头。",
            Regex = "^(.+)://(.+)$|^[^/]+/(.*)$|^(\\d+)$", Length = "[0,150]")]
        public String LinkUri { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [FormField("Enabled", Text = "是否使用")]
        [SelectField(Data="启用=True;停用=False",UseDrop=true)]
        public bool Enabled { get; set; }
    }
}
