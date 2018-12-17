//
// Copyright (C) 2007-2008 OPSoft INC,All rights reseved.
// 
// Project: OPS.OPSite.Enterprise
// FileName : UtilityConfig.cs
// Author : PC-CWLIU (new.min@msn.com)
// Create : 2012/4/10 11:27:52
// Description :
//
// Get infromation of this software,please visit our site http://www.j6.cc
//
//

namespace J6.Cms
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Obsolete]
    internal class UtilityConfig
    {
        internal static int templateUrlIndex = 1;

        /// <summary>
        /// 设置模板URL方案
        /// </summary>
        /// <param name="index"></param>
        public static void SetTemplateUrl(int index)
        {
            if (index > -1 && index < 3)
            {

                templateUrlIndex = index;
            }
            else
            {
                throw new ArgumentOutOfRangeException("只能为0,1和2");
            }
        }

        /// <summary>
        /// 设置模板自定义URL
        /// </summary>
        /// <param name="urls"></param>
        public static void SetCustomeTemplateUrl(params string[] urls)
        {
            for (int i = 0; i < urls.Length; i++)
            {
                //Template.CmsTemplates.urls[0, i] = urls[i];
            }
        }
    }
}
