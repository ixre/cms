//http://hi.baidu.com/suchshow/item/79e7681553a537761109b532
//http://sailorlee.iteye.com/blog/340523
//http://hegz.iteye.com/blog/627712
//wizardmin.com/2012/08/left-right-code-tree/
//http://www.haogongju.net/art/1362951
//http://www.dayanmei.com/blog.php/ID_352.htm


//根节点只能有一个

using System.Collections.Generic;
using T2.Cms.Domain.Interface.Site.Extend;
using T2.Cms.Domain.Interface.Site.Template;
using T2.Cms.Infrastructure;
using T2.Cms.Models;

namespace T2.Cms.Domain.Interface.Site.Category
{
    /// <summary>
    /// 栏目
    /// </summary>
    public interface ICategory:IDomain<int>
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        CmsCategoryEntity Get();

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        Error Set(CmsCategoryEntity category);

        /// <summary>
        /// 站点编号
        /// </summary>
        //int SiteId { get; set; }
        ISite Site();


        /// <summary>
        /// 扩展属性
        /// </summary>
        IList<IExtendField> ExtendFields { get; set; }
        
        /// <summary>
        /// 设置模板
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Error SetTemplates(TemplateBind[] arr);

        /// <summary>
        /// 获取模板
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IList<TemplateBind> GetTemplates();

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        Error Save();

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

        /// <summary>
        /// 向上移动排序
        /// </summary>
        void MoveSortUp();
       
        /// <summary>
        /// 向下移动排序
        /// </summary>
        void MoveSortDown();

        /// <summary>
        /// 保存排序号码
        /// </summary>
        void SaveSortNumber();

        /// <summary>
        /// 强制更新栏目路径
        /// </summary>
        void ForceUpdatePath();
    }
}