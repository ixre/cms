using System;
using System.Collections.Generic;
using System.Linq;
using T2.Cms.Dal;
using T2.Cms.Domain.Implement.Site.Category;
using T2.Cms.Domain.Interface.Site;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Extend;
using T2.Cms.Domain.Interface.Site.Template;
using T2.Cms.Infrastructure;
using T2.Cms.Infrastructure.Ioc;
using T2.Cms.Models;

namespace T2.Cms.ServiceRepository
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
                return __siteRep ?? (__siteRep = Ioc.GetInstance<ISiteRepository>());
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


        /// <summary>
        /// 加载分类词典
        /// </summary>
        private void GetCategoryDictionary()
        {

            RepositoryDataCache._categories = new Dictionary<int, IList<ICategory>>();

            IList<ICategory> categories = new List<ICategory>();
            ICategory ic;
            categoryDal.GetAllCategories(rd =>
            {
                while (rd.Read())
                {
                    CmsCategoryEntity category = new CmsCategoryEntity();
                    category.Title = (rd["page_title"] ?? "").ToString();
                    category.Description = (rd["page_description"] ?? "").ToString();
                    category.Icon = Convert.ToString(rd["icon"]);
                    category.Keywords = Convert.ToString(rd["page_keywords"]);
                    category.Location = Convert.ToString(rd["location"]);
                    category.Name = Convert.ToString(rd["name"]);
                    category.Tag = Convert.ToString(rd["tag"]);
                    category.ParentId = Convert.ToInt32(rd["parent_id"]);
                    category.SiteId = Convert.ToInt32(rd["site_id"]);
                    category.Flag = Convert.ToInt32(rd["flag"]);
                    category.Path = Convert.ToString(rd["path"]);
                    category.SortNumber = Convert.ToInt32(rd["sort_number"]);
                
                    ic = this.CreateCategory(category);
                    if (ic.Site() != null)
                    {
                        categories.Add(ic);
                    }
                }
            });

            //排序
            // IOrderedEnumerable<ICategory> categories2 = categories.OrderBy(a => a.ID);

            foreach (ICategory _category in categories)
            {
                if (!RepositoryDataCache._categories.ContainsKey(_category.Site().GetAggregaterootId()))
                {
                    RepositoryDataCache._categories.Add(_category.Site().GetAggregaterootId(), new List<ICategory>());
                }
                RepositoryDataCache._categories[_category.Site().GetAggregaterootId()].Add(_category);

                //添加Tag映射
                String key = this.catTagKey(_category.Site().GetAggregaterootId(), _category.Get().Tag);
                Kvdb.PutInt(key,_category.Lft);

                /*
                if (_category.Site.Id == 1 && _category.Tag.IndexOf("duct") != -1)
                {
                    var x = "1:cache:t:lft:duct-machine" == String.Format("{0}:cache:t:lft:{1}",
                    _category.Site.Id.ToString(),
                    _category.Tag);
                }
                */

                //添加Id映射
                key = this.catIdKey(_category.Site().GetAggregaterootId(), _category.GetDomainId());
                Kvdb.PutInt(key,_category.Lft);

            }
            //categories2 = null;
            categories = null;
        }

        public string catIdKey(int siteId,int categoryId)
        {
            return String.Format("cat:{0}:cache:id:lft:{1}",
                   siteId.ToString(),
                   categoryId.ToString());
        }
        public string catTagKey(int siteId,string tag)
        {
            return String.Format("cat:{0}:cache:t:lft:{1}",
                     siteId.ToString(),
                     tag);
        }
        

        public int GetCategoryLftByTag(int siteId, string tag)
        {
            this.ChkPreload();
            string key = this.catTagKey(siteId,tag);
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
            string key = this.catIdKey(siteId, id);
            return Kvdb.GetInt(key);
        }

        public int SaveCategory(ICategory ic)
        {
            ICategory parentCategory = ic.Parent;

            //* 树状结构,只有一个根节点(默认Left为1)
            int parentLft = 1;
            int siteId = ic.Site().GetAggregaterootId();

            if (parentCategory != null)
            {
                parentLft = parentCategory.Lft;
            }
            CmsCategoryEntity category = ic.Get();
            Error err = categoryDal.SaveCategory(category);

            RepositoryDataCache._categories = null;

            return ic.Get().ID;
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
            if (siteId > 1 && categoryId == 0)
            {
                return siteId * 1000;
            }
            return categoryId + 1;
        }


        public ICategory GetCategoryById(int categoryId)
        {
            IList<ISite> sites = this._siteRep.GetSites();
            foreach (ISite site in sites)
            {
                var category = this.GetCategoryById(site.GetAggregaterootId(), categoryId);
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

        public ICategory CreateCategory(CmsCategoryEntity value)
        {
            return base.CreateCategory(this,this._siteRep, this._extendRep, this._tempRep,value);
        }

        public IList<ICategory> GetCategories(int siteId)
        {
            if (!this.Categories.ContainsKey(siteId))
                return new List<ICategory>();
            return this.Categories[siteId];
        }


        public IEnumerable<ICategory> GetCategories(int siteId, int catId, CategoryContainerOption option)
        {
            if (!this.Categories.ContainsKey(siteId))
                throw new Exception("站点无栏目!");
            // return CategoryFilter.GetCategories(lft, rgt, this.Categories[siteId], option);
            return CategoryFilter.GetCategories(catId, this.Categories[siteId], option);
        }


        public IEnumerable<ICategory> GetChilds(ICategory category)
        {
            //获取所有子类
            /* SELECT * FROM tree WHERE lft BETWEEN @rootLft AND @rootRgt ORDER BY lft ASC'); */
            //return this.Categories[category.Site.ID].Where(a => a.Lft > category.Lft && a.Rgt < category.Lft);

            //获取第一级子类
            return this.Categories[category.Site().GetAggregaterootId()].Where(a => a.Lft > category.Lft && a.Rgt < category.Rgt);
        }


        public IEnumerable<ICategory> GetNextLevelChilds(ICategory category)
        {
            return CategoryFilter.GetCategories(category.Lft,
                category.Rgt,
                this.Categories[category.Site().GetAggregaterootId()],
                CategoryContainerOption.NextLevel
                );
        }

        public void SaveCategorySortNumber(int id, int sortNumber)
        {
            this.categoryDal.SaveSortNumber(id, sortNumber);
        }


        public ICategory GetParent(ICategory category)
        {
            if (category == null) return null;
            if (!this.Categories.ContainsKey(category.Site().GetAggregaterootId()))
            {
                return null;
            }
            try
            {
                //获取父类
                /* SELECT TOP 1 * FROM 'tree' WHERE lft<@lft AND rgt>@rgt ORDER BY lft DESC  */
                IEnumerable<ICategory> list =
                    this.Categories[category.Site().GetAggregaterootId()].Where(a => a.Lft < category.Lft && a.Rgt > category.Rgt);


                /*
            ICategory _category = list.LastOrDefault();
            foreach (ICategory c in list)
            {
                if (_category == null || c.Lft > _category.Lft) { _category = c; }
            }
            */

                return list.LastOrDefault();
            }
            catch (NullReferenceException exc)
            {

            }
            return null;
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

            RepositoryDataCache._categories = null;
        }



        public ICategory GetNext(ICategory category)
        {
            /*
            return this.GetCategories(category.Site().GetAggregaterootId(), category.Lft, category.Rgt, CategoryContainerOption.SameLevelNext).FirstOrDefault();
            */
            return null;
        }

        public ICategory GetPrevious(ICategory category)
        {
            return null;
            /*
            return this.GetCategories(category.Site().GetAggregaterootId(), category.Lft, category.Rgt, CategoryContainerOption.SameLevelPrevious).FirstOrDefault();
            */
        }

        public int GetMaxSortNumber(int siteId)
        {
            return categoryDal.GetMaxCategoryId(siteId);
        }

        public bool CheckTagMatch(string tag, int siteId, int catId)
        {
            return categoryDal.CheckTagMatch(tag, siteId, catId);
        }
    }
}
