/*
* Copyright(C) 2010-2013 OPS.CC
* 
* File Name	: TemplateUrlRule
* author_id	: Newmin (new.min@msn.com)
* Create	: 2013/04/04 15:59:54
* Description	:
*
*/

using System;
using System.Collections.Generic;
using JR.Cms.Domain.Interface.Enum;

namespace JR.Cms
{
    /// <summary>
    /// 
    /// </summary>
    public static class TemplateUrlRule
    {
        private static readonly string[,] urls;

        /// <summary>
        /// 
        /// </summary>
        public static string[,] Urls => urls;

        static TemplateUrlRule()
        {
            urls = new[,]
            {
                // ID:0 自定义Url
                {
                    "{0}",
                    "{0}/{1}",
                    "文档显示URL",
                    "搜索第一页URL",
                    "搜索其他页URL",
                    "", "", "", ""
                },

                // ID:1 适用于MVC
                {
                    "admin",
                    "{0}", //栏目页面
                    "{0}/{1}/", //栏目分页
                    "{0.html",
                    "{0}/{1}.html", //文档页面
                    "search?w={0}&c={1}", //搜索页面
                    "search?w={0}&c={1}&p={2}", //搜索分页
                    "tag?t={0}", //Tags页面
                    "tag?t={0}&p={1}" //Tags分页
                },

                // ID:2 适用于WEBForm
                {
                    "admin.aspx",
                    "list.aspx?c={0}",
                    "list.aspx?c={0}&p={1}",
                    "view.aspx?a={1}#{0}",
                    "view.aspx?a={1}#{0}",
                    "search.aspx?w={0}",
                    "search.aspx?w={0}&p={1}",
                    "tag.aspx?t={0}", //Tags页面
                    "tag.aspx?t={0}&p={1}" //Tags分页
                }
            };
        }

        private static int templateUrlRuleIndex = 1;

        /// <summary>
        /// 设置模板URL方案
        /// </summary>
        public static void SetRule(UrlRuleType type)
        {
            templateUrlRuleIndex = (int) type;
        }

        /// <summary>
        /// 规则索引
        /// </summary>
        public static int RuleIndex => templateUrlRuleIndex;

        /// <summary>
        /// 设置URL
        /// </summary>
        /// <param name="urlDict"></param>
        public static void SetUrl(UrlRuleType rule, IDictionary<UrlRulePageKeys, string> urlDict)
        {
            int ruleID;
            var num = urls.GetLength(1); //typeof(UrlRulePageKeys).GetFields().Length-1;
            if (num != urlDict.Count) throw new ArgumentOutOfRangeException();

            ruleID = (int) rule;

            foreach (var p in urlDict) urls[ruleID, (int) p.Key] = p.Value;
        }
    }
}