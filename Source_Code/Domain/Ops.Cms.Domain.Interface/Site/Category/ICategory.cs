//http://hi.baidu.com/suchshow/item/79e7681553a537761109b532
//http://sailorlee.iteye.com/blog/340523
//http://hegz.iteye.com/blog/627712
//wizardmin.com/2012/08/left-right-code-tree/
//http://www.haogongju.net/art/1362951
//http://www.dayanmei.com/blog.php/ID_352.htm


//根节点只能有一个

namespace Ops.Cms.Domain.Interface.Site.Category
{
    using Ops.Cms.Domain.Interface.Site;
using Ops.Cms.Domain.Interface.Site.Extend;
using Ops.Cms.Domain.Interface.Site.Template;
using System.Collections.Generic;

    /// <summary>
    /// 栏目
    /// </summary>
    public interface ICategory:IDomain<int>
    {

        /// <summary>
        /// 站点编号
        /// </summary>
        //int SiteId { get; set; }
        ISite Site { get; }

        /// <summary>
        /// 左ID,在管理的时坐为主键
        /// </summary>
        int Lft { get; set; }

        //右ID
        int Rgt { get; set; }

        /// <summary>
        /// 排序编号
        /// </summary>
        int OrderIndex { get; set; }

        /// <summary>
        ///模块ID,如新闻,单页等
        /// </summary>
        int ModuleId { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        string Tag { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        string Icon { get; set; }

        /// <summary>
        /// 名称(唯一)
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 页面标题
        /// </summary>
        string PageTitle { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        string Keywords { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 定位路径（打开栏目页定位到的路径）
        /// </summary>
        string Location { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        IList<IExtendField> ExtendFields { get; set; }

        /// <summary>
        /// 模板绑定
        /// </summary>
        IList<ITemplateBind> Templates { get; set; }


        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        int Save();

        /// <summary>
        /// 父栏目
        /// </summary>
        ICategory Parent { get; set; }

        /// <summary>
        /// 下一个栏目
        /// </summary>
        ICategory Next { get; }

        /// <summary>
        /// 上一个栏目
        /// </summary>
        ICategory Previous { get; }

        /// <summary>
        /// 子栏目
        /// </summary>
        IEnumerable<ICategory> Childs { get; }

        /// <summary>
        /// 下一级子栏目
        /// </summary>
        IEnumerable<ICategory> NextLevelChilds { get;}

        /// <summary>
        /// 定位路径
        /// </summary>
        string UriPath { get; }

        /// <summary>
        /// 清理
        /// </summary>
        void ClearSelf();



        /************ 便于生成树的属性 *************/
        ///// <summary>
        ///// 下一级栏目列表
        ///// </summary>
        //IEnumerable<ICategory> NextLevelCategories { get; set; }

        ///// <summary>
        ///// 是否标识过,如在树型中是否已经处理
        ///// </summary>
        //bool IsSign { get; set; }

        ///// <summary>
        ///// 层级数
        ///// </summary>
        //int Level { get; set; }


    }
}