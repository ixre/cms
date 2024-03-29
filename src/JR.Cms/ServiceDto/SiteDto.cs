﻿/*
* Copyright(C) 2010-2013 OPS.CC
* 
* File Name	: Site.cs
* author_id	: Newmin (new.min@msn.com)
* Create	: 2013/05/21 19:59:54
* Description	:
*
*/

using JR.Cms.Domain.Interface.Common.Language;
using JR.Cms.Domain.Interface.Site;
using JR.Stand.Core.Framework.Automation;

namespace JR.Cms.ServiceDto
{
    /// <summary>
    /// 站点
    /// </summary>
    [EntityForm]
    public struct SiteDto
    {
        /// <summary>
        /// 从站点领域对象转换成为数据传输对象
        /// </summary>
        /// <param name="ist"></param>
        /// <returns></returns>
        public static SiteDto ConvertFromSite(ISite ist)
        {
            var site = ist.Get();
            return new SiteDto
            {
                ProAddress = site.ProAddress,
                AppPath = site.AppPath,
                Domain = site.Domain,
                FullDomain = ist.FullDomain,
                Location = site.Location,
                ProEmail = site.ProEmail,
                ProFax = site.ProFax,
                Language = ist.Language(),
                ProPost = site.ProPost,
                Name = site.Name,
                Note = site.Note,
                AloneBoard = site.AloneBoard,
                ProNotice = site.ProNotice,
                ProPhone = site.ProPhone,
                ProIm = site.ProIm,
                RunType = (int)ist.RunType(),
                SeoDescription = site.SeoDescription,
                SeoKeywords = site.SeoKeywords,
                SeoTitle = site.SeoTitle,
                SeoForceHttps = site.SeoForceHttps,
                SeoForceRedirect = site.SeoForceRedirect,
                SiteId = site.SiteId,
                ProSlogan = site.ProSlogan,
                State = ist.State(),
                ProTel = site.ProTel,
                Tpl = site.Tpl,
                BeianNo = site.BeianNo,
            };
        }


        public static ISite CopyTo(SiteDto dto, ISite ist)
        {
            var site = ist.Get();
            site.Name = dto.Name;
            site.ProAddress = dto.ProAddress;
            site.AppPath = dto.AppPath.Trim();
            site.Domain = dto.Domain.Trim();
            site.Location = dto.Location.Trim();
            site.ProEmail = dto.ProEmail;
            site.ProFax = dto.ProFax.Trim();
            site.Language = (int)dto.Language;
            site.ProPost = dto.ProPost.Trim();
            site.Note = dto.Note.Trim();
            site.AloneBoard = dto.AloneBoard;
            site.ProNotice = dto.ProNotice.Trim();
            site.ProPhone = dto.ProPhone.Trim();
            site.ProIm = dto.ProIm;
            site.SeoDescription = dto.SeoDescription;
            site.SeoKeywords = dto.SeoKeywords;
            site.SeoForceHttps = dto.SeoForceHttps;
            site.SeoForceRedirect = dto.SeoForceRedirect;
            site.SeoTitle = dto.SeoTitle;
            site.ProSlogan = dto.ProSlogan;
            site.State = (int)dto.State;
            site.ProTel = dto.ProTel;
            site.Tpl = dto.Tpl;
            site.BeianNo = dto.BeianNo;
            return ist;
        }


        /// <summary>
        /// 站点ID
        /// </summary>
        [FormField("SiteId", Group = "basic", Text = "站点Id", DisableEdit = true)]
        public int SiteId { get; set; }

        /// <summary>
        /// 站点名称
        /// </summary>
        [FormField("Name", Group = "basic", Text = "站点名称", IsRequired = true, Length = "[0,30]", Description = "站点显示的名称")]
        public string Name { get; set; }

        /// <summary>
        /// 站点备注
        /// </summary>
        [FormField("Note", Group = "basic", Text = "备注名称", Length = "[0,20]", Description = "不显示在站点中，如：中文网站")]
        public string Note { get; set; }

        /// <summary>
        /// 域名绑定
        /// </summary>
        [FormField("domain", Group = "basic", Text = "绑定域名", Description = "多个域名用空格隔开，并确保域名解析正确！")]
        public string Domain { get; set; }


        /// <summary>
        /// 目录名称
        /// </summary>
        [FormField("AppPath", Group = "basic", Text = "虚拟目录", Description = "可指定虚拟目录名称,以\"http://abc.com/目录名称/\"访问站点",
            Regex = "[A-Za-z0-9_]{0,10}")]
        public string AppPath { get; set; }

        /// <summary>
        /// 重定向地址
        /// </summary>
        [FormField("location", Group = "basic", Text = "重定向",
            Description = "可将首页重定向至指定URL,如：index.aspx将定位到www.xxx.com/index.aspx")]
        public string Location { get; set; }


        public string FullDomain { get; set; }

        /// <summary>
        /// 站点使用语言
        /// </summary>
        [FormField("language", Group = "basic", Text = "国际化/语言", IsRequired = true)]
        [SelectField(UseDrop = true,
            Data =
                "中文简体(Chinese Simplified)=2;中文繁体(Chinese Traditional)=3;英语(Unit States)=1;西班牙语(Spanish)=4;法语(French)=5;泰语(Thai)=6;俄语(Russian)=7;阿拉伯语(Arabic)=8")]
        public Languages Language { get; set; }


        /// <summary>
        /// 模板
        /// </summary>
        [FormField("tpl", Group = "basic", Text = "页面模板", DisableEdit = true, IsRequired = true)]
        public string Tpl { get; set; }

        /// <summary>
        /// 站点状态
        /// </summary>
        [FormField("State", Group = "basic", Text = "站点状态", IsRequired = true)]
        [SelectField(UseDrop = true, Data = "正常(Normal)=1;暂停访问(Paused)=2;关闭(Stopped)=3")]
        public SiteState State { get; set; }


        /// <summary>
        /// 独立面板
        /// </summary>
        [FormField("AloneBoard", Group = "basic", Text = "独立管理面板",
            Description = "开启单独管理后台")]
        [SelectField(Data = "关闭=0")]
        public int AloneBoard { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [FormField("ProTel", Group = "profile", Text = "联系电话", Length = "[0,50]")]
        public string ProTel { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [FormField("ProPhone", Group = "profile", Text = "手机号码", Length = "[0,11]")]
        public string ProPhone { get; set; }

        /// <summary>
        /// 传真号码
        /// </summary>
        [FormField("ProFax", Group = "profile", Text = "传真号码", Length = "[0,50]")]
        public string ProFax { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        [FormField("ProAddress", Group = "profile", Text = "联系地址", Length = "[0,100]")]
        public string ProAddress { get; set; }

        /// <summary>
        /// 邮编号码
        /// </summary>
        [FormField("ProPost", Group = "profile", Text = "邮编号码")]
        public string ProPost { get; set; }


        /// <summary>
        /// 电子邮箱
        /// </summary>
        [FormField("ProEmail", Group = "profile", Text = "电子邮箱", Length = "[0,100]")]
        public string ProEmail { get; set; }

        /// <summary>
        /// Im (qq/msn)
        /// </summary>
        [FormField("ProIm", Group = "profile", Text = "即时通讯", Description = "可以填写QQ、微信、MSN、淘宝等聊天工具账号", Length = "[0,50]")]
        public string ProIm { get; set; }

        /// <summary>
        /// 网站公告
        /// </summary>
        [FormField("ProNotice", Group = "profile", Text = "网站公告", MultiLine = true, Length = "[0,200]")]
        public string ProNotice { get; set; }

        /// <summary>
        /// 网站标语
        /// </summary>
        [FormField("ProSlogan", Group = "profile", Text = "网站标语", MultiLine = true, Length = "[0,200]")]
        public string ProSlogan { get; set; }

        /// <summary>
        /// 备案号
        /// </summary>
        [FormField("BeianNo", Group = "profile", Text = "备案号", MultiLine = true, Length = "[0,40]")]

        public string BeianNo { get; set; }

        /// <summary>
        /// 运行类型
        /// </summary>
        public int RunType { get; set; }

        /// <summary>
        /// 强制使用HTTPS
        /// </summary>
        [FormField("SeoForceHttps", Group = "seo", Text = "强制HTTPS",
            Description = "强制仅启用HTTPS,普通访问将跳转到HTTPS")]
        [SelectField(Data = "关闭=0;开启=1;")]
        public int SeoForceHttps { get; set; }

        /// <summary>
        /// 强制定向
        /// </summary>
        [FormField("SeoForceRedirect", Group = "seo", Text = "强制定向",
            Description = "")]
        [SelectField(Data = "关闭=0;定向www.域名=1;定向到顶级域名=2")]
        public int SeoForceRedirect { get; set; }

        /// <summary>
        /// SEO标题
        /// </summary>
        [FormField("SeoTitle", Group = "seo", Text = "SEO标题", MultiLine = true, Description = "选填,不超过100字",
            Length = "[0,100]")]
        public string SeoTitle { get; set; }

        /// <summary>
        /// SEO关键字
        /// </summary>
        [FormField("SeoKeywords", Group = "seo", Text = "首页关键字", MultiLine = true, Description = "不超过150字",
            Length = "[0,150]")]
        public string SeoKeywords { get; set; }

        /// <summary>
        /// SEO描述
        /// </summary>
        [FormField("SeoDescription", Group = "seo", Text = "首页SEO描述", MultiLine = true, Length = "[0,150]")]
        public string SeoDescription { get; set; }

    }
}