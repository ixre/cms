

using System.Diagnostics;

namespace J6.Cms.Template
{
    using J6.Cms;
    using DataTransfer;
    using J6.DevFw.Template;
    using J6.DevFw.Web;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using J6.Cms.BLL;
    using J6.Cms.Cache.CacheCompoment;
    using J6.Cms.CacheService;
    using J6.Cms.Conf;
    using J6.Cms.Domain.Interface;
    using J6.Cms.Domain.Interface.Common.Language;
    using J6.Cms.Domain.Interface.Content.Archive;
    using J6.Cms.Domain.Interface.Enum;
    using J6.Cms.Domain.Interface.Models;
    using J6.Cms.Domain.Interface.Site.Category;
    using J6.Cms.Domain.Interface.Site.Extend;
    using J6.Cms.Domain.Interface.Site.Link;
    using J6.Cms.Utility;
    using J6.DevFw;
    using J6.DevFw.Framework;
    using J6.DevFw.Framework.Extensions;
    using J6.DevFw.Framework.Web.UI;
    using J6.DevFw.Framework.Xml.AutoObject;

    public abstract class CmsTemplateCore : IDisposable
    {
        private static PropertyInfo[] archivePros = typeof(ArchiveDto).GetProperties(BindingFlags.Instance | BindingFlags.Public); //文档的属性
        protected static MicroTemplateEngine TplEngine =
                            new MicroTemplateEngine(null); //模板引擎

        protected ArchiveDto archive;           //当前读取的文档,使用require(archiveID)获取的文档
        private SettingFile _settingsFile;      //设置文件，用于保存当前实例的状态
        // private LangLabelReader langReader;     //语言字典读取器
        protected string _resourceUri;           //资源域名

        /// <summary>
        /// 当前站点
        /// </summary>
        protected SiteDto _site;

        /// <summary>
        /// 当前站点编号
        /// </summary>
        protected int SiteId;

        /// <summary>
        /// 模板设置
        /// </summary>
        protected TemplateSetting TplSetting;

        public CmsTemplateCore()
        {
            this._site = Cms.Context.CurrentSite;
            this.SiteId = this._site.SiteId;

            //缓存=》模板设置
            string settingCacheKey = String.Format("{0}_{1}_settings", CacheSign.Template.ToString(), this._site.Tpl);
            object settings = Cms.Cache.Get(settingCacheKey);
            if (settings == null)
            {
                this.TplSetting = new TemplateSetting(this._site.Tpl);
                Cms.Cache.Insert(settingCacheKey, this.TplSetting, String.Format("{0}templates/{1}/tpl.conf", Cms.PyhicPath, this._site.Tpl));
            }
            else
            {
                this.TplSetting = settings as TemplateSetting;
            }
        }


        /************************ 辅助方法 ********************************/

        #region

        /// <summary>
        /// 格式化地址
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual string FormatPageUrl(UrlRulePageKeys key, params string[] data)
        {
            string url = TemplateUrlRule.Urls[TemplateUrlRule.RuleIndex, (int)key];
            if (data != null)
            {
                url = String.Format(url, data);
            }
            return this.ConcatUrl(url);
        }

        /// <summary>
        /// 连接URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual string ConcatUrl(string url)
        {
            if (url.IndexOf("//", StringComparison.Ordinal) != -1
                || url.IndexOf("javascript:", StringComparison.Ordinal) != -1)
            {
                return url;
            }

            if (!String.IsNullOrEmpty(url) && url[0] != '/')
            {
                url = String.Concat("/", url);
            }

            if (Settings.TPL_UseFullPath)
            {
                return String.Concat(Cms.Context.SiteDomain, url);
            }

            return url;
        }


        /// <summary>
        /// 获取文档的地址
        /// </summary>
        /// <param name="location"></param>
        /// <param name="flags"></param>
        /// <param name="category"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual string GetArchiveUrl(string location, string flags, CategoryDto category, string id)
        {
            if (!string.IsNullOrEmpty(location)) return this.ConcatUrl(location);
            if (String.IsNullOrEmpty(flags) || (flags.IndexOf("p:'1'", StringComparison.Ordinal) == -1))
            {
                return this.FormatPageUrl(UrlRulePageKeys.Archive, category.UriPath, id);
            }
            return this.FormatPageUrl(UrlRulePageKeys.SinglePage, id);
        }

        private string GetLocationUrl(string location)
        {
            //如果定义了跳转地址
            // if (Regex.IsMatch(location, "^http(s?)://", RegexOptions.IgnoreCase))
            if (location.IndexOf("//", StringComparison.Ordinal) != -1)
            {
                return location;
            }
            return this.ConcatUrl(location);
        }

        /// <summary>
        /// 获取栏目的地址
        /// </summary>
        /// <param name="category"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public virtual string GetCategoryUrl(CategoryDto category, int pageIndex)
        {
            if (!String.IsNullOrEmpty(category.Location)) return ConcatUrl(category.Location);
            if (pageIndex < 2)
            {
                return this.FormatPageUrl(UrlRulePageKeys.Category, category.UriPath);
            }
            else
            {
                return this.FormatPageUrl(UrlRulePageKeys.CategoryPager, category.UriPath, pageIndex.ToString());
            }
        }

        #region 分页


        /// <summary>
        /// 设置分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <param name="recordCount"></param>
        /// <param name="firstLink"></param>
        /// <param name="link"></param>
        public virtual void SetPager(int pageIndex, int pageCount, int recordCount, string firstLink, string link)
        {

            //string pagerPack = "";
            //switch (this.site.Language)
            //{
            //    default:
            //    case Languages.En_US:
            //        pagerPack = en_us_pack; break;
            //    case Languages.Zh_CN:
            //        pagerPack = zh_cn_pack; break;
            //    case Languages.Zh_TW:
            //        pagerPack = zh_tw_pack; break;
            //}

            //string[] pagerLangs = pagerPack.Split('|');

            // const string pagerTpl = "<div class=\"pager\">{0}</div>";

            IPagingGetter getter = new CustomPagingGetter(
                firstLink,
                link,
                 Cms.Language.Get(LanguagePackageKey.PAGER_PrePageText),
                 Cms.Language.Get(LanguagePackageKey.PAGER_NextPageText),
                 Cms.Language.Get(LanguagePackageKey.PAGER_PrePageText),
                 Cms.Language.Get(LanguagePackageKey.PAGER_NextPageText)
                );

            UrlPager p = UrlPaging.NewPager(pageIndex, pageCount, getter);
            p.LinkCount = 10;
            p.PagerTotal = "<span class=\"pagerinfo\">" + Cms.Language.Get(LanguagePackageKey.PAGER_PagerTotal) + "</span>";
            p.RecordCount = recordCount;
            /* StringBuilder sb = new StringBuilder();
           
           sb.Append("<div class=\"pager\">");

           
           p.RecordCount = recordCount;
           p.FirstPageLink = firstLink;
           p.LinkFormat = link;

           p.PreviousPageText = j6.Language.Get(LanguagePackageKey.PAGER_PrePageText);
           p.NextPageText = j6.Language.Get(LanguagePackageKey.PAGER_NextPageText);
           p.PageTextFormat = "{0}";
           p.SelectPageText = j6.Language.Get(LanguagePackageKey.PAGER_SelectPageText);
           p.Style = PagerStyle.Custom;
           p.EnableSelect = true;
           p.PagerTotal = j6.Language.Get(LanguagePackageKey.PAGER_PagerTotal);
           p.LinkCount = 10;*/

            string key = this.PushPagerKey();
            Cms.Context.Items[key] = p.Pager(); // String.Format(pagerTpl, p.ToString());
        }

        protected string PopPagerKey()
        {
            int pagerNumber = 0;
            object pagerNum = Cms.Context.Items["pagerNumber"];
            if (pagerNum == null)
            {
                pagerNumber = 0;
            }
            else
            {
                int.TryParse(pagerNum.ToString(), out pagerNumber);
                --pagerNumber;
            }
            Cms.Context.Items["pagerNumber"] = pagerNumber;
            return String.Format("pager_{0}", (pagerNumber + 1).ToString());
        }

        protected string PushPagerKey()
        {
            int pagerNumber = 0;
            object pagerNum = Cms.Context.Items["pagerNumber"];
            if (pagerNum == null)
            {
                pagerNumber = 1;
            }
            else
            {
                int.TryParse(pagerNum.ToString(), out pagerNumber);
                ++pagerNumber;
            }
            Cms.Context.Items["pagerNumber"] = pagerNumber;
            return String.Format("pager_{0}", pagerNumber.ToString());
        }

        #endregion

        /// <summary>
        /// 获取绑定的链接地址
        /// </summary>
        /// <param name="bindStr"></param>
        /// <returns></returns>
        protected string GetBingLinkUrl(string bindStr)
        {
            string[] binds = (bindStr ?? "").Split(':');
            if (binds.Length == 2 && binds[1] != String.Empty)
            {
                if (binds[0] == "category")
                {
                    CategoryDto category = ServiceCall.Instance.SiteService.GetCategory(this._site.SiteId, int.Parse(binds[1]));
                    if (category.Id > 0)
                    {
                        return this.GetCategoryUrl(category, 1);
                    }
                }
                else if (binds[0] == "archive")
                {
                    int archiveId;
                    int.TryParse(binds[1], out archiveId);

                    ArchiveDto archiveDto = ServiceCall.Instance.ArchiveService
                        .GetArchiveById(this.SiteId, archiveId);

                    if (archiveDto.Id > 0)
                    {
                        return this.GetArchiveUrl(
                            archiveDto.Location,
                            archiveDto.Flags,
                            archiveDto.Category,
                            String.IsNullOrEmpty(archiveDto.Alias)
                                ? archiveDto.StrId
                                : archiveDto.Alias);
                    }
                }
            }

            return "javascript:void(0,'no-such-link')";

        }


        /// <summary>
        /// 返回是否为True
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected bool IsTrue(string str)
        {
            return str == "1" || String.Compare(str, "true", StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// 获取缩略图地址
        /// </summary>
        /// <param name="thumbnail"></param>
        /// <returns></returns>
        protected string GetThumbnailUrl(string thumbnail)
        {
            if (String.IsNullOrEmpty(thumbnail))
            {
                return String.Concat("/", CmsVariables.FRAMEWORK_ARCHIVE_NoPhoto);
            }

            if (!Settings.TPL_UseFullPath           //不使用完整路径
                || thumbnail.IndexOf("://", StringComparison.Ordinal) != -1)    //如果是包含协议的地址
            {
                return thumbnail;
            }
            else
            {
                //获取包含完整URL的图片地址
                if (this._resourceUri == null)
                {
                    this._resourceUri = WebCtx.Current.Domain;
                }
                return String.Concat(this._resourceUri, thumbnail);
            }

        }

        /// <summary>
        /// 模板提示
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected string TplMessage(string msg)
        {
            if (this.TplSetting.CFG_ShowError)
            {
                return String.Format("提示：{0}", msg);
            }
            return String.Empty;
        }

        #endregion


        /************************ 模板方法 *******************************/

        #region  模板方法

        [TemplateMethod]
        protected string EachCategory(string param, string dataNum, string refTag, string refName, string refUri, string format)
        {
            //
            // @param : 如果为int,则返回模块下的栏目，
            //                 如果为字符串tag，则返回该子类下的栏目
            //

            int siteId = this._site.SiteId;

            int num = 0;
            if (Regex.IsMatch(dataNum, "^\\d+$"))
            {
                int.TryParse(dataNum, out num);
            }


            #region 取得栏目
            IEnumerable<CategoryDto> categories1;
            if (param == String.Empty)
            {
                categories1 = ServiceCall.Instance.SiteService.GetCategories(siteId);
            }
            else
            {
                if (Regex.IsMatch(param, "^\\d+$"))
                {
                    int moduleId = int.Parse(param);
                    categories1 = ServiceCall.Instance.SiteService.GetCategories(siteId).Where(a => a.ModuleId == moduleId);
                }
                else
                {
                    CategoryDto category = ServiceCall.Instance.SiteService.GetCategory(this.SiteId, param);
                    if (category.Id > 0)
                    {
                        categories1 = ServiceCall.Instance.SiteService.GetCategories(this.SiteId, category.Lft, category.Rgt, CategoryContainerOption.NextLevel);
                    }
                    else
                    {
                        categories1 = null;
                    }
                }
            }
            #endregion

            if (categories1 == null) return String.Empty;
            else
            {
                IList<CategoryDto> categories = new List<CategoryDto>(categories1.OrderBy(a => a.SortNumber));
                StringBuilder sb = new StringBuilder(400);
                int i = 0;

                foreach (CategoryDto c in categories)
                {
                    if (num != 0 && ++i >= num)
                    {
                        break;
                    }
                    if (c.SiteId == this.SiteId)
                    {
                        sb.Append(TplEngine.FieldTemplate(format, field =>
                        {
                            switch (field)
                            {
                                default:
                                    if (field == refName) return c.Name;
                                    if (field == refTag) return c.Tag;
                                    if (field == refUri) return this.GetCategoryUrl(c, 1);
                                    return "{" + field + "}";

                                case "description": return c.Description;
                                case "keywords": return c.Keywords;
                                case "index": return i.ToString();
                                case "class":
                                    if (i == categories.Count - 1) return "last";
                                    else if (i == 0) return "first";
                                    return string.Empty;
                            }
                        }));
                    }
                }
                return sb.ToString();
            }
        }

        [TemplateMethod]
        protected string EachCategory(string dataNum, string refTag, string refName, string refUri, string format)
        {
            string id = HttpContext.Current.Items["category.tag"] as string;
            if (String.IsNullOrEmpty(id))
            {
                return this.TplMessage("Error: 此标签不允许在当前页面中调用!");
            }
            return EachCategory(id, dataNum, refTag, refName, refUri, format);
        }

        [TemplateMethod]
        [Obsolete]
        protected string EachCategory2(string dataNum, string refTag, string refName, string refUri, string format)
        {
            object id = Cms.Context.Items["module.id"];
            if (id == null)
            {
                return this.TplMessage("此标签不允许在当前页面中调用!");
            }
            return EachCategory(id.ToString(), dataNum, refTag, refName, refUri, format);
        }


        #endregion

        /************************ 基本标签 ********************************/


        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Request(string id)
        {
            return HttpContext.Current.Request[id] ?? String.Empty;
        }

        /// <summary>
        /// 获取项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Item(string id)
        {
            return (Cms.Context.Items[id] ?? String.Empty).ToString();
        }

        /// <summary>
        /// 字典标签
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [TemplateTag]
        public string Label(string key)
        {
            string cacheKey = String.Format("{0}_label_{1}", CacheSign.Site.ToString(), this._site.SiteId.ToString());
            if (this._settingsFile == null)
            {
                //读取数据
                this._settingsFile = Cms.Cache.Get(cacheKey) as SettingFile;
                if (this._settingsFile == null)
                {
                    string phyPath = String.Format("{0}templates/{1}/label.conf", Cms.PyhicPath, this._site.Tpl);
                    this._settingsFile = new SettingFile(phyPath);

                    //缓存数据
                    Cms.Cache.Insert(cacheKey, this._settingsFile, phyPath);
                }
            }
            return this._settingsFile[key];
        }

        /// <summary>
        /// 语言标签
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取字典数据", @"")]
        public string Lang(string key)
        {
            return Cms.Language.Get(this._site.Language, key);
        }

        /// <summary>
        /// 获取网站的资料
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Site(string key)
        {
            switch (key.ToLower())
            {
                case "name":
                    return this._site.Name;
                case "tel":
                    return this._site.ProTel ?? "";
                case "phone":
                    return this._site.ProPhone ?? "";
                case "fax":
                    return this._site.ProFax ?? "";
                case "address":
                    return this._site.ProAddress ?? "";
                case "email":
                    return this._site.ProEmail ?? "";
                case "im":
                case "qq": //todo:需删除
                    return this._site.ProIm ?? "";
                case "post":
                    return this._site.ProPost ?? "";
                case "notice":
                    return this._site.ProNotice ?? "";
                case "slogan":
                    return this._site.ProSlogan ?? "";
                case "tpl":
                    return this._site.Tpl ?? "default";
            }
            return key;
        }

        /// <summary>
        /// 重定向文档
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Archive_Redirect(string id)
        {
            ArchiveDto a = ServiceCall.Instance.ArchiveService.GetArchiveByIdOrAlias(this._site.SiteId, id);

            if (a.Id > 0)
            {
                HttpResponse response = HttpContext.Current.Response;
                string appPath = Cms.Context.SiteAppPath;
                if (appPath != "/") appPath += "/";


                response.StatusCode = 301;
                response.RedirectLocation = String.Format("{0}{1}/{2}.html",
                    appPath,
                    a.Category.UriPath,
                    String.IsNullOrEmpty(a.Alias) ? a.StrId : a.Alias
                    );
                response.End();
            }
            return "";
        }


        /// <summary>
        /// 请求读取文档
        /// </summary>
        /// <param name="idOrAlias"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Require(string idOrAlias)
        {
            ArchiveDto a = ServiceCall.Instance.ArchiveService.GetArchiveByIdOrAlias(this.SiteId, idOrAlias);
            if (a.Id > 0) archive = a;
            return string.Empty;
        }

        /// <summary>
        /// 绑定字段
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Bind(string field)
        {
            if (archive.Id <= 0) return this.TplMessage("请先使用标签$require('id')获取文档后再调用属性");

            PropertyInfo p = Array.Find(archivePros, a => String.Compare(a.Name, field, StringComparison.OrdinalIgnoreCase) == 0);
            if (p != null)
            {
                return (p.GetValue(archive, null) ?? "").ToString();
            }

            //
            //TODO:返回扩展字段
            //
            return "";
        }


        /// <summary>
        /// 文档评论
        /// </summary>
        /// <param name="allowAmous"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Comment_Editor(string allowAmous, string html)
        {

            ArchiveDto archive = (ArchiveDto)(Cms.Context.Items["archive"] ?? default(ArchiveDto));

            const string cmEditorTpl = @"<!-- 提交评论 -->
            <div class=""comment_editor"">
                <form method=""POST"" action="""" style=""margin:0"" id=""cmeditor"">
                     <input name=""action"" value=""comment"" type=""hidden"" />
                     <input name=""ce_id"" value=""{id}"" type=""hidden"" />
                     {html}
                     <div>{nickname}</div>
                     <div>验证码：<input class=""ui-validate"" name=""ce_verifycode"" style=""width:60px"" type=""text"" id=""ce_verifycode""><img class=""verifyimg"" src=""{img}"" onclick=""this.src='{img}&rnd='+Math.random();"" alt=""验证码。看不清楚？换一个"" /></div>
                     <div>
                              <textarea id=""ce_content"" name=""ce_content"" class=""ui-validate""  rows=""3"" onfocus=""if(this.value=='请在这里输入评论内容')this.value='';"" onblur=""if(this.value=='')this.value='请在这里输入评论内容';"">请在这里输入评论内容</textarea>
                    </div>
                    <div><a href=""javascript:submitComment();"" id=""ce_submit"" class=""ce_submit"" title=""点击发表评论"">发表评论</a><span id=""ce_tip""></span></div>
                </form>
            </div>
                <script type=""text/javascript"">
                    var ce_n=j6.$('ce_nickname'),
                          ce_v=j6.$('ce_verifycode'),
                          ce_vl=ce_v.nextSibling,
                          ce_c=j6.$('ce_content'),
                          ce_t=j6.$('ce_tip'),
                          ce_s=j6.$('ce_submit');

                        j6.lazyRun(function(){
                            if(!j6.form)j6.ld('form');
                            if(!j6.validator)j6.ld('validate')
                        });

                        j6.lazyRun(function(){
                            if(ce_n){
                                ce_n.value = j6.cookie.read('viewname');
                                ce_n.onblur=function(){
                                     if(ce_n.value=='' || ce_n.value.length>10){
                                          j6.validator.setTip(ce_n,false,'','昵称长度为1-10个字符!');
                                     }else{
                                          j6.validator.removeTip(ce_n);
                                          j6.cookie.write('viewname', this.value, 60 * 60 * 24 * 3);
                                     }
                                  };
                            }
                            ce_c.onblur=function(){
                                     if(ce_c.value==''){
                                          j6.validator.setTip(ce_c,false,'','请输入评论内容!');
                                     }else if(ce_c.value.length>200){
                                          j6.validator.setTip(ce_c,false,'','评论内容必须少于200字!');
                                     }
                                     else{
                                          j6.validator.removeTip(ce_c);
                                     }
                              };
                            ce_v.onblur=function(){
                                     if(ce_v.value==''){
                                          j6.validator.setTip(ce_vl,false,'','请输入验证码!');
                                     }
                                     else{
                                          j6.validator.removeTip(ce_vl);
                                     }
                              };

                   });
                   function submitComment(){
                        j6.lazyRun(function(){
                            if(j6.validator.validate('cmeditor')){
                                 cetip(true,'提交中...');
                                 j6.form.asyncSubmit('cmeditor');
                            }
                        });
                   }
                  function cetip(b,m){ce_t.innerHTML=(b?'<span style=\'color:green\'':'<span style=\'color:red\'')+'>'+m+'</span>';if(!b){ce_s.removeAttribute('disabled','');}else{ ce_s.setAttribute('disabled','disabled');}}
                   function clientCall(script){if(script)eval(script);}

                </script>";

            bool amouSubmit = this.IsTrue(allowAmous);

            if (archive.Id <= 0) return this.TplMessage("请先使用标签$require('id')获取文档后再调用属性");



            StringBuilder sb = new StringBuilder();

            string content = TplEngine.FieldTemplate(cmEditorTpl, a =>
            {
                switch (a)
                {
                    case "id":
                        return archive.StrId;
                    case "html":
                        return html;
                    case "img":
                        return this.FormatPageUrl(UrlRulePageKeys.Common, "Cms_Core/verifyimg?length=4&opt=1");

                    case "nickname":
                        Member member = UserState.Member.Current;
                        if (member == null)
                        {
                            if (amouSubmit)
                            {
                                return @"昵称：<input class=""ui-validate"" name=""ce_nickname"" type=""text"" id=""ce_nickname"">";
                            }
                            else
                            {
                                return @"昵称：<span style=""color:red"">不允许匿名评论，请先登录后继续操作！</span>";
                            }
                        }
                        else
                        {
                            return String.Format("昵称：{0}", member.Nickname);
                        }
                }
                return String.Empty;
            });

            if (Settings.TPL_UseCompress)
            {
                return PageUtility.CompressHtml(content);
            }
            else
            {
                return content;
            }
        }

        /// <summary>
        /// 文档评论
        /// </summary>
        /// <param name="format"></param>
        /// <param name="usePager"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Comment(string format, string asc, string usePager)
        {
            int replayCount;

            ArchiveDto archive = Cms.Context.Items["archive"] == null ? default(ArchiveDto)
                : (ArchiveDto)Cms.Context.Items["archive"];
            if (archive.Id <= 0)
                throw new ArgumentNullException("archive", "不允许在当前页中使用!");

            StringBuilder sb = new StringBuilder();

            //获取评论列表
            DataTable dt = CmsLogic.Comment.GetArchiveComments(archive.StrId, !this.IsTrue(asc));

            replayCount = dt.Rows.Count;
            int memberId;
            string nickname, content;
            Match match;
            Member member = null;

            //如无评论，则提示
            if (replayCount == 0)
            {
                return "<p style=\"text-align:center;\" class=\"noreplay\">暂无评论</p>";
            }

            //迭代获取评论信息
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                content = dt.Rows[i]["content"].ToString();
                memberId = int.Parse(dt.Rows[i]["memberid"].ToString());

                if (memberId > 0)
                {
                    //会员
                    member = CmsLogic.Member.GetMember(memberId);
                    if (member == null)
                    {
                        nickname = "已删除会员";
                    }
                    else
                    {
                        nickname = member.Nickname ?? member.Username;
                    }
                }
                else
                {
                    //游客
                    match = Regex.Match(content, "\\(u:'(?<user>.+?)'\\)");

                    if (match != null)
                    {
                        nickname = match.Groups["user"].Value;
                        content = Regex.Replace(content, "\\(u:'(.+?)'\\)", String.Empty);
                    }
                    else
                    {
                        nickname = "游客";
                    }

                }

                sb.Append(TplEngine.FieldTemplate(format, field =>
                {
                    switch (field)
                    {
                        default: return String.Empty;

                        //会员昵称
                        case "nickname": return nickname;


                        //
                        //UNDONE:未对会员设置自定义链接
                        //

                        //会员链接
                        case "url":
                            return memberId == 0 ? "javascript:;" :
                                // (Settings.TPL_UseFullPath ? Settings.SYS_DOMAIN : String.Empty)+
                                 String.Format("/member/{1}", memberId.ToString());

                        //会员编号
                        case "mid":
                        case "memberid": return memberId.ToString();

                        //评论编号
                        case "id": return dt.Rows[i]["id"].ToString();

                        //索引，从1开始
                        case "index": return (i + 1).ToString();

                        //样式
                        case "class":
                            if ((i + 1) % 2 == 0)
                            {
                                return replayCount == i + 1 ? "even last" : "even";
                            }
                            else
                            {
                                return i == 0 ? "first" : "";
                            }

                        //评论时间
                        case "date":
                        case "createdate":
                            return String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt.Rows[i]["createDate"]);

                        //评论内容
                        case "content":
                            return content;

                        //会员头像
                        case "avatar":
                            return member == null ? "/uploads/avatar/nopic.gif" : member.Avatar;
                    }
                }));
            }

            if (Settings.TPL_UseCompress)
            {
                return PageUtility.CompressHtml(sb.ToString());
            }
            else
            {
                return sb.ToString();
            }

        }

        /// <summary>
        /// 表单
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Form(string formId)
        {
            //table name :cms_form_tableid
            //column name:cms_form_tableid_columnid
            //button name:cms_form_tableid_btn

            StringBuilder sb = new StringBuilder();
            Table tb = CmsLogic.Table.GetTable(int.Parse(formId));
            if (tb == null)
            {
                return "Form not exists!";
            }

            string tableId = tb.Id.ToString();
            string token = String.Empty.RandomLetters(16);    //验证token

            //存入SESSION
            global::System.Web.HttpContext.Current.Session[String.Format("cms_form_{0}_token", tb.Id.ToString())] = token;

            //构造HTML
            if (!String.IsNullOrEmpty(tb.Note))
            {
                sb.Append("<div class=\"fnote\">").Append(tb.Note).Append("</div>");
            }
            sb.Append("<form class=\"customform\" id=\"cms_form_").Append(tb.Id.ToString()).Append("\">");

            IList<TableColumn> columns = CmsLogic.Table.GetColumns(tb.Id);
            if (columns != null)
            {
                foreach (TableColumn c in columns)
                {
                    sb.Append("<div class=\"item\"><div class=\"fn\" style=\"float:left\">").Append(c.Name).Append(":</div>")
                        .Append("<div class=\"fi\"><input type=\"text\" class=\"ui-validate\" field=\"field_")
                        .Append(tableId).Append("_").Append(c.Id.ToString())
                        .Append("\" id=\"field_").Append(tableId).Append("_").Append(c.Id.ToString()).Append("\"");

                    if (!String.IsNullOrEmpty(c.ValidFormat))
                    {
                        sb.Append(" ").Append(c.ValidFormat);
                    }
                    sb.Append("/></div>");

                    if (!String.IsNullOrEmpty(c.Note))
                    {
                        sb.Append("<span class=\"fnote\">").Append(c.Note)
                        .Append("</span>");
                    }

                    sb.Append("</div>");
                }
            }

            sb.Append("<div class=\"fbtn\"><input type=\"button\" id=\"cms_form_")
                .Append(tableId).Append("_btn\" value=\"提交\"/><span id=\"cms_form_")
                .Append(tableId).Append("_summary").Append("\"></span></div></form><script type=\"text/javascript\">")
                .Append("if(!window.cms){alert('缺少cms核心脚本库!');}else{")
                .Append("j6.lazyRun(function(){ if(!j6.form)j6.ld('form');if(!j6.validator)j6.ld('validate'); });")
                .Append("document.getElementById('cms_form_").Append(tb.Id.ToString()).Append("_btn').onclick=function(){")
                .Append("j6.lazyRun(function(){")
                .Append("var cfs=j6.$('cms_form_").Append(tableId).Append("_summary');")
                .Append("if(j6.validator.validate('cms_form_").Append(tableId).Append("')){cfs.innerHTML='提交中...';j6.xhr.post('")

                .Append(this.FormatPageUrl(UrlRulePageKeys.Common, "Cms_Core/submitform?tableid="))
                .Append(tableId).Append("&token=").Append(token).Append("',j6.json.toObject('cms_form_")
                .Append(tableId).Append("'),function(r){var result;eval('result='+r);cfs.innerHTML=result.tag==-1?'<span style=\"color:red\">'+result.message+'</span>':result.message;},function(){cfs.innerHTML='<span style=\"color:red\">提交失败，请重试!</span>';});")
                .Append("}});};}</script>");

            return sb.ToString();
        }

        /// <summary>
        /// 根据栏目Tag产生站点地图
        /// </summary>
        /// <param name="categoryTag"></param>
        /// <returns></returns>
        [TemplateTag]
        [ContainSetting]
        protected string Sitemap(string categoryTag)
        {
            string cacheKey = String.Format("{0}_site{1}_sitemap_{2}", CacheSign.Category, SiteId.ToString(), categoryTag);
            return Cms.Cache.GetCachedResult(cacheKey, () =>
            {
                return CategoryCacheManager.GetSitemapHtml(this.SiteId,
                    categoryTag,
                    this.TplSetting.CFG_SitemapSplit,
                this.FormatPageUrl(UrlRulePageKeys.Category, null));
            }, DateTime.Now.AddHours(Settings.OptiDefaultCacheHours));
        }

        /// <summary>
        /// 分页控件
        /// </summary>
        /// <returns></returns>
        [TemplateTag]
        protected string Pager()
        {
            return (Cms.Context.Items[this.PopPagerKey()] ?? "") as string;
        }

        /// <summary>
        /// 流量统计
        /// </summary>
        /// <returns></returns>
        [TemplateTag]
        [ContainSetting]
        protected string Traffic()
        {
            string ipaddress = HttpContext.Current.Request.UserHostAddress;
            TrafficCounter.Record(ipaddress);

            string format = this.TplSetting.CFG_TrafficFormat;

            string result = TplEngine.FieldTemplate(format, key =>
            {
                switch (key)
                {
                    case "todayip": return TrafficCounter.GetTodayIPAmount().ToString();
                    case "todaypv": return TrafficCounter.GetTodayPVAmount().ToString();
                    case "totalip": return TrafficCounter.GetTotalIPAmount().ToString();
                    case "totalpv": return TrafficCounter.GetTotalPVAmount().ToString();
                }
                return String.Empty;
            });

            return result;
        }


        /// <summary>
        /// 导航
        /// </summary>
        /// <param name="childFormat"></param>
        /// <param name="index">选中索引</param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Navigator(string format, string childFormat, string index)
        {
            const string tpl = @"<div id=""navigator"" class=""page-navigator""><div class=""left""></div><div class=""right""></div><div class=""navs""><ul>{0}</ul><div class=""clear-fix""></div></div></div>";

            StringBuilder sb = new StringBuilder();
            IList<SiteLinkDto> links = new List<SiteLinkDto>(
                ServiceCall.Instance.SiteService.GetLinksByType(this.SiteId, SiteLinkType.Navigation, false));
            int total = links.Count;
            IList<SiteLinkDto> childs;

            bool isLast;
            int navIndex;
            if (index == "")
            {
                navIndex = -1;           //如果为空，则默认不选中
            }
            else
            {
                int.TryParse(index, out navIndex);
            }

            int j = 0;
            string tempLinkStr;


            LinkGenerateGBehavior bh = (int genTotal, ref int current, int selected, bool child, SiteLinkDto link, int childCount) =>
            {
                StringBuilder sb2 = new StringBuilder();

                /* *********************
                 *  辨别选中的导航
                 * *********************/
                isLast = current == genTotal - 1;
                String clsName = "";

                if (childCount != 0)
                {
                    clsName = "parent ";
                }

                if (selected == current)
                {
                    clsName = String.Concat(clsName, "current ");
                }

                if (isLast)
                {
                    clsName = String.Concat(clsName, "last ");
                }
                else if (current == 0)
                {
                    clsName = String.Concat(clsName, "first "); ;
                }

                sb2.Append(clsName.Length == 0 ? "<li>" : "<li class=\"" + clsName + "\">");

                //解析格式
                tempLinkStr = TplEngine.FieldTemplate(child ? childFormat : format, a =>
                {
                    switch (a)
                    {
                        case "url":return this.ConcatUrl(String.IsNullOrEmpty(link.Bind) ? link.Uri : this.GetBingLinkUrl(link.Bind));
                        case "text": return link.Text;
                        case "imgurl": return link.ImgUrl;
                    }
                    return "{" + a + "}";
                });

                //添加链接目标
                if (!String.IsNullOrEmpty(link.Target))
                {
                    tempLinkStr = tempLinkStr.Replace("<a ", "<a target=\"" + link.Target + "\" ");
                }


                sb2.Append(tempLinkStr).Append("</li>");


                return sb2.ToString();
            };

            string phtml;
            int cTotal;
            int cCurrent;

            for (int i = 0; i < links.Count; i++)
            {
                if (links[i].Pid == 0)
                {
                    j = i;

                    childs = new List<SiteLinkDto>(links.Where(a => a.Pid == links[i].Id));
                    phtml = bh(total, ref j, navIndex, false, links[i], childs.Count);

                    if ((cTotal = childs.Count) != 0)
                    {
                        phtml = phtml.Replace("</li>",
                            String.Format("<div id=\"{0}_child{1}\" class=\"child child{1}\"><div class=\"top\"></div><div class=\"box\">" +
                                          "<ul class=\"menu\">",
                            links[i].Type.ToString().ToLower(),
                            links[i].Id.ToString())
                            );

                        cCurrent = 0;
                        for (int k = 0; k < childs.Count; k++)
                        {
                            phtml += bh(cTotal, ref cCurrent, -1, true, childs[k], 0);
                            cCurrent++;
                            //links.Remove(childs[k]);
                        }
                        phtml += "</ul></div></div></li>";
                    }
                    sb.Append(phtml);
                }
            }
            return String.Format(tpl, sb.ToString());
        }


        /********************** 基本数据 *************************/

        /// <summary>
        /// 文档内容
        /// </summary>
        /// <param name="idOrAlias"></param>
        /// <returns></returns>
        protected string Archive(int _siteId, object idOrAlias, string format)
        {
            ArchiveDto archiveDto;
            if (idOrAlias is int)
            {
                archiveDto = ServiceCall.Instance.ArchiveService.GetArchiveById(_siteId, Convert.ToInt32(idOrAlias));
            }
            else
            {
                archiveDto = ServiceCall.Instance.ArchiveService.GetArchiveByIdOrAlias(_siteId, idOrAlias.ToString());
            }

            if (archiveDto.Id <= 0) return TplMessage(String.Format("不存在编号（或别名）为:{0}的文档!", idOrAlias));


            StringBuilder sb = new StringBuilder(500);
            this.FormatArchive(sb, archiveDto, ref format, null, -1);
            return sb.ToString();
        }


        /// <summary>
        /// 上一篇文章
        /// </summary>
        /// <param name="id"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string PrevArchive(string id, string format)
        {
            ArchiveDto archiveDto = ServiceCall.Instance.ArchiveService.GetSameCategoryPreviousArchive(SiteId, int.Parse(id));
            if (!(archiveDto.Id > 0)) return Cms.Language.Get(LanguagePackageKey.ARCHIVE_NoPrevious);

            StringBuilder sb = new StringBuilder(500);
            this.FormatArchive(sb, archiveDto, ref format, null, -1);
            return sb.ToString();
        }



        /// <summary>
        /// 下一篇文章
        /// </summary>
        /// <param name="id"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string NextArchive(string id, string format)
        {
            ArchiveDto archiveDto = ServiceCall.Instance.ArchiveService.GetSameCategoryNextArchive(this.SiteId, int.Parse(id));

            if (!(archiveDto.Id > 0)) return Cms.Language.Get(LanguagePackageKey.ARCHIVE_NoNext);

            StringBuilder sb = new StringBuilder(500);
            this.FormatArchive(sb, archiveDto, ref format, null, -1);
            return sb.ToString();

        }

        /// <summary>
        /// 栏目链接列表
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        protected string CategoryList(string param, string format)
        {
            IList<CategoryDto> categories = new List<CategoryDto>();

            if (String.IsNullOrEmpty(param))
            {
                return TplMessage("请指定参数:param的值");
            }

            /*
            else if (Regex.IsMatch(param, "^\\d+$"))
            {
                //从模块加载
                int moduleID = int.Parse(param);
                if (CmsLogic.Module.GetModule(moduleID) != null)
                {
                    isModule = true;
                    CmsLogic.Category.HandleCategoryTree(CmsLogic.Category.GetCategories()[0].Name, (c, level) =>
                    {
                        if (level == 0 && c.SiteId == this.siteId)
                        {
                            if (c.ModuleID == moduleID)
                            {
                                categories.Add(c);
                            }
                        }
                    });
                }
            }
            */

            bool isModule = false;
            if (!isModule)
            {
                CategoryDto category = ServiceCall.Instance.SiteService.GetCategory(this.SiteId, param);

                if (!(category.Id > 0) || (category.SiteId != this._site.SiteId && category.Lft != 1))
                {
                    return TplMessage("不存在栏目!标识:" + param);
                }

                categories = new List<CategoryDto>(ServiceCall.Instance.SiteService.GetCategories(this.SiteId,
                    category.Lft, category.Rgt, CategoryContainerOption.NextLevel));

                //如果没有下级了,则获取当前级
                //if (categories.Count == 0)
                //{
                //    if (nullSameLevel) //是否使用同级
                //    {
                //        categories = new List<CategoryDto>(ServiceCall.Instance.SiteService.GetCategories(this.siteId,
                //            category.Lft, category.Rgt, CategoryContainerOption.SameLevel));
                //    }
                //}
            }

            if (categories.Count == 0) return String.Empty;

            StringBuilder sb = new StringBuilder(400);
            int i = 0;

            foreach (CategoryDto c in categories.OrderBy(a => a.SortNumber))
            {
                if (c.SiteId == this._site.SiteId)
                {
                    sb.Append(TplEngine.FieldTemplate(format, field =>
                    {
                        switch (field)
                        {
                            default: return String.Empty;

                            case "name": return c.Name;
                            case "url": return this.GetCategoryUrl(c, 1);
                            case "tag": return c.Tag;
                            case "thumbnail":
                            case "icon": return this.GetThumbnailUrl(c.Icon);
                            case "id": return c.Id.ToString();

                            //case "pid":  return c.PID.ToString();

                            case "description": return c.Description;
                            case "keywords": return c.Keywords;
                            case "index":return  (i+1).ToString();
                            case "class":
                                if (i == categories.Count - 1) return "c last";
                                else if (i == 0) return "c first";
                                return "c";
                        }
                    }));
                }
                ++i;
            }
            return sb.ToString();

        }


        /// <summary>
        /// 根据数据表生成HTML
        /// </summary>
        /// <param name="archives"></param>
        /// <param name="splitSize"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        protected virtual string ArchiveList(ArchiveDto[] archives, int splitSize, string format)
        {
            StringBuilder sb = new StringBuilder();
            bool listContainer = format.EndsWith("</li>");
            int tmpInt = 0;
            int intTotal = archives.Length;
            foreach (ArchiveDto archiveDto in archives)
            {
                this.FormatArchive(sb, archiveDto, ref format, archives, tmpInt++);
                this.AppendSplitHtm(sb, intTotal, tmpInt, splitSize, listContainer);
            }
            return sb.ToString();
        }

        protected void FormatArchive(StringBuilder sb, ArchiveDto archiveDto, ref string format, IEnumerable<ArchiveDto> dt, int index)
        {
            string id = string.IsNullOrEmpty(archiveDto.Alias) ? archiveDto.StrId : archiveDto.Alias;

            //读取自定义扩展数据字段
            IDictionary<string, string> extendFields = null;

            sb.Append(TplEngine.FieldTemplate(format,
                field =>
                {
                    switch (field)
                    {
                        case "id": return archiveDto.Id.ToString();
                        case "str_id": return id;      //用于链接的ID标识
                        case "title": return archiveDto.Title;
                        case "small_title": return archiveDto.SmallTitle;
                        case "special_title": return !ArchiveFlag.GetFlag(archiveDto.Flags, BuiltInArchiveFlags.IsSpecial) ?
                             archiveDto.Title : "<span class=\"special\">" + archiveDto.Title + "</span>";

                        case "publisher_id": return archiveDto.PublisherId.ToString();

                        //
                        //TODO:Archive应持有一个author
                        //
                        //case "authorname": return ArchiveUtility.GetAuthorName(dr["author"].ToString());
                        case "source": return archiveDto.Source == "" ? "原创" : archiveDto.Source;
                        case "fmt_outline": return ArchiveUtility.GetFormatedOutline(
                             archiveDto.Outline,
                             archiveDto.Content,
                             this.TplSetting.CFG_OutlineLength);
                        case "outline": return ArchiveUtility.GetOutline(String.IsNullOrEmpty(archiveDto.Outline) ? archiveDto.Content : archiveDto.Outline, this.TplSetting.CFG_OutlineLength);
                        case "tags": return archiveDto.Tags;
                        case "replay": return CmsLogic.Comment.GetArchiveCommentsCount(archiveDto.StrId).ToString();
                        case "count": return archiveDto.ViewCount.ToString();

                        //时间
                        case "modify_time": return String.Format("{0:yyyy-MM-dd HH:mm}", archiveDto.LastModifyDate);
                        case "modify_date": return String.Format("{0:yyyy-MM-dd}", archiveDto.LastModifyDate);
                        case "modify_sdate": return String.Format("{0:MM-dd}", archiveDto.LastModifyDate);
                        case "create_time": return String.Format("{0:yyyy-MM-dd HH:mm}", archiveDto.CreateDate);
                        case "create_date": return String.Format("{0:yyyy-MM-dd}", archiveDto.CreateDate);

                        //栏目
                        // case "categoryid":
                        // case "cid": return archive.Category.ID.ToString();
                        case "category_name": return archiveDto.Category.Name;
                        case "category_tag": return archiveDto.Category.Tag;
                        case "category_url": return this.GetCategoryUrl(archiveDto.Category, 1);


                        //
                        //TODO:
                        //
                        //链接
                        case "url":
                            return GetArchiveUrl(archiveDto.Location, archiveDto.Flags, archiveDto.Category, id);

                        //内容
                        case "content": return archiveDto.Content;

                        //压缩过的内容
                        case "content2":
                            return Regex.Replace(archiveDto.Content, "\\r|\\t|\\s\\s", String.Empty);

                        //图片元素
                        case "img":
                            string url = this.GetThumbnailUrl(archiveDto.Thumbnail);
                            return String.IsNullOrEmpty(url) ? "" : String.Format("<img class=\"thumb thumbnail\" src=\"{0}\" alt=\"{1}\"/>", url, archiveDto.Title);

                        //缩略图
                        case "thumb": return this.GetThumbnailUrl(archiveDto.FirstImageUrl);

                        case "thumbnail": return this.GetThumbnailUrl(archiveDto.Thumbnail);

                        case "index":  return (index+1).ToString();

                        // 项目顺序类
                        case "class":
                            if (dt == null || index < 0) return String.Empty;
                            if (index == dt.Count() - 1) return "a last";
                            else if (index == 0) return "a first";
                            return String.Concat("a a", (index+1).ToString());

                        //特性列表
                        case "prolist":
                        case "property_list":
                            StringBuilder sb2 = new StringBuilder();
                            sb.Append("<ul class=\"extend_field_list\">");

                            foreach (IExtendValue f in archiveDto.ExtendValues)
                            {
                                if (!String.IsNullOrEmpty(f.Value))
                                {
                                    sb.Append("<li class=\"extend_")
                                       .Append(f.Field.ToString()).Append("\"><span class=\"attrName\">")
                                        .Append(f.Field.Name).Append(":</span><span class=\"value\">")
                                        .Append(f.Value).Append("</span></li>");
                                }
                            }

                            sb2.Append("</ul>");
                            return sb2.ToString();

                        default:

                            //读取自定义长度大纲
                            const string matchPattern = "outline\\[(\\d+)\\]";
                            if (Regex.IsMatch(field, matchPattern))
                            {
                                int length = int.Parse(Regex.Match(field, matchPattern).Groups[1].Value);
                                return ArchiveUtility.GetOutline(String.IsNullOrEmpty(archiveDto.Outline) ? archiveDto.Content : archiveDto.Outline, length);
                            }

                            //读取自定义属性
                            if (extendFields == null)
                            {
                                extendFields = new Dictionary<string, string>();
                                foreach (IExtendValue value in archiveDto.ExtendValues)
                                {
                                    extendFields.Add(value.Field.Name, value.Value);
                                }
                            }
                            if (extendFields.ContainsKey(field))
                                return extendFields[field];
                            return "";
                    }
                }));

        }


        /// <summary>
        /// 分页文档列表
        /// </summary>
        /// <param name="categoryTag">栏目Tag</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="splitSize">分隔条数</param>
        /// <param name="format">格式</param>
        /// <param name="skipSize">跳过的页码</param>
        /// <returns></returns>
        protected string Paging_Archives(string categoryTag, string pageIndex, string pageSize, int skipSize, int splitSize, string format)
        {
            int _pageIndex,
                 _pageSize,
                 _records,
                 _pages;

            int archiveId;
            int categoryId;



            CategoryDto category = ServiceCall.Instance.SiteService.GetCategory(this.SiteId, categoryTag);
            if (!(category.Id > 0))
            {
                return TplMessage("Error:栏目不存在!");
            }

            bool listContainer = format.EndsWith("</li>");

            int.TryParse(pageIndex, out _pageIndex);
            int.TryParse(pageSize, out _pageSize);

            /**** 显示列表 ****/
            StringBuilder sb = new StringBuilder(1000);

            CategoryDto archiveCategory = category;

            //读取自定义扩展数据字段
            IDictionary<int, IDictionary<string, string>> extendValues;


            int archiveIndex = 0,
                totalNum = 0;
            DataRowCollection drs;

            drs = ServiceCall.Instance.ArchiveService.GetPagedArchives(
                this.SiteId,
                category.Lft,
                category.Rgt,
                _pageSize,
                skipSize,
                ref _pageIndex,
                out _records,
                out _pages,
                out extendValues).Rows;

            totalNum = drs.Count;


            string id;
            foreach (DataRow dr in drs)
            {
                ++archiveIndex;

                //获取栏目，如果栏目关联不起来则调到下一次
                categoryId = int.Parse(dr["cid"].ToString());
                archiveId = int.Parse(dr["id"].ToString());
                id = (string.IsNullOrEmpty((dr["alias"] ?? "").ToString()) ? dr["str_id"] : dr["alias"]).ToString();      //用于链接的ID标识
                if (categoryId != archiveCategory.Id)
                {
                    archiveCategory = ServiceCall.Instance.SiteService.GetCategory(this.SiteId, categoryId);
                    if (!(archiveCategory.Id > 0)) continue;
                }


                sb.Append(TplEngine.FieldTemplate(format,
                    field =>
                    {
                        switch (field)
                        {
                            case "special_title": return !ArchiveFlag.GetFlag(dr["flags"].ToString(), BuiltInArchiveFlags.IsSpecial) ? dr["title"].ToString() : "<span class=\"special\">" + dr["title"].ToString() + "</span>";
                            case "title": return dr["title"].ToString();
                            case "small_title": return (dr["small_title"] ?? "").ToString();
                            case "publisher_id": return dr["publisher_id"].ToString();
                            case "author_name": return ArchiveUtility.GetPublisherName(Convert.ToInt32(dr["publisher_id"] ?? 0));
                            case "source": return dr["source"].ToString();
                            case "fmt_outline": return ArchiveUtility.GetFormatedOutline(
                                 (dr["outline"] ?? "").ToString(),
                                 dr["content"].ToString(),
                                 this.TplSetting.CFG_OutlineLength);
                            case "outline": return ArchiveUtility.GetOutline(String.IsNullOrEmpty((dr["outline"] ?? "").ToString()) ? dr["content"].ToString() : dr["outline"].ToString(), this.TplSetting.CFG_OutlineLength);
                            case "intid": return dr["id"].ToString();
                            case "id": return id;
                            case "alias": return dr["alias"].ToString();
                            case "tags": return dr["tags"].ToString();
                            case "replay": return CmsLogic.Comment.GetArchiveCommentsCount(dr["id"].ToString()).ToString();
                            case "count": return dr["view_count"].ToString();

                            //时间
                            case "modify_time": return String.Format("{0:yyyy-MM-dd HH:mm}", dr["lastmodifydate"]);
                            case "modify_date": return String.Format("{0:yyyy-MM-dd}", dr["lastmodifydate"]);
                            case "modify_sdate": return String.Format("{0:MM-dd}", dr["lastmodifydate"]);
                            case "create_time": return String.Format("{0:yyyy-MM-dd HH:mm}", dr["createdate"]);
                            case "create_date": return String.Format("{0:yyyy-MM-dd}", dr["createdate"]);

                            //栏目
                            case "category_id": return archiveCategory.Id.ToString();

                            case "category_name": return archiveCategory.Name;

                            case "category_tag": return archiveCategory.Tag;

                            case "category_url": return this.GetCategoryUrl(archiveCategory, 1);

                            //链接
                            case "url":
                                return GetArchiveUrl(dr["location"].ToString(), dr["flags"].ToString(), archiveCategory, id);

                            //内容
                            case "content": return dr["content"].ToString();

                            //图片元素
                            case "img":
                                string url = this.GetThumbnailUrl(dr["thumbnail"].ToString());
                                return String.IsNullOrEmpty(url) ? "" : String.Format("<img class=\"thumb thumbnail\" src=\"{0}\" alt=\"{1}\"/>", url, dr["title"].ToString());


                            //缩略图
                            case "thumbnail": return this.GetThumbnailUrl(dr["thumbnail"].ToString());

                            // 项目顺序类
                            case "class":
                                if (archiveIndex == 1) return "first a";
                                if (archiveIndex == totalNum) return "last a";
                                return "a";

                            case "index":
                                return archiveIndex.ToString();

                            default:
                                if (extendValues.ContainsKey(archiveId) &&
                                    extendValues[archiveId].ContainsKey(field))
                                {
                                    return extendValues[archiveId][field];
                                }
                                return "";
                        }
                    }));



                // 添加分割栏
                this.AppendSplitHtm(sb, totalNum, archiveIndex, splitSize, listContainer);
            }

            //设置分页
            this.SetPager(
                _pageIndex,
                _pages,
                _records,
                 this.GetCategoryUrl(category, 1),
                 this.GetCategoryUrl(category, 99).Replace("99", "{0}")
                );

            return sb.ToString();
        }

        /// <summary>
        /// 添加分割栏
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="total"></param>
        /// <param name="index"></param>
        /// <param name="splitSize"></param>
        /// <param name="listContainer"></param>
        private void AppendSplitHtm(StringBuilder sb, int total, int index, int splitSize, bool listContainer)
        {
            const string splitHtm = "<div class=\"break-space\"><div></div></div>";
            const string splitHtmEven = "<div class=\"break-space break-even\"><div></div></div>";
            if (splitSize > 0 && total != index && index % splitSize == 0)
            {
                bool isEven = index / splitSize % 2 == 0;
                if (listContainer)
                {
                    if (isEven)
                    {
                        sb.Append("<li class=\"break break-even\">").Append(splitHtmEven);
                    }
                    else
                    {
                        sb.Append("<li class=\"break\">").Append(splitHtm);
                    }
                    sb.Append("</li>");
                }
                else
                {
                    sb.Append(isEven ? splitHtmEven : splitHtm);
                }
            }
        }


        protected string Special_Archives(string tag, string num, int skipSize, int splitSize, bool container, string format)
        {
            int intNum;
            int.TryParse(num, out intNum);

            //获取栏目
            var category = ServiceCall.Instance.SiteService.GetCategory(this.SiteId, tag);

            if (!(category.Id > 0))
            {
                return String.Format("ERROR:模块或栏目不存在!参数:{0}", tag);
            }

            ArchiveDto[] dt = container
                ? ServiceCall.Instance.ArchiveService.GetSpecialArchives(this.SiteId, category.Lft, category.Rgt, intNum, skipSize)
                : ServiceCall.Instance.ArchiveService.GetSpecialArchives(this.SiteId, category.Tag, intNum, skipSize);

            return this.ArchiveList(dt, splitSize, format);
        }

        protected string Archives(string tag, string num, int skipSize, int splitSize, bool container, string format)
        {
            int intNum;
            int.TryParse(num, out intNum);

            //栏目
            var category = ServiceCall.Instance.SiteService.GetCategory(this.SiteId, tag);

            if (!(category.Id > 0))
            {
                return String.Format("ERROR:模块或栏目不存在!参数:{0}", tag);
            }
            ArchiveDto[] archives = container ? ServiceCall.Instance.ArchiveService.GetArchivesContainChildCategories(this.SiteId, category.Lft, category.Rgt, intNum, skipSize) :
                ServiceCall.Instance.ArchiveService.GetArchivesByCategoryTag(this.SiteId, category.Tag, intNum, skipSize);

            return this.ArchiveList(archives, splitSize, format);
        }


        /// <summary>
        /// 搜索列表
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryTagOrModuleId"></param>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="splitSize"></param>
        /// <param name="format"></param>
        /// <param name="pageCount"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        protected string SearchArchives(int siteId, string categoryTagOrModuleId, string keyword,
            string pageIndex, string pageSize, string splitSize, string format, out int pageCount,
            out int recordCount)
        {
            int intPageIndex,
                intPageSize,
                total = 0,
                pages = 0;

            int C_LENGTH = this.TplSetting.CFG_OutlineLength;
            int intSplitSize;
            bool hasSetCategory = false;                               //是否在搜索中指定栏目参数

            CategoryDto category = default(CategoryDto);
            J6.Cms.Domain.Interface.Models.Module module = null;
            Regex keyRegex;

            int.TryParse(pageIndex, out intPageIndex);
            int.TryParse(pageSize, out intPageSize);
            int.TryParse(splitSize, out intSplitSize);

            keyword = keyword.Replace("+", String.Empty);                  //删除+号连接符
            keyRegex = new Regex(keyword, RegexOptions.IgnoreCase);



            bool listContainer = format.EndsWith("</li>");

            /**** 显示列表 ****/
            StringBuilder sb = new StringBuilder(1000);


            string alias,
                title,
                content,
                title_hightlight;


            int i = 0;

            IEnumerable<ArchiveDto> searchArchives = null;

            if (!String.IsNullOrEmpty(categoryTagOrModuleId))
            {
                //按模块搜索
                //if (Regex.IsMatch(categoryTagOrModuleID, "^\\d+$"))
                //{
                //    module = CmsLogic.Module.GetModule(int.Parse(categoryTagOrModuleID));
                //    if (module != null)
                //    {
                //        searchArchives = CmsLogic.Archive.SearchByModule(keyword, int.Parse(categoryTagOrModuleID), _pageSize, _pageIndex, out _records, out _pages, "ORDER BY CreateDate DESC").Rows;
                //    }
                //}

                //如果模块不存在，则按栏目搜索
                if (searchArchives == null)
                {
                    category = ServiceCall.Instance.SiteService.GetCategory(siteId, categoryTagOrModuleId);
                    if (category.Id > 0)
                    {
                        hasSetCategory = true;
                        searchArchives = ServiceCall.Instance.ArchiveService.SearchArchivesByCategory(siteId,
                            category.Lft, category.Rgt, keyword, intPageSize, intPageIndex, out total, out pages,
                            "ORDER BY CreateDate DESC");
                    }
                    else
                    {
                        pageCount = 0;
                        recordCount = 0;
                        return this.TplMessage("ERROR:栏目不存在!");
                    }

                }
            }

            //如果未设置模块或栏目参数
            if (searchArchives == null)
            {
                searchArchives = ServiceCall.Instance.ArchiveService.SearchArchives(siteId, 0, 0, false, keyword, intPageSize, intPageIndex, out total, out pages, "ORDER BY CreateDate DESC");
            }

            IDictionary<string, string> extendFields = null;


            foreach (ArchiveDto archive in searchArchives)
            {
                //if (!hasSetCategory)
                //{
                //    //获取栏目，如果栏目关联不起来则调到下一次
                //    category = ServiceCall.Instance.SiteService.GetCategory(this.siteId, archive.Category.ID);
                //    if (!(category.ID > 0)) continue;
                //}

                category = archive.Category;

                #region 处理关键词
                alias = String.IsNullOrEmpty(archive.Alias) ? archive.StrId : archive.Alias;
                title = title_hightlight = archive.Title;
                content = RegexHelper.FilterHtml(archive.Content);

                if (keyRegex.IsMatch(title_hightlight))
                {
                    title_hightlight = keyRegex.Replace(title_hightlight, "<span class=\"search_hightlight\">$0</span>");
                }

                //关键词前数字索引算法
                int contentLength = content.Length;
                int firstSplitIndex = contentLength % 11; ;

                if (keyRegex.IsMatch(content))
                {
                    Match match = keyRegex.Match(content);
                    if (contentLength > C_LENGTH)
                    {
                        if (match.Index > firstSplitIndex)
                        {
                            //如果截取包含关键词的长度仍大于内容长度时
                            if (contentLength - match.Index > C_LENGTH)
                            {
                                content = content.Substring(match.Index - firstSplitIndex, C_LENGTH - firstSplitIndex);
                            }
                            else
                            {
                                content = content.Substring(match.Index - firstSplitIndex);
                            }
                        }
                        else
                        {
                            content = content.Remove(C_LENGTH);
                        }
                    }
                    content = keyRegex.Replace(content, "<span class=\"search_hightlight\">$0</span>") + "...";
                }
                else
                {
                    if (contentLength > C_LENGTH)
                    {
                        content = content.Substring(0, C_LENGTH) + "...";
                    }
                }

                #endregion


                int archivesCount = searchArchives.Count();
                sb.Append(TplEngine.FieldTemplate(format,
                    field =>
                    {
                        switch (field)
                        {
                            case "id":return archive.Id.ToString();
                            case "str_id": return alias;
                            case "special_title": return title_hightlight;
                            case "small_title": return archive.SmallTitle;
                            case "title": return archive.Title;
                            case "publisher_id": return archive.PublisherId.ToString();
                            case "author_name": return ArchiveUtility.GetPublisherName(archive.PublisherId);
                            case "source": return archive.Source;
                            case "outline": return content;
                            case "tags": return archive.Tags;
                            case "replay": return CmsLogic.Comment.GetArchiveCommentsCount(archive.StrId).ToString();
                            case "count": return archive.ViewCount.ToString();

                            //时间
                            case "modify_time": return String.Format("{0:yyyy-MM-dd HH:mm:ss}", archive.LastModifyDate);
                            case "modify_date": return String.Format("{0:yyyy-MM-dd}", archive.LastModifyDate);
                            case "modify_sdate": return String.Format("{0:MM-dd}", archive.LastModifyDate);
                            case "create_time": return String.Format("{0:yyyy-MM-dd HH:mm:ss}", archive.CreateDate);
                            case "create_date": return String.Format("{0:yyyy-MM-dd}", archive.CreateDate);

                            //栏目
                            case "category_name": return category.Name;
                            case "category_tag": return category.Tag;
                            case "category_url": return this.GetCategoryUrl(category, 1);
                            //链接
                            case "url":
                                return GetArchiveUrl(archive.Location, archive.Flags, category, alias);

                            //内容
                            case "content": return archive.Content;

                            //图片元素
                            case "img":
                                string url = this.GetThumbnailUrl(archive.Thumbnail);
                                return String.IsNullOrEmpty(url) ? "" : String.Format("<img class=\"thumb thumbnail\" src=\"{0}\" alt=\"{1}\"/>", url, archive.Title);

                            //缩略图
                            case "thumbnail": return this.GetThumbnailUrl(archive.Thumbnail);

                            // 项目顺序类
                            case "class":
                                if (i == archivesCount - 1) return " class=\"a last\"";
                                else if (i == 0) return " class=\"a first\"";
                                return " class=\"a\"";

                            default:
                                //读取自定义属性
                                if (extendFields == null)
                                {
                                    extendFields = new Dictionary<string, string>();
                                    foreach (IExtendValue value in archive.ExtendValues)
                                    {
                                        extendFields.Add(value.Field.Name, value.Value);
                                    }
                                }
                                if (extendFields.ContainsKey(field))
                                    return extendFields[field];
                                return "";
                        }
                    }));


                // 添加分割栏
                this.AppendSplitHtm(sb, total, i++, intSplitSize, listContainer);



                ++i;
            }


            pageCount = pages;
            recordCount = total;

            //如果无搜索结果
            if (sb.Length < 30)
            {
                string message;
                switch (_site.Language)
                {
                    default:
                    case Languages.En:
                        message = "No result";
                        break;
                    case Languages.Zh_CN:
                        message = "没有相关记录";
                        break;
                }

                sb.Append(String.Format("<div class=\"noresult\">{0}</div>", message));
            }
            else
            {
                //设置分页
                this.SetPager(
                    intPageIndex,
                    pages,
                    total,
                    this.FormatPageUrl(UrlRulePageKeys.Search, HttpUtility.UrlEncode(keyword), categoryTagOrModuleId ?? ""),
                    this.FormatPageUrl(UrlRulePageKeys.SearchPager, HttpUtility.UrlEncode(keyword), categoryTagOrModuleId ?? "", "{0}")
                    );
            }



            return sb.ToString();
        }

        /// <summary>
        /// 链接
        /// </summary>
        /// <param name="type"></param>
        /// <param name="format"></param>
        /// <param name="number"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected string Link(string type, string format, int number, string index)
        {
            int _type = 2;
            int.TryParse(type, out _type);
            SiteLinkType linkType = (SiteLinkType)_type;

            StringBuilder sb = new StringBuilder();

            IList<SiteLinkDto> links = new List<SiteLinkDto>(
                ServiceCall.Instance.SiteService
                .GetLinksByType(this.SiteId, linkType, false));

            SiteLinkDto link;

            bool isLast;
            int navIndex;
            if (index == "")
            {
                navIndex = -1;           //如果为空，则默认不选中
            }
            else
            {
                int.TryParse(index, out navIndex);
            }


            if (number < 1) number = links.Count;
            else if (number > links.Count) number = links.Count;

            for (int i = 0; i < number; i++)
            {
                link = links[i];

                /* *********************
                 *  辨别选中的导航
                 * *********************/
                isLast = i == links.Count - 1;

                sb.Append(TplEngine.FieldTemplate(format, field =>
                {
                    switch (field)
                    {
                        default: return String.Empty;
                        case "text": return link.Text;
                        case "imgurl": return link.ImgUrl;
                        case "img": return String.IsNullOrEmpty(link.ImgUrl) ? "" : String.Format("<img src=\"{0}\" alt=\"{1}\"/>", link.ImgUrl, link.Text);
                        case "url": return String.IsNullOrEmpty(link.Bind) ? link.Uri : this.GetBingLinkUrl(link.Bind);
                        case "target": return String.IsNullOrEmpty(link.Target) || String.Compare(link.Target, "_self", true) == 0 ? String.Empty : " target=\"_blank\"";
                        case "id": return link.Id.ToString();
                        case "class":
                            if (navIndex == i)
                            {
                                return i == 0 ? " class=\"current first\"" : (!isLast ? " class=\"current\"" : " class=\"current last\"");
                            }
                            else
                            {
                                return i == 0 ? " class=\"first\"" : (!isLast ? String.Empty : " class=\"last\"");
                            }
                    }
                }));


            }

            return sb.ToString();


        }


        /// <summary>
        /// 标签
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Tags(string tags, string format)
        {
            if (String.IsNullOrEmpty(tags))
            {
                return String.Empty;
            }
            StringBuilder sb = new StringBuilder();
            string[] tagArr = tags.Split(',');
            int i = tagArr.Length;

            if (i > 0)
            {
                sb.Append(Cms.Language.Get(LanguagePackageKey.ARCHIVE_Tags)).Append("：");
            }
            foreach (string tag in tagArr)
            {
                sb.Append(TplEngine.FieldTemplate(format, a =>
                {
                    switch (a)
                    {
                        case "tagName":
                        case "name":
                            return tag;

                        case "eTagName":
                        case "ename":
                        case "urlname":
                            return HttpUtility.UrlEncode(tag);

                        //搜索页URL
                        case "searchurl": return this.FormatPageUrl(UrlRulePageKeys.Search, HttpUtility.UrlEncode(tag), String.Empty);

                        //Tag页URL
                        case "url": return this.FormatPageUrl(UrlRulePageKeys.Tag, HttpUtility.UrlEncode(tag));
                    }
                    return tag;
                }));

                if (--i != 0)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 标签文档列表
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string TagArchives(string tag, string pageIndex, string pageSize, string format)
        {
            int _pageIndex,
                _pageSize,
                _records,
                _pages;



            CategoryDto category;
            Regex keyRegex;

            int.TryParse(pageIndex, out _pageIndex);
            int.TryParse(pageSize, out _pageSize);

            tag = tag.Replace("+", String.Empty);                  //删除+号连接符
            keyRegex = new Regex(tag, RegexOptions.IgnoreCase);

            /**** 显示列表 ****/
            StringBuilder sb = new StringBuilder(1000);


            string alias,
               title,
               content,
               title_hightlight;


            int i = 0;

            int C_LENGTH = this.TplSetting.CFG_OutlineLength;
            IEnumerable<ArchiveDto> searchArchives = ServiceCall.Instance.ArchiveService
                .SearchArchives(this.SiteId, 0, 0, false, tag,
                _pageSize, _pageIndex,
                out _records, out _pages, "ORDER BY CreateDate DESC");


            IDictionary<string, string> extendFields = null;


            foreach (ArchiveDto archive in searchArchives)
            {
                //if (!hasSetCategory)
                //{
                //    //获取栏目，如果栏目关联不起来则调到下一次
                //    category = ServiceCall.Instance.SiteService.GetCategory(this.siteId, archive.Category.ID);
                //    if (!(category.ID > 0)) continue;
                //}

                category = archive.Category;

                #region 处理关键词
                alias = String.IsNullOrEmpty(archive.Alias) ? archive.StrId : archive.Alias;
                title = title_hightlight = archive.Title;
                content = RegexHelper.FilterHtml(archive.Content);

                if (keyRegex.IsMatch(title_hightlight))
                {
                    title_hightlight = keyRegex.Replace(title_hightlight, "<span class=\"search_hightlight\">$0</span>");
                }

                //关键词前数字索引算法
                int contentLength = content.Length;
                int firstSplitIndex = contentLength % 11; ;

                if (keyRegex.IsMatch(content))
                {
                    Match match = keyRegex.Match(content);
                    if (contentLength > C_LENGTH)
                    {
                        if (match.Index > firstSplitIndex)
                        {
                            //如果截取包含关键词的长度仍大于内容长度时
                            if (contentLength - match.Index > C_LENGTH)
                            {
                                content = content.Substring(match.Index - firstSplitIndex, C_LENGTH - firstSplitIndex);
                            }
                            else
                            {
                                content = content.Substring(match.Index - firstSplitIndex);
                            }
                        }
                        else
                        {
                            content = content.Remove(C_LENGTH);
                        }
                    }
                    content = keyRegex.Replace(content, "<span class=\"search_hightlight\">$0</span>") + "...";
                }
                else
                {
                    if (contentLength > C_LENGTH)
                    {
                        content = content.Substring(0, C_LENGTH) + "...";
                    }
                }

                #endregion


                int archivesCount = searchArchives.Count();
                sb.Append(TplEngine.FieldTemplate(format,
                    field =>
                    {
                        switch (field)
                        {
                            case "id":
                                return archive.Id.ToString();
                            case "str_id": return alias;
                            case "special_title": return title_hightlight;
                            case "title": return archive.Title;
                            case "small_title": return archive.SmallTitle;
                            case "publisher_id": return archive.PublisherId.ToString();
                            case "author_name": return ArchiveUtility.GetPublisherName(archive.PublisherId);
                            case "source": return archive.Source;
                            case "outline": return content;
                            case "tags": return archive.Tags;
                            case "replay": return CmsLogic.Comment.GetArchiveCommentsCount(archive.StrId).ToString();
                            case "count": return archive.ViewCount.ToString();

                            //时间
                            case "modify_time": return String.Format("{0:yyyy-MM-dd HH:mm:ss}", archive.LastModifyDate);
                            case "modify_date": return String.Format("{0:yyyy-MM-dd}", archive.LastModifyDate);
                            case "modify_sdate": return String.Format("{0:MM-dd}", archive.LastModifyDate);
                            case "create_time": return String.Format("{0:yyyy-MM-dd HH:mm:ss}", archive.CreateDate);
                            case "create_date": return String.Format("{0:yyyy-MM-dd}", archive.CreateDate);

                            //栏目
                            //case "categoryid":
                            //case "cid": return category.ID.ToString();
                            case "category_name": return category.Name;
                            case "category_tag": return category.Tag;
                            case "category_url": return this.GetCategoryUrl(category, 1);
                            //链接
                            case "url":
                                return GetArchiveUrl(archive.Location, archive.Flags, category, alias);

                            //内容
                            case "content": return archive.Content;

                            //图片元素
                            case "img":
                                string url = this.GetThumbnailUrl(archive.Thumbnail);
                                return String.IsNullOrEmpty(url) ? "" : String.Format("<img class=\"thumb thumbnail\" src=\"{0}\" alt=\"{1}\"/>", url, archive.Title);

                            //缩略图
                            case "thumbnail": return this.GetThumbnailUrl(archive.Thumbnail);

                            // 项目顺序类
                            case "class":
                                if (i == archivesCount - 1) return " class=\"last\"";
                                else if (i == 0) return " class=\"first\"";
                                return string.Empty;

                            default:
                                //读取自定义属性
                                if (extendFields == null)
                                {
                                    extendFields = new Dictionary<string, string>();
                                    foreach (IExtendValue value in archive.ExtendValues)
                                    {
                                        extendFields.Add(value.Field.Name, value.Value);
                                    }
                                }
                                if (extendFields.ContainsKey(field))
                                    return extendFields[field];
                                return "";
                        }
                    }));





                ++i;
            }




            //如果无搜索结果
            if (sb.Length < 30)
            {
                sb.Append(String.Format("<div class=\"noresult\">没有相关记录！</div>", tag));
            }
            else
            {
                //设置分页
                this.SetPager(
                    _pageIndex,
                    _pages,
                    _records,
                    this.FormatPageUrl(UrlRulePageKeys.Tag, HttpUtility.UrlEncode(tag)),
                    this.FormatPageUrl(UrlRulePageKeys.TagPager, HttpUtility.UrlEncode(tag), "{0}")
                    );
            }
            //sb.Append("</div>");


            return sb.ToString();
        }


        /*========================= 树形数据 ================================*/
        protected delegate bool CategoryResultTreeHandler(CategoryDto category);
        protected void CategoryTree_Iterator(CategoryDto category, StringBuilder sb, CategoryResultTreeHandler handler, bool isRoot)
        {
            /*if (category.IsSign)
            {
                category.IsSign = false;
                return;
            }
            else
            {*/
            if (!isRoot)
            {
                sb.Append("<li><a href=\"")
                    //
                    //TODO:
                    //
                    //.Append(this.GetCategoryUrl(category, 1)).Append("\" tag=\"")
                    .Append(category.Tag).Append("\" lft=\"").Append(category.Lft.ToString()).Append("\">")
                    .Append(category.Name).Append("</a>");
            }

            IList<CategoryDto> childs = new List<CategoryDto>(ServiceCall.Instance.SiteService.GetCategories(
                this.SiteId,
                category.Lft,
                category.Rgt,
                CategoryContainerOption.NextLevel));


            if (childs.Count != 0)
            {
                sb.Append("<ul>");

                foreach (CategoryDto c in childs)
                {
                    if (isRoot || handler(category))
                    {
                        CategoryTree_Iterator(c, sb, handler, false);
                    }
                }
                sb.Append("</ul>");
            }

            if (!isRoot)
            {
                sb.Append("</li>");
            }

            //category.IsSign = true;
            // }
        }


        /// <summary>
        /// 栏目嵌套树
        /// </summary>
        /// <param name="categoryTag"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        [TemplateTag]
        public string Category_Tree(string categoryTag)
        {
            if (categoryTag == "") categoryTag = "root";

            //读取缓存
            string cacheKey = String.Format("{0}_site{1}_tree_{2}", CacheSign.Category.ToString(), SiteId.ToString(), categoryTag);

            BuiltCacheResultHandler<String> bh = () =>
            {
                //无缓存,则继续执行
                StringBuilder sb = new StringBuilder(400);

                CategoryDto category = ServiceCall.Instance.SiteService.GetCategory(this.SiteId, categoryTag);
                if (!(category.Id > 0))
                {
                    return TplMessage("不存在栏目!标识:" + categoryTag);
                }
                sb.Append("<div class=\"category_tree\">");

                this.CategoryTree_Iterator(category, sb, a => { return true; }, true);

                sb.Append("</div>");

                return sb.ToString();
            };

            return Cms.Cache.GetCachedResult(cacheKey, bh, DateTime.Now.AddHours(Settings.OptiDefaultCacheHours));
        }

        /// <summary>
        /// 栏目选择树
        /// </summary>
        /// <param name="categoryTag"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Category_Option(string categoryTag, string split)
        {
            //读取缓存
            string cacheKey = String.Format("{0}_site{1}_select_tree_{2}_{3}",
                CacheSign.Category.ToString(),
                this.SiteId.ToString(),
                categoryTag,
                split);

            object cache = Cms.Cache.Get(cacheKey);

            if (cache != null)
            {
                return cache as string;
            }

            //无缓存,则继续执行


            //IList<Category> categories = new List<Category>();
            bool isModule = false;
            int moduleId = 0;


            StringBuilder sb = new StringBuilder(600);
            CategoryTreeHandler treeHandler = (_category, _level, isLast) =>
            {
                /*
                if (isModule)
                {
                    if (_category.ModuleID != moduleID)
                    {
                        sb.Append("<optgroup label=\"").Append(_category.Name)
                            .Append("\">").Append("</optgroup>");
                    } return;
                }
                */

                if (_category.Site.Id != SiteId) return;

                sb.Append("<option value=\"").Append(_category.Tag)
                    .Append("\" path=\"")

                    //
                    //TODO:
                    //
                    //.Append(ArchiveUtility.GetCategoryUrlPath(_category))
                    .Append("\">");
                for (int i = 0; i < _level; i++)
                {
                    sb.Append(split);
                }
                sb.Append(_category.Name).Append("</option>");
            };


            if (String.IsNullOrEmpty(categoryTag))
            {
                return TplMessage("请指定参数:param的值");
            }
            else if (Regex.IsMatch(categoryTag, "^\\d+$"))
            {
                //从模块加载
                moduleId = int.Parse(categoryTag);
                if (CmsLogic.Module.GetModule(moduleId) != null)
                {
                    isModule = true;
                    ServiceCall.Instance.SiteService.HandleCategoryTree(this.SiteId, 1, treeHandler);
                }
            }

            if (!isModule)
            {

                CategoryDto category = ServiceCall.Instance.SiteService.GetCategory(this.SiteId, categoryTag);
                if (!(category.Id > 0))
                {
                    return TplMessage("不存在栏目!标识:" + categoryTag);
                }

                ServiceCall.Instance.SiteService.HandleCategoryTree(this.SiteId, category.Lft, treeHandler);
            }



            string result = sb.ToString();

            //缓存
            Cms.Cache.Insert(cacheKey, result, DateTime.Now.AddDays(Settings.OptiDefaultCacheHours));

            return result;

        }

        /// <summary>
        /// 栏目选择树
        /// </summary>
        [TemplateTag]
        public string Category_Option(string categoryTag)
        {
            return Category_Option(categoryTag, "一");
        }




        #region 析构

        public void Dispose()
        {
            _settingsFile = null;
        }

        ~CmsTemplateCore()
        {
            this.Dispose();
        }
        #endregion
    }
}
