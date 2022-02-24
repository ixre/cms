using System;
using System.Collections.Generic;
using System.Linq;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Domain.Site.Category;
using JR.Cms.Infrastructure;
using JR.Cms.Infrastructure.Ioc;
using JR.Cms.Library.DataAccess.DAL;
using JR.Stand.Core.Extensions;

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
            _extendRep = extendRep;
            _tempRep = tempRep;
            //this._siteRep = ObjectFactory.GetInstance<ISiteRepository>();

            // GetCategoryDictionary();
        }

        private ISiteRepo _siteRep => __siteRep ?? (__siteRep = Ioc.GetInstance<ISiteRepo>());

        private IDictionary<int, IList<ICategory>> Categories
        {
            get
            {
                if (RepositoryDataCache._categories == null) InitCategoryDictionary();
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
                    var category = new CmsCategoryEntity();
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

                    ic = CreateCategory(category);
                    if (ic.Site() != null) categories.Add(ic);
                }
            });

            foreach (var _category in categories)
            {
                if (!RepositoryDataCache._categories.ContainsKey(_category.Site().GetAggregateRootId()))
                    RepositoryDataCache._categories.Add(_category.Site().GetAggregateRootId(), new List<ICategory>());
                RepositoryDataCache._categories[_category.Site().GetAggregateRootId()].Add(_category);

                //添加Id->Path映射
                var key = catIdKey(_category.Site().GetAggregateRootId(), _category.GetDomainId());
                Kvdb.Put(key, _category.Get().Path);
            }

            categories = null;
        }

        public string catIdKey(int siteId, int categoryId)
        {
            return string.Format("cat:{0}:cache:id:lft:{1}",
                siteId.ToString(),
                categoryId.ToString());
        }

        public string catPathKey(int siteId, string path)
        {
            return string.Format("cat:{0}:cache:t:path:{1}",
                siteId.ToString(), path.Replace("/", "-"));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public ICategory GetCategoryByPath(int siteId, string path)
        {
            ChkPreload();
            if (Categories.ContainsKey(siteId))
            {
                var list = GetCategories(siteId);
                return list.FirstOrDefault(a => String.CompareOrdinal(a.Get().Path, path) == 0);
            }

            return null;
        }

        private void ChkPreload()
        {
            if (RepositoryDataCache._categories == null) InitCategoryDictionary();
        }

        public int GetCategoryLftById(int siteId, int id)
        {
            //添加ID映射
            var key = catIdKey(siteId, id);
            return Kvdb.GetInt(key);
        }

        public Error SaveCategory(CmsCategoryEntity category)
        {
            var err = categoryDal.SaveCategory(category);
            if (err == null) RepositoryDataCache._categories = null;
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
            var categoryId = categoryDal.GetMaxCategoryId(siteId);
            if (siteId > 1 && categoryId == 0) return siteId * 1000;
            return categoryId + 1;
        }


        public ICategory GetCategory(int siteId, int catId)
        {
            ChkPreload();
            if (siteId > 0)
            {
                IList<ICategory> list;
                if (Categories.ContainsKey(siteId))
                {
                    list = GetCategories(siteId);
                    return list.FirstOrDefault(a => a.GetDomainId() == catId);
                }
            }
            else
            {
                // todo; 兼容旧版本
                foreach (var p in Categories)
                {
                    var b = p.Value.FirstOrDefault(a => a.GetDomainId() == catId);
                    if (b != null) return b;
                }
            }

            return null;
        }

        public ICategory CreateCategory(CmsCategoryEntity value)
        {
            return base.CreateCategory(this, _siteRep, _extendRep, _tempRep, value);
        }

        public IList<ICategory> GetCategories(int siteId)
        {
            if (!Categories.ContainsKey(siteId)) return new List<ICategory>();
            return Categories[siteId];
        }


        /// <summary>
        /// 获取栏目下所有子栏目
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catPath"></param>
        /// <returns></returns>
        public IEnumerable<ICategory> GetChilds(int siteId, string catPath)
        {
            ChkPreload();
            if (!Categories.ContainsKey(siteId)) return new List<ICategory>();
            var path = catPath + "/";
            return GetCategories(siteId).Where(a => a.Get().Path.StartsWith(path));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public IEnumerable<ICategory> GetNextLevelChildren(ICategory category)
        {
            ChkPreload();
            var catId = category.GetDomainId();
            var categories = GetCategories(category.Get().SiteId);
            return categories.Where(a => a.Get().ParentId == catId);
        }

        /// <inheritdoc />
        public void SaveCategorySortNumber(int id, int sortNumber)
        {
            categoryDal.SaveSortNumber(id, sortNumber);
        }


        /// <inheritdoc />
        public int GetArchiveCount(int siteId, string catPath)
        {
            return categoryDal.GetCategoryArchivesCount(siteId, catPath);
        }

        /// <inheritdoc />
        public void DeleteCategory(int siteId, int catId)
        {
            var result = categoryDal.DeleteSelfAndChildCategoy(siteId, catId);
            if (result) RepositoryDataCache._categories = null;
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
            _dal.ReplaceArchivePath(siteId, oldPath, path);
        }
    }
}