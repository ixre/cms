
/*
* Copyright(C) 2010-2013 S1N1.COM
* 
* File Name	: Site.cs
* Author	: Newmin (new.min@msn.com)
* Create	: 2013/05/21 19:59:54
* Description	:
*
*/

using AtNet.Cms.Domain.Interface.Common.Language;
using AtNet.Cms.Domain.Interface.Site;
using AtNet.DevFw.Framework.Automation;

namespace AtNet.Cms.DataTransfer
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
        /// <param name="site"></param>
        /// <returns></returns>
        public static SiteDto ConvertFromSite(ISite site)
        {
            return new SiteDto
            {
                Address = site.Address,
                DirName = site.DirName,
                Domain = site.Domain,
                FullDomain =  site.FullDomain,
                Location = site.Location,
                Email = site.Email,
                Fax = site.Fax,
                Language = site.Language,
                PostCode = site.PostCode,
                Name = site.Name,
                Note = site.Note,
                Notice = site.Notice,
                Phone = site.Phone,
                Im = site.Im,
                RunType = (int)site.RunType,
                SeoDescription = site.SeoDescription,
                SeoKeywords = site.SeoKeywords,
                SeoTitle = site.SeoTitle,
                SiteId = site.Id,
                Slogan = site.Slogan,
                State = site.State,
                Tel = site.Tel,
                Tpl = site.Tpl
            };
        }


        /// <summary>
        /// 站点ID
        /// </summary>
        [FormField("siteid",Group="basic",Text="站点Id",DisableEdit=true)]
        public int SiteId { get; set; }

        /// <summary>
        /// 站点名称
        /// </summary>
        [FormField("name", Group = "basic", Text = "<span class=\"red\">*</span>站点名称", IsRequired = true, Length = "[0,30]")]
        public string Name { get; set; }

        /// <summary>
        /// 站点备注
        /// </summary>
        [FormField("note", Group = "basic", Text = "站点备注", Length = "[0,5]",Descript="如：中文")]
        public string Note { get; set; }

        /// <summary>
        /// 目录名称
        /// </summary>
        [FormField("dirname", Group = "basic", Text = "目录名称", Descript = "如果如顶级域名绑定，则填写目录名称,以\"http://abc.com/目录名称/\"访问网站", Regex = "[A-Za-z0-9_]{0,10}")]
        public string DirName { get; set; }

        /// <summary>
        /// 域名绑定
        /// </summary>
        [FormField("domain", Group = "basic", Text = "域名绑定", Descript = "选填,可绑定(解析到本站的)域名.如：www.abc.com;多个域名请用空格隔开。")]
        public string Domain { get; set; }

        /// <summary>
        /// 重定向地址
        /// </summary>
        [FormField("location", Group = "basic", Text = "重定向", Descript = "选填,可将首页重定向至指定位置,如：index.aspx将定位到www.xxx.com/index.aspx")]
        public string Location { get; set; }


        public string FullDomain { get; set; }

        /// <summary>
        /// 站点使用语言
        /// </summary>
        [FormField("language", Group = "basic", Text = "<span class=\"red\">*</span>国际化/语言")]
        [SelectField(UseDrop = true, Data = "中文简体(Chinese Simplified)=1;中文繁体(Chinese Traditional)=4;英语(Unit States)=2;西班牙语(Español)=3;泰语(ภาษาไทย)=5")]
        public Languages Language { get; set; }


        /// <summary>
        /// 模板
        /// </summary>
        [FormField("tpl", Group = "basic", Text = "<span class=\"red\">*</span>页面模板", DisableEdit = true)]
        public string Tpl { get; set; }

        /// <summary>
        /// 站点状态
        /// </summary>
        [FormField("State", Group = "basic", Text = "<span class=\"red\">*</span>站点状态")]
        [SelectField(UseDrop = true, Data = "正常(Normal)=1;暂停访问(Paused)=2;关闭(Stopped)=3")]
        public SiteState State { get; set; }

        /// <summary>
        /// SEO标题
        /// </summary>
        [FormField("SeoTitle", Group = "basic", Text = "SEO标题", MultLine = true, Descript = "选填,不超过100字", Length = "[0,100]")]
        public string SeoTitle { get; set; }

        /// <summary>
        /// SEO关键字
        /// </summary>
        [FormField("SeoKeywords", Group = "basic", Text = "SEO关键字", MultLine = true, Descript = "选填,不超过150字", Length = "[0,150]")]
        public string SeoKeywords { get; set; }

        /// <summary>
        /// SEO描述
        /// </summary>
        [FormField("SeoDescription", Group = "basic", Text = "SEO描述", MultLine = true, Length = "[0,150]")]
        public string SeoDescription { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [FormField("tel", Group = "Profile", Text = "联系电话", Length = "[0,50]")]
        public string Tel { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [FormField("phone", Group = "Profile", Text = "手机号码", Length = "[0,11]")]
        public string Phone { get; set; }

        /// <summary>
        /// 传真号码
        /// </summary>
        [FormField("fax", Group = "Profile", Text = "传真号码", Length = "[0,50]")]
        public string Fax { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        [FormField("address", Group = "Profile", Text = "联系地址", Length = "[0,100]")]
        public string Address { get; set; }

        /// <summary>
        /// 邮编号码
        /// </summary>
        [FormField("postcode", Group = "Profile",Text = "邮编号码")]
        public string PostCode { get; set; }


        /// <summary>
        /// 电子邮箱
        /// </summary>
        [FormField("email", Group = "Profile", Text = "电子邮箱", Length = "[0,100]")]
        public string Email { get; set; }

        /// <summary>
        /// Im (qq/msn)
        /// </summary>
        [FormField("im", Group = "Profile", Text = "即时通讯", Descript="可以填写QQ、微信、MSN、淘宝等聊天工具账号", Length = "[0,50]")]
        public string Im{get;set;}

        /// <summary>
        /// 网站公告
        /// </summary>
        [FormField("notice", Group = "Profile", Text = "网站公告", MultLine = true, Length = "[0,200]")]
        public string Notice { get; set; }

        /// <summary>
        /// 网站标语
        /// </summary>
        [FormField("slogan", Group = "Profile", Text = "网站标语", MultLine = true, Length = "[0,200]")]
        public string Slogan { get; set; }

        /// <summary>
        /// 运行类型
        /// </summary>
        public int RunType { get; set; }

    }
}
