/*
* Copyright(C) 2010-2012 S1N1.COM
* 
* File Name	: UrlPager
* Author	: Administrator
* Create	: 2012/10/9 21:49:44
* Description	:
*
*/

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace JR.Stand.Core.Framework.Web.UI
{
    [Flags]
    public enum PagingFlag
    {
        Control = 1,
        Previous = 1 << 1,
        Next = 1 << 2
    }


    public interface IPagingGetter
    {
        string Get(int page, int total, int nowPage, PagingFlag flag, out string text);
    }

    internal class DefaultPagingGetter : IPagingGetter
    {
        private static readonly CustomPagingGetter Getter;

        static DefaultPagingGetter()
        {
            Getter = new CustomPagingGetter(
                "",
                "?page=%d",
                "上一页",
                "下一页"
                );
        }

        public string Get(int page, int total, int nowPage, PagingFlag flag, out string text)
        {
            return Getter.Get(page, total, nowPage, flag, out text);
        }
    }

    public class CustomPagingGetter : IPagingGetter
    {
        private readonly string _nextPageText;
        private readonly string _previousPageText;
        private readonly string _pagerLinkFormat;
        private readonly string _firstLinkFormat;

        public CustomPagingGetter(
            string firstLinkFormat,
            string pagerLinkFormat,
            string previousPageText,
            string nextPageText)
        {
            this._firstLinkFormat = firstLinkFormat;
            this._pagerLinkFormat = pagerLinkFormat;
            this._nextPageText = nextPageText;
            this._previousPageText = previousPageText;
        }

        public string Get(int page, int total, int nowPage, PagingFlag flag, out string text)
        {
            if ((flag & PagingFlag.Control) != 0)
            {
                if ((flag & PagingFlag.Previous) != 0)
                {
                    text = this._previousPageText;
                    if (page == 1)
                    {
                        return "javascript:;";
                    }
                    else
                    {
                        if (nowPage == 1)
                        {
                            return this._firstLinkFormat;
                        }
                        return String.Format(this._pagerLinkFormat, nowPage);
                    }
                }
                else if ((flag & PagingFlag.Next) != 0)
                {
                    text = this._nextPageText;
                    if (page == total)
                    {
                        return "javascript:;";
                    }
                    else
                    {
                        return String.Format(this._pagerLinkFormat, nowPage);
                    }
                }
            }

            text = nowPage.ToString();
            if (nowPage == 1 && this._firstLinkFormat.Length != 0)
            {
                return this._firstLinkFormat;
            }
            return String.Format(this._pagerLinkFormat, nowPage);
        }
    }


    public class UrlPager
    {
        private int _linkCount;


        internal UrlPager(int currentPageIndex, int pageCount, bool enableSelect)
        {
            CurrentPageIndex = currentPageIndex;
            PageCount = pageCount;
            EnableSelect = enableSelect;
        }

        /// <summary>
        /// 当前页面索引（从1开始）
        /// </summary>
        private int CurrentPageIndex { get; set; }

        /// <summary>
        /// 页面总数
        /// </summary>
        private int PageCount { get; set; }

        //获取分页数据
        public IPagingGetter Getter { get; set; }

        /// <summary>
        /// 链接长度,创建多少个跳页链接
        /// </summary>
        public int LinkCount
        {
            get
            {
                if (this._linkCount == 0)
                {
                    this._linkCount = 10;
                }
                return this._linkCount;
            }
            set { this._linkCount = value; }
        }

        /// <summary>
        /// 记录条数
        /// </summary>
        public int RecordCount { get; set; }


        /// <summary>
        /// 下一栏链接文字
        /// </summary>
        public string NextPagerLinkText { get; set; }

        /// <summary>
        /// 选页框文本
        /// </summary>
        public string SelectPageText { get; set; }

        /// <summary>
        /// 是否允许输入页码调页
        /// </summary>
        public bool EnableInput { get; set; }

        /// <summary>
        /// 使用选页
        /// </summary>
        private bool EnableSelect { get; }

        /// <summary>
        /// 分页详细记录,如果为空字符则用默认,为空则不显示
        /// </summary>
        public String PagerTotal { get; set; }


        /// <summary>
        /// 输入分页链接HTML代码
        /// </summary>
        /// <returns></returns>
        public string Pager()
        {
            string cls;
            string linkText;
            string linkUrl;

            StringBuilder sb = new StringBuilder();

            string pageCount = (this.PageCount == 0 ? 1 : this.PageCount).ToString();

            //Div Wrap
            sb.Append("<div class=\"pagination mod-pagination\">");

            //输出上一页
            if (this.CurrentPageIndex > 1)
            {
                cls = "previous";
                linkUrl = this.Getter.Get(this.CurrentPageIndex, this.PageCount,
                    this.CurrentPageIndex - 1, PagingFlag.Control | PagingFlag.Previous, out linkText);
            }
            else
            {
                cls = "disabled";
                linkUrl = this.Getter.Get(this.CurrentPageIndex, this.PageCount,
                    this.CurrentPageIndex, PagingFlag.Control | PagingFlag.Previous, out linkText);
            }
            sb.Append(String.Format(@"<span class=""pagination__item it {0}""><a class=""pagination__item--link"" href=""{1}"">{2}</a></span>"
                , cls, linkUrl, linkText));


            //起始页:CurrentPageIndex / 10 * 10+1
            //结束页:(CurrentPageIndex%10==0?CurrentPageIndex-1: CurrentPageIndex) / 10 * 10
            //当前页数能整除10的时候需要减去10页，否则不能选中


            //链接页码数量(默认10)
            int c = this.LinkCount;
            int startPage = (CurrentPageIndex - 1) / c * c + 1;

            for (int i = 1, j = startPage;
                i <= c && j <= PageCount;
                i++, j = (CurrentPageIndex % c == 0 ? CurrentPageIndex - 1 : CurrentPageIndex) / c * c + i)
            {
                //输出页面
                if (j == CurrentPageIndex)
                {
                    var gotoPrevious = j != 1 && j % c == 1; //是否上一栏分页
                    if (gotoPrevious)
                    {
                        linkUrl = this.Getter.Get(this.CurrentPageIndex, this.PageCount,
                            j - 1, 0, out linkText);
                        sb.Append(String.Format(@"<span class=""pagination__item it page""><a class=""pagination__item--link"" href=""{1}"">{2}</a></span>",
                            cls, linkUrl, "..."));
                    }

                    //如果为页码为当前页

                    sb.Append("<span class=\"pagination__item it current\"><a class=\"pagination__item--link\" href=\"javascript:void(0)\">")
                        .Append(j.ToString())
                        .Append("</a></span>");


                    //如果为最后一个页码，则显示下一栏
                    if (!gotoPrevious && j % c == 0 && j != PageCount)
                    {
                        linkUrl = this.Getter.Get(this.CurrentPageIndex, this.PageCount,
                            j + 1, 0, out linkText);

                        sb.Append(String.Format(@"<span class=""pagination__item it page""><a class=""pagination__item--link"" href=""{1}"">{2}</a></span>",
                            cls, linkUrl, "..."));
                    }
                }
                else
                {
                    //页码不为当前页，则输出页码
                    //如果为第一页，用第一页格式

                    linkUrl = this.Getter.Get(this.CurrentPageIndex, this.PageCount,
                        j, 0, out linkText);
                    sb.Append($"<span class=\"pagination__item it page\"><a class=\"pagination__item--link\" href=\"{linkUrl}\">{linkText}</a></span>");
                }
            }


            //显示输入页码框
            //if (EnableInput) sb.Append("<input type=\"text\" size=\"2\"/><a href=\"#\" class=\"go\" onclick=\"gotoPage(this)\">").Append(InputButtonText ?? "跳页").Append("</a>");


            //输出下一页链接
            if (this.CurrentPageIndex < this.PageCount)
            {
                cls = "next";
                linkUrl = this.Getter.Get(this.CurrentPageIndex, this.PageCount,
                    this.CurrentPageIndex + 1, PagingFlag.Control | PagingFlag.Next, out linkText);
            }
            else
            {
                cls = "disabled";
                linkUrl = this.Getter.Get(this.CurrentPageIndex, this.PageCount,
                    this.CurrentPageIndex, PagingFlag.Control | PagingFlag.Next, out linkText);
            }
            sb.Append($@"<span class=""pagination__item it {cls}""><a class=""pagination__item--link"" href=""{linkUrl}"">{linkText}</a></span>");



            //显示下拉选页框
            if (EnableSelect)
            {
                linkUrl = this.Getter.Get(this.CurrentPageIndex, this.PageCount,
                    1, 0, out linkText);

                //选页框
                StringBuilder selectSb = new StringBuilder();
                selectSb.Append("<select class=\"page-select\" onchange=\"").Append("location.href='")
                    .Append(linkUrl.Replace("1", "#")).Append("'.replace('#',this.value);").Append("\">");

                if (this.PageCount == 0)
                {
                    selectSb.Append("<option value=\"1\" selected=\"selected\">1</option>");
                }
                else
                {
                    for (int i = 1; i <= this.PageCount; i++)
                    {
                        selectSb.Append("<option value=\"").Append(i.ToString());
                        if (i == this.CurrentPageIndex)
                        {
                            selectSb.Append("\" selected=\"selected");
                        }
                        selectSb.Append("\">").Append(i.ToString())
                            .Append("</option>");
                    }
                }
                selectSb.Append("</select>");

                //设置下拉框HTML格式
                if (String.IsNullOrEmpty(this.SelectPageText)) this.SelectPageText = "{0}";
                else if (this.SelectPageText.IndexOf("{0}", StringComparison.Ordinal) == -1) this.SelectPageText += "{0}";

                //将选页框添加到内容中
                sb.Append(String.Format(String.Concat("<span class=\"pagination__item select\">", this.SelectPageText, "</span>"),
                    selectSb.ToString()));
            }


            //显示信息
            if (this.PagerTotal != null)
            {
                const string pagerTotalFormat = @"&nbsp;<span class=""pagination__item page-info"">第{0}/{1}页，共{2}条。</span>";
                string pagerTotal = this.PagerTotal;
                if (pagerTotal == String.Empty)
                {
                    pagerTotal = pagerTotalFormat;
                }
                sb.Append(String.Format(pagerTotal,
                    CurrentPageIndex.ToString(), pageCount,
                    this.RecordCount.ToString()));
            }

            //Wrap Close
            sb.Append("</div>");

            return Regex.Replace(sb.ToString(), "\\s\\s|\\r|\\t", String.Empty);
        }
    }

    public static class UrlPaging
    {
        private static readonly IPagingGetter DefaultGetter = new DefaultPagingGetter();

        public static UrlPager NewPager(int page, int pageCount, IPagingGetter pg)
        {
            if (pageCount == 0) pageCount = 1;
            if (page == 0) page = 1;

            UrlPager p = new UrlPager(page, pageCount, false);
            if (pg == null)
            {
                pg = DefaultGetter;
            }
            else
            {
                p.Getter = pg;
            }

            return p;
        }

        /// <summary>
        /// 创建分页信息
        /// </summary>
        /// <param name="format"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <param name="firstFormat"></param>
        /// <returns></returns>
        public static string PagerHtml(string firstFormat, string format,
            int currentPageIndex, int recordCount, int pageCount)
        {
            IPagingGetter pg = new CustomPagingGetter(
                firstFormat,
                format,
                "&lt;&lt;上一页",
                "&gt;&gt;下一页"
                );


            UrlPager p = NewPager(currentPageIndex, pageCount, pg);

            // p.PreviousPageText = "<<";
            // p.NextPageText = ">>";
            p.EnableInput = true;
            p.SelectPageText = "跳页";
            p.PagerTotal = String.Empty;
            p.RecordCount = recordCount;
            p.PagerTotal = String.Empty;
            return p.Pager();
        }
    }
}