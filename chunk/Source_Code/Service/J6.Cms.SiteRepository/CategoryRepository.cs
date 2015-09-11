using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using J6.Cms.Dal;
using J6.Cms.Domain.Implement.Site.Category;
using J6.Cms.Domain.Interface.Site;
using J6.Cms.Domain.Interface.Site.Category;
using J6.Cms.Domain.Interface.Site.Extend;
using J6.Cms.Domain.Interface.Site.Template;
using J6.Cms.Infrastructure;
using J6.DevFw.Data.Extensions;

namespace J6.Cms.ServiceRepository
{
    public class CategoryRepository : BaseCategoryRepository, ICategoryRepository
    {
        private IExtendFieldRepository _extendRep;



        private CategoryDal categoryDal = new CategoryDal();
        private ISiteRepository __siteRep;
        private ITemplateRepository _tempRep;

        public CategoryRepository(
            //ISiteRepository siteRep, 
            ITemplateRepository tempRep,
            IExtendFieldRepository extendRep)
        {
            //
            //NOTE:会循环依赖
            //
            this._extendRep = extendRep;
            this._tempRep = tempRep;
            //this._siteRep = ObjectFactory.GetInstance<ISiteRepository>();

            // GetCategoryDictionary();
        }

        private ISiteRepository _siteRep
        {
            get
            {
                return __siteRep ?? (__siteRep = ObjectFactory.GetInstance<ISiteRepository>());
            }
        }

        private IDictionary<int, IList<ICategory>> Categories
        {
            get
            {
                if (RepositoryDataCache._categories == null)
                {
                    GetCategoryDictionary();
                }
                return RepositoryDataCache._categories;
            }
        }


        public int GetCategoryIdByTag(int siteId, string tag)
        {
            /*
            if (RepositoryDataCache._categoryIdMaps == null)
            {
                GetCategoryDictionary();
            }

            string key = tag + "@" + siteId.ToString();
            if (RepositoryDataCache._categoryIdMaps.ContainsKey(key))
            {
                return RepositoryDataCache._categoryIdMaps[key];
            }
            return -1;
             */
            return -1;
        }

        /// <summary>
        /// 加载分类词典
        /// </summary>
        private void GetCategoryDictionary()
        {

            RepositoryDataCache._categories = new Dictionary<int, IList<ICategory>>();

            IList<ICategory> categories = new List<ICategory>();
            ICategory category;
            categoryDal.GetAllCategories(rd =>
            {
                while (rd.Read())
                {
                    category = this.CreateCategory(
                        int.Parse(rd["id"].ToString()),
                        _siteRep.GetSiteById(int.Parse(rd["site_id"].ToString()))
                        );
                    category.PageTitle = (rd["page_title"] ?? "").ToString();
                    category.Description = (rd["page_description"] ?? "").ToString();
                    category.Icon = Convert.ToString(rd["icon"]);
                    category.Keywords = Convert.ToString(rd["page_keywords"]);
                    category.Lft = Convert.ToInt32(rd["lft"]);
                    category.Rgt = Convert.ToInt32(rd["rgt"]);
                    category.Location = Convert.ToString(rd["location"]);
                    category.Name = Convert.ToString(rd["name"]);
                    category.Tag = Convert.ToString(rd["tag"]);
                    category.SortNumber = Convert.ToInt32(rd["sort_number"]);

                    categories.Add(category);
                }
            });

            //排序
           // IOrderedEnumerable<ICategory> categories2 = categories.OrderBy(a => a.ID);

            foreach (ICategory _category in categories)
            {
                if (!RepositoryDataCache._categories.ContainsKey(_category.Site.Id))
                {
                    RepositoryDataCache._categories.Add(_category.Site.Id, new List<ICategory>());
                }
                RepositoryDataCache._categories[_category.Site.Id].Add(_category);

                //添加Tag映射
                Kvdb.Put(String.Format("{0}:cache:t:lft:{1}",
                    _category.Site.Id.ToString(),
                    _category.Tag),
                    _category.Lft.ToString());

                if (_category.Site.Id ==1 &&_category.Tag.IndexOf("duct")!=-1)
                {
                    var x = "1:cache:t:lft:duct-machine" == String.Format("{0}:cache:t:lft:{1}",
                    _category.Site.Id.ToString(),
                    _category.Tag);
                }
 
                //添加Id映射
                Kvdb.Put(String.Format("{0}:cache:id:lft:{1}",
                   _category.Site.Id.ToString(),
                   _category.Id.ToString()),
                   _category.Lft.ToString());

            }

            //categories2 = null;
            categories = null;
        }



        public int GetCategoryLftByTag(int siteId, string tag)
        {
            this.ChkPreload();
            string key =  String.Format("{0}:cache:t:lft:{1}", siteId.ToString(), tag);
            return Kvdb.GetInt(key);
        }

        private void ChkPreload()
        {
            if (RepositoryDataCache._categories == null)
            {
                GetCategoryDictionary();
            }
        }

        public int GetCategoryLftById(int siteId, int id)
        {
            //添加ID映射
            string key = String.Format("{0}:cache:id:lft:{1}", siteId.ToString(), id.ToString());
            return Kvdb.GetInt(key);
        }

        public int SaveCategory(ICategory category)
        {
            ICategory parentCategory = category.Parent;

            //* 树状结构,只有一个根节点(默认Left为1)
            int parentLft = 1;
            int siteId = category.Site.Id;

            if (parentCategory != null)
            {
                parentLft = parentCategory.Lft;
            }

            if (category.Id <= 0)
            {

                #region SQL
                /*
            * 添加同一层次的节点的方法如下：
            * 
            * Sql代码
            * LOCK TABLE nested_category WRITE;   
            * SELECT @myRight := rgt FROM nested_category   
            * WHERE name = 'Cherry';   
            * UPDATE nested_category SET rgt = rgt + 2 WHERE rgt > @myRight;   
            * UPDATE nested_category SET lft = lft + 2 WHERE lft > @myRight;   
            * INSERT INTO nested_category(name, lft, rgt) VALUES('Strawberry', @myRight + 1, @myRight + 2);   
            * UNLOCK TABLES;  
            * 
            * 添加树的子节点的方法如下：
            * 
            * Sql代码
            * LOCK TABLE nested_category WRITE;   
            * SELECT @myLeft := lft FROM nested_category   
            * WHERE name = 'Beef';   
            * UPDATE nested_category SET rgt = rgt + 2 WHERE rgt > @myLeft;   
            * UPDATE nested_category SET lft = lft + 2 WHERE lft > @myLeft;   
            * INSERT INTO nested_category(name, lft, rgt) VALUES('charqui', @myLeft + 1, @myLeft + 2);   
            * UNLOCK TABLES; 
            * 
            * 
            * 每次插入节点之后都可以用以下SQL进行查看验证：
            * SELECT CONCAT( REPEAT( ' ', (COUNT(parent.name) - 1) ), node.name) AS name  
            * FROM nested_category AS node,   
            * nested_category AS parent   
            * WHERE node.lft BETWEEN parent.lft AND parent.rgt   
            * GROUP BY node.name  
            * ORDER BY node.lft;  
            * 
            */
                #endregion


                /*
                //添加树的子节点方法
                string parentName = category.Parent.Name;
                if (String.IsNullOrEmpty(parentName))
                {
                    parentName = this.GetCategories()[0].Name;
                }*/



                //更新
                categoryDal.UpdateInsertLftRgt(siteId, parentLft);

                int categoryId = this.GetNewCategoryId(siteId);

                category.Id = categoryDal.Insert(siteId,
                    categoryId,
                     parentLft + 1, parentLft + 2,
                     category.Name,
                     category.Tag,
                     category.Icon,
                     category.PageTitle,
                     category.Keywords,
                     category.Description,
                     category.Location,
                     category.SortNumber
                     );
            }
            else
            {

                #region 算法
                /*
             * refence url: http://wizardmin.com/2012/08/left-right-code-tree/
             * 
             * 移动操作
             * 主要是改变父节点和同层节点调换操作
             * 移动操作基本基于一个公式：任何树所占的数字数目 = 根的右值 – 根的左值 + 1。
             * 1.改变父节点
             * 先看下移动PHP到客户端，即PHP的父节点换成客户端，这种情况下树节点上的变化
             * 以PHP为根的树（虽然只有一个节点）的节点值变化是以新父节点的原右值为依据，
             * 如上图，新父节点的原右值为6，那PHP新的左值就是6，这样重新遍历PHP为根的树，
             * 就相当于这棵子树上的每个节点更新为：原右值 – (原左值 – 新左值)，
             * 
             * 所以PHP的右值就等于7 = 9 – (8 – 6)。
             * 
             * 而除了PHP为根的树，其他节点更新的有一定范围，范围就是新父节点的右值和
             * PHP原先的左值（或旧父节点的左值），因为PHP是向前移，范围内的节点值是往后移，
             * 
             * 需要加上PHP树的所占的数字数目，见上面公式，这里可以总结一个定律：
             * 新父节点_right <= ([left, right] + (移动_right – 移动_left + 1)) < 移动_left
             * 
             * 再看下PHP向后移的情况，与前移类似，也是更新一定范围的节点，变成减定律：
             * 移动_right < ([left, right] – (移动_right – 移动_left + 1)) < 新父节点_left
             * 
             */

                #endregion

                bool leftRightChanged = false;
                //ICategory oldCategory = this.GetCategoryById(category.Site.ID, category.ID);
                //Category newCategory = null;

                //更新
                categoryDal.Update(category.Id, siteId,
                    category.Name,
                    category.Tag,
                    category.Icon,
                    category.PageTitle,
                    category.Keywords,
                    category.Description,
                    category.Location,
                    category.SortNumber);


                #region 修改父类(暂时没有涉及到,需要从DB中获取旧数据来判断是否更改了父类

                if (parentLft > 0)
                {
                    //获取父类
                    /* SELECT TOP 1 * FROM 'tree' WHERE lft<@lft AND rgt>@rgt ORDER BY lft DESC  */

                    /*
                    leftRightChanged = parentCategory.Lft != parentLft;




                    //维护左右值
                    if (leftRightChanged)
                    {
                        int lft,
                            rgt,
                            parentRgt;

                        //要移动到的新类
                        newCategory = this.Get(a => a.Lft == parentLft);

                        //原左值
                        lft = entity.Lft;
                        rgt = entity.Rgt;
                        parentRgt = newCategory.Rgt;


                        #region 左右值语句

                        //栏目的左值为新父节点的右值
                        // entity.Lft = newCategory.Rgt;

                        //栏目的右值为原右值-(原左值-新左值)
                        // entity.Rgt = entity.Rgt - (orgialLft - entity.Lft);

                     * 
                     */
                    /*
                     * 其子节点的数目为$count = ($right_node - $left_node -1 )/2 , 节点A左值为$A_left_node ,
                     * UPDATE `tree` SET `left_node`=`left_node`-$right_node-$left_node-1 WHERE `left_node`>$right_node
                     * AND `left_node`<=$A_left_node

                     * UPDATE `tree` SET `right_node`=`right_node`-$right_node-$left_node-1 WHERE `right_node`>$right_node
                     * AND `right_node`<$A_left_node
                 
                     * UPDATE `tree` SET `left_node`=`left_node`+$A_left_node-$right_node , 
                     * `right_node`=`right_node`+$A_left_node-$right_node WHERE `left_node`>=$left_node
                     * AND `right_node`<=$right_node
                     * 
                     */
                    /*
            public string Category_ChangeUpdateTreeLeft = 
                "UPDATE $PREFIX_category SET lft=lft-@rgt-@lft-1 WHERE lft>@rgt AND lft<=@tolft";
                     * 
            public string Category_ChangeUpdateTreeRight=
                "UPDATE $PREFIX_category SET rgt=rgt-@rgt-@lft-1 WHERE rgt>@rgt AND rgt<@tolft";
                     * 
            public string Category_ChangeUpdateTreeBettown =
                "UPDATE $PREFIX_category SET lft=lft+@tolft-@rgt,
                     * rgt=rgt+@tolft-@rgt WHERE lft>=@lft AND rgt<=@rgt"
                    */


                    /*
                    Array ( [lft] => 5 [rgt] => 10 [width] => 6 )
                    Array2 ( [id] => 5 [lft] => 2 [rgt] => 17 [width] => 16 )
                    Array ( [id] => 5 [lft] => 2 [rgt] => 17 [width] => 16 )

  
                    mysql_query("UPDATE entryCategory SET rgt = rgt + %d - %d, lft = lft + %d - %d
                                  WHERE rgt <= %d and lft >= %d;",$row2["rgt"],$row["lft"],
                                  $row2["rgt"],$row["lft"],$row["rgt"],$row["lft"]);
  
  
                    mysql_query("UPDATE entryCategory SET rgt = rgt + %d
                                WHERE id=%d;",$row["width"],$row2["id"]);
                     * 
                    mysql_query("UPDATE entryCategory SET rgt = rgt - %d,
                                lft = lft - %d  WHERE rgt > %d and lft > %d;",
                                $row["width"],$row["width"],$row["rgt"],$row["rgt"]);
                     * 
                    */

                    /*
                    this.dal.UpdateMoveLftRgt(parentLft, entity.Lft, entity.Rgt);
                    */


                    /*
                    Array ( [lft] => 17 [rgt] => 22 [width] => 6 )
                    Array2 ( [id] => 5 [lft] => 3 [rgt] => 18 [width] => 16 )
                    Array ( [id] => 5 [lft] => 2 [rgt] => 23 [width] => 16 )

                     */

                    /*
                    var _dal = new CategoryDAL();
                    int width = rgt - lft + 1;

                    _dal.db.ExecuteNonQuery("UPDATE O_Categories SET rgt = rgt + " + (parentRgt - lft)
                        + ", lft = lft +" + (parentRgt - lft) + " WHERE rgt <= " + rgt + " and lft >= " + lft);

                    _dal.db.ExecuteNonQuery("UPDATE O_Categories SET rgt = rgt + " + width
                        + " WHERE id=" + newCategory.ID);


                    _dal.db.ExecuteNonQuery("UPDATE O_Categories SET rgt = rgt - " + width
                        + ", lft = lft -" + (width) + " WHERE rgt > " + rgt + " and lft > " + rgt);
                    */

                    /*
                        if (parentRgt > rgt)
                        {
                            categoryDal.UpdateMoveLftRgt(parentRgt, entity.Lft, entity.Rgt);
                        }
                        else
                        {
                            categoryDal.UpdateMoveLftRgt2(parentRgt, entity.Lft, entity.Rgt);
                        }


                    }*/
                        #endregion

                    #region 修改父类
                    /*
                goto planover;

            
            plan2:

                //维护左右值
                if (leftRightChanged)
                {
                     newCategory = this.Get(a => a.Lft == parentLft);

                    //原左值
                    int orgialLft = entity.Lft;

                    //栏目的左值为新父节点的右值
                    entity.Lft = newCategory.Rgt;

                    //栏目的右值为原右值-(原左值-新左值)
                    entity.Rgt = entity.Rgt - (orgialLft - entity.Lft);

                    /*
                     * 其子节点的数目为$count = ($right_node - $left_node -1 )/2 , 节点A左值为$A_left_node ,
                     * UPDATE `tree` SET `left_node`=`left_node`-$right_node-$left_node-1 WHERE `left_node`>$right_node
                     * AND `left_node`<=$A_left_node

                     * UPDATE `tree` SET `right_node`=`right_node`-$right_node-$left_node-1 WHERE `right_node`>$right_node
                     * AND `right_node`<$A_left_node
                 
                     * UPDATE `tree` SET `left_node`=`left_node`+$A_left_node-$right_node , 
                     * `right_node`=`right_node`+$A_left_node-$right_node WHERE `left_node`>=$left_node
                     * AND `right_node`<=$right_node
                     * 
                     */
                    /*
                                public string Category_ChangeUpdateTreeLeft = 
                                    "UPDATE $PREFIX_category SET lft=lft-@rgt-@lft-1 WHERE lft>@rgt AND lft<=@tolft";
                                         * 
                                public string Category_ChangeUpdateTreeRight=
                                    "UPDATE $PREFIX_category SET rgt=rgt-@rgt-@lft-1 WHERE rgt>@rgt AND rgt<@tolft";
                                         * 
                                public string Category_ChangeUpdateTreeBettown =
                                    "UPDATE $PREFIX_category SET lft=lft+@tolft-@rgt,
                                         * rgt=rgt+@tolft-@rgt WHERE lft>=@lft AND rgt<=@rgt"
                    */

                    //this.dal.ChangeUpdateTreeLeft(parentLft, entity.Lft, entity.Rgt);
                    //this.dal.ChangeUpdateTreeRight(parentLft, entity.Lft, entity.Rgt);
                    //this.dal.ChangeUpdateTreeBettown(parentLft, entity.Lft, entity.Rgt);

                    // }

                    //更新
                    //this.dal.Update(entity.ID, entity.Lft, entity.Rgt, entity.ModuleID, entity.Name, entity.Tag, entity.Keywords, entity.Description, entity.SortNumber);


                    //清除缓存
                    //planover:

                    #endregion
                }
            }

            RepositoryDataCache._categories = null;

            return category.Id;
        }

        /// <summary>
        /// 获取最大的栏目编号
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public int GetNewCategoryId(int siteId)
        {
            //根据站点编号 * 1000 递增，为了更好的将各个站点数据分离开
            //todo: 如有空余的应该填充
            int categoryId = this.categoryDal.GetMaxCategoryId(siteId);
            if (categoryId == 0)
            {
                return siteId*1000;
            }
            return categoryId + 1;
        }


        public ICategory GetCategoryById(int categoryId)
        {
            IList<ISite> sites = this._siteRep.GetSites();
            foreach (ISite site in sites)
            {
                var category = this.GetCategoryById(site.Id, categoryId);
                if (category != null) return category;
            }
            return null;
        }

        public ICategory GetCategoryById(int siteId, int categoryId)
        {
            IList<ICategory> list;
            if (this.Categories.ContainsKey(siteId))
            {
                list = this.Categories[siteId];
                int lft = this.GetCategoryLftById(siteId, categoryId);
                return BinarySearch.IntSearch(list, 0, list.Count, lft, a => a.Lft);
            }
            return null;
        }

        public ICategory CreateCategory(int categoryId, ISite site)
        {
            return base.CreateCategory(this, this._extendRep, this._tempRep, categoryId, site);
        }

        public IList<ICategory> GetCategories(int siteId)
        {
            if (!this.Categories.ContainsKey(siteId))
                return new List<ICategory>();
            return this.Categories[siteId];
        }


        public IEnumerable<ICategory> GetCategories(int siteId, int lft, int rgt, CategoryContainerOption option)
        {
            if (!this.Categories.ContainsKey(siteId))
                throw new Exception("站点无栏目!");
            return CategoryFilter.GetCategories(lft, rgt, this.Categories[siteId], option);
        }


        public IEnumerable<ICategory> GetChilds(ICategory category)
        {
            //获取所有子类
            /* SELECT * FROM tree WHERE lft BETWEEN @rootLft AND @rootRgt ORDER BY lft ASC'); */
            //return this.Categories[category.Site.ID].Where(a => a.Lft > category.Lft && a.Rgt < category.Lft);

            //获取第一级子类
            return this.Categories[category.Site.Id].Where(a => a.Lft > category.Lft && a.Rgt < category.Rgt);
        }


        public IEnumerable<ICategory> GetNextLevelChilds(ICategory category)
        {
            return CategoryFilter.GetCategories(category.Lft,
                category.Rgt,
                this.Categories[category.Site.Id],
                CategoryContainerOption.NextLevel
                );
        }


        public ICategory GetParent(ICategory category)
        {

            //获取父类
            /* SELECT TOP 1 * FROM 'tree' WHERE lft<@lft AND rgt>@rgt ORDER BY lft DESC  */
            IEnumerable<ICategory> list = this.Categories[category.Site.Id].Where(a => a.Lft < category.Lft && a.Rgt > category.Rgt);


            /*
            ICategory _category = list.LastOrDefault();
            foreach (ICategory c in list)
            {
                if (_category == null || c.Lft > _category.Lft) { _category = c; }
            }
            */

            return list.LastOrDefault(); ;
        }


        public ICategory GetCategoryByLft(int siteId, int lft)
        {
            return this.Categories[siteId].SingleOrDefault(a => a.Lft == lft);
        }


        public int GetArchiveCount(int siteId, int lft, int rgt)
        {
            return this.categoryDal.GetCategoryArchivesCount(siteId, lft, rgt);
        }

        public void DeleteCategory(int siteId, int lft, int rgt)
        {

            /*
                 * 删除所有子节点？
                 * DELETE FROM `tree` WHERE `left_node`>父节点的左值 AND `right_node`>父节点的右值
                 * 9、删除一个节点及其子节点？
                 * 在上例中的<号>号后面各加一个=号
                 */

            bool result = this.categoryDal.DeleteSelfAndChildCategoy(siteId, lft, rgt);

            if (result)
            {
                //更新删除的左右值
                this.categoryDal.UpdateDeleteLftRgt(siteId, lft, rgt);

                //删除视图设置
                // this.tplDal.RemoveErrorCategoryBind(); 

                //tb.RemoveBind(TemplateBindType.CategoryTemplate, lft.ToString());
                //tb.RemoveBind(TemplateBindType.CategoryArchiveTemplate, lft.ToString());

            }
            //return result;

        }



        public ICategory GetNext(ICategory category)
        {
            return this.GetCategories(category.Site.Id, category.Lft, category.Rgt, CategoryContainerOption.SameLevelNext).FirstOrDefault();
        }

        public ICategory GetPrevious(ICategory category)
        {
            return this.GetCategories(category.Site.Id, category.Lft, category.Rgt, CategoryContainerOption.SameLevelPrevious).FirstOrDefault();
        }




    }
}
