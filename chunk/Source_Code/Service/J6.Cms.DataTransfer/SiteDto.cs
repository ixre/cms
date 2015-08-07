
/*
* Copyright(C) 2010-2013 K3F.NET
* 
* File Name	: Site.cs
* publisher_id	: Newmin (new.min@msn.com)
* Create	: 2013/05/21 19:59:54
* Description	:
*
*/

using J6.Cms.Domain.Interface.Common.Language;
using J6.Cms.Domain.Interface.Site;
using J6.DevFw.Framework.Automation;

namespace J6.Cms.DataTransfer
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
                ProAddress = site.ProAddress,
                DirName = site.DirName,
                Domain = site.Domain,
                FullDomain =  site.FullDomain,
                Location = site.Location,
                ProEmail = site.ProEmail,
                ProFax = site.ProFax,
                Language = site.Language,
                ProPost = site.ProPost,
                Name = site.Name,
                Note = site.Note,
                ProNotice = site.ProNotice,
                ProPhone = site.ProPhone,
                ProIm = site.ProIm,
                RunType = (int)site.RunType,
                SeoDescription = site.SeoDescription,
                SeoKeywords = site.SeoKeywords,
                SeoTitle = site.SeoTitle,
                SiteId = site.Id,
                ProSlogan = site.ProSlogan,
                State = site.State,
                ProTel = site.ProTel,
                Tpl = site.Tpl
            };
        }


        /// <summary>
        /// 站点ID
        /// </summary>
        [FormField("SiteId",Group="basic",Text="站点Id",DisableEdit=true)]
        public int SiteId { get; set; }

        /// <summary>
        /// 站点名称
        /// </summary>
        [FormField("Name", Group = "basic", Text = "站点名称", IsRequired = true, Length = "[0,30]")]
        public string Name { get; set; }

        /// <summary>
        /// 站点备注
        /// </summary>
        [FormField("Note", Group = "basic", Text = "站点备注", Length = "[0,5]",Descript="如：中文")]
        public string Note { get; set; }

        /// <summary>
        /// 目录名称
        /// </summary>
        [FormField("DirName", Group = "basic", Text = "目录名称", Descript = "如果如顶级域名绑定，则填写目录名称,以\"http://abc.com/目录名称/\"访问网站", Regex = "[A-Za-z0-9_]{0,10}")]
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
        [FormField("language", Group = "basic", Text = "国际化/语言",IsRequired = true)]
        [SelectField(UseDrop = true, Data = "中文简体(Chinese Simplified)=1;中文繁体(Chinese Traditional)=4;英语(Unit States)=2;西班牙语(Español)=3;泰语(ภาษาไทย)=5")]
        public Languages Language { get; set; }


        /// <summary>
        /// 模板
        /// </summary>
        [FormField("tpl", Group = "basic", Text = "页面模板", DisableEdit = true,IsRequired = true)]
        public string Tpl { get; set; }

        /// <summary>
        /// 站点状态
        /// </summary>
        [FormField("State", Group = "basic", Text = "站点状态",IsRequired = true)]
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
        [FormField("ProTel", Group = "Profile", Text = "联系电话", Length = "[0,50]")]
        public string ProTel { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [FormField("ProPhone", Group = "Profile", Text = "手机号码", Length = "[0,11]")]
        public string ProPhone { get; set; }

        /// <summary>
        /// 传真号码
        /// </summary>
        [FormField("ProFax", Group = "Profile", Text = "传真号码", Length = "[0,50]")]
        public string ProFax { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        [FormField("ProAddress", Group = "Profile", Text = "联系地址", Length = "[0,100]")]
        public string ProAddress { get; set; }

        /// <summary>
        /// 邮编号码
        /// </summary>
        [FormField("ProPost", Group = "Profile",Text = "邮编号码")]
        public string ProPost { get; set; }


        /// <summary>
        /// 电子邮箱
        /// </summary>
        [FormField("ProEmail", Group = "Profile", Text = "电子邮箱", Length = "[0,100]")]
        public string ProEmail { get; set; }

        /// <summary>
        /// Im (qq/msn)
        /// </summary>
        [FormField("ProIm", Group = "Profile", Text = "即时通讯", Descript="可以填写QQ、微信、MSN、淘宝等聊天工具账号", Length = "[0,50]")]
        public string ProIm{get;set;}

        /// <summary>
        /// 网站公告
        /// </summary>
        [FormField("ProNotice", Group = "Profile", Text = "网站公告", MultLine = true, Length = "[0,200]")]
        public string ProNotice { get; set; }

        /// <summary>
        /// 网站标语
        /// </summary>
        [FormField("ProSlogan", Group = "Profile", Text = "网站标语", MultLine = true, Length = "[0,200]")]
        public string ProSlogan { get; set; }

        /// <summary>
        /// 运行类型
        /// </summary>
        public int RunType { get; set; }
    }
}
