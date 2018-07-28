//
// HTML创建
//  修改说明：
//      2012-12-29  newmin  [+]:Link & SAList
//      2013-03-05  newmin  [+]: 标签缩写
//      2013-03-06  newmin  [+]: 评论模块
//      2013-03-07  newmin  [+]: 添加数据标签参数符 "[ ]",主要用于outline[200]
//      2013-03-11  newmin  [+]: 添加authorname列，用于显示作者名称
//


namespace Spc.Template
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using Spc.BLL;
    using Spc.Models;
    using Spc.Utility;
    using J6.Json;
    using J6.Template;
    using J6.Web.UI;
    using Spc.Web;

    public class MultLangCmsTemplates : CmsTemplates
    {
        private static Type __type;
        private static readonly LangLabelReader langReader;
        static MultLangCmsTemplates()
        {
            __type = typeof(MultLangCmsTemplates);
            langReader = new LangLabelReader(AppDomain.CurrentDomain.BaseDirectory + "config/lang.conf");
        }

        /// <summary>
        /// 编译模板
        /// </summary>
        internal static TemplateHandler<object> CompliedTemplate = (object classInstance, ref string html) =>
        {
            MicroTemplateEngine mctpl = new MicroTemplateEngine(classInstance);
            html = mctpl.Execute(html);
        };

        /// <summary>
        /// 格式化地址
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        public override string FormatUrl(UrlRulePageKeys key, params string[] datas)
        {
            string lang = global::System.Web.HttpContext.Current.Items["lang"] as string;
            if (!String.IsNullOrEmpty(lang))
            {
                string urlFormat = (Settings.TPL_UseFullPath ? Settings.SYS_DOMAIN : String.Empty)
                    + TemplateUrlRule.Urls[TemplateUrlRule.RuleIndex, (int)key];

                urlFormat = urlFormat.Replace("{lang}", lang);

                return datas == null ? urlFormat : String.Format(urlFormat, datas);
            }

            return base.FormatUrl(key, datas);
        }

        protected string Lang(string key)
        {
            string lang = HttpContext.Current.Items["lang"] as string;
            try
            {
                return langReader.GetValue(key, lang);
            }
            catch
            {
                return String.Format("不存在值，参数：{0}->{1}", key, lang);
            }
        }

        public override void SetPager(int pageIndex, int pageCount, int recordCount, string firstLink, string link)
        {
            const string pagerPack = "上一页|下一页|{0}|选择页码：{0}页";
            string pagerPackConfig = this.Lang("pager");

            string[] pagerLangs = pagerPackConfig.Split('|');
            if (pagerLangs.Length != 4)
            {
                pagerLangs = pagerPack.Split('|');
            }

            const string pagerTpl = "<div class=\"pager\">{0}</div>";
            UrlPager p = new UrlPager(pageIndex, pageCount);
            StringBuilder sb = new StringBuilder();

            sb.Append("<div class=\"pager\">");

            p.RecordCount = recordCount;
            p.FirstPageLink = firstLink;
            p.LinkFormat = link;

            p.PreviousPageText = pagerLangs[0];
            p.NextPageText = pagerLangs[1];
            p.PageTextFormat = pagerLangs[2];
            p.Style = PagerStyle.Custom;
            p.EnableSelect = true;
            p.DisplayInfo = true;
            p.LinkCount = 10;
            p.SelectPageText = pagerLangs[3];

            Cms.Context.Items[this.PushPagerKey()] = String.Format(pagerTpl, p.ToString());
        }
    }
}