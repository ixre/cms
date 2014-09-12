

namespace Ops.Cms.Template
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Ops.Framework;
    using Ops.Cms;

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
		private bool cfg_showError=true;
        private int cfg_outlineLength = 80;
        private string cfg_sitemapSplit = ">";
        private string cfg_ArchiveTagsFormat = "<a href=\"{url}\">{text}</a>";
        private string cfg_navigatorLinkFormat = "<a href=\"{url}\" title=\"{text}\">{text}</a>";
        private string cfg_navigatorChildFormat = "<a href=\"{url}\" title=\"{text}\">{text}</a>";
        private int cfg_friendShowNum = 50;
        private string cfg_friendLinkFormat = "<a href=\"{url}\">{text}</a>";
        private string cfg_trafficFormat = "今日IP:{todayip},今日PV:{todaypv},历史PV:{totalpv},历史IP:{totalip}";
        private string cfg_commentEditorHtml = String.Empty;
        private string cfg_archiveFormat = @"<div class=""archive"">
                                                      <h1>{title2}</h1><div class=""meta"">发布时间：{createtime2}&nbsp;&nbsp;
                                                        栏目：<a href=""{curl}"">{cname}</a>&nbsp;&nbsp;浏览：{count}</div>
                                                       <div class=""content"">{content}</div>
                                                </div>";
        private string cfg_archiveLinkFormat = "<li><a href=\"{url}\" title=\"{title}\">{title}</a></li>";
        private string cfg_prevArchiveFormat = "<a href=\"{url}\" title=\"上一篇：{title}\">{title}</a>";
        private string cfg_nextArchiveFormat = "<a href=\"{url}\" title=\"下一篇：{title}\">{title}</a>";
        private string cfg_categoryLinkFormat = "<li><a href=\"{url}\" title=\"{title}\">{title}</a></li>";
        private bool cfg_allowAmousComment = true;
        
        public TemplateSetting(string tpl)
        {
            this.tplName = tpl;
            sf = new SettingFile(String.Format("{0}templates/{1}/tpl.conf", AppDomain.CurrentDomain.BaseDirectory, tpl));

            #region 获取设置
            if(sf.Contains("CFG_ShowError"))
            {
                this.cfg_showError = sf["CFG_ShowError"]=="true";
            }
            if (sf.Contains("CFG_SitemapSplit"))
            {
                this.cfg_sitemapSplit = sf["CFG_SitemapSplit"];
            }
            if (sf.Contains("CFG_ArchiveTagsFormat"))
            {
                this.cfg_ArchiveTagsFormat = sf["CFG_ArchiveTagsFormat"];
            }
            if (sf.Contains("CFG_NavigatorLinkFormat"))
            {
                this.cfg_navigatorLinkFormat = sf["CFG_NavigatorLinkFormat"];
            }
            if (sf.Contains("CFG_NavigatorChildFormat"))
            {
               this.cfg_navigatorChildFormat= sf["CFG_NavigatorChildFormat"];
            }
            if (sf.Contains("CFG_FriendShowNum"))
            {
                int.TryParse(sf["CFG_FriendShowNum"], out this.cfg_friendShowNum);
            }
            if (sf.Contains("CFG_OutlineLength"))
            {
                int.TryParse(sf["CFG_OutlineLength"], out this.cfg_outlineLength);
            }
            if (sf.Contains("CFG_FriendLinkFormat"))
            {
                this.cfg_friendLinkFormat= sf["CFG_FriendLinkFormat"];
            }
            if (sf.Contains("CFG_TrafficFormat"))
            {
                this.cfg_trafficFormat = sf["CFG_TrafficFormat"];
            }
            if (sf.Contains("CFG_CommentEditorHtml"))
            {
               this.cfg_commentEditorHtml = sf["CFG_CommentEditorHtml"];
            }
            if (sf.Contains("CFG_allowAmousComment"))
            {
                this.cfg_allowAmousComment= sf["CFG_allowAmousComment"] == "true";
            }
            if (sf.Contains("CFG_ArchiveFormat"))
            {
               this.cfg_archiveFormat = sf["CFG_ArchiveFormat"];
            }
            if (sf.Contains("CFG_PrevArchiveFormat"))
            {
               this.cfg_prevArchiveFormat= sf["CFG_PrevArchiveFormat"];
            }
            if (sf.Contains("CFG_NextArchiveFormat"))
            {
               this.cfg_nextArchiveFormat = sf["CFG_NextArchiveFormat"];
            }
            if (sf.Contains("CFG_ArchiveLinkFormat"))
            {
                this.cfg_archiveLinkFormat= sf["CFG_ArchiveLinkFormat"];
            }
            if (sf.Contains("CFG_CategoryLinkFormat"))
            {
               this.cfg_categoryLinkFormat = sf["CFG_CategoryLinkFormat"];
            }
            sf = null;

            #endregion
        }

        /// <summary>
        /// 是否显示模板错误
        /// </summary>
        public bool CFG_ShowError{
        	get{return cfg_showError;}
        	set{cfg_showError=value;}
        }

        /// <summary>
        /// 摘要长度
        /// </summary>
        public int CFG_OutlineLength
        {
            get
            {
                return cfg_outlineLength;
            }
            set
            {
                cfg_outlineLength = value;
            }
        }

        /// <summary>
        /// 模板设置-Sitemap分割符
        /// </summary>
        public string CFG_SitemapSplit
        {
            get
            {
                return cfg_sitemapSplit;
            }
            set
            {
                cfg_sitemapSplit = value;
            }
        }

        /// <summary>
        /// 模板设置-Tag格式
        /// </summary>
        public string CFG_ArchiveTagsFormat
        {
            get
            {
                return cfg_ArchiveTagsFormat;
            }
            set
            {
                cfg_ArchiveTagsFormat = value;
            }
        }

        /// <summary>
        /// 导航链接格式
        /// </summary>
        public string CFG_NavigatorLinkFormat
        {
            get
            {
                return cfg_navigatorLinkFormat;
            }
            set
            {
                cfg_navigatorLinkFormat = value;
            }
        }

        /// <summary>
        /// 子菜单链接格式
        /// </summary>
        public string CFG_NavigatorChildFormat
        {
            get
            {
                return cfg_navigatorChildFormat;
            }
            set
            {
                cfg_navigatorChildFormat = value;
            }
        }

        /// <summary>
        /// 模板设置-友情链接显示数量
        /// </summary>
        public int CFG_FriendShowNum
        {
            get
            {
                return cfg_friendShowNum;
            }
            set
            {
                cfg_friendShowNum = value;
            }
        }

        /// <summary>
        /// 模板设置-友情链接格式
        /// </summary>
        public string CFG_FriendLinkFormat
        {
            get
            {
                return cfg_friendLinkFormat;
            }
            set
            {
                cfg_friendLinkFormat = value;
            }
        }

        /// <summary>
        /// 流量统计格式
        /// </summary>
        public string CFG_TrafficFormat
        {
            get
            {
                return cfg_trafficFormat;
            }
            set
            {
                cfg_trafficFormat = value;
            }
        }

        /// <summary>
        /// 评论编辑器HTML
        /// </summary>
        public string CFG_CommentEditorHtml
        {
            get
            {
                return cfg_commentEditorHtml;
            }
            set
            {
                cfg_commentEditorHtml = value;
            }
        }

        /// <summary>
        /// 是否允许匿名评论
        /// </summary>
        public bool CFG_AllowAmousComment
        {
            get
            {
                return cfg_allowAmousComment;
            }
            set
            {
                cfg_allowAmousComment = value;
            }
        }

        /// <summary>
        /// 文档格式
        /// </summary>
        public string CFG_ArchiveFormat
        {
            get
            {
                return cfg_archiveFormat;
            }
            set
            {
                cfg_archiveFormat = value;
            }
        }

        /// <summary>
        /// 文档链接格式
        /// </summary>
        public string CFG_ArchiveLinkFormat
        {
            get
            {
                return cfg_archiveLinkFormat;
            }
            set
            {
                cfg_archiveLinkFormat = value;
            }
        }

        /// <summary>
        /// 上一篇链接格式
        /// </summary>
        public string CFG_PrevArchiveFormat
        {
            get
            {
                return cfg_prevArchiveFormat;
            }
            set
            {
                cfg_prevArchiveFormat = value;
            }
        }

        /// <summary>
        /// 下一篇链接格式
        /// </summary>
        public string CFG_NextArchiveFormat
        {
             get
            {
                return cfg_nextArchiveFormat;
            }
            set
            {
                cfg_nextArchiveFormat = value;
            }
        }

        /// <summary>
        /// 分类链接格式
        /// </summary>
        public string CFG_CategoryLinkFormat
        {
            get
            {
                return cfg_categoryLinkFormat;
            }
            set
            {
                cfg_categoryLinkFormat = value;
            }
        }

        public void Save()
        {
            SettingFile sf = new SettingFile(String.Format("{0}templates/{1}/tpl.conf",Cms.PyhicPath,this.tplName));

            /**************** 模板设置 ****************/
             if (sf.Contains("CFG_ShowErrror"))
            {
                sf["CFG_ShowErrror"] = this.CFG_ShowError?"true":"false";
            }
            else
            {
            	sf.Append("CFG_ShowErrror", this.CFG_ShowError?"true":"false");
            }
            
            if (sf.Contains("CFG_SitemapSplit"))
            {
                sf["CFG_SitemapSplit"] = this.cfg_sitemapSplit;
            }
            else
            {
                sf.Append("CFG_SitemapSplit", this.cfg_sitemapSplit);
            }
            if (sf.Contains("CFG_ArchiveTagsFormat"))
            {
                sf["CFG_ArchiveTagsFormat"] = this.cfg_ArchiveTagsFormat;
            }
            else
            {
                sf.Append("CFG_ArchiveTagsFormat", this.cfg_ArchiveTagsFormat);
            }
            if (sf.Contains("CFG_NavigatorLinkFormat"))
            {
                sf["CFG_NavigatorLinkFormat"] = this.cfg_navigatorLinkFormat;
            }
            else
            {
                sf.Append("CFG_NavigatorLinkFormat", this.cfg_navigatorLinkFormat);
            }
            if (sf.Contains("CFG_NavigatorChildFormat"))
            {
                sf["CFG_NavigatorChildFormat"] = this.cfg_navigatorChildFormat;
            }
            else
            {
                sf.Append("CFG_NavigatorChildFormat", this.cfg_navigatorChildFormat);
            }
            if (sf.Contains("CFG_FriendShowNum"))
            {
                sf["CFG_FriendShowNum"] = this.cfg_friendShowNum.ToString();
            }
            else
            {
                sf.Append("CFG_FriendShowNum", this.cfg_friendShowNum.ToString());
            }
            if (sf.Contains("CFG_FriendLinkFormat"))
            {
                sf["CFG_FriendLinkFormat"] = this.cfg_friendLinkFormat;
            }
            else
            {
                sf.Append("CFG_FriendLinkFormat", this.cfg_friendLinkFormat);
            }
            if (sf.Contains("CFG_TrafficFormat"))
            {
                sf["CFG_TrafficFormat"] = this.cfg_trafficFormat;
            }
            else
            {
                sf.Append("CFG_TrafficFormat", this.cfg_trafficFormat);
            }
            if (sf.Contains("CFG_CommentEditorHtml"))
            {
                sf["CFG_CommentEditorHtml"] = this.cfg_commentEditorHtml;
            }
            else
            {
                sf.Append("CFG_CommentEditorHtml", this.cfg_commentEditorHtml);
            }
            if (sf.Contains("CFG_allowAmousComment"))
            {
                sf["CFG_allowAmousComment"] = this.cfg_allowAmousComment ? "true" : "false";
            }
            else
            {
                sf.Append("CFG_allowAmousComment", this.cfg_allowAmousComment ? "true" : "false");
            }
            if (sf.Contains("CFG_OutlineLength"))
            {
                sf["CFG_OutlineLength"] = this.cfg_outlineLength.ToString();
            }
            else
            {
                sf.Append("CFG_OutlineLength", this.cfg_outlineLength.ToString());
            }
            if (sf.Contains("CFG_ArchiveFormat"))
            {
                sf["CFG_ArchiveFormat"] = this.cfg_archiveFormat;
            }
            else
            {
                sf.Append("CFG_ArchiveFormat", this.cfg_archiveFormat);
            }
            if (sf.Contains("CFG_ArchiveLinkFormat"))
            {
                sf["CFG_ArchiveLinkFormat"] =this.cfg_archiveLinkFormat;
            }
            else
            {
                sf.Append("CFG_ArchiveLinkFormat", this.cfg_archiveLinkFormat);
            }
            if (sf.Contains("CFG_PrevArchiveFormat"))
            {
                sf["CFG_PrevArchiveFormat"] = this.cfg_prevArchiveFormat;
            }
            else
            {
                sf.Append("CFG_PrevArchiveFormat", this.cfg_prevArchiveFormat);
            }
            if (sf.Contains("CFG_NextArchiveFormat"))
            {
                sf["CFG_NextArchiveFormat"] = this.cfg_nextArchiveFormat;
            }
            else
            {
                sf.Append("CFG_NextArchiveFormat", this.cfg_nextArchiveFormat);
            }

            if (sf.Contains("CFG_CategoryLinkFormat"))
            {
                sf["CFG_CategoryLinkFormat"] = this.cfg_categoryLinkFormat;
            }
            else
            {
                sf.Append("CFG_CategoryLinkFormat", this.cfg_categoryLinkFormat);
            }
            sf.Flush();
        }

    }
    
}
