using System.Collections.Generic;
using System.Text;
using JR.Cms.Conf;
using JR.Cms.Domain.Interface.Common.Language;
using JR.Cms.Domain.Interface.Enum;
using JR.Cms.Library.CacheService;
using JR.Cms.Library.Utility;
using JR.Cms.ServiceDto;
using JR.Stand.Core.Template.Impl;
using JR.Stand.Core.Web;

namespace JR.Cms.Web.Portal.Template.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class PageArchive : ITemplateVariableInstance
    {
        private string _properties;
        private IDictionary<string, string> _dict;
        private string _tagsHtml;
        private string _url;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archive"></param>
        public PageArchive(ArchiveDto archive)
        {
            Archive = archive;
            _dict = new Dictionary<string, string>();
        }

        private static string FormatUrl(UrlRulePageKeys key, string[] dataArray)
        {
            var urlFormat = (Settings.TPL_FULL_URL_PATH ? Cms.Context.SiteDomain + "/" : Cms.Context.SiteAppPath)
                            + TemplateUrlRule.Urls[TemplateUrlRule.RuleIndex, (int) key];
            return dataArray == null ? urlFormat : string.Format(urlFormat, dataArray);
        }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> Data
        {
            get
            {
                if (_dict == null)
                {
                    _dict = new Dictionary<string, string>();
                    if (Archive.ExtendValues != null)
                    {
                        foreach (var value in Archive.ExtendValues) _dict.Add(value.Field.Name, value.Value);
                    }
                }
                return _dict;
            }
        }

        /// <summary>
        /// 文档
        /// </summary>
        internal ArchiveDto Archive { get; private set; }

        /// <summary>
        /// 编号
        /// </summary>
        [TemplateVariableField("编号")]
        public string Id => Archive.Id.ToString();

        [TemplateVariableField("别名")]
        public string Alias => string.IsNullOrEmpty(Archive.Alias) ? Archive.StrId : Archive.Alias;

        [TemplateVariableField("地址")]
        public string Url
        {
            get
            {
                if (_url == null)
                {
                    var prefix = Settings.TPL_FULL_URL_PATH ? Cms.Context.SiteDomain : Cms.Context.SiteAppPath;
                    _url = prefix + "/" + Archive.Path + ".html";
                }

                return _url;
            }
        }


        /// <summary>
        /// 标题
        /// </summary>
        [TemplateVariableField("标题")]
        public string Title => Archive.Title;

        [TemplateVariableField("子标题")] public string SmallTitle => Archive.SmallTitle;

        [TemplateVariableField("子标题")]
        public string ContactSmallTitle => string.IsNullOrEmpty(Archive.SmallTitle) ? "" : "-" + Archive.SmallTitle;


        /// <summary>
        /// 作者
        /// </summary>
        [TemplateVariableField("作者")]
        public string Author => LocalService.Instance.UserService.GetUserRealName(Archive.PublisherId) ?? "未知";

        /// <summary>
        /// 来源
        /// </summary>
        [TemplateVariableField("来源")]
        public string Source
        {
            get
            {
                var source = Archive.Source;
                return string.IsNullOrEmpty(source) ? "原创" : source;
            }
        }

        /// <summary>
        /// 大纲
        /// </summary>
        [TemplateVariableField("大纲")]
        public string Outline
        {
            get
            {
                if (!string.IsNullOrEmpty(Archive.Outline))
                    return Archive.Outline;
                return ArchiveUtility.GetOutline(Archive.Outline, 200);
            }
        }

        /// <summary>
        /// 内容
        /// </summary>
        [TemplateVariableField("内容")]
        public string Content => Archive.Content;

        /// <summary>
        /// 缩略图
        /// </summary>
        [TemplateVariableField("缩略图")]
        public string Thumbnail => Archive.Thumbnail;

        /// <summary>
        /// 标签
        /// </summary>
        [TemplateVariableField("标签Tags")]
        public string Tags => Archive.Tags;


        /// <summary>
        /// 标签
        /// </summary>
        [TemplateVariableField("标签Tags，HTML内容")]
        public string TagsHtml
        {
            get
            {
                if (_tagsHtml == null)
                {
                    if (Archive.Tags == string.Empty) return Cms.Language.Get(LanguagePackageKey.PAGE_NO_TAGS);

                    var sb = new StringBuilder();
                    var tagArr = Archive.Tags.Split(',');

                    var j = 0;
                    foreach (var tag in tagArr)
                    {
                        if (j++ != 0) sb.Append(",");

                        sb.Append("<a href=\"")
                            .Append(FormatUrl(UrlRulePageKeys.Tag, new[]{HttpUtils.UrlEncode(tag)}))
                            .Append("\" search-url=\"")
                            .Append(FormatUrl(UrlRulePageKeys.Search, new[]{HttpUtils.UrlEncode(tag), string.Empty}))
                            .Append("\">")
                            .Append(tag)
                            .Append("</a>");
                    }

                    _tagsHtml = sb.ToString();
                }

                return _tagsHtml;
            }
        }

        /// <summary>
        /// 访问统计
        /// </summary>
        [TemplateVariableField("访问量")]
        public string Count => Archive.ViewCount.ToString();

        /// <summary>
        /// 发布时间
        /// </summary>
        [TemplateVariableField("发布时间")]
        public string Publish => string.Format("{0:yyyy-MM-dd HH:mm}", Archive.CreateTime);

        /// <summary>
        /// 修改时间
        /// </summary>
        [TemplateVariableField("修改时间")]
        public string Modify => string.Format("{0:yyyy-MM-dd HH:mm}", Archive.UpdateTime);

        /// <summary>
        /// 扩展属性列表
        /// </summary>
        [TemplateVariableField("扩展属性")]
        public string Properties
        {
            get
            {
                if (_properties == null)
                {
                    var sb = new StringBuilder();
                    sb.Append("<ul class=\"extend_field_list\">");

                    foreach (var value in Archive.ExtendValues)
                        if (!string.IsNullOrEmpty(value.Value))
                            sb.Append("<li class=\"extend_")
                                .Append(value.Field.GetDomainId().ToString()).Append("\"><span class=\"attrName\">")
                                .Append(value.Field.Name).Append(":</span><span class=\"value\">")
                                .Append(value.Value).Append("</span></li>");

                    sb.Append("</ul>");
                    _properties = sb.ToString();
                }

                return _properties;
            }
        }

        public void AddData(string key, string data)
        {
            _dict.Add(key, data);
        }

        public void RemoveData(string key)
        {
            _dict.Remove(key);
        }
    }
}