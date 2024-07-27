using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using JR.Cms.Conf;
using JR.Cms.Core;
using JR.Cms.Domain.Interface;
using JR.Cms.Domain.Interface.Common.Language;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Enum;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Domain.Interface.Site.Link;
using JR.Cms.Library.CacheProvider.CacheComponent;
using JR.Cms.Library.CacheService;
using JR.Cms.Library.DataAccess.BLL;
using JR.Cms.Library.Utility;
using JR.Cms.ServiceDto;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core;
using JR.Stand.Core.Framework;
using JR.Stand.Core.Framework.Extensions;
using JR.Stand.Core.Framework.Web.UI;
using JR.Stand.Core.Framework.Xml.AutoObject;
using JR.Stand.Core.Template;
using JR.Stand.Core.Template.Impl;
using JR.Stand.Core.Web;
using Module = JR.Cms.Domain.Interface.Models.Module;

namespace JR.Cms.Web.Portal.Template.Rule
{

    public abstract class CmsTemplateCore : ITemplateResolver, IDisposable
    {
        private IDataContainer _container = null;

        private static PropertyInfo[] archivePros =
            typeof(ArchiveDto).GetProperties(BindingFlags.Instance | BindingFlags.Public); //文档的属性

        protected static MicroTemplateEngine TplEngine = new MicroTemplateEngine(null); //模板引擎

        protected ArchiveDto archive; //当前读取的文档,使用require(archiveID)获取的文档

        private SettingFile _settingsFile; //设置文件，用于保存当前实例的状态

        // private LangLabelReader langReader;     //语言字典读取器
        private string _resourceUri; //资源域名

        protected ICompatibleHttpContext _context;

        protected CmsTemplateCore(ICompatibleHttpContext context)
        {
            this._context = context;

            _ctx = Cms.Context;
            _site = _ctx.CurrentSite;
            SiteId = _site.SiteId;
        }

        /// <summary>
        /// 当前站点
        /// </summary>
        protected SiteDto _site;

        /// <summary>
        /// 当前站点编号
        /// </summary>
        protected int SiteId;

        protected CmsContext _ctx;

        /// <summary>
        /// 模板设置
        /// </summary>
        private TemplateSetting _tplSetting;

        public void SetContainer(IDataContainer container)
        {
            this._container = container;
        }

        public IDataContainer GetContainer()
        {
            return this._container;
        }

        protected TemplateSetting GetSetting()
        {
            return _tplSetting ?? (_tplSetting = Cms.TemplateManager.Get(_ctx.CurrentSite.Tpl));
        }


        /************************ 辅助方法 ********************************/

        #region

        /// <summary>
        /// 格式化地址
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual string FormatPageUrl(UrlRulePageKeys key, object[] data)
        {
            var url = TemplateUrlRule.Urls[TemplateUrlRule.RuleIndex, (int)key];
            if (data != null) url = string.Format(url, data);

            return ConcatUrl(url);
        }

        /// <summary>
        /// 连接URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected string ConcatUrl(string url)
        {
            if (url.IndexOf("//", StringComparison.Ordinal) != -1) return url;
            if (url.StartsWith("javascript:", StringComparison.Ordinal)) return url;
            if (!string.IsNullOrEmpty(url) && url[0] != '/') url = string.Concat("/", url);
            if (Settings.TPL_FULL_URL_PATH) return string.Concat(Cms.Context.SiteDomain, url);
            return url;
        }

        private bool FlagAnd(int flag, BuiltInArchiveFlags b)
        {
            var x = (int)b;
            return (flag & x) == x;
        }

        /// <summary>
        /// 获取文档的地址
        /// </summary>
        /// <param name="location"></param>
        /// <param name="flag"></param>
        /// <param name="archivePath"></param>
        /// <returns></returns>
        private string GetArchiveUrl(string location, int flag, string archivePath)
        {
            if (!string.IsNullOrEmpty(location)) return ConcatUrl(location);
            if (!FlagAnd(flag, BuiltInArchiveFlags.AsPage))
            {
                return FormatPageUrl(UrlRulePageKeys.Archive, new[] { archivePath });
            }
            return FormatPageUrl(UrlRulePageKeys.SinglePage, new[] { archivePath });
        }

        private string GetLocationUrl(string location)
        {
            //如果定义了跳转地址
            // if (Regex.IsMatch(location, "^http(s?)://", RegexOptions.IgnoreCase))
            if (location.IndexOf("//", StringComparison.Ordinal) != -1) return location;

            return ConcatUrl(location);
        }

        /// <summary>
        /// 获取栏目的地址
        /// </summary>
        /// <param name="category"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        protected string GetCategoryUrl(CategoryDto category, int pageIndex)
        {
            if (!string.IsNullOrEmpty(category.Location)) return ConcatUrl(category.Location);
            if (pageIndex < 2)
            {
                return ConcatUrl(category.Path);
            }
            else
            {
                return FormatPageUrl(UrlRulePageKeys.CategoryPager, new object[] { category.Path, pageIndex.ToString() });
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
            //    case Languages.zh_CN:
            //        pagerPack = zh_cn_pack; break;
            //    case Languages.zh_TW:
            //        pagerPack = zh_tw_pack; break;
            //}

            //string[] pagerLangs = pagerPack.Split('|');

            // const string pagerTpl = "<div class=\"pager\">{0}</div>";

            IPagingGetter getter = new CustomPagingGetter(
                firstLink,
                link,
                Cms.Language.Get(LanguagePackageKey.PAGER_PrePageText),
                Cms.Language.Get(LanguagePackageKey.PAGER_NextPageText)
            );

            var p = UrlPaging.NewPager(pageIndex, pageCount, getter);
            p.LinkCount = 10;
            p.PagerTotal = "<span class=\"pagination-info\">" + Cms.Language.Get(LanguagePackageKey.PAGER_PagerTotal) +
                           "</span>";
            p.RecordCount = recordCount;
            /* StringBuilder sb = new StringBuilder();
           
           sb.Append("<div class=\"pager\">");

           
           p.RecordCount = recordCount;
           p.FirstPageLink = firstLink;
           p.LinkFormat = link;

           p.PreviousPageText = jr.Language.Get(LanguagePackageKey.PAGER_PrePageText);
           p.NextPageText = jr.Language.Get(LanguagePackageKey.PAGER_NextPageText);
           p.PageTextFormat = "{0}";
           p.SelectPageText = jr.Language.Get(LanguagePackageKey.PAGER_SelectPageText);
           p.Style = PagerStyle.Custom;
           p.EnableSelect = true;
           p.PagerTotal = jr.Language.Get(LanguagePackageKey.PAGER_PagerTotal);
           p.LinkCount = 10;*/

            var key = PushPagerKey();
            Cms.Context.Items[key] = p.Pager(); // String.Format(pagerTpl, p.ToString());
        }

        protected string PopPagerKey()
        {
            var pagerNumber = 0;
            var pagerNum = Cms.Context.Items["pagerNumber"];
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
            return string.Format("pager_{0}", (pagerNumber + 1).ToString());
        }

        protected string PushPagerKey()
        {
            var pagerNumber = 0;
            var pagerNum = Cms.Context.Items["pagerNumber"];
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
            return string.Format("pager_{0}", pagerNumber.ToString());
        }

        #endregion

        /// <summary>
        /// 获取绑定的链接地址
        /// </summary>
        /// <param name="bindStr"></param>
        /// <returns></returns>
        private string GetBingLinkUrl(string bindStr)
        {
            var binds = (bindStr ?? "").Split(':');
            if (binds.Length == 2 && binds[1] != string.Empty)
            {
                if (binds[0] == "category")
                {
                    var category = LocalService.Instance.SiteService.GetCategory(_site.SiteId, int.Parse(binds[1]));
                    if (category.ID > 0) return GetCategoryUrl(category, 1);
                }
                else if (binds[0] == "archive")
                {
                    int.TryParse(binds[1], out var archiveId);

                    var archiveDto = LocalService.Instance.ArchiveService
                        .GetArchiveById(SiteId, archiveId);

                    if (archiveDto.Id > 0)
                    {
                        return GetArchiveUrl(archiveDto.Location, archiveDto.Flag, archiveDto.Path);
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
            return str == "1" || string.Compare(str, "true", StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// 获取缩略图地址
        /// </summary>
        /// <param name="thumbnail"></param>
        /// <returns></returns>
        protected string GetThumbnailUrl(string thumbnail)
        {
            if (string.IsNullOrEmpty(thumbnail)) return string.Concat("/", CmsVariables.FRAMEWORK_ARCHIVE_NoPhoto);

            if (!Settings.TPL_FULL_URL_PATH //不使用完整路径
                || thumbnail.IndexOf("://", StringComparison.Ordinal) != -1) //如果是包含协议的地址
            {
                return thumbnail;
            }
            else
            {
                //获取包含完整URL的图片地址
                if (_resourceUri == null) _resourceUri = WebCtx.Current.Domain;

                return string.Concat(_resourceUri, thumbnail);
            }
        }

        /// <summary>
        /// 模板提示
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected string TplMessage(string msg)
        {
            if (GetSetting().CfgShowError) return string.Format("提示：{0}", msg);

            return string.Empty;
        }

        #endregion


        /************************ 模板方法 *******************************/

        #region 模板方法

        [TemplateMethod]
        protected string Each_Category(string param, string dataNum, string refTag, string refName, string refUri, string format)
        {
            //
            // @param : 如果为int,则返回模块下的栏目，
            //                 如果为字符串tag，则返回该子类下的栏目
            //

            var siteId = _site.SiteId;

            var num = 0;
            if (Regex.IsMatch(dataNum, "^\\d+$")) int.TryParse(dataNum, out num);


            #region 取得栏目

            IEnumerable<CategoryDto> categories1;
            if (param == string.Empty)
            {
                categories1 = LocalService.Instance.SiteService.GetCategories(siteId);
            }
            else
            {
                if (Regex.IsMatch(param, "^\\d+$"))
                {
                    var moduleId = int.Parse(param);
                    categories1 = LocalService.Instance.SiteService.GetCategories(siteId)
                        .Where(a => a.ModuleId == moduleId);
                }
                else
                {
                    var category = LocalService.Instance.SiteService.GetCategory(SiteId, param);
                    if (category.ID > 0)
                        categories1 = LocalService.Instance.SiteService.GetCategories(SiteId, category.Path);
                    else
                        categories1 = null;
                }
            }

            #endregion

            if (categories1 == null)
            {
                return string.Empty;
            }

            IList<CategoryDto> categories = new List<CategoryDto>(categories1.OrderBy(a => a.SortNumber));
            var sb = new StringBuilder(400);
            var i = 0;
            var total = categories.Count;
            foreach (var c in categories)
            {
                if (num != 0 && ++i >= num) break;
                if (c.SiteId == SiteId)
                {
                    sb.Append(TplEngine.ResolveFields(format, field =>
                    {
                        switch (field)
                        {
                            default:
                                if (field == refName) return c.Name;
                                if (field == refTag) return c.Path;
                                if (field == refUri) return GetCategoryUrl(c, 1);
                                return "${" + field + "}";
                            case "description": return c.Description;
                            case "keywords": return c.Keywords;
                            case "index": return i.ToString();
                            case "class":
                                return GetCssClass(total, i, "c", c.Icon);
                        }
                    }));
                }
            }

            return sb.ToString();
        }

        [TemplateMethod]
        protected string Each_Category(string dataNum, string refTag, string refName, string refUri, string format)
        {
            if (this._context.TryGetItem<string>("category.path", out var id))
            {
                if (!string.IsNullOrEmpty(id)) return Each_Category(id, dataNum, refTag, refName, refUri, format);
            }

            return TplMessage("Error: 此标签不允许在当前页面中调用!");
        }

        [TemplateMethod]
        [Obsolete]
        protected string EachCategory2(string dataNum, string refTag, string refName, string refUri, string format)
        {
            var id = Cms.Context.Items["module.id"];
            if (id == null) return TplMessage("此标签不允许在当前页面中调用!");

            return Each_Category(id.ToString(), dataNum, refTag, refName, refUri, format);
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
            return this._context.Request.Query(id);
        }

        /// <summary>
        /// 获取项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Item(string id)
        {
            return (Cms.Context.Items[id] ?? string.Empty).ToString();
        }

        /// <summary>
        /// 字典标签
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [TemplateTag]
        public string Label(string key)
        {
            var cacheKey = $"{CacheSign.Site.ToString()}_label_{_site.SiteId.ToString()}";
            if (_settingsFile == null)
            {
                //读取数据
                _settingsFile = Cms.Cache.Get(cacheKey) as SettingFile;
                if (_settingsFile == null)
                {
                    var phyPath = $"{Cms.PhysicPath}templates/{_site.Tpl}/label.conf";
                    _settingsFile = new SettingFile(phyPath);

                    //缓存数据
                    Cms.Cache.Insert(cacheKey, _settingsFile, phyPath);
                }
            }

            return _settingsFile[key];
        }

        /// <summary>
        /// 语言标签
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取语言文本,连词用\"-\"", @"")]
        public string Lang(string key)
        {
            var lang = this._ctx.UserLanguage;
            return Cms.Language.Gets(lang, key.Split('-')) ?? "# missing lang:" + key;
        }

        /// <summary>
        /// 变量标签
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值,当传值时设置变量，不传值时返回变量</param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("变量标签", @"$nav='hello';$var(nav)")]
        public string Var(String key)
        {
            Object v = _container.GetVariable(key);
            return (string)v ?? "";
        }

        /// <summary>
        /// 判断标签
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("变量标签", @"$equals(var:nav,1,'active','not-active')")]
        public string Equals(String key, String value, String trueText, String falseText)
        {
            String dst = key;
            if (key.StartsWith("var:"))
            {
                Object v = _container.GetVariable(key.Substring(4));
                dst = (string)v ?? "";
            }
            if (key.StartsWith("param:"))
            {
                dst = this._context.Request.Query(key.Substring(6));
            }
            return dst.Equals(value) ? trueText : falseText;
        }



        /// <summary>
        /// 语言标签
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取翻译数据,以小写开头", @"")]
        public string Lang_lower(string key)
        {
            var lang = this._ctx.UserLanguage;
            var s = Cms.Language.Get(lang, key);
            if (s != null) return s.ToLower();
            return "# missing lang:" + key;
        }


        /// <summary>
        /// 重定向文档
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Archive_Redirect(string id)
        {
            var a = LocalService.Instance.ArchiveService.GetArchiveByPath(_site.SiteId, id);

            if (a.Id > 0)
            {
                var response = this._context.Response;
                var appPath = Cms.Context.SiteAppPath;
                if (appPath != "/") appPath += "/";

                response.StatusCode(301);
                response.AddHeader("Location",
                    $"{appPath}{a.Category.Path}/{(string.IsNullOrEmpty(a.Alias) ? a.StrId : a.Alias)}.html");
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
            var a = LocalService.Instance.ArchiveService.GetArchiveByPath(SiteId, idOrAlias);
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
            if (archive.Id <= 0) return TplMessage("请先使用标签$require('id')获取文档后再调用属性");

            var p = Array.Find(archivePros,
                a => string.Compare(a.Name, field, StringComparison.OrdinalIgnoreCase) == 0);
            if (p != null) return (p.GetValue(archive, null) ?? "").ToString();

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
            var archive = (ArchiveDto)(Cms.Context.Items["archive"] ?? default(ArchiveDto));

            const string cmEditorTpl = @"<!-- 提交评论 -->
            <div class=""comment_editor"">
                <form method=""POST"" action="""" style=""margin:0"" id=""cmeditor"">
                     <input name=""action"" value=""comment"" type=""hidden"" />
                     <input name=""ce_id"" value=""{id}"" type=""hidden"" />
                     {html}
                     <div>{nickname}</div>
                     <div>验证码：<input class=""ui-validate"" name=""ce_verify_code"" style=""width:60px"" type=""text"" id=""ce_verify_code""><img class=""verifyimg"" src=""{img}"" onclick=""this.src='{img}&rnd='+Math.random();"" alt=""验证码。看不清楚？换一个"" /></div>
                     <div>
                              <textarea id=""ce_content"" name=""ce_content"" class=""ui-validate""  rows=""3"" onfocus=""if(this.value=='请在这里输入评论内容')this.value='';"" onblur=""if(this.value=='')this.value='请在这里输入评论内容';"">请在这里输入评论内容</textarea>
                    </div>
                    <div><a href=""javascript:submitComment();"" id=""ce_submit"" class=""ce_submit"" title=""点击发表评论"">发表评论</a><span id=""ce_tip""></span></div>
                </form>
            </div>
                <script type=""text/javascript"">
                    var ce_n=jr.$('ce_nickname'),
                          ce_v=jr.$('ce_verify_code'),
                          ce_vl=ce_v.nextSibling,
                          ce_c=jr.$('ce_content'),
                          ce_t=jr.$('ce_tip'),
                          ce_s=jr.$('ce_submit');

                        jr.lazyRun(function(){
                            if(!jr.form)jr.ld('form');
                            if(!jr.validator)jr.ld('validate')
                        });

                        jr.lazyRun(function(){
                            if(ce_n){
                                ce_n.value = jr.cookie.read('viewname');
                                ce_n.onblur=function(){
                                     if(ce_n.value=='' || ce_n.value.length>10){
                                          jr.validator.setTip(ce_n,false,'','昵称长度为1-10个字符!');
                                     }else{
                                          jr.validator.removeTip(ce_n);
                                          jr.cookie.write('viewname', this.value, 60 * 60 * 24 * 3);
                                     }
                                  };
                            }
                            ce_c.onblur=function(){
                                     if(ce_c.value==''){
                                          jr.validator.setTip(ce_c,false,'','请输入评论内容!');
                                     }else if(ce_c.value.length>200){
                                          jr.validator.setTip(ce_c,false,'','评论内容必须少于200字!');
                                     }
                                     else{
                                          jr.validator.removeTip(ce_c);
                                     }
                              };
                            ce_v.onblur=function(){
                                     if(ce_v.value==''){
                                          jr.validator.setTip(ce_vl,false,'','请输入验证码!');
                                     }
                                     else{
                                          jr.validator.removeTip(ce_vl);
                                     }
                              };

                   });
                   function submitComment(){
                        jr.lazyRun(function(){
                            if(jr.validator.validate('cmeditor')){
                                 ce_tip(true,'提交中...');
                                 jr.form.asyncSubmit('cmeditor');
                            }
                        });
                   }
                  function ce_tip(b,m){ce_t.innerHTML=(b?'<span style=\'color:green\'':'<span style=\'color:red\'')+'>'+m+'</span>';if(!b){ce_s.removeAttribute('disabled','');}else{ ce_s.setAttribute('disabled','disabled');}}
                   function clientCall(script){if(script)eval(script);}

                </script>";

            var amouSubmit = IsTrue(allowAmous);

            if (archive.Id <= 0) return TplMessage("请先使用标签$require('id')获取文档后再调用属性");


            var sb = new StringBuilder();

            var content = TplEngine.ResolveHolderFields(cmEditorTpl, a =>
            {
                switch (a)
                {
                    case "id":
                        return archive.StrId;
                    case "html":
                        return html;
                    case "img":
                        return FormatPageUrl(UrlRulePageKeys.Common,
                            new[]
                            {
                                CmsVariables.DEFAULT_CONTROLLER_NAME + "/verifyimg?length=4&opt=1"
                            });

                    case "nickname":
                        var member = UserState.Member.Current;
                        if (member == null)
                        {
                            if (amouSubmit)
                                return
                                    @"昵称：<input class=""ui-validate"" name=""ce_nickname"" type=""text"" id=""ce_nickname"">";
                            else
                                return @"昵称：<span style=""color:red"">不允许匿名评论，请先登录后继续操作！</span>";
                        }
                        else
                        {
                            return $"昵称：{member.Nickname}";
                        }
                }

                return string.Empty;
            });
            return content;
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

            var archive = Cms.Context.Items["archive"] == null
                ? default(ArchiveDto)
                : (ArchiveDto)Cms.Context.Items["archive"];
            if (archive.Id <= 0)
                throw new ArgumentNullException("archive", "不允许在当前页中使用!");

            var sb = new StringBuilder();

            //获取评论列表
            var dt = CmsLogic.Comment.GetArchiveComments(archive.StrId, !IsTrue(asc));

            replayCount = dt.Rows.Count;
            int memberId;
            string nickname, content;
            Match match;
            Member member = null;

            //如无评论，则提示
            if (replayCount == 0) return "<p style=\"text-align:center;\" class=\"noreplay\">暂无评论</p>";

            //迭代获取评论信息
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                content = dt.Rows[i]["content"].ToString();
                memberId = int.Parse(dt.Rows[i]["memberid"].ToString());

                if (memberId > 0)
                {
                    //会员
                    member = CmsLogic.Member.GetMember(memberId);
                    if (member == null)
                        nickname = "已删除会员";
                    else
                        nickname = member.Nickname ?? member.Username;
                }
                else
                {
                    //游客
                    match = Regex.Match(content, "\\(u:'(?<user>.+?)'\\)");

                    if (match != null)
                    {
                        nickname = match.Groups["user"].Value;
                        content = Regex.Replace(content, "\\(u:'(.+?)'\\)", string.Empty);
                    }
                    else
                    {
                        nickname = "游客";
                    }
                }

                sb.Append(TplEngine.ResolveHolderFields(format, field =>
                {
                    switch (field)
                    {
                        default: return string.Empty;

                        //会员昵称
                        case "nickname": return nickname;


                        //
                        //UNDONE:未对会员设置自定义链接
                        //

                        //会员链接
                        case "url":
                            return memberId == 0
                                ? "javascript:;"
                                :
                                // (Settings.TPL_UseFullPath ? Settings.SYS_DOMAIN : String.GetCssClass)+
                                string.Format("/member/{0}", memberId.ToString());

                        //会员编号
                        case "mid":
                        case "memberid": return memberId.ToString();

                        //评论编号
                        case "id": return dt.Rows[i]["id"].ToString();

                        //索引，从1开始
                        case "index": return (i + 1).ToString();

                        //样式
                        case "class":
                            return GetCssClass(replayCount, i, "cm", null);

                        //评论时间
                        case "date":
                        case "create_time":
                        case "publish_time":
                        case "create_date":
                            return string.Format("{0:yyyy-MM-dd HH:mm:ss}", dt.Rows[i]["create_time"]);

                        //评论内容
                        case "content":
                            return content;

                        //会员头像
                        case "avatar":
                            return member == null ? "/uploads/avatar/nopic.gif" : member.Avatar;
                    }
                }));
            }
            return sb.ToString();
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

            var sb = new StringBuilder();
            var tb = CmsLogic.Table.GetTable(int.Parse(formId));
            if (tb == null) return "Form not exists!";

            var tableId = tb.Id.ToString();
            var token = string.Empty.RandomLetters(16); //验证token

            //存入SESSION
            this._context.Session.SetString($"cms_form_{tb.Id.ToString()}_token", token);

            //构造HTML
            if (!string.IsNullOrEmpty(tb.Note)) sb.Append("<div class=\"fnote\">").Append(tb.Note).Append("</div>");

            sb.Append("<form class=\"customform\" id=\"cms_form_").Append(tb.Id.ToString()).Append("\">");

            var columns = CmsLogic.Table.GetColumns(tb.Id);
            if (columns != null)
                foreach (var c in columns)
                {
                    sb.Append("<div class=\"item\"><div class=\"fn\" style=\"float:left\">").Append(c.Name)
                        .Append(":</div>")
                        .Append("<div class=\"fi\"><input type=\"text\" class=\"ui-validate\" field=\"field_")
                        .Append(tableId).Append("_").Append(c.Id.ToString())
                        .Append("\" id=\"field_").Append(tableId).Append("_").Append(c.Id.ToString()).Append("\"");

                    if (!string.IsNullOrEmpty(c.ValidFormat)) sb.Append(" ").Append(c.ValidFormat);

                    sb.Append("/></div>");

                    if (!string.IsNullOrEmpty(c.Note))
                        sb.Append("<span class=\"fnote\">").Append(c.Note)
                            .Append("</span>");

                    sb.Append("</div>");
                }

            sb.Append("<div class=\"fbtn\"><input type=\"button\" id=\"cms_form_")
                .Append(tableId).Append("_btn\" value=\"提交\"/><span id=\"cms_form_")
                .Append(tableId).Append("_summary").Append("\"></span></div></form><script type=\"text/javascript\">")
                .Append("if(!window.cms){alert('缺少cms核心脚本库!');}else{")
                .Append("jr.lazyRun(function(){ if(!jr.form)jr.ld('form');if(!jr.validator)jr.ld('validate'); });")
                .Append("document.getElementById('cms_form_").Append(tb.Id.ToString())
                .Append("_btn').onclick=function(){")
                .Append("jr.lazyRun(function(){")
                .Append("var cfs=jr.$('cms_form_").Append(tableId).Append("_summary');")
                .Append("if(jr.validator.validate('cms_form_").Append(tableId)
                .Append("')){cfs.innerHTML='提交中...';jr.xhr.post('")
                .Append(FormatPageUrl(UrlRulePageKeys.Common,
                    new[] { CmsVariables.DEFAULT_CONTROLLER_NAME + "/submitform?table_id=" }))
                .Append(tableId).Append("&token=").Append(token).Append("',jr.json.toObject('cms_form_")
                .Append(tableId).Append(
                    "'),function(r){var result;eval('result='+r);cfs.innerHTML=result.tag==-1?'<span style=\"color:red\">'+result.message+'</span>':result.message;},function(){cfs.innerHTML='<span style=\"color:red\">提交失败，请重试!</span>';});")
                .Append("}});};}</script>");

            return sb.ToString();
        }

        /// <summary>
        /// 根据栏目Tag产生站点地图
        /// </summary>
        /// <param name="catPath"></param>
        /// <returns></returns>
        [TemplateTag]
        [ContainSetting]
        protected string Sitemap(string catPath)
        {
            var cacheKey = string.Format("{0}_site{1}_sitemap_{2}", CacheSign.Category, SiteId.ToString(), catPath);
            return Cms.Cache.GetCachedResult(cacheKey, () =>
            {
                return CategoryCacheManager.GetSitemapHtml(SiteId,
                    catPath, GetSetting().CFG_SitemapSplit,
                    FormatPageUrl(UrlRulePageKeys.Category, null));
            }, DateTime.Now.AddHours(Settings.OptiDefaultCacheHours));
        }

        /// <summary>
        /// 分页控件
        /// </summary>
        /// <returns></returns>
        [TemplateTag]
        protected string Pager()
        {
            return (Cms.Context.Items[PopPagerKey()] ?? "") as string;
        }

        /// <summary>
        /// 流量统计
        /// </summary>
        /// <returns></returns>
        [TemplateTag]
        [ContainSetting]
        protected string Traffic()
        {
            var ipaddress = WebCtx.Current.UserIpAddress;
            TrafficCounter.Record(ipaddress);

            var format = GetSetting().CFG_TrafficFormat;

            var result = TplEngine.ResolveHolderFields(format, key =>
            {
                switch (key)
                {
                    case "todayip": return TrafficCounter.GetTodayIPAmount().ToString();
                    case "todaypv": return TrafficCounter.GetTodayPVAmount().ToString();
                    case "totalip": return TrafficCounter.GetTotalIPAmount().ToString();
                    case "totalpv": return TrafficCounter.GetTotalPVAmount().ToString();
                }

                return string.Empty;
            });

            return result;
        }


        /// <summary>
        /// 导航,使用$nav=1来设置高亮的索引
        /// </summary>
        /// <param name="childFormat"></param>
        /// <param name="index">选中索引</param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Navigator(string format, string childFormat, string index)
        {
            const string tpl =
                @"<div class=""mod-navigator navigator"">
                    <div class=""left mod-navigator-left navigator__left""></div>
                    <div class=""right mod-navigator-right navigator__right""></div>
                    <div class=""nav mod-navigator-nav navigator__nav flex"">
                        {0}
                        <div class=""clearfix""></div>
                    </div>
                </div>";

            var sb = new StringBuilder();
            IList<SiteLinkDto> links = new List<SiteLinkDto>(
                LocalService.Instance.SiteService.GetLinksByType(SiteId, SiteLinkType.Navigation, false));
            var total = links.Count;

            int navIndex;
            if (index == ""){
                navIndex = -1; //如果为空，则默认不选中
            }
            else
            {
                int.TryParse(index, out navIndex);
            }
            var j = 0;
            string tempLinkStr;


            LinkGenerateGBehavior bh = (int genTotal, ref int current, string levelCls, int selected, bool child, SiteLinkDto link,
                    int childCount) =>
                {
                    var sb2 = new StringBuilder();
                    /* *********************
                     *  辨别选中的导航
                     * *********************/
                    var clsName = levelCls;
                    if (childCount != 0) clsName = string.Concat(clsName, " navigator__parent parent");
                    if (selected == current) clsName = string.Concat(clsName, " navigator__active current");
                    if (current == 0) clsName = string.Concat(clsName, " navigator__first first");
                    if (current == genTotal - 1) clsName = string.Concat(clsName, " navigator__last last");
                    sb2.Append("<div class=\"navigator__item " + clsName + "\">");
                    //解析格式
                    tempLinkStr = TplEngine.ResolveHolderFields(child ? childFormat : format, a =>
                    {
                        switch (a)
                        {
                            case "url":
                                return this.ConcatUrl(string.IsNullOrEmpty(link.Bind)
                                    ? link.Uri : GetBingLinkUrl(link.Bind));
                            case "target": return string.IsNullOrEmpty(link.Target) ? "_self" : link.Target;
                            case "text": return link.Text;
                            case "img_url": return link.ImgUrl;
                        }

                        return "{" + a + "}";
                    });
                    //添加链接目标
                    if (!string.IsNullOrEmpty(link.Target))
                        tempLinkStr = tempLinkStr.Replace("<a ", "<a target=\"" + link.Target + "\" ");

                    sb2.Append(tempLinkStr).Append("</div>");
                    return sb2.ToString();
                };

            for (var i = 0; i < links.Count; i++)
            {
                if (links[i].Pid == 0)
                {
                    j = i;
                    IList<SiteLinkDto> children = new List<SiteLinkDto>(links.Where(a => a.Pid == links[i].Id));
                    var parentHtml = bh(total, ref j, "l1", navIndex, false, links[i], children.Count);
                    var secondTotal = 0;
                    if ((secondTotal = children.Count) != 0)
                    {
                        parentHtml = parentHtml.Replace("</a></div>",
                            string.Format(
                                @"<i class=""navigator__item--arrow nav-arrow""/></a>
                                <div id=""{0}_child{1}"" class=""navigator__child mod-navigator-child child child{1}"">
                                <div class=""navigator__child--top top""></div>
                                <div class=""navigator__child--box box"">
                                <div class=""nagigator__child--menu menu"">",
                                links[i].Type.ToString().ToLower(),
                                links[i].Id.ToString()));
                        var secondCurrent = 0;
                        foreach (var t in children)
                        {
                            parentHtml += bh(secondTotal, ref secondCurrent, "l2", -1, true, t, 0);
                            secondCurrent++;
                        }

                        parentHtml += "</div></div></div></div>";
                    }

                    sb.Append(parentHtml);
                }
            }
            return string.Format(tpl, sb);
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
                archiveDto = LocalService.Instance.ArchiveService.GetArchiveById(_siteId, Convert.ToInt32(idOrAlias));
            else
                archiveDto = LocalService.Instance.ArchiveService.GetArchiveByPath(_siteId, idOrAlias.ToString());

            if (archiveDto.Id <= 0) return TplMessage(string.Format("不存在编号（或别名）为:{0}的文档!", idOrAlias));


            var sb = new StringBuilder(500);
            ResolveArchivePlaceHolder(sb, archiveDto, ref format, null, -1);
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
            var archiveDto =
                LocalService.Instance.ArchiveService.GetSameCategoryPreviousArchive(SiteId, int.Parse(id));
            if (!(archiveDto.Id > 0)) return Cms.Language.Get(LanguagePackageKey.ARCHIVE_NoPrevious);

            var sb = new StringBuilder(500);
            ResolveArchivePlaceHolder(sb, archiveDto, ref format, null, -1);
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
            var archiveDto =
                LocalService.Instance.ArchiveService.GetSameCategoryNextArchive(SiteId, int.Parse(id));

            if (!(archiveDto.Id > 0)) return Cms.Language.Get(LanguagePackageKey.ARCHIVE_NoNext);

            var sb = new StringBuilder(500);
            ResolveArchivePlaceHolder(sb, archiveDto, ref format, null, -1);
            return sb.ToString();
        }

        /// <summary>
        /// 栏目链接列表
        /// </summary>
        /// <param name="catPath"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        protected string CategoryList(string catPath, string format)
        {
            IList<CategoryDto> categories = new List<CategoryDto>();

            if (string.IsNullOrEmpty(catPath)) return TplMessage("请指定参数:param的值");

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

            var isModule = false;
            if (!isModule)
            {
                categories = new List<CategoryDto>(LocalService.Instance.SiteService
                    .GetCategories(SiteId, catPath));
            }

            //如果没有下级了,则获取当前级
            //if (categories.Count == 0)
            //{
            //    if (nullSameLevel) //是否使用同级
            //    {
            //        categories = new List<CategoryDto>(ServiceCall.Instance.SiteService.GetCategories(this.siteId,
            //            category.Lft, category.Rgt, CategoryContainerOption.SameLevel));
            //    }
            //}

            var total = categories.Count;
            if (total == 0) return string.Empty;

            var sb = new StringBuilder(400);
            var i = 0;

            foreach (var c in categories.OrderBy(a => a.SortNumber))
            {
                if (c.SiteId == _site.SiteId)
                    sb.Append(TplEngine.ResolveHolderFields(format, field =>
                    {
                        switch (field)
                        {
                            default: return string.Empty;

                            case "name": return c.Name;
                            case "url": return GetCategoryUrl(c, 1);
                            case "tag": return c.Tag;
                            case "path": return c.Path;
                            case "thumbnail":
                            case "icon": return GetThumbnailUrl(c.Icon);
                            case "id": return c.ID.ToString();

                            //case "pid":  return c.PID.ToString();

                            case "description": return c.Description;
                            case "keywords": return c.Keywords;
                            case "index": return (i + 1).ToString();
                            case "class":
                                return GetCssClass(total, i, "c", c.Icon);
                        }
                    }));

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
            var sb = new StringBuilder();
            var listContainer = format.EndsWith("</li>");
            var tmpInt = 0;
            var intTotal = archives.Length;
            foreach (var archiveDto in archives)
            {
                ResolveArchivePlaceHolder(sb, archiveDto, ref format, archives, tmpInt++);
                AppendSplitHtm(sb, intTotal, tmpInt, splitSize, listContainer);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 格式化文档占位符
        /// 
        /// 时间：time_fmt[YYYY-MM]
        /// 大纲：outline[300]
        /// 语言: lang[contact]
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="archiveDto"></param>
        /// <param name="format"></param>
        /// <param name="dt"></param>
        /// <param name="index"></param>
        private void ResolveArchivePlaceHolder(StringBuilder sb, ArchiveDto archiveDto, ref string format,
            IEnumerable<ArchiveDto> dt, int index)
        {
            var id = string.IsNullOrEmpty(archiveDto.Alias) ? archiveDto.StrId : archiveDto.Alias;

            //读取自定义扩展数据字段
            IDictionary<string, string> extendFields = null;

            sb.Append(TplEngine.ResolveHolderFields(format,
                field =>
                {
                    switch (field)
                    {
                        case "id": return archiveDto.Id.ToString();
                        case "str_id": return id; //用于链接的ID标识
                        case "raw_title": return archiveDto.Title; //原标题
                        case "small_title": return archiveDto.SmallTitle;
                        case "title":
                            return !FlagAnd(archiveDto.Flag, BuiltInArchiveFlags.IsSpecial)
                                ? archiveDto.Title
                                : "<span class=\"special\">" + archiveDto.Title + "</span>";
                        case "author_id": return archiveDto.PublisherId.ToString();

                        //
                        //TODO:Archive应持有一个author
                        //
                        //case "author_name": return ArchiveUtility.GetAuthorName(dr["author"].ToString());
                        case "source": return archiveDto.Source == "" ? "原创" : archiveDto.Source;
                        case "outline":
                            return ArchiveUtility.GetOutline(
                                string.IsNullOrEmpty(archiveDto.Outline) ? archiveDto.Content : archiveDto.Outline,
                                GetSetting().CfgOutlineLength);
                        case "tags": return archiveDto.Tags;
                        case "replay": return CmsLogic.Comment.GetArchiveCommentsCount(archiveDto.StrId).ToString();
                        case "count": return archiveDto.ViewCount.ToString();

                        //时间
                        case "edit_time":
                        case "modify_time": return $"{archiveDto.UpdateTime:yyyy-MM-dd HH:mm}";
                        case "edit_date":
                        case "modify_date": return $"{archiveDto.UpdateTime:yyyy-MM-dd}";
                        case "publish_short_date": return $"{archiveDto.CreateTime:MM-dd}";
                        case "publish_month": return string.Format("{0:MM}", archiveDto.CreateTime);
                        case "publish_day": return string.Format("{0:dd}", archiveDto.CreateTime);

                        case "publish_time":
                        case "create_time": return $"{archiveDto.CreateTime:yyyy-MM-dd HH:mm}";
                        case "publish_date":
                        case "create_date": return $"{archiveDto.CreateTime:yyyy-MM-dd}";

                        //栏目
                        // case "categoryid":
                        // case "cid": return archive.Category.ID.ToString();
                        case "category_name": return archiveDto.Category.Name;
                        case "category_path": return archiveDto.Category.Path;
                        case "category_url": return GetCategoryUrl(archiveDto.Category, 1);



                        // 路径，用于调用接口
                        case "path":
                            return archiveDto.Path;
                        //链接
                        case "url":
                            return GetArchiveUrl(archiveDto.Location, archiveDto.Flag, archiveDto.Path);

                        //内容
                        case "content": return archiveDto.Content;

                        //压缩过的内容
                        case "content2":
                            return Regex.Replace(archiveDto.Content, "\\r|\\t|\\s\\s", string.Empty);

                        //图片元素
                        case "img":
                            return ThumbnailTag(archiveDto.Thumbnail, archiveDto.Title, true);
                        case "img2":
                            return ThumbnailTag(archiveDto.Thumbnail, archiveDto.Title, false);
                        //缩略图
                        case "thumbnail": return GetThumbnailUrl(archiveDto.Thumbnail);

                        case "index": return (index + 1).ToString();

                        // 项目顺序类
                        case "class":
                            if (dt == null) return string.Empty;
                            return GetCssClass(dt.Count(), index, "a", archiveDto.Thumbnail);

                        //特性列表
                        case "properties":
                            var sb2 = new StringBuilder();
                            sb.Append("<ul class=\"extend_field_list mod-archive-extends\">");
                            foreach (var f in archiveDto.ExtendValues)
                                if (!string.IsNullOrEmpty(f.Value))
                                    sb.Append("<li class=\"extend_")
                                        .Append(f.Field.ToString()).Append("\"><span class=\"attrName\">")
                                        .Append(f.Field.Name).Append(":</span><span class=\"value\">")
                                        .Append(f.Value).Append("</span></li>");

                            sb2.Append("</ul>");
                            return sb2.ToString();
                        default:
                            if (field.IndexOf("[", StringComparison.Ordinal) != -1)
                            {
                                // 格式化时间
                                var p = GetArchiveHolderTagParam(field, "time_fmt[");
                                if (p != null) return string.Format("{0:" + p + "}", archiveDto.CreateTime);

                                //读取自定义长度大纲
                                p = GetArchiveHolderTagParam(field, "outline[");
                                if (p != null)
                                    return ArchiveUtility.GetOutline(
                                        string.IsNullOrEmpty(archiveDto.Outline)
                                            ? archiveDto.Content
                                            : archiveDto.Outline, int.Parse(p));

                                // 返回语言项
                                p = GetArchiveHolderTagParam(field, "lang[");
                                if (p != null) return Cms.Language.Get(_ctx.UserLanguage, p) ?? "{" + p + "}";
                            }

                            //读取自定义属性
                            if (extendFields == null)
                            {
                                extendFields = new Dictionary<string, string>();
                                foreach (var value in archiveDto.ExtendValues)
                                    extendFields.Add(value.Field.Name, value.Value);
                            }

                            // 查找自定义属性
                            if (extendFields.ContainsKey(field)) return extendFields[field];
                            return string.Empty;
                    }
                }));
        }

        /// <summary>
        /// 获取文档占位标签参数
        /// </summary>
        /// <param name="field"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        private string GetArchiveHolderTagParam(string field, string prefix)
        {
            if (field.StartsWith(prefix))
            {
                var len = prefix.Length;
                return field.Substring(len, field.Length - len - 1);
            }

            return null;
        }

        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="useDefault"></param>
        /// <returns></returns>
        private string ThumbnailTag(string url, string title, bool useDefault)
        {
            if (!useDefault && string.IsNullOrEmpty(url)) return "";
            url = GetThumbnailUrl(url);
            return string.Format("<img class=\"thumb thumbnail\" src=\"{0}\" alt=\"{1}\"/>", url, title);
        }

        /// <summary>
        /// 获取css样式
        /// </summary>
        /// <param name="total"></param>
        /// <param name="index"></param>
        /// <param name="prefix"></param>
        /// <param name="thumb"></param>
        /// <returns></returns>
        private static string GetCssClass(int total, int index, string prefix, string thumb)
        {
            if (total == 0 || index < 0) return string.Empty;
            var noThumb = thumb != null && thumb.Length == 0;
            var cls = string.Format("{0} {1}{2}{3}{4}", prefix, prefix, (index + 1).ToString(),
                (index + 1) % 2 == 0 ? " even" : "", noThumb ? "  no-thumb" : string.Empty);
            if (index == total - 1) return cls + " last";
            else if (index == 0) return cls + " first";
            return cls;
        }


        /// <summary>
        /// 分页文档列表
        /// </summary>
        /// <param name="categoryPath">栏目Tag</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="splitSize">分隔条数</param>
        /// <param name="format">格式</param>
        /// <param name="skipSize">跳过的页码</param>
        /// <returns></returns>
        protected string Paging_Archives(string categoryPath, string pageIndex, string pageSize, int skipSize,
            int splitSize, string format)
        {
            var category = LocalService.Instance.SiteService.GetCategory(SiteId, categoryPath);
            if (!(category.ID > 0)) return TplMessage("Error:栏目不存在!");

            var listContainer = format.EndsWith("</li>");

            int.TryParse(pageIndex, out var _pageIndex);
            int.TryParse(pageSize, out var _pageSize);

            /**** 显示列表 ****/
            var sb = new StringBuilder(1000);

            var archiveCategory = category;

            //读取自定义扩展数据字段
            IDictionary<int, IDictionary<string, string>> extendValues;


            int archiveIndex = 0,
                totalNum = 0;
            DataRowCollection drs;

            drs = LocalService.Instance.ArchiveService.GetPagedArchives(
                SiteId,
                categoryPath,
                _pageSize,
                skipSize,
                ref _pageIndex,
                out var _records,
                out var _pages,
                out extendValues).Rows;

            totalNum = drs.Count;

            string id = null;
            foreach (DataRow dr in drs)
            {
                ++archiveIndex;

                //获取栏目，如果栏目关联不起来则调到下一次
                var categoryId = int.Parse(dr["cat_id"].ToString());
                var archiveId = int.Parse(dr["id"].ToString());
                var archivePath = dr["path"].ToString();
                if (string.IsNullOrEmpty(archivePath))
                {
                    continue;
                }

                if (categoryId != archiveCategory.ID)
                {
                    archiveCategory = LocalService.Instance.SiteService.GetCategory(SiteId, categoryId);
                    if (!(archiveCategory.ID > 0)) continue;
                }

                var createTime = TimeUtils.UnixTime(Convert.ToInt32(dr["create_time"]), TimeZone.CurrentTimeZone);
                sb.Append(TplEngine.ResolveHolderFields(format,
                    field =>
                    {
                        switch (field)
                        {
                            case "title":
                                return !FlagAnd(Convert.ToInt32(dr["flag"]), BuiltInArchiveFlags.IsSpecial)
                                    ? dr["title"].ToString()
                                    : "<span class=\"special\">" + dr["title"] + "</span>";
                            case "raw_title": return dr["title"].ToString();
                            case "small_title": return (dr["small_title"] ?? "").ToString();
                            case "author_id": return dr["author_id"].ToString();
                            case "author_name":
                                return ArchiveUtility.GetPublisherName(Convert.ToInt32(dr["author_id"] ?? 0));
                            case "source": return dr["source"].ToString();
                            case "fmt_outline":
                                return ArchiveUtility.GetFormatedOutline(
                                    (dr["outline"] ?? "").ToString(),
                                    dr["content"].ToString(),
                                    GetSetting().CfgOutlineLength);
                            case "outline":
                                return ArchiveUtility.GetOutline(
                                    string.IsNullOrEmpty((dr["outline"] ?? "").ToString())
                                        ? dr["content"].ToString()
                                        : dr["outline"].ToString(), GetSetting().CfgOutlineLength);
                            case "int_id": return dr["id"].ToString();
                            case "id": return id;
                            case "alias": return dr["alias"].ToString();
                            case "tags": return dr["tags"].ToString();
                            case "replay":
                                return CmsLogic.Comment.GetArchiveCommentsCount(dr["id"].ToString()).ToString();
                            case "count": return dr["view_count"].ToString();

                            //时间
                            case "modify_time": return string.Format("{0:yyyy-MM-dd HH:mm}", createTime);
                            case "modify_date": return string.Format("{0:MM-dd}", dr["update_time"]);
                            case "publish_short_date": return $"{createTime:MM-dd}";
                            case "publish_month": return string.Format("{0:MM}", createTime);
                            case "publish_day": return string.Format("{0:dd}", createTime);

                            case "publish_time":
                            case "create_time": return string.Format("{0:yyyy-MM-dd HH:mm}", createTime);
                            case "publish_date":
                            case "create_date": return string.Format("{0:yyyy-MM-dd}", createTime);

                            //栏目
                            case "category_id": return archiveCategory.ID.ToString();

                            case "category_name": return archiveCategory.Name;

                            case "category_path": return archiveCategory.Path;

                            case "category_url": return GetCategoryUrl(archiveCategory, 1);

                            case "path":
                                return dr["path"].ToString();
                            //链接
                            case "url":
                                return GetArchiveUrl(dr["location"].ToString(), Convert.ToInt32(dr["flag"]),
                                    archivePath);

                            //内容
                            case "content": return dr["content"].ToString();

                            //图片元素
                            case "img":
                                return ThumbnailTag(dr["thumbnail"].ToString(), dr["title"].ToString(), true);
                            case "img2": return ThumbnailTag(dr["thumbnail"].ToString(), dr["title"].ToString(), false);

                            //缩略图
                            case "thumbnail": return GetThumbnailUrl(dr["thumbnail"].ToString());

                            // 项目顺序类
                            case "class":
                                return GetCssClass(totalNum, archiveIndex - 1, "a", dr["thumbnail"].ToString());
                            case "index":
                                return archiveIndex.ToString();

                            default:
                                if (field.IndexOf("[") != -1)
                                {
                                    // 格式化时间
                                    var p = GetArchiveHolderTagParam(field, "time_fmt[");
                                    if (p != null) return string.Format("{0:" + p + "}", createTime);

                                    //读取自定义长度大纲
                                    p = GetArchiveHolderTagParam(field, "outline[");
                                    if (p != null)
                                        return ArchiveUtility.GetOutline(
                                            string.IsNullOrEmpty(dr["outline"].ToString())
                                                ? dr["content"].ToString()
                                                : dr["outline"].ToString(), int.Parse(p));
                                    // 返回语言项
                                    p = GetArchiveHolderTagParam(field, "lang[");
                                    if (p != null) return Cms.Language.Get(_ctx.UserLanguage, p) ?? "{" + p + "}";
                                }

                                if (extendValues.ContainsKey(archiveId) && extendValues[archiveId].ContainsKey(field))
                                {
                                    return extendValues[archiveId][field];
                                }
                                return "{" + field + "}";
                        }
                    }));


                // 添加分割栏
                AppendSplitHtm(sb, totalNum, archiveIndex, splitSize, listContainer);
            }

            //设置分页
            SetPager(
                _pageIndex,
                _pages,
                _records,
                GetCategoryUrl(category, 1),
                GetCategoryUrl(category, 99).Replace("99", "{0}")
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
                var isEven = index / splitSize % 2 == 0;
                if (listContainer)
                {
                    if (isEven)
                        sb.Append("<li class=\"break break-even\">").Append(splitHtmEven);
                    else
                        sb.Append("<li class=\"break\">").Append(splitHtm);

                    sb.Append("</li>");
                }
                else
                {
                    sb.Append(isEven ? splitHtmEven : splitHtm);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="catPath"></param>
        /// <param name="num"></param>
        /// <param name="skipSize"></param>
        /// <param name="splitSize"></param>
        /// <param name="container"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        protected string Special_Archives(string catPath, string num, int skipSize, int splitSize, bool container,
            string format)
        {
            int.TryParse(num, out var intNum);
            //获取栏目
            var category = LocalService.Instance.SiteService.GetCategory(SiteId, catPath);

            if (!(category.ID > 0)) return $"ERROR:模块或栏目不存在!参数:{catPath}";
            var dt = LocalService.Instance.ArchiveService.GetSpecialArchives(
                SiteId, category.Path, container, intNum, skipSize);
            return ArchiveList(dt, splitSize, format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="catPath"></param>
        /// <param name="num"></param>
        /// <param name="skipSize"></param>
        /// <param name="splitSize"></param>
        /// <param name="container"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        protected string Archives(string catPath, string num, int skipSize, int splitSize, bool container,
            string format)
        {
            int.TryParse(num, out var intNum);

            //栏目
            var category = LocalService.Instance.SiteService.GetCategory(SiteId, catPath);

            if (!(category.ID > 0)) return $"ERROR:模块或栏目不存在!参数:{catPath}";

            var archives = LocalService.Instance.ArchiveService
                .GetArchivesByCategoryPath(SiteId, category.Path, container, intNum, skipSize);

            return ArchiveList(archives, splitSize, format);
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

            var C_LENGTH = GetSetting().CfgOutlineLength;
            int intSplitSize;
            var hasSetCategory = false; //是否在搜索中指定栏目参数

            var category = default(CategoryDto);
            Module module = null;
            Regex keyRegex = null;

            int.TryParse(pageIndex, out intPageIndex);
            int.TryParse(pageSize, out intPageSize);
            int.TryParse(splitSize, out intSplitSize);

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Replace("+", string.Empty); //删除+号连接符
                keyRegex = new Regex(keyword, RegexOptions.IgnoreCase);
            }

            var listContainer = format.EndsWith("</li>");

            /**** 显示列表 ****/
            var sb = new StringBuilder(1000);


            string alias,
                title,
                content,
                title_hightlight;


            var i = 0;

            IEnumerable<ArchiveDto> searchArchives = null;

            if (!string.IsNullOrEmpty(categoryTagOrModuleId))
                //按模块搜索
                //if (Regex.IsMatch(categoryTagOrModuleID, "^\\d+$"))
                //{
                //    module = CmsLogic.Module.GetModule(int.Parse(categoryTagOrModuleID));
                //    if (module != null)
                //    {
                //        searchArchives = CmsLogic.Archive.SearchByModule(keyword, int.Parse(categoryTagOrModuleID), _pageSize, _pageIndex, out _records, out _pages, "ORDER BY create_time DESC").Rows;
                //    }
                //}

                //如果模块不存在，则按栏目搜索
                if (searchArchives == null)
                {
                    category = LocalService.Instance.SiteService.GetCategory(siteId, categoryTagOrModuleId);
                    if (category.ID > 0)
                    {
                        hasSetCategory = true;
                        searchArchives = LocalService.Instance.ArchiveService.SearchArchivesByCategory(siteId,
                            category.Path, keyword, intPageSize, intPageIndex, out total, out pages,
                            "ORDER BY create_time DESC");
                    }
                    else
                    {
                        pageCount = 0;
                        recordCount = 0;
                        return TplMessage("ERROR:栏目不存在!");
                    }
                }

            //如果未设置模块或栏目参数
            if (searchArchives == null)
                searchArchives = LocalService.Instance.ArchiveService.SearchArchives(siteId, "", false,
                    keyword, intPageSize, intPageIndex, out total, out pages, "ORDER BY create_time DESC");

            IDictionary<string, string> extendFields = null;


            foreach (var archive in searchArchives)
            {
                //if (!hasSetCategory)
                //{
                //    //获取栏目，如果栏目关联不起来则调到下一次
                //    category = ServiceCall.Instance.SiteService.GetCategory(this.siteId, archive.Category.ID);
                //    if (!(category.ID > 0)) continue;
                //}

                category = archive.Category;

                #region 处理关键词

                alias = string.IsNullOrEmpty(archive.Alias) ? archive.StrId : archive.Alias;
                title = title_hightlight = archive.Title;
                content = RegexHelper.FilterHtml(archive.Content);

                if (keyRegex != null && keyRegex.IsMatch(title_hightlight))
                    title_hightlight = keyRegex.Replace(title_hightlight, "<span class=\"high-light\">$0</span>");

                //关键词前数字索引算法
                var contentLength = content.Length;
                var firstSplitIndex = contentLength % 11;
                ;

                if (keyRegex != null && keyRegex.IsMatch(content))
                {
                    var match = keyRegex.Match(content);
                    if (contentLength > C_LENGTH)
                    {
                        if (match.Index > firstSplitIndex)
                        {
                            //如果截取包含关键词的长度仍大于内容长度时
                            if (contentLength - match.Index > C_LENGTH)
                                content = content.Substring(match.Index - firstSplitIndex, C_LENGTH - firstSplitIndex);
                            else
                                content = content.Substring(match.Index - firstSplitIndex);
                        }
                        else
                        {
                            content = content.Remove(C_LENGTH);
                        }
                    }

                    content = keyRegex.Replace(content, "<span class=\"high-light\">$0</span>") + "...";
                }
                else
                {
                    if (contentLength > C_LENGTH) content = content.Substring(0, C_LENGTH) + "...";
                }

                #endregion


                sb.Append(TplEngine.ResolveHolderFields(format,
                    field =>
                    {
                        switch (field)
                        {
                            case "id": return archive.Id.ToString();
                            case "str_id": return alias;
                            case "small_title": return archive.SmallTitle;
                            case "raw_title": return archive.Title;
                            case "title": return title_hightlight;
                            case "author_id": return archive.PublisherId.ToString();
                            case "author_name": return ArchiveUtility.GetPublisherName(archive.PublisherId);
                            case "source": return archive.Source;
                            case "outline": return content;
                            case "tags": return archive.Tags;
                            case "replay": return CmsLogic.Comment.GetArchiveCommentsCount(archive.StrId).ToString();
                            case "count": return archive.ViewCount.ToString();

                            //时间
                            case "modify_time": return string.Format("{0:yyyy-MM-dd HH:mm:ss}", archive.UpdateTime);
                            case "modify_date": return string.Format("{0:yyyy-MM-dd}", archive.UpdateTime);
                            case "create_time": return string.Format("{0:yyyy-MM-dd HH:mm:ss}", archive.CreateTime);
                            case "create_date": return string.Format("{0:yyyy-MM-dd}", archive.CreateTime);
                            case "publish_short_date": return $"{archive.CreateTime:MM-dd}";
                            case "publish_month": return string.Format("{0:MM}", archive.CreateTime);
                            case "publish_day": return string.Format("{0:dd}", archive.CreateTime);
                            //栏目
                            case "category_name": return category.Name;
                            case "category_path": return category.Path;
                            case "category_url": return GetCategoryUrl(category, 1);
                            //链接
                            case "url":
                                return GetArchiveUrl(archive.Location, archive.Flag, archive.Path);

                            //内容
                            case "content": return archive.Content;

                            //图片元素
                            case "img":
                                return ThumbnailTag(archive.Thumbnail, archive.Title, true);
                            case "img2":
                                return ThumbnailTag(archive.Thumbnail, archive.Title, false);

                            //缩略图
                            case "thumbnail": return GetThumbnailUrl(archive.Thumbnail);

                            // 项目顺序类
                            case "class":
                                return GetCssClass(total, i, "a", archive.Thumbnail);

                            default:
                                if (field.IndexOf("[") != -1)
                                {
                                    // 格式化时间
                                    var p = GetArchiveHolderTagParam(field, "time_fmt[");
                                    if (p != null) return string.Format("{0:" + p + "}", archive.CreateTime);

                                    //读取自定义长度大纲
                                    p = GetArchiveHolderTagParam(field, "outline[");
                                    if (p != null)
                                        return ArchiveUtility.GetOutline(
                                            string.IsNullOrEmpty(archive.Outline) ? archive.Content : archive.Outline,
                                            int.Parse(p));

                                    // 返回语言项
                                    p = GetArchiveHolderTagParam(field, "lang[");
                                    if (p != null) return Cms.Language.Get(_ctx.UserLanguage, p) ?? "{" + p + "}";
                                }

                                //读取自定义属性
                                if (extendFields == null)
                                {
                                    extendFields = new Dictionary<string, string>();
                                    foreach (var value in archive.ExtendValues)
                                        extendFields.Add(value.Field.Name, value.Value);
                                }

                                if (extendFields.ContainsKey(field)) return extendFields[field];
                                return "{" + field + "}";
                        }
                    }));


                // 添加分割栏
                AppendSplitHtm(sb, total, i++, intSplitSize, listContainer);
                ++i;
            }


            pageCount = pages;
            recordCount = total;

            //如果无搜索结果
            if (sb.Length < 30)
            {
                string message;
                switch (_ctx.UserLanguage)
                {
                    default:
                    case Languages.en_US:
                        message = "No result";
                        break;
                    case Languages.zh_CN:
                        message = "没有相关记录";
                        break;
                }

                sb.Append(string.Format("<div class=\"noresult\">{0}</div>", message));
            }
            else
            {
                //设置分页
                SetPager(
                    intPageIndex,
                    pages,
                    total,
                    FormatPageUrl(UrlRulePageKeys.Search, new[]
                    {
                        HttpUtils.UrlEncode(keyword),
                        categoryTagOrModuleId ?? ""
                    }),
                    FormatPageUrl(UrlRulePageKeys.SearchPager, new[]
                    {
                        HttpUtils.UrlEncode(keyword),
                        categoryTagOrModuleId ?? "", "{0}"
                    })
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
            var _type = 2;
            int.TryParse(type, out _type);
            var linkType = (SiteLinkType)_type;

            var sb = new StringBuilder();

            IList<SiteLinkDto> links = new List<SiteLinkDto>(
                LocalService.Instance.SiteService
                    .GetLinksByType(SiteId, linkType, false));

            SiteLinkDto link;

            bool isLast;
            int navIndex;
            if (index == "")
                navIndex = -1; //如果为空，则默认不选中
            else
                int.TryParse(index, out navIndex);

            if (number < 1) number = links.Count;
            else if (number > links.Count) number = links.Count;

            for (var i = 0; i < number; i++)
            {
                link = links[i];

                /* *********************
                 *  辨别选中的导航
                 * *********************/
                isLast = i == links.Count - 1;

                sb.Append(TplEngine.ResolveHolderFields(format, field =>
                {
                    switch (field)
                    {
                        default: return string.Empty;
                        case "text": return link.Text;
                        case "img_url": return link.ImgUrl;
                        case "img": return ThumbnailTag(link.ImgUrl, link.Text, true);
                        case "url": return string.IsNullOrEmpty(link.Bind) ? link.Uri : GetBingLinkUrl(link.Bind);
                        case "target": return string.IsNullOrEmpty(link.Target) ? "_self" : link.Target;
                        case "id": return link.Id.ToString();
                        case "class":
                            if (navIndex == i)
                                return i == 0
                                    ? " class=\"current first\""
                                    : !isLast
                                        ? " class=\"current\""
                                        : " class=\"current last\"";
                            else
                                return i == 0 ? " class=\"first\"" : !isLast ? string.Empty : " class=\"last\"";
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
            if (string.IsNullOrEmpty(tags)) return string.Empty;

            var sb = new StringBuilder();
            var tagArr = tags.Split(',');
            var i = tagArr.Length;

            if (i > 0) sb.Append(Cms.Language.Get(LanguagePackageKey.ARCHIVE_Tags)).Append("：");

            foreach (var tag in tagArr)
            {
                sb.Append(TplEngine.ResolveHolderFields(format, a =>
                {
                    switch (a)
                    {
                        case "tagName":
                        case "name":
                            return tag;

                        case "eTagName":
                        case "ename":
                        case "urlname":
                            return HttpUtils.UrlEncode(tag);

                        //搜索页URL
                        case "searchurl":
                            return FormatPageUrl(UrlRulePageKeys.Search, new[] { HttpUtils.UrlEncode(tag), string.Empty });

                        //Tag页URL
                        case "url": return FormatPageUrl(UrlRulePageKeys.Tag, new[] { HttpUtils.UrlEncode(tag) });
                    }

                    return tag;
                }));

                if (--i != 0) sb.Append(",");
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

            tag = tag.Replace("+", string.Empty); //删除+号连接符
            keyRegex = new Regex(tag, RegexOptions.IgnoreCase);

            /**** 显示列表 ****/
            var sb = new StringBuilder(1000);


            string alias;
            string content;
            string titleHightLight;


            var i = 0;

            var cLength = GetSetting().CfgOutlineLength;
            var searchArchives = LocalService.Instance.ArchiveService
                .SearchArchives(SiteId, "", false, tag,
                    _pageSize, _pageIndex,
                    out _records, out _pages, "ORDER BY create_time DESC");


            IDictionary<string, string> extendFields = null;
            var total = searchArchives.Count();


            foreach (var archive in searchArchives)
            {
                //if (!hasSetCategory)
                //{
                //    //获取栏目，如果栏目关联不起来则调到下一次
                //    category = ServiceCall.Instance.SiteService.GetCategory(this.siteId, archive.Category.ID);
                //    if (!(category.ID > 0)) continue;
                //}

                category = archive.Category;

                #region 处理关键词

                alias = string.IsNullOrEmpty(archive.Alias) ? archive.StrId : archive.Alias;
                titleHightLight = archive.Title;
                content = RegexHelper.FilterHtml(archive.Content);

                if (keyRegex.IsMatch(titleHightLight))
                    titleHightLight =
                        keyRegex.Replace(titleHightLight, "<span class=\"search-high-light\">$0</span>");

                //关键词前数字索引算法
                var contentLength = content.Length;
                var firstSplitIndex = contentLength % 11;

                if (keyRegex.IsMatch(content))
                {
                    var match = keyRegex.Match(content);
                    if (contentLength > cLength)
                    {
                        if (match.Index > firstSplitIndex)
                        {
                            //如果截取包含关键词的长度仍大于内容长度时
                            if (contentLength - match.Index > cLength)
                                content = content.Substring(match.Index - firstSplitIndex, cLength - firstSplitIndex);
                            else
                                content = content.Substring(match.Index - firstSplitIndex);
                        }
                        else
                        {
                            content = content.Remove(cLength);
                        }
                    }

                    content = keyRegex.Replace(content, "<span class=\"search-high-light\">$0</span>") + "...";
                }
                else
                {
                    if (contentLength > cLength) content = content.Substring(0, cLength) + "...";
                }

                #endregion


                var archivesCount = searchArchives.Count();
                sb.Append(TplEngine.ResolveHolderFields(format,
                    field =>
                    {
                        switch (field)
                        {
                            case "id":
                                return archive.Id.ToString();
                            case "str_id": return alias;
                            case "title": return titleHightLight;
                            case "raw_title": return archive.Title;
                            case "small_title": return archive.SmallTitle;
                            case "author_id": return archive.PublisherId.ToString();
                            case "author_name": return ArchiveUtility.GetPublisherName(archive.PublisherId);
                            case "source": return archive.Source;
                            case "outline": return content;
                            case "tags": return archive.Tags;
                            case "replay": return CmsLogic.Comment.GetArchiveCommentsCount(archive.StrId).ToString();
                            case "count": return archive.ViewCount.ToString();

                            //时间
                            case "modify_time": return string.Format("{0:yyyy-MM-dd HH:mm:ss}", archive.UpdateTime);
                            case "modify_date": return string.Format("{0:yyyy-MM-dd}", archive.UpdateTime);
                            case "create_time": return string.Format("{0:yyyy-MM-dd HH:mm:ss}", archive.CreateTime);
                            case "create_date": return string.Format("{0:yyyy-MM-dd}", archive.CreateTime);
                            case "publish_short_date": return $"{archive.CreateTime:MM-dd}";
                            case "publish_month": return string.Format("{0:MM}", archive.CreateTime);
                            case "publish_day": return string.Format("{0:dd}", archive.CreateTime);

                            //栏目
                            //case "categoryid":
                            //case "cid": return category.ID.ToString();
                            case "category_name": return category.Name;
                            case "category_tag": return category.Tag;
                            case "category_path": return category.Path;
                            case "category_url": return GetCategoryUrl(category, 1);
                            //链接
                            case "url":
                                return GetArchiveUrl(archive.Location, archive.Flag, archive.Path);

                            //内容
                            case "content": return archive.Content;

                            //图片元素
                            case "img":
                                return ThumbnailTag(archive.Thumbnail, archive.Title, true);
                            case "img2":
                                return ThumbnailTag(archive.Thumbnail, archive.Title, false);

                            //缩略图
                            case "thumbnail": return GetThumbnailUrl(archive.Thumbnail);

                            // 项目顺序类
                            case "class":
                                return GetCssClass(total, i, "a", archive.Thumbnail);

                            default:
                                //读取自定义属性
                                if (extendFields == null)
                                {
                                    extendFields = new Dictionary<string, string>();
                                    foreach (var value in archive.ExtendValues)
                                        extendFields.Add(value.Field.Name, value.Value);
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
                sb.Append(string.Format("<div class=\"noresult\">没有相关记录！</div>", tag));
            else
                //设置分页
                SetPager(
                    _pageIndex,
                    _pages,
                    _records,
                    FormatPageUrl(UrlRulePageKeys.Tag, new[] { HttpUtils.UrlEncode(tag) }),
                    FormatPageUrl(UrlRulePageKeys.TagPager, new[] { HttpUtils.UrlEncode(tag), "{0}" })
                );
            //sb.Append("</div>");


            return sb.ToString();
        }


        /*========================= 树形数据 ================================*/
        protected delegate bool CategoryResultTreeHandler(CategoryDto category);

        protected void CategoryTree_Iterator(CategoryDto category, StringBuilder sb, CategoryResultTreeHandler handler,
            bool isRoot)
        {
            /*if (category.IsSign)
            {
                category.IsSign = false;
                return;
            }
            else
            {*/
            if (!isRoot)
                sb.Append("<li><a href=\"")
                    //
                    //TODO:
                    //
                    //.Append(this.GetCategoryUrl(category, 1)).Append("\" tag=\"")
                    .Append(category.Path).Append("\" path=\"").Append(category.Path).Append("\">")
                    .Append(category.Name).Append("</a>");

            IList<CategoryDto> childs = new List<CategoryDto>(LocalService.Instance.SiteService.GetCategories(
                SiteId, category.Path));


            if (childs.Count != 0)
            {
                sb.Append("<ul>");

                foreach (var c in childs)
                    if (isRoot || handler(category))
                        CategoryTree_Iterator(c, sb, handler, false);

                sb.Append("</ul>");
            }

            if (!isRoot) sb.Append("</li>");

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
            var cacheKey = string.Format("{0}_site{1}_tree_{2}", CacheSign.Category.ToString(), SiteId.ToString(),
                categoryTag);

            BuiltCacheResultHandler<string> bh = () =>
            {
                //无缓存,则继续执行
                var sb = new StringBuilder(400);

                var category = LocalService.Instance.SiteService.GetCategory(SiteId, categoryTag);
                if (!(category.ID > 0)) return TplMessage("不存在栏目!标识:" + categoryTag);

                sb.Append("<div class=\"category_tree\">");

                CategoryTree_Iterator(category, sb, a => { return true; }, true);

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
            var cacheKey = string.Format("{0}_site{1}_select_tree_{2}_{3}",
                CacheSign.Category.ToString(),
                SiteId.ToString(),
                categoryTag,
                split);

            var cache = Cms.Cache.Get(cacheKey);

            if (cache != null) return cache as string;

            //无缓存,则继续执行


            //IList<Category> categories = new List<Category>();
            var isModule = false;
            var moduleId = 0;


            var sb = new StringBuilder(600);
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

                if (_category.Site().GetAggregateRootId() != SiteId) return;

                sb.Append("<option value=\"").Append(_category.Get().Path)
                    .Append("\" path=\"").Append(_category.Get().Path)
                    .Append("\">");
                for (var i = 0; i < _level; i++) sb.Append(split);

                sb.Append(_category.Get().Name).Append("</option>");
            };


            if (string.IsNullOrEmpty(categoryTag))
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
                    LocalService.Instance.SiteService.HandleCategoryTree(SiteId, 1, treeHandler);
                }
            }

            if (!isModule)
            {
                var category = LocalService.Instance.SiteService.GetCategory(SiteId, categoryTag);
                if (!(category.ID > 0)) return TplMessage("不存在栏目!标识:" + categoryTag);

                LocalService.Instance.SiteService.HandleCategoryTree(SiteId, category.ID, treeHandler);
            }


            var result = sb.ToString();

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
            Dispose();
        }

        #endregion
    }
}