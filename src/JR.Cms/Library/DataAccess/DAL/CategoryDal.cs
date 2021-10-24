//
// ------------------------------------
// CategoryDAL
// Copyright 2011 @ OPS
// author:newmin
// date:2010/02/20 14:05
// --------------------------------------
// 2011/02/20  newmin:修改GetJson(CategoryType type)生成的json多了','的Bug
//

using System;
using System.Collections.Generic;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Infrastructure;
using JR.Stand.Core.Data;

namespace JR.Cms.Library.DataAccess.DAL
{
    public class CategoryDal : DalBase
    {
        /// <summary>
        /// 从数据库读取栏目
        /// </summary>
        /// <returns></returns>
        public void GetAllCategories(DataReaderFunc func)
        {
            ExecuteReader(NewQuery(DbSql.CategoryGetAllCategories, null), func);
        }

        /// <summary>
        /// 更新栏目
        /// </summary>
        public bool Update(int id, int siteId, string name,
            string tag, string icon, string pagetitle, string keywords,
            string description, string location, int orderIndex)
        {
            return ExecuteNonQuery(
                SqlQueryHelper.Create(DbSql.CategoryUpdate,
                    new object[,]
                    {
                        {"@siteId", siteId},
                        {"@name", name},
                        {"@tag", tag},
                        {"@icon", icon},
                        {"@pagetitle", pagetitle},
                        {"@keywords", keywords},
                        {"@description", description},
                        {"@location", location},
                        {"@sortNumber", orderIndex},
                        {"@id", id}
                    })) == 1;
        }


        public Error SaveCategory(CmsCategoryEntity category)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("@tag", category.Tag);
            data.Add("@site_id", category.SiteId);
            data.Add("@parent_id", category.ParentId);
            data.Add("@code", category.Code);
            data.Add("@path", category.Path);
            data.Add("@flag", category.Flag);
            data.Add("@module_id", category.ModuleId);
            data.Add("@name", category.Name);
            data.Add("@icon", category.Icon);
            data.Add("@page_title", category.Title);
            data.Add("@page_keywords", category.Keywords);
            data.Add("@page_description", category.Description);
            data.Add("@location", category.Location);
            data.Add("@sort_number", category.SortNumber);
            data.Add("@id", category.ID);
            if (category.ID <= 0)
            {
                ExecuteNonQuery(SqlQueryHelper.Create(DbSql.CategoryInsert, data));
                category.ID = GetMaxCategoryId(category.SiteId);
            }
            else
            {
                ExecuteNonQuery(CreateQuery(DbSql.CategoryUpdate, data));
            }

            return null;
        }

        public int Insert(int siteId, int categoryId, int left, int right,
            string name, string tag, string icon, string pagetitle,
            string keywords, string description,
            string location, int orderIndex
        )
        {
            ExecuteNonQuery(
                SqlQueryHelper.Create(DbSql.CategoryInsert,
                    new object[,]
                    {
                        {"@categoryId", categoryId},
                        {"@siteId", siteId},
                        {"@lft", left},
                        {"@rgt", right},
                        {"@name", name},
                        {"@tag", tag},
                        {"@icon", icon},
                        {"@pagetitle", pagetitle},
                        {"@keywords", keywords},
                        {"@description", description},
                        {"@sortNumber", orderIndex},
                        {"@location", location}
                    })
            );

            return int.Parse(ExecuteScalar(SqlQueryHelper.Create(
                "SELECT MAX(id) FROM $PREFIX_category WHERE site_id=@siteId",
                new object[,]
                {
                    {"@siteId", siteId}
                })).ToString());
        }

        /// <summary>
        /// 删除栏目包含子栏目
        /// </summary>
        /// <returns></returns>
        public bool DeleteSelfAndChildCategoy(int siteId, int catId)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("@siteId", siteId);
            data.Add("@catId", catId);
            return ExecuteNonQuery(CreateQuery(DbSql.Category_DeleteById, data)) >= 1;
        }


        public int GetCategoryArchivesCount(int siteId, string catPath)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("@siteId", siteId);
            data.Add("@path", catPath + "/%");
            return int.Parse(ExecuteScalar(
                CreateQuery(DbSql.Archive_GetArchivesCountByPath, data)).ToString());
        }

        public int GetMaxCategoryId(int siteId)
        {
            const string sql = "SELECT MAX(id) FROM $PREFIX_category WHERE site_id={0}";
            var query = NewQuery(string.Format(sql, siteId.ToString()), null);
            var obj = ExecuteScalar(query);
            if (obj == DBNull.Value) return 0;
            return int.Parse(obj.ToString());
        }

        public void SaveSortNumber(int id, int sortNumber)
        {
            var sql = string.Format("UPDATE $PREFIX_category SET sort_number={0} WHERE id={1}",
                sortNumber.ToString(), id.ToString());
            ExecuteNonQuery(NewQuery(sql, null));
        }

        public bool CheckTagMatch(int siteId, int parentCatId, string tag, int catId)
        {
            const string sql = @"SELECT id FROM $PREFIX_category WHERE site_id=@siteId 
                    AND parent_id=@parentId AND tag=@tag AND id<>@id";
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("@siteId", siteId);
            data.Add("@parentId", parentCatId);
            data.Add("@tag", tag);
            data.Add("@id", catId);
            var query = CreateQuery(sql, data);
            var obj = ExecuteScalar(query);
            return obj == null || obj == DBNull.Value;
        }
    }
}