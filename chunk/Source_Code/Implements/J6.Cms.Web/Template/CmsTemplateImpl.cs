//
// HTML创建
//  修改说明：
//      2012-12-29  newmin  [+]:Link & SAList
//      2013-03-05  newmin  [+]: 标签缩写
//      2013-03-06  newmin  [+]: 评论模块
//      2013-03-07  newmin  [+]: 添加数据标签参数符 "[ ]",主要用于outline[200]
//      2013-03-11  newmin  [+]: 添加authorname列，用于显示作者名称
//  2013-04-25  22:28 newmin [+]:PagerArchiveList添加
//  2013-06-07  22:15 newmin [+]:MCategoryTree,MCategoryList
//  2013-06-08  10:02 newmin [!]:CategoryTree_Iterator 加入TreeResultHandle,并加入isRoot,判断root类是否模块相同
//  2013-06-08  10:22 newmin [-]:删除MCategoryTree
//  2013-09-05  07:14 newmin [+]:添加region
//

using J6.Cms.BLL;
using J6.Cms.Cache.CacheCompoment;
using J6.Cms.CacheService;
using J6.Cms.Conf;
using J6.Cms.Domain.Interface.Enum;
using J6.Cms.Domain.Interface.Models;
using J6.Cms.Domain.Interface.Site.Category;
using J6.DevFw.Framework.Xml.AutoObject;
using J6.DevFw.Toolkit.Region;

namespace J6.Cms.Template
{
    using J6.Cms;
    using J6.Cms.DataTransfer;
    using J6.DevFw.Template;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    [XmlObject("CmsTemplateTag", "模板标签")]
    public partial class CmsTemplateImpl : CmsTemplateDataMethod
    {
        private static Type __type;

        //标签文件
        static CmsTemplateImpl()
        {
            __type = typeof(CmsTemplateImpl);
        }


        #region 文档

        /* ==================================== 查看文档 ====================================*/

        /// <summary>
        /// 文档内容
        /// </summary>
        /// <param name="idOrAlias"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取文档", @"
        	<b>参数：</b><br />
        	==========================<br />
			1:文档的编号或别名（可以文档列表中查询）
			2:HTML格式
		")]
        public string Archive(string idOrAlias, string format)
        {
            object id;
            if (Regex.IsMatch(idOrAlias, "^\\d+$"))
            {
                id = int.Parse(idOrAlias);
            }
            else
            {
                id = idOrAlias;
            }
            return base.Archive(this._site.SiteId, id, format);
        }

        [TemplateTag]
        [XmlObjectProperty("获取文档", @"
        	<b>参数：</b><br />
        	==========================<br />
			1:HTML格式
		")]
        public string Archive(string format)
        {
            object id = HttpContext.Current.Items["archive.id"];
            if (id == null || id == String.Empty)
            {
                return this.TplMessage("Error: 此标签只能在文档页面中调用!");
            }
            return base.Archive(this._site.SiteId, id, format);
        }


        [TemplateTag]
        [XmlObjectProperty("获取上一篇文档", @"
        	<b>参数：</b><br />
        	==========================<br />
			1:HTML格式
		")]
        public string Prev_Archive(string format)
        {
            object id = HttpContext.Current.Items["archive.id"];
            if (id == null)
            {
                return this.TplMessage("Error: 此标签只能在文档页面中调用!");
            }
            return PrevArchive(id.ToString(), format);
        }

        [TemplateTag]
        [XmlObjectProperty("获取下一篇文档", @"
        	<b>参数：</b><br />
        	==========================<br />
			1:HTML格式
		")]
        public string Next_Archive(string format)
        {
            object id = HttpContext.Current.Items["archive.id"];
            if (id == null)
            {
                return this.TplMessage("Error: 此标签只能在文档页面中调用!");
            }
            return NextArchive(id.ToString(), format);
        }


        [TemplateTag]
        [ContainSetting]
        [XmlObjectProperty("获取文档(默认格式)", @"")]
        public string Archive()
        {
            return this.Archive(base.TplSetting.CFG_ArchiveFormat);
        }

        [TemplateTag]
        [XmlObjectProperty("获取上一篇文档(默认格式)", @"")]
        public string Prev_Archive()
        {
            return Prev_Archive(base.TplSetting.CFG_PrevArchiveFormat);
        }

        [TemplateTag]
        [XmlObjectProperty("获取下一篇文档(默认格式)", @"")]
        public string Next_Archive()
        {
            return Next_Archive(base.TplSetting.CFG_NextArchiveFormat);
        }


        #endregion

        #region 文档列表

        //====================== 普通列表 ==========================//

        /// <summary>
        /// 文档列表
        /// </summary>
        [XmlObjectProperty("获取栏目下的文档列表。", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：栏目Tag<br />
        	2：数目<br />
            3：跳过的数目<br />
            4：分割数目<br />
            5：是否包含子分类,可选值[0|1],1代表包含。<br />
			6：HTML格式
		")]
        public string Archives(string tag, string num,string skipSize,string splitSize, string container, string format)
        {
            int intSkipSize, intSplitSize;
            int.TryParse(skipSize, out intSkipSize);
            int.TryParse(splitSize, out intSplitSize);
            return base.Archives(tag, num, intSkipSize, intSplitSize, IsTrue(container), format);
        }

        /// <summary>
        /// 文档列表(包含子类)
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取栏目（包含子栏目）下的文档列表。", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：栏目Tag<br />
        	2：数目<br />
			3：HTML格式
		")]
        public string Archives(string tag, string num, string format)
        {
            if (Regex.IsMatch(tag, "^\\d+$"))
            {
                int _num;
                ArchiveDto[] dt = null;
                Module module = null;
                int.TryParse(num, out _num);

                module = CmsLogic.Module.GetModule(int.Parse(tag));
                if (module != null)
                {
                    dt = ServiceCall.Instance.ArchiveService.GetArchivesByModuleId(this.SiteId, module.ID, _num);
                    return ArchiveList(dt,0, format);
                }
            }

            return this.Archives(tag, num, 0,0,true,format);
        }


        /// <summary>
        /// 文档列表(包含子类)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取栏目（包含子栏目）下的文档列表。", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：数目<br />
			2：HTML格式
		")]
        public string Archives(string num, string format)
        {
            string tag = HttpContext.Current.Items["category.tag"] as string;
            if (String.IsNullOrEmpty(tag))
            {
                return this.TplMessage("Error: 此标签不允许在当前页面中调用!");
            }
            return Archives(tag, num,0,0,true, format);
        }


        /// <summary>
        /// 文档列表(包含子类)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [TemplateTag]
        [ContainSetting]
        [XmlObjectProperty("获取栏目（包含子栏目）下的文档列表。", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：数目
		")]
        public string Archives(string num)
        {
            return Archives(num, base.TplSetting.CFG_ArchiveLinkFormat);
        }

        /// <summary>
        /// 文档列表(不包含子类)
        /// </summary>
        /// <param name="categoryTag"></param>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取栏目（不含子栏目）下的文档列表。", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：栏目Tag<br />
        	2：数目<br />
			3：HTML格式
		")]
        public string Self_Archives(string categoryTag, string num, string format)
        {
            return this.Archives(categoryTag, num, 0,0,false,format);
        }

        /// <summary>
        /// 文档列表(不包含子类)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取栏目（不含子栏目）下的文档列表。", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：数目<br />
			2：HTML格式
		")]
        public string Self_Archives(string num, string format)
        {
            string tag = HttpContext.Current.Items["category.tag"] as string;
            if (String.IsNullOrEmpty(tag))
            {
                return this.TplMessage("Error: 此标签不允许在当前页面中调用!");
            }
            return Self_Archives(tag, num, format);
        }

        /// <summary>
        /// 文档列表(不包含子类)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [TemplateTag]
        [ContainSetting]
        [XmlObjectProperty("获取栏目（不含子栏目）下的文档列表。", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：数目
		")]
        public string Self_Archives(string num)
        {
            return Self_Archives(num, base.TplSetting.CFG_ArchiveLinkFormat);
        }



        //====================== 特殊文档列表 ==========================//

      
        /// <summary>
        /// 特殊文档(包含子栏目)
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="num"></param>
        /// <param name="container"></param>
        /// <param name="format"></param>
        /// <param name="skipSize"></param>
        /// <param name="splitSize"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取标记为【推荐】的文档列表,默认包含子栏目", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：栏目编号<br />
        	2：数目<br />
            3：跳过的数目<br />
            4：分割数目<br />
            5：是否包含子分类,可选值[0|1],1代表包含。<br />
			6：HTML格式
		")]
        public string Special_Archives(string tag, string num, string skipSize, string splitSize,
            string container, string format)
        {
            int intSkipSize, intSplitSize;
            int.TryParse(skipSize, out intSkipSize);
            int.TryParse(splitSize, out intSplitSize);
            return this.Special_Archives(tag, num, intSkipSize, intSplitSize, IsTrue(container), format);
        }

        /// <summary>
        /// 特殊文档(包含子栏目)
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取标记为【推荐】的文档列表,默认包含子栏目", @"
        	<p class=""red"">仅能在栏目页中（默认文件：category.html) 使用此标签</p>
        	
        	<b>参数：</b><br />
        	==========================<br />
        	1：栏目编号<br />
        	2：数目<br />
			3：HTML格式
		")]
        public string Special_Archives(string tag,string num, string format)
        {
            return Special_Archives(tag,num, 0, 0, true, format);
        }

        /// <summary>
        /// 特殊文档(包含子栏目)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取标记为【推荐】的文档列表,默认包含子栏目", @"
        	<p class=""red"">仅能在栏目页中（默认文件：category.html) 使用此标签</p>
        	
        	<b>参数：</b><br />
        	==========================<br />
        	1：数目<br />
			2：HTML格式
		")]
        public string Special_Archives(string num, string format)
        {
            string id = HttpContext.Current.Items["category.tag"] as string;
            if (String.IsNullOrEmpty(id))
            {
                return this.TplMessage("Error: 此标签不允许在当前页面中调用!");
            }
            return Special_Archives(id, num, 0, 0, true, format);
        }

        /// <summary>
        /// 特殊文档(包含子栏目)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [TemplateTag]
        [ContainSetting]
        [XmlObjectProperty("获取标记为【推荐】的文档列表,默认包含子栏目,Html格式使用后台设置的格式。", @"
        	<p class=""red"">仅能在栏目页中（默认文件：category.html) 使用此标签</p>
        	<b>参数：</b><br />
        	==========================<br />
        	1：数目
		")]
        public string Special_Archives(string num)
        {
            return Special_Archives(num, base.TplSetting.CFG_ArchiveLinkFormat);
        }

        /// <summary>
        /// 特殊文档(包含子栏目)
        /// </summary>
        /// <param name="param"></param>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取标记为【推荐】的文档列表,默认包含子栏目,Html格式使用后台设置的格式。", @"
        	<p class=""red"">仅能在栏目页中（默认文件：category.html) 使用此标签</p>
        	<b>参数：</b><br />
        	==========================<br />
        	1：栏目Tag<br />
        	2：显示数量<br />
        	3：HTML格式
		")]
        public string Self_Special_Archives(string param, string num, string format)
        {
            return this.Special_Archives(param, num, 0,0,false,format);
        }


        /// <summary>
        /// 特殊文档(包含子栏目)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取标记为【推荐】的文档列表,默认包含子栏目,Html格式使用后台设置的格式。", @"
        	<p class=""red"">仅能在栏目页中（默认文件：category.html) 使用此标签</p>
        	<b>参数：</b><br />
        	==========================<br />
        	1：显示数量<br />
        	2：HTML格式
		")]
        public string Self_Special_Archives(string num, string format)
        {
            string id = HttpContext.Current.Items["category.tag"] as string;
            if (String.IsNullOrEmpty(id))
            {
                return this.TplMessage("Error: 此标签不允许在当前页面中调用!");
            }
            return Special_Archives(id, num, 0,0,false,format);
        }

        /// <summary>
        /// 特殊文档(包含子栏目)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取标记为【推荐】的文档列表,默认包含子栏目,Html格式使用后台设置的格式。", @"
        	<p class=""red"">仅能在栏目页中（默认文件：category.html) 使用此标签</p>
        	<b>参数：</b><br />
        	==========================<br />
        	1：显示数量
		")]
        public string Self_Special_Archives(string num)
        {
            return Self_Special_Archives(num, base.TplSetting.CFG_ArchiveLinkFormat);
        }


        //====================== 浏览排行列表 ==========================//

        /// <summary>
        /// 按点击排行文档列表
        /// </summary>
        /// <param name="container"></param>
        /// <param name="categoryTag"></param>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取热门文档列表", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：栏目Tag<br />
        	2：显示数量<br />
        	3：HTML格式<br />
        	4：是否包含子栏目
		")]
        public string Hot_Archives(string categoryTag, string num, string format, bool container)
        {
            int _num;
            ArchiveDto[] dt = null;
            CategoryDto category = default(CategoryDto);
            int.TryParse(num, out _num);

            category = ServiceCall.Instance.SiteService.GetCategory(this.SiteId, categoryTag);
            if (!(category.Id > 0))
            {
                return String.Format("ERROR:模块或栏目不存在!参数:{0}", categoryTag);
            }

            dt = container ?
                ServiceCall.Instance.ArchiveService.GetArchivesByViewCount(this.SiteId, category.Lft, category.Rgt, _num) :
                ServiceCall.Instance.ArchiveService.GetArchivesByViewCount(this.SiteId, category.Tag, _num);


            return this.ArchiveList(dt,0, format);
        }

        /// <summary>
        /// 按点击排行文档列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取热门文档列表", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：栏目Tag<br />
        	2：显示数量<br />
        	3：HTML格式
		")]
        public string Hot_Archives(string param, string num, string format)
        {
            //如果传入为模块编号
            if (Regex.IsMatch(param, "^\\d+$"))
            {
                int _num = 0;
                ArchiveDto[] dt = null;
                Module module = null;
                int.TryParse(num, out _num);

                module = CmsLogic.Module.GetModule(int.Parse(param));
                if (module != null)
                {
                    dt = ServiceCall.Instance.ArchiveService.GetArchivesByViewCountByModuleId(this.SiteId, module.ID, _num);
                    return this.ArchiveList(dt,0, format);
                }
            }

            return Hot_Archives(param, num, format, true);
        }

        /// <summary>
        /// 按点击排行文档列表
        /// </summary>
        /// <param name="container"></param>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取热门文档列表", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：显示数量<br />
        	2：HTML格式
		")]
        public string Hot_Archives(string num, string format)
        {
            string tag = HttpContext.Current.Items["category.tag"] as string;
            if (String.IsNullOrEmpty(tag))
            {
                return this.TplMessage("Error: 此标签不允许在当前页面中调用!");
            }
            return Hot_Archives(tag, num, format, true);
        }

        /// <summary>
        /// 按点击排行文档列表
        /// </summary>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取热门文档列表", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：显示数量
		")]
        public string Hot_Archives(string num)
        {
            return Hot_Archives(num, base.TplSetting.CFG_ArchiveLinkFormat);
        }


        /// <summary>
        /// 按点击排行文档列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取热门文档列表(不含子栏目)", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：栏目Tag<br />
        	2：显示数量<br />
        	3：HTML格式
		")]
        public string Self_Hot_Archives(string param, string num, string format)
        {
            return Hot_Archives(param, num, format, false);
        }

        /// <summary>
        /// 按点击排行文档列表
        /// </summary>
        /// <param name="container"></param>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取热门文档列表(不含子栏目)", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：显示数量<br />
        	2：HTML格式
		")]
        public string Self_Hot_Archives(string num, string format)
        {
            string tag = HttpContext.Current.Items["category.tag"] as string;
            if (String.IsNullOrEmpty(tag))
            {
                return this.TplMessage("Error: 此标签不允许在当前页面中调用!");
            }
            return Hot_Archives(tag, num, format, false);
        }

        /// <summary>
        /// 按点击排行文档列表
        /// </summary>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("获取热门文档列表(不含子栏目)", @"
        	<b>参数：</b><br />
        	==========================<br />
        	2：显示数量
		")]
        public string Self_Hot_Archives(string num)
        {
            return Self_Hot_Archives(num, base.TplSetting.CFG_ArchiveLinkFormat);
        }

        /// <summary>
        /// 根据模快获取文档
        /// </summary>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        // [TemplateTag]
        [Obsolete]
        protected string ArchivesByCount2(string num, string format)
        {
            return "";
            object moduleID = Cms.Context.Items["module.id"];
            if (moduleID == null)
            {
                return this.TplMessage("此标签不允许在当前页面中调用!");
            }
            // return HotArchives(moduleID.ToString(),"true",num, format);
        }


        //
        //TODO:特殊文档按点击数
        //

        #endregion

        #region 栏目

        /// <summary>
        /// 栏目链接列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        //[TemplateTag]
        protected string CategoryList_Old(string param, string format)
        {
            /*
            //
            // @param : 如果为int,则返回模块下的栏目，
            //                 如果为字符串tag，则返回该子类下的栏目
            //

            #region 取得栏目
            IEnumerable<Category> categories1;
            if (param == String.Empty)
            {
                categories1 = CmsLogic.Category.GetCategories();
            }
            else
            {
                if (Regex.IsMatch(param, "^\\d+$"))
                {
                    int moduleID = int.Parse(param);
                    categories1 = CmsLogic.Category.GetCategories(a => a.ModuleID == moduleID);
                }
                else
                {
                    Category c = CmsLogic.Category.Get(a =>a.SiteId==this.site.SiteId && String.Compare(a.Tag, param, true) == 0);
                    if (c != null)
                    {
                        throw new NotImplementedException();
                        //categories1 = CmsLogic.Category.getc(c.Lft, c.Rgt);
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
                IList<Category> categories = new List<Category>(categories1);
                StringBuilder sb = new StringBuilder(400);
                int i = 0;

                foreach (Category c in categories)
                {
                    sb.Append(tplengine.FieldTemplate(format, field =>
                    {
                        switch (field)
                        {
                            default: return String.Empty;

                            case "domain": return Settings.SYS_DOMAIN;

                            case "name": return c.Name;
                            case "url": return this.GetCategoryUrl(c, 1);
                            case "tag": return c.Tag;
                            case "id": return c.ID.ToString();

                            //case "pid":  return c.PID.ToString();

                            case "description": return c.Description;
                            case "keywords": return c.Keywords;
                            case "class":
                                if (i == categories.Count - 1) return " class=\"last\"";
                                else if (i == 0) return " class=\"first\"";
                                return string.Empty;
                        }
                    }));
                    ++i;
                }
                return sb.ToString();
            }
             * */
            return "";
        }

        [TemplateTag]
        [XmlObjectProperty("显示栏目列表", @"
        	参数1：栏目Tag<br />
        	参数2：显示格式
		")]
        public string Categories(string categoryTag, string format)
        {
            return base.CategoryList(categoryTag, format);
        }

        [TemplateTag]
        [XmlObjectProperty("显示栏目列表（栏目页或文档页中）", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：显示格式
		")]
        public string Categories(string format)
        {
            string id = HttpContext.Current.Items["category.tag"] as string;
            if (String.IsNullOrEmpty(id))
            {
                return this.TplMessage("Error: 此标签不允许在当前页面中调用!");
            }
            return Categories(id, format);
        }

        [TemplateTag]
        [XmlObjectProperty("显示栏目列表（栏目页或文档页中）", @"")]
        public string Categories()
        {
            return Categories(base.TplSetting.CFG_CategoryLinkFormat);
        }

        //        [TemplateTag]
        //        [XmlObjectProperty("显示栏目(不含子栏目)列表", @"
        //        	<b>参数：</b><br />
        //        	==========================<br />
        //        	1：栏目Tag<br />
        //        	2：显示格式
        //		")]
        //        public string Categories2(string categoryTag, string format)
        //        {
        //            return Categories(categoryTag, format, !true);
        //        }

        //        [TemplateTag]
        //        [XmlObjectProperty("显示栏目(不含子栏目)列表（栏目页或文档页中）", @"
        //        	<b>参数：</b><br />
        //        	==========================<br />
        //        	1：显示格式
        //		")]
        //        public string Categories2(string format)
        //        {
        //            string id = HttpContext.Current.Items["category.tag"] as string;
        //            if (String.IsNullOrEmpty(id))
        //            {
        //                return this.TplMessage("Error: 此标签不允许在当前页面中调用!");
        //            }
        //            return Categories(id, format, !true);
        //        }

        //[TemplateTag]
        //[XmlObjectProperty("显示栏目(不含子栏目)列表（栏目页或文档页中）",@"")]
        //public string Categories2()
        //{
        //    return Categories2(base.TplSetting.CFG_CategoryLinkFormat);
        //}

        #endregion


        [TemplateTag]
        [XmlObjectProperty("显示栏目分页文档结果", @"
        	<p class=""red"">只能在栏目页或文档页中使用！</p>
        	<b>参数：</b><br />
        	==========================<br />
        	1：显示数量<br />
        	2：显示格式
		")]
        public string Paging_Archives(string pageSize, string format)
        {
            string tag = HttpContext.Current.Items["category.tag"] as string;
            object pageindex = HttpContext.Current.Items["page.index"];


            if (String.IsNullOrEmpty(tag) || pageindex == null)
            {
                return this.TplMessage("Error: 此标签不允许在当前页面中调用!");
            }
            return base.Paging_Archives(tag, pageindex.ToString(), pageSize,0,0,format);
        }

        [TemplateTag]
        [XmlObjectProperty("显示栏目分页文档结果", @"
        	<p class=""red"">只能在栏目页或文档页中使用！</p>
        	<b>参数：</b><br />
        	==========================<br />
        	1：显示数量<br />
            2 : 分割条数<br />
        	3：显示格式
		")]
        public string Paging_Archives(string pageSize,string splitSize,string format)
        {
            string tag = HttpContext.Current.Items["category.tag"] as string;
            object pageindex = HttpContext.Current.Items["page.index"];
            
            if (String.IsNullOrEmpty(tag) || pageindex == null)
            {
                return this.TplMessage("Error: 此标签不允许在当前页面中调用!");
            }

            int intSplitSize;
            int.TryParse(splitSize, out intSplitSize);

            return base.Paging_Archives(tag, pageindex.ToString(), pageSize, 0, intSplitSize, format);
        }

        [TemplateTag]
        [XmlObjectProperty("显示栏目分页文档结果", @"
        	<p class=""red"">只能在栏目页或文档页中使用！</p>
        	<b>参数：</b><br />
        	==========================<br />
            1 : 栏目标识<br />
            2 : 当前页码<br />
        	3：显示数量<br />
            4 : 分割条数<br />
        	5：显示格式
		")]
        public string Paging_Archives(string categoryTag, string pageIndex,string pageSize, string splitSize, string format)
        {
            int intSplitSize;
            int.TryParse(splitSize, out intSplitSize);
            return base.Paging_Archives(categoryTag, pageIndex, pageSize, 0, intSplitSize, format);
        }





        [TemplateTag]
        [XmlObjectProperty("显示文档搜索结果", @"
        	<p class=""red"">只能在搜索页中使用！</p>
        	<b>参数：</b><br />
        	==========================<br />
        	1：显示数量<br />
        	2：显示格式
		")]
        public string Search_Archives(string pageSize, string format)
        {
            string key = HttpContext.Current.Items["search.key"] as string;
            string param = HttpContext.Current.Items["search.param"] as string;
            object pageindex = HttpContext.Current.Items["page.index"];


            if (String.IsNullOrEmpty(key) || pageindex == null)
            {
                return this.TplMessage("Error: 此标签不允许在当前页面中调用!");
            }
            return this.Search_Archives(param, key, pageindex.ToString(), pageSize,"0", format);
        }

        [TemplateTag]
        [XmlObjectProperty("显示文档搜索结果", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：关键词<br />
        	2：显示数量<br />
            3 : 分割条数<br />
        	4：显示格式
		")]
        public string Search_Archives(string keyword, string pageSize,string splitSize, string format)
        {
            string pageindex = HttpContext.Current.Items["page.index"] as string;
            return this.Search_Archives(null, keyword, pageindex, pageSize,splitSize, format);
        }

        /// <summary>
        /// 自定义分页搜索
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageSize"></param>
        /// <param name="splitSize"></param>
        /// <param name="format"></param>
        /// <param name="pagerLinkPath">分页地址路径</param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("显示文档搜索结果", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：关键词<br />
        	2：显示数量<br />
            3 : 分割条数<br />
        	4：显示格式<br />
        	5：页面路径(可用于自定义搜索页的URL)
		")]
        public string Search_Archives(string keyword, string pageSize,string splitSize, string format, string pagerLinkPath)
        {
            int pageIndex,
                recordCount,
                pageCount;
            string c = Request("c");
            int.TryParse(Request("p"), out pageIndex);
            if (pageIndex < 1) pageIndex = 1;

            string html = this.SearchArchives(
                this._site.SiteId,
                c,
                keyword,
                pageIndex.ToString(),
                pageSize,
                splitSize,
                format,
                out pageCount,
                out recordCount);

            //添加查询符串
            pagerLinkPath += pagerLinkPath.IndexOf("?") == -1 ? "?" : "&";

            //替换链接
            Regex reg = new Regex("([^\\?]+\\?*)(.+)", RegexOptions.IgnoreCase);

            string link1 = String.Format(TemplateUrlRule.Urls[TemplateUrlRule.RuleIndex, (int)UrlRulePageKeys.Search], HttpUtility.UrlEncode(keyword), c ?? ""),
                   link2 = String.Format(TemplateUrlRule.Urls[TemplateUrlRule.RuleIndex, (int)UrlRulePageKeys.SearchPager], HttpUtility.UrlEncode(keyword), c ?? "", "{0}");

            this.SetPager(
                        pageIndex,
                        pageCount,
                        recordCount,
                        reg.Replace(link1, String.Format("{0}$2", pagerLinkPath)),
                        reg.Replace(link2, String.Format("{0}$2", pagerLinkPath))
                    );

            return html;
        }

        /// <summary>
        /// 搜索文档列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="categoryTagOrModuleId"></param>
        /// <param name="splitSize"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("显示指定栏目下的文档搜索结果", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：栏目Tag
        	2：关键词<br />
        	3：当前页码<br />
        	4：显示数量<br />
            5 : 分割条数<br />
        	6：显示格式
		")]
        public string Search_Archives(string categoryTagOrModuleId, string keyword, string pageIndex, string pageSize,string splitSize,string format)
        {
            int pageCount, recordCount;
            return this.SearchArchives(this._site.SiteId, categoryTagOrModuleId, keyword, pageIndex, pageSize,splitSize, format, out pageCount, out recordCount);
        }


        /// <summary>
        /// 链接
        /// </summary>
        /// <param name="type"></param>
        /// <param name="number"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("自定义显示链接", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：链接类型，可选[1|2|3],1:导航,2:友情链接,3:自定义链接]<br />
        	2：显示数量<br />
        	3：显示格式
		")]
        public string Links(string type, string number, string format)
        {
            return base.Link(type, format, int.Parse(number), "-1");
        }

        /// <summary>
        /// 链接
        /// </summary>
        /// <param name="type"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [XmlObjectProperty("使用自定义格式显示<b>全部</b>链接", @"
        	<b>参数：</b><br />
        	==========================<br />
        	1：链接类型，可选[1|2|3],1:导航,2:友情链接,3:自定义链接]<br />
        	2：显示格式
		")]
        public string Link(string type, string format)
        {
            return base.Link(type, format, 1000, "-1");
        }

        [TemplateTag]
        [ContainSetting]
        [XmlObjectProperty("显示Tags", @"
        	<p class=""red"">仅能在文档页中使用此标签</p>
        	<b>参数：</b><br />
        	==========================<br />
        	1：标签数据，多个标签用"",""隔开。如：电脑,手机,相机
		")]
        public string Tags(string tags)
        {
            return base.Tags(tags, base.TplSetting.CFG_ArchiveTagsFormat);
        }

        [TemplateTag]
        [ContainSetting]
        [XmlObjectProperty("显示Tags", @"
        	<p class=""red"">只能在文档页中使用,格式在【模板设置】中进行设置</p>
		")]
        public string Tags()
        {
            if (!(archive.Id > 0)) return this.TplMessage("请先使用标签$require('id')获取文档后再调用属性");
            return Tags(archive.Tags, base.TplSetting.CFG_ArchiveTagsFormat);
        }

        [TemplateTag]
        [XmlObjectProperty("显示标签文档结果", @"
        	<p class=""red"">只能在标签搜索页(tags.html)中使用！</p>
        	<b>参数：</b><br />
        	==========================<br />
        	1：显示数量<br />
        	2：显示格式
		")]
        public string Tag_Archives(string pageSize, string format)
        {
            string key = HttpContext.Current.Items["tag.key"] as string;
            object pageindex = HttpContext.Current.Items["page.index"];

            if (String.IsNullOrEmpty(key) || pageindex == null)
            {
                return this.TplMessage("Error: 此标签不允许在当前页面中调用!");
            }
            return base.TagArchives(key, pageindex.ToString(), pageSize, format);
        }


        [TemplateTag]
        [ContainSetting]
        [XmlObjectProperty("显示评论框", @"
        	<p class=""red"">只能在文档页中使用！</p>
		")]
        public string Comment_Editor()
        {
            return this.Comment_Editor(base.TplSetting.CFG_AllowAmousComment ? "true" : "false",
                base.TplSetting.CFG_CommentEditorHtml);
        }



        [TemplateTag]
        [XmlObjectProperty("显示站点地图", @"
        	<p class=""red"">只能在栏目页或文档页中使用！</p>
		")]
        public string Sitemap()
        {
            string tag = Cms.Context.Items["category.tag"] as string;
            if (string.IsNullOrEmpty(tag))
            {
                return this.TplMessage("无法在当前页面调用此标签!\r\n解决方法:使用标签$sitemap('栏目标签')或设置Cms.Context.Items[\"category.tag\"]");
            }
            return Sitemap(tag);
        }


        /// <summary>
        /// 带缓存的导航
        /// </summary>
        /// <returns></returns>
        [TemplateTag]
        [ContainSetting]
        [XmlObjectProperty("显示网站导航", @"
        	<p class=""red"">显示格式在【模板设置】中修改.</p>
		")]
        public string Navigator()
        {
            string cache = SiteLinkCache.GetNavigatorBySiteId(SiteId);
            if (cache == null)
            {
                cache = base.Navigator(base.TplSetting.CFG_NavigatorLinkFormat, base.TplSetting.CFG_NavigatorChildFormat, "-1");
                SiteLinkCache.SetNavigatorForSite(SiteId, cache);
            }
            return cache;
        }

        [TemplateTag]
        [ContainSetting]
        [XmlObjectProperty("显示友情链接", @"
        	<p class=""red"">显示条数，以及格式均在【模板设置】中修改.</p>
		")]
        public string Friend_Link()
        {
            return this.Friend_Link(base.TplSetting.CFG_FriendShowNum.ToString(), base.TplSetting.CFG_FriendLinkFormat);
        }

        [TemplateTag]
        [XmlObjectProperty("显示友情链接", @"
        	<p class=""red"">仅能在文档页中使用此标签</p>
        	<b>参数：</b><br />
        	==========================<br />
        	1：显示数目<br />
			2：HTML格式,如:<a href=""{url}"" {target}>{text}</a>
		")]
        public string Friend_Link(string num, string format)
        {
            string cache = SiteLinkCache.GetFLinkBySiteId(SiteId);
            if (cache == null)
            {
                cache = this.Link("2", format, int.Parse(num), "-1");
                SiteLinkCache.SetFLinkForSite(SiteId, cache);
            }
            return cache;
        }



        #region 地区Region标签

        [TemplateTag]
        protected string Province(string path)
        {
            bool hasQuery = path.IndexOf('?') != -1;
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul class=\"provinces\">");
            foreach (Province p in Region.Provinces)
            {
                sb.Append("<li><a href=\"").Append(path)
                    .Append(hasQuery ? "&" : "?").Append("prv=").Append(p.ID.ToString()).Append("\">")
                    .Append(p.Text).Append("</a></li>");
            }
            sb.Append("</ul>");

            return sb.ToString();
        }


        [TemplateTag]
        protected string City(string path)
        {
            bool hasQuery = path.IndexOf('?') != -1;
            int provinceID = int.Parse(this.Request("prv") ?? "1");
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul class=\"cities\">");
            foreach (City p in Region.GetCities(provinceID))
            {
                sb.Append("<li><a href=\"").Append(path)
                    .Append(hasQuery ? "&" : "?").Append("prv=").Append(p.Pid.ToString()).Append("&cty=").Append(p.ID.ToString()).Append("\">")
                    .Append(p.Text).Append("</a></li>");
            }
            sb.Append("</ul>");

            return sb.ToString();
        }

        #endregion

        #region 兼容标签


        [TemplateTag]
        protected string SearchList(string keyword, string pageSize, string format)
        {
            string pageindex = HttpContext.Current.Items["page.index"] as string;
            return Search_Archives(null, keyword, pageindex, pageSize, format);
        }

        [TemplateTag]
        protected string SearchList(string pageSize, string format)
        {
            return Search_Archives(pageSize, format);
        }

        /// <summary>
        /// 自定义分页搜索
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageSize"></param>
        /// <param name="format"></param>
        /// <param name="pagerLinkPath">分页地址路径</param>
        /// <returns></returns>
        [TemplateTag]
        protected string SearchList(string keyword, string pageSize, string format, string pagerLinkPath)
        {
            return this.Search_Archives(keyword, pageSize, format, pagerLinkPath);
        }


        [TemplateTag]
        [XmlObjectProperty("显示栏目分页文档结果", @"
        	<p class=""red"">只能在栏目页或文档页中使用！</p>
        	<b>参数：</b><br />
        	==========================<br />
        	1：显示数量<br />
        	2：显示格式
		")]
        public string Pager_Archives(string pageSize, string format)
        {
            return this.Paging_Archives(pageSize, format);
        }



        /// <summary>
        /// 标签文档列表
        /// </summary>
        /// <param name="categoryTag"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="flags">t,o,p</param>
        /// <param name="url">是否webform</param>
        /// <returns></returns>
        [TemplateTag]
        protected string TagPagerArchiveList(string tag, string pageIndex, string pageSize, string format)
        {
            return base.TagArchives(tag, pageIndex, pageSize, format);
        }

        [TemplateTag]
        protected string TagPagerArchiveList(string pageSize, string format)
        {
            return this.Tag_Archives(pageSize, format);
        }


        /// <summary>
        /// 文档列表
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="container"></param>
        /// <param name="num"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        protected string Archives(string tag, string container, string num, string format)
        {
            return this.IsTrue(container) ? this.Archives(tag, num, format) : this.Self_Archives(tag, num, format);
        }


#endregion


        #region 过期
        /// <summary>
        /// 模块栏目标签
        /// </summary>
        /// <param name="id"></param>
        /// <param name="root"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [TemplateTag]
        [Obsolete]
        protected string MCategoryList(string id, String root, string format)
        {
            IList<ICategory> categories = new List<ICategory>();
            bool onlyRoot = this.IsTrue(root);

            if (String.IsNullOrEmpty(id))
            {
                return TplMessage("请指定参数:id的值");
            }

            if (Regex.IsMatch(id, "^\\d+$"))
            {
                //从模块加载
                int moduleID = int.Parse(id);
                if (CmsLogic.Module.GetModule(moduleID) != null)
                {
                    ServiceCall.Instance.SiteService.HandleCategoryTree(this.SiteId, 1, (c, level) =>
                    {
                        if (!onlyRoot || (onlyRoot && level == 0))
                        {
                            if (c.ModuleId == moduleID)
                            {
                                categories.Add(c);
                            }
                        }
                    });
                }
            }

            if (categories.Count == 0) return String.Empty;

            StringBuilder sb = new StringBuilder(400);
            int i = 0;

            foreach (ICategory c in categories.OrderBy(a => a.SortNumber ))
            {
                sb.Append(TplEngine.FieldTemplate(format, field =>
                {
                    switch (field)
                    {
                        default: return String.Empty;

                        case "name": return c.Name;

                        //
                        //TODO:
                        //
                        //case "url": return this.GetCategoryUrl(c, 1);
                        case "tag": return c.Tag;
                        case "id": return c.Id.ToString();

                        //case "pid":  return c.PID.ToString();

                        case "description": return c.Description;
                        case "keywords": return c.Keywords;
                        case "class":
                            if (i == categories.Count - 1) return " class=\"last\"";
                            else if (i == 0) return " class=\"first\"";
                            return string.Empty;
                    }
                }));
                ++i;
            }
            return sb.ToString();

        }

        [TemplateTag]
        [Obsolete]
        protected string MCategoryList(string id, string format)
        {
            return this.MCategoryList(id, "true", format);
        }

        [TemplateTag]
        [Obsolete]
        protected string MCategoryList(string format)
        {
            object id = Cms.Context.Items["module.id"];
            if (id == null)
            {
                return this.TplMessage("此标签不允许在当前页面中调用!请使用$MCategoryList(moduleid,isRoot,format)标签代替");
            }

            return this.MCategoryList(id.ToString(), "true", format);
        }


        [Obsolete]
        protected string _MCategoryTree(string moduleID)
        {
            //读取缓存
            string cacheKey = String.Format("{0}_site{1}_mtree_{2}", CacheSign.Category.ToString(), this.SiteId.ToString(), moduleID);

            BuiltCacheResultHandler<String> bh = () =>
            {
                //无缓存,则继续执行
                StringBuilder sb = new StringBuilder(400);

                int _moduleID = int.Parse(moduleID);

                //从模块加载
                if (CmsLogic.Module.GetModule(_moduleID) == null)
                {
                    return TplMessage("不存在模块!ID:" + moduleID);
                }
                sb.Append("<div class=\"category_tree mtree\">");

                CategoryDto dto = ServiceCall.Instance.SiteService.GetCategoryByLft(this.SiteId, 1);

                this.CategoryTree_Iterator(dto, sb, a => { return a.ModuleId == _moduleID; }, true);

                sb.Append("</div>");

                return sb.ToString();
            };


            return Cms.Cache.GetCachedResult(cacheKey, bh, DateTime.Now.AddHours(Settings.OptiDefaultCacheHours));
        }

        #endregion



    }
}

