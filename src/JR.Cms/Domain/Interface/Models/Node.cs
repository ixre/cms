//http://hi.baidu.com/suchshow/item/79e7681553a537761109b532
//http://sailorlee.iteye.com/blog/340523
//http://hegz.iteye.com/blog/627712
//wizardmin.com/2012/08/left-right-code-tree/
//http://www.haogongju.net/art/1362951
//http://www.dayanmei.com/blog.php/ID_352.htm


//根节点只能有一个

using System.Collections.Generic;

namespace JR.Cms.Domain.Interface.Models
{
    /// <summary>
    /// 栏目
    /// </summary>
    public class Category
    {
        /// <summary>
        /// 编号(文档索引时主键)
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 站点编号
        /// </summary>
        public int SiteID { get; set; }

        /// <summary>
        /// 左ID,在管理的时坐为主键
        /// </summary>
        public int Lft { get; set; }

        //右ID
        public int Rgt { get; set; }

        public int OrderIndex { get; set; }

        /// <summary>
        ///模块ID,如新闻,单页等
        /// </summary>
        public int ModuleID { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 名称(唯一)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }


        /************ 便于生成树的属性 *************/
        /// <summary>
        /// 下一级栏目列表
        /// </summary>
        public IEnumerable<Category> NextLevelCategories { get; set; }

        /// <summary>
        /// 是否标识过,如在树型中是否已经处理
        /// </summary>
        public bool IsSign { get; set; }

        /// <summary>
        /// 层级数
        /// </summary>
        public int Level { get; set; }
    }
}