using System;
using System.Collections.Generic;
using System.Linq;
using JR.Cms.Dal;
using JR.Cms.Domain.Implement.Site.Category;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Infrastructure;
using JR.Cms.Infrastructure.Ioc;
using JR.Cms.Models;

namespace JR.Cms.Repository
{
    public class CategoryRepository : BaseCategoryRepository, ICategoryRepo
    {
        private IExtendFieldRepository _extendRep;
        private CategoryDal categoryDal = new CategoryDal();
        private ArchiveDal _dal = new ArchiveDal();
        private ISiteRepo __siteRep;
        private ITemplateRepo _tempRep;

        public CategoryRepository(
            //ISiteRepository siteRep, 
            ITemplateRepo tempRep,
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

        private ISiteRepo _siteRep
        {
            get
            {
                return __siteRep ?? (__siteRep = Ioc.GetInstance<ISiteRepo>());
            }
        }

        private IDictionary<int, IList<ICategory>> Categories
        {
            get
            {
                if (RepositoryDataCache._categories == null)
                {
                    InitCategoryDictionary();
                }
                return RepositoryDataCache._categories;
            }
        }


        /// <summary>
        /// 加载分类词典
        /// </summary>
        private void InitCategoryDictionary()
        {

            RepositoryDataCache._categories = new Dictionary<int, IList<ICategory>>();

            IList<ICategory> categories = new List<ICategory>();
            ICategory ic;
            categoryDal.GetAllCategories(rd =>
            {
                while (rd.Read())
                {
                    CmsCategoryEntity category = new CmsCategoryEntity();
                    category.ID = Convert.ToInt32(rd["id"]);
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

            foreach (ICategory _category in categories)
            {
                if (!RepositoryDataCache._categories.ContainsKey(_category.Site().GetAggregaterootId()))
                {
                    RepositoryDataCache._categories.Add(_category.Site().GetAggregaterootId(), new List<ICategory>());
                }
                RepositoryDataCache._categories[_category.Site().GetAggregaterootId()].Add(_category);

                //添加Id->Path映射
                String key = this.catIdKey(_category.Site().GetAggregaterootId(), _category.GetDomainId());
                Kvdb.Put(key, _category.Get().Path);
            }
            categories = null;
        }

        public string catIdKey(int siteId,int categoryId)
        {
            return String.Format("cat:{0}:cache:id:lft:{1}",
                   siteId.ToString(),
                   categoryId.ToString());
        }
        public string catPathKey(int siteId,string path)
        {
            return String.Format("cat:{0}:cache:t:path:{1}",
                     siteId.ToString(), path.Replace("/","-"));
        }


        public ICategory GetCategoryByPath(int siteId, string path)
        {
            this.ChkPreload();
            IList<ICategory> list;
            if (this.Categories.ContainsKey(siteId))
            {
                list = this.GetCategories(siteId);
                return list.FirstOrDefault(a => String.Compare(a.Get().Path, path) == 0);
            }
            return null;
        }

        private void ChkPreload()
        {
            if (RepositoryDataCache._categories == null)
            {
                InitCategoryDictionary();
            }
        }

        public int GetCategoryLftById(int siteId, int id)
        {
            //添加ID映射
            string key = this.catIdKey(siteId, id);
            return Kvdb.GetInt(key);
        }

        public Error SaveCategory(CmsCategoryEntity category)
        {
            Error err = categoryDal.SaveCategory(category);
            if (err == null)
            {
                RepositoryDataCache._categories = null;
            }
            return err;
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

        

        public ICategory GetCategory(int siteId, int catId)
        {
            this.ChkPreload();
            if (siteId > 0)
            {
                IList<ICategory> list;
                if (this.Categories.ContainsKey(siteId))
                {
                    list = this.GetCategories(siteId);
                    return list.FirstOrDefault(a => a.GetDomainId() == catId);
                }
            }
            else
            {
                // todo; 兼容旧版本
                foreach(KeyValuePair<int,IList<ICategory>> p in this.Categories)
                {
                    ICategory b = p.Value.FirstOrDefault(a => a.GetDomainId() == catId);
                    if (b != null) return b;
                }
            }
            return null;
        }

        public ICategory CreateCategory(CmsCategoryEntity value)
        {
            return base.CreateCategory(this,this._siteRep, this._extendRep, this._tempRep,value);
        }

        public IList<ICategory> GetCategories(int siteId)
        {
            if (!this.Categories.ContainsKey(siteId)) return new List<ICategory>();
            return this.Categories[siteId];
        }

        
        /// <summary>
        /// 获取栏目下所有子栏目
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catPath"></param>
        /// <returns></returns>
        public IEnumerable<ICategory> GetChilds(int siteId, string catPath)
        {
            this.ChkPreload();
            if (!this.Categories.ContainsKey(siteId)) return new List<ICategory>();
            String path = catPath + "/";
            return this.GetCategories(siteId).Where(a => a.Get().Path.StartsWith(path));
        }


        public IEnumerable<ICategory> GetNextLevelChilds(ICategory category)
        {
            this.ChkPreload();
            int catId = category.GetDomainId();
            IList<ICategory> catgories = this.GetCategories(category.Get().SiteId);
            return catgories.Where(a => a.Get().ParentId == catId);
        }

        public void SaveCategorySortNumber(int id, int sortNumber)
        {
            this.categoryDal.SaveSortNumber(id, sortNumber);
        }
        
        
        public int GetArchiveCount(int siteId,String catPath)
        {
            return this.categoryDal.GetCategoryArchivesCount(siteId,catPath);
        }

        public void DeleteCategory(int siteId, int catId)
        {
            bool result = this.categoryDal.DeleteSelfAndChildCategoy(siteId,catId);
            if (result)
            {
                RepositoryDataCache._categories = null;
            }
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

        public bool CheckTagMatch(int siteId, int parentCatId, string tag, int catId)
        {
            return categoryDal.CheckTagMatch(siteId, parentCatId, tag, catId);
        }

        public void ReplaceArchivePath(int siteId, string oldPath, string path)
        {
            this._dal.ReplaceArchivePath(siteId, oldPath, path);
        }
    }
}
