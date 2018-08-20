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
using JR.DevFw.Data;
using T2.Cms.Infrastructure;
using T2.Cms.Models;

namespace T2.Cms.Dal
{
    public class CategoryDal : DalBase
    {
        /// <summary>
        /// 从数据库读取栏目
        /// </summary>
        /// <returns></returns>
        public void GetAllCategories(DataReaderFunc func)
        {
            base.ExecuteReader(base.NewQuery(DbSql.CategoryGetAllCategories,null), func);
        }

        /// <summary>
        /// 更新栏目
        /// </summary>
        public bool Update(int id, int siteId, string name,
            string tag, string icon, string pagetitle, string keywords,
            string description, string location, int orderIndex)
        {
            return base.ExecuteNonQuery(
                SqlQueryHelper.Create(DbSql.CategoryUpdate,
                    new object[,]{
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
            IDictionary<String, object> data = new Dictionary<string, object>();
            data.Add("@siteId", category.SiteId);
            data.Add("@name", category.Name);
            data.Add("@tag", category.Tag);
            data.Add("@icon", category.Icon);
            data.Add("@pagetitle", category.Title);
            data.Add("@keywords", category.Keywords);
            data.Add("@description", category.Description);
            data.Add("@location", category.Location);
            data.Add("@sortNumber", category.SortNumber);
            data.Add("@id", category.ID);
            if (category.ID <= 0)
            {
                base.ExecuteNonQuery(SqlQueryHelper.Create(DbSql.CategoryInsert, data));
                category.ID = this.GetMaxCategoryId(category.SiteId);
            }
            else
            {
                base.ExecuteNonQuery(base.CreateQuery(DbSql.CategoryInsert, data));
            }
            return null;
        }

        public int Insert(int siteId, int categoryId, int left, int right,
            string name, string tag, string icon, string pagetitle,
            string keywords, string description,
            string location, int orderIndex
            )
        {

            base.ExecuteNonQuery(
                SqlQueryHelper.Create(DbSql.CategoryInsert,
                    new object[,]{
                {"@categoryId",categoryId},
                {"@siteId",siteId},
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

            return int.Parse(base.ExecuteScalar(SqlQueryHelper.Create(
                "SELECT MAX(id) FROM $PREFIX_category WHERE site_id=@siteId",
                new object[,]{
                    {"@siteId",siteId}
                })).ToString());
        }

        /// <summary>
        /// 删除栏目包含子栏目
        /// </summary>
        /// <returns></returns>
        public bool DeleteSelfAndChildCategoy(int siteId, int lft, int rgt)
        {
            return base.ExecuteNonQuery(
                 SqlQueryHelper.Create(DbSql.Category_DeleteByLft,
                     new object[,]
                     {
                        {"@siteId", siteId},
                        {"@lft", lft},
                        {"@rgt", rgt}
                     })) >= 1;
        }



        public int GetMaxRight(int siteId)
        {
            object obj = base.ExecuteScalar(
                SqlQueryHelper.Create(DbSql.Category_GetMaxRight,
                new object[,] {
                    {"@siteId", siteId} }
                ));
            if (obj == null) return 1;
            return int.Parse(obj.ToString());
        }
      

        public void UpdateDeleteLftRgt(int siteId, int lft, int rgt)
        {
            int val = rgt - lft + 1;

            object[,] pa = new object[,]{
                {"@siteId", siteId},
                {"@lft",lft},
                {"@rgt",rgt},
                {"@val",val}
            };
            var parameters = base.Db.CreateParametersFromArray(pa);

            base.ExecuteNonQuery(
                base.NewQuery(DbSql.Category_UpdateDeleteLft, parameters),
                base.NewQuery(DbSql.Category_UpdateDeleteRgt, parameters)
               );
        }
        public int GetCategoryArchivesCount(int siteId, int lft, int rgt)
        {
            var parameters = base.Db.CreateParametersFromArray(new object[,]{
                       {"@siteId",siteId},
                       {"@lft",lft},
                       {"@rgt",rgt}
                    });

            return int.Parse(base.ExecuteScalar(
                base.NewQuery(DbSql.Archive_GetCategoryArchivesCount,parameters)).ToString());

        }

        public int GetMaxCategoryId(int siteId)
        {
            const string sql = "SELECT MAX(id) FROM $PREFIX_category WHERE site_id={0}";
            SqlQuery query = base.NewQuery(String.Format(sql, siteId.ToString()),null);
            object obj = base.ExecuteScalar(query);
            if (obj == DBNull.Value) return 0;
            return int.Parse(obj.ToString());
        }

        public void SaveSortNumber(int id, int sortNumber)
        {
            String sql = String.Format("UPDATE $PREFIX_category SET sort_number={0} WHERE id={1}",
                sortNumber.ToString(), id.ToString());
            base.ExecuteNonQuery(base.NewQuery(sql,null));
        }
    }
}