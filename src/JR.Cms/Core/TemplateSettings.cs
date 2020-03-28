using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JR.Stand.Core.Framework;
using Newtonsoft.Json;

namespace JR.Cms.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ContainSetting : Attribute
    {
    }

    /// <summary>
    /// 模板设置
    /// </summary>
    public class TemplateSetting
    {
        private SettingFile sf;
        private string tplName;
        private bool _cfgShowError = true;
        private int _cfgOutlineLength = 80;
        private string _cfgSitemapSplit = ">";
        private string _cfgArchiveTagsFormat = "<a href=\"{url}\">{text}</a>";
        private string _cfgNavigatorLinkFormat = "<a href=\"{url}\" class=\"l1\"><span>{text}</span></a>";
        private string _cfgNavigatorChildFormat = "<a href=\"{url}\" class=\"l2\"><span>{text}</span></a>";
        private int _cfgFriendShowNum = 50;
        private string _cfgFriendLinkFormat = "<a href=\"{url}\">{text}</a>";
        private string _cfgTrafficFormat = "今日IP:{todayip},今日PV:{todaypv},历史PV:{totalpv},历史IP:{totalip}";
        private string _cfgCommentEditorHtml = string.Empty;

        private string _cfgArchiveFormat = @"<div class=""archive"">
                                                      <h1>{title2}</h1><div class=""meta"">发布时间：{createtime2}&nbsp;&nbsp;
                                                        栏目：<a href=""{curl}"">{cname}</a>&nbsp;&nbsp;浏览：{count}</div>
                                                       <div class=""content"">{content}</div>
                                                </div>";

        private string _cfgArchiveLinkFormat = "<li><a href=\"{url}\" title=\"{title}\">{title}</a></li>";
        private string _cfgPrevArchiveFormat = "<a href=\"{url}\" title=\"上一篇：{title}\">{title}</a>";
        private string _cfgNextArchiveFormat = "<a href=\"{url}\" title=\"下一篇：{title}\">{title}</a>";
        private string _cfgCategoryLinkFormat = "<li><a href=\"{url}\" title=\"{title}\">{title}</a></li>";
        private bool _cfgAllowAmousComment = true;
        private bool _cfgEnabledMobiPage = false;
        private string _tplDirName;


        public TemplateSetting(string tplName, string confPath)
        {
            _tplDirName = tplName;
            this.tplName = tplName;
            sf = new SettingFile(confPath);
            if (sf.Contains("TPL_NAME")) this.tplName = sf.Get("TPL_NAME");

            #region 获取设置

            if (sf.Contains("CFG_ShowError")) _cfgShowError = sf["CFG_ShowError"] == "true";

            if (sf.Contains("CFG_EnabledMobiPage")) _cfgEnabledMobiPage = sf["CFG_EnabledMobiPage"] == "true";

            if (sf.Contains("CFG_SitemapSplit")) _cfgSitemapSplit = sf["CFG_SitemapSplit"];
            if (sf.Contains("CFG_ArchiveTagsFormat")) _cfgArchiveTagsFormat = sf["CFG_ArchiveTagsFormat"];
            if (sf.Contains("CFG_NavigatorLinkFormat")) _cfgNavigatorLinkFormat = sf["CFG_NavigatorLinkFormat"];
            if (sf.Contains("CFG_NavigatorChildFormat")) _cfgNavigatorChildFormat = sf["CFG_NavigatorChildFormat"];
            if (sf.Contains("CFG_FriendShowNum")) int.TryParse(sf["CFG_FriendShowNum"], out _cfgFriendShowNum);
            if (sf.Contains("CFG_OutlineLength")) int.TryParse(sf["CFG_OutlineLength"], out _cfgOutlineLength);
            if (sf.Contains("CFG_FriendLinkFormat")) _cfgFriendLinkFormat = sf["CFG_FriendLinkFormat"];
            if (sf.Contains("CFG_TrafficFormat")) _cfgTrafficFormat = sf["CFG_TrafficFormat"];
            if (sf.Contains("CFG_CommentEditorHtml")) _cfgCommentEditorHtml = sf["CFG_CommentEditorHtml"];
            if (sf.Contains("CFG_allowAmousComment")) _cfgAllowAmousComment = sf["CFG_allowAmousComment"] == "true";
            if (sf.Contains("CFG_ArchiveFormat")) _cfgArchiveFormat = sf["CFG_ArchiveFormat"];
            if (sf.Contains("CFG_PrevArchiveFormat")) _cfgPrevArchiveFormat = sf["CFG_PrevArchiveFormat"];
            if (sf.Contains("CFG_NextArchiveFormat")) _cfgNextArchiveFormat = sf["CFG_NextArchiveFormat"];
            if (sf.Contains("CFG_ArchiveLinkFormat")) _cfgArchiveLinkFormat = sf["CFG_ArchiveLinkFormat"];
            if (sf.Contains("CFG_CategoryLinkFormat")) _cfgCategoryLinkFormat = sf["CFG_CategoryLinkFormat"];
            sf = null;

            #endregion
        }


        /// <summary>
        /// 是否显示模板错误
        /// </summary>
        public bool CfgShowError
        {
            get => _cfgShowError;
            set => _cfgShowError = value;
        }

        /// <summary>
        /// 是否启用Mobi页面
        /// </summary>
        public bool CfgEnabledMobiPage
        {
            get => _cfgEnabledMobiPage;
            set => _cfgEnabledMobiPage = value;
        }

        /// <summary>
        /// 摘要长度
        /// </summary>
        public int CfgOutlineLength
        {
            get => _cfgOutlineLength;
            set => _cfgOutlineLength = value;
        }

        /// <summary>
        /// 模板设置-Sitemap分割符
        /// </summary>
        public string CFG_SitemapSplit
        {
            get => _cfgSitemapSplit;
            set => _cfgSitemapSplit = value;
        }

        /// <summary>
        /// 模板设置-Tag格式
        /// </summary>
        public string CFG_ArchiveTagsFormat
        {
            get => _cfgArchiveTagsFormat;
            set => _cfgArchiveTagsFormat = value;
        }

        /// <summary>
        /// 导航链接格式
        /// </summary>
        public string CFG_NavigatorLinkFormat
        {
            get => _cfgNavigatorLinkFormat;
            set => _cfgNavigatorLinkFormat = value;
        }

        /// <summary>
        /// 子菜单链接格式
        /// </summary>
        public string CFG_NavigatorChildFormat
        {
            get => _cfgNavigatorChildFormat;
            set => _cfgNavigatorChildFormat = value;
        }

        /// <summary>
        /// 模板设置-友情链接显示数量
        /// </summary>
        public int CFG_FriendShowNum
        {
            get => _cfgFriendShowNum;
            set => _cfgFriendShowNum = value;
        }

        /// <summary>
        /// 模板设置-友情链接格式
        /// </summary>
        public string CFG_FriendLinkFormat
        {
            get => _cfgFriendLinkFormat;
            set => _cfgFriendLinkFormat = value;
        }

        /// <summary>
        /// 流量统计格式
        /// </summary>
        public string CFG_TrafficFormat
        {
            get => _cfgTrafficFormat;
            set => _cfgTrafficFormat = value;
        }

        /// <summary>
        /// 评论编辑器HTML
        /// </summary>
        public string CFG_CommentEditorHtml
        {
            get => _cfgCommentEditorHtml;
            set => _cfgCommentEditorHtml = value;
        }

        /// <summary>
        /// 是否允许匿名评论
        /// </summary>
        public bool CFG_AllowAmousComment
        {
            get => _cfgAllowAmousComment;
            set => _cfgAllowAmousComment = value;
        }

        /// <summary>
        /// 文档格式
        /// </summary>
        public string CFG_ArchiveFormat
        {
            get => _cfgArchiveFormat;
            set => _cfgArchiveFormat = value;
        }

        /// <summary>
        /// 文档链接格式
        /// </summary>
        public string CFG_ArchiveLinkFormat
        {
            get => _cfgArchiveLinkFormat;
            set => _cfgArchiveLinkFormat = value;
        }

        /// <summary>
        /// 上一篇链接格式
        /// </summary>
        public string CFG_PrevArchiveFormat
        {
            get => _cfgPrevArchiveFormat;
            set => _cfgPrevArchiveFormat = value;
        }

        /// <summary>
        /// 下一篇链接格式
        /// </summary>
        public string CFG_NextArchiveFormat
        {
            get => _cfgNextArchiveFormat;
            set => _cfgNextArchiveFormat = value;
        }

        /// <summary>
        /// 分类链接格式
        /// </summary>
        public string CFG_CategoryLinkFormat
        {
            get => _cfgCategoryLinkFormat;
            set => _cfgCategoryLinkFormat = value;
        }

        public void Save()
        {
            var sf = new SettingFile(string.Format("{0}templates/{1}/tpl.conf", Cms.PhysicPath, _tplDirName));

            /**************** 模板设置 ****************/
            sf.Set("TPL_NAME", tplName);
            sf.Set("CFG_ShowErrror", CfgShowError ? "true" : "false");
            sf.Set("CFG_SitemapSplit", _cfgSitemapSplit);
            sf.Set("CFG_ArchiveTagsFormat", _cfgArchiveTagsFormat);
            sf.Set("CFG_NavigatorLinkFormat", _cfgNavigatorLinkFormat);
            sf.Set("CFG_NavigatorChildFormat", _cfgNavigatorChildFormat);
            sf.Set("CFG_FriendShowNum", _cfgFriendShowNum.ToString());
            sf.Set("CFG_FriendLinkFormat", _cfgFriendLinkFormat);
            sf.Set("CFG_TrafficFormat", _cfgTrafficFormat);
            sf.Set("CFG_CommentEditorHtml", _cfgCommentEditorHtml);
            sf.Set("CFG_allowAmousComment", _cfgAllowAmousComment ? "true" : "false");
            sf.Set("CFG_OutlineLength", _cfgOutlineLength.ToString());
            sf.Set("CFG_ArchiveFormat", _cfgArchiveFormat);
            sf.Set("CFG_ArchiveLinkFormat", _cfgArchiveLinkFormat);
            sf.Set("CFG_PrevArchiveFormat", _cfgPrevArchiveFormat);
            sf.Set("CFG_NextArchiveFormat", _cfgNextArchiveFormat);
            sf.Set("CFG_CategoryLinkFormat", _cfgCategoryLinkFormat);
            sf.Set("CFG_EnabledMobiPage", _cfgEnabledMobiPage ? "true" : "false");
            sf.Flush();
        }

        public IDictionary<string, string> GetNameDictionary()
        {
            var sf = new SettingFile(string.Format("{0}templates/{1}/tpl.conf", Cms.PhysicPath, _tplDirName));
            string json;
            if (!sf.Contains("TPL_NAMES") || string.IsNullOrEmpty(json = sf.Get("TPL_NAMES")))
                return new Dictionary<string, string>();
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public void SaveTemplateNames(string json)
        {
            var sf = new SettingFile(string.Format("{0}templates/{1}/tpl.conf", Cms.PhysicPath, _tplDirName));
            sf.Set("TPL_NAMES", json);
            sf.Flush();
        }
    }
}