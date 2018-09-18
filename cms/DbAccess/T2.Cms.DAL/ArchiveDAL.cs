/*
 * Copyright 2010 OPS,All rights reseved!
 * name     : ArchiveDAL
 * author   : newmin
 * date     : 2010/11/15
 * 
 * 2010/12/08 [!] newmin: GetArchivesOrderByCreateDate方法从数据库中选取了Archive的Content,需要删除
 * 2013-05-15 05:40 newmin [!]: 修改了GetPagedArchives基于左右值
 */

using System;
using System.Data;
using System.Text;
using T2.Cms.Domain.Interface.Content.Archive;
using JR.DevFw.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace T2.Cms.Dal
{
    public partial class ArchiveDal : DalBase
    {
        /// <summary>
        /// 插入文章并返回ID
        /// </summary>
        /// <returns></returns>
        public bool Add(int siteId,string strId, string alias, int categoryId,
            int publisherId, string title, string smallTitle, string source, string thumbnail,
            string outline, string content, string tags, string flags, string location, int sortNumber)
        {
            string date = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
            var pa = new object[,]{
                {"@siteId",siteId },
                {"@strId", strId},
                {"@alias", alias},
                {"@catId", categoryId},
                {"@publisherId", publisherId},
                {"@Title", title},
                {"@smallTitle", smallTitle??""},
                {"@Flags",flags},
                {"@location", location},
                {"@sortNumber",sortNumber},
                {"@Source", source??""},
                {"@thumbnail",thumbnail??""},
                {"@Outline", outline??""},
                {"@Content", content},
                {"@Tags", tags??""},
                {"@CreateDate",date},
                {"@LastModifyDate",date}
                     };
            var parameters = base.Db.CreateParametersFromArray(pa);
            int rowcount = base.ExecuteNonQuery(base.NewQuery(DbSql.ArchiveAdd, parameters));
            return rowcount == 1;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="a"></param>
        public void Update(int id, int categoryID, string title, string smallTitle, string alias,
            string source, string thumbnail, string outline, string content,
            string tags, string flags, string location, int sortNumber)
        {
            string date = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
            var pa = new object[,]{
                                {"@catId", categoryID},
                                {"@Title", title},
                                {"@smallTitle", smallTitle??""},
                                {"@Flags", flags},
                                {"@Alias", alias??""},
                                {"@location", location},
                                {"@sortNumber",sortNumber},
                                {"@Source", source??""},
                                {"@thumbnail",thumbnail??""},
                                {"@Outline", outline??""},
                                {"@Content", content},
                                {"@Tags", tags??""},
                                {"@lastModifyDate",date},
                                {"@id", id}
                 };
            var parameters = base.Db.CreateParametersFromArray(pa);
            base.ExecuteNonQuery(base.NewQuery(DbSql.ArchiveUpdate, parameters));
        }

        /// <summary>
        /// 刷新文档
        /// </summary>
        /// <param name="archiveId"></param>
        public void RePublish(int siteId, int archiveId)
        {
            var pa = new object[,]{
                {"@CreateDate",String.Format("{0:yyyy-MM-dd HH:mm:ss}",DateTime.Now)},
                {"@id", archiveId},
                {"@siteId",siteId}
        };
            var parameters = base.Db.CreateParametersFromArray(pa);

            base.ExecuteNonQuery(
                base.NewQuery(DbSql.ArchiveRepublish, parameters));
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int siteId, int id)
        {
            base.ExecuteNonQuery(
                base.NewQuery(DbSql.Archive_Delete,
                base.Db.CreateParametersFromArray(
                     new object[,]{
                        {"@siteId",siteId},
                        {"@id", id}
                     })));
        }

        /// <summary>
        /// 检查别名是否存在
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public bool CheckAliasIsExist(int siteID, string alias)
        {
            var pa = new object[,]{
                        {"@siteId",siteID},
                        {"@alias", alias}
                     };
            var parameters = base.Db.CreateParametersFromArray(pa);

            return base.ExecuteScalar(
                base.NewQuery(DbSql.Archive_CheckAliasIsExist, parameters)
                ) != null;
        }


        /// <summary>
        /// 添加查看次数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="number"></param>
        public void AddViewCount(int siteId, int id, int count)
        {
            var pa = new object[,]{
                     {"@siteId",siteId},
                {"@Count", count},
                {"@id", id}
                 };
            var parameters = base.Db.CreateParametersFromArray(pa);

            base.ExecuteNonQuery(base.NewQuery(DbSql.Archive_AddViewCount, parameters)
                );
        }

        public int GetMaxArchiveId(int siteId)
        {
            throw new NotImplementedException();
        }

        #region 获取文档


        /// <summary>
        /// 获取文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void GetArchive(int siteId, string archiveIdOrAlias, DataReaderFunc func)
        {
            var pa = new object[,]{
                        {"@siteId",siteId},
                        {"@strid", archiveIdOrAlias}
                     };
            var parameters = base.Db.CreateParametersFromArray(pa);
            base.ExecuteReader(
                   base.NewQuery(DbSql.Archive_GetArchiveByStrIDOrAlias, parameters)
                     ,
                   func
                   );
        }



        public void GetArchiveById(int siteId, int archiveId, DataReaderFunc func)
        {
            var pa = new object[,]{
                        {"@siteId",siteId},
                        {"@id", archiveId}
            };

            var parameters = base.Db.CreateParametersFromArray(pa);

            base.ExecuteReader(
                   base.NewQuery(DbSql.Archive_GetArchiveById, parameters)
                       ,
                   func
                   );
        }

        public DataTable GetAllArchives()
        {
            return base.GetDataSet(base.NewQuery(DbSql.Archive_GetAllArchive, null)).Tables[0];
        }

        [Obsolete]
        public DataTable GetArchives(string sqlcondition)
        {
            return base.GetDataSet(new SqlQuery(String.Format(base.OptimizeSql(DbSql.Archive_GetArchivesByCondition), sqlcondition), DalBase.EmptyParameter)).Tables[0];
        }


        /// <summary>
        /// 获取制定栏目的最新文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <param name="number"></param>
        /// <param name="skipSize"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public void GetArchiveList(int siteId, int[] catIdArray, int number, int skipSize, DataReaderFunc func)
        {
            String sql = DbSql.Archive_GetArchiveList;
            sql = SQLRegex.Replace(sql, m =>
             {
                 switch (m.Groups[1].Value)
                 {
                     case "catIdArray":
                         return this.IntArrayToString(catIdArray);
                 }
                 return null;
             });

            IDictionary<String, object> paramters = new Dictionary<string, object>();
            paramters.Add("@siteId", siteId);
            SqlQuery query = new SqlQuery(String.Format(base.OptimizeSql(sql), skipSize, number), paramters);
            base.ExecuteReader(query, func);
        }

        /// <summary>
        /// 获取指定栏目和数量的文档
        /// </summary>
        public void GetArchives(int siteId, string categoryTag, int number, int skipSize, DataReaderFunc func)
        {
            base.ExecuteReader(
              new SqlQuery(String.Format(base.OptimizeSql(DbSql.Archive_GetArchivesByCategoryAlias), skipSize, number),
                   new object[,]{
                {"@siteId", siteId},
                {"@tag", categoryTag}
                    }), func);
        }

        /// <summary>
        /// 获取指定模块类型的最新文档
        /// </summary>
        public void GetArchivesByModuleId(int siteId, int moduleId, int number, DataReaderFunc func)
        {
            var pa = new object[,]{
                {"@siteId",siteId},
                {"@ModuleId", moduleId}
                 };
            var parameters = base.Db.CreateParametersFromArray(pa);

            base.ExecuteReader(
                base.NewQuery(String.Format(DbSql.Archive_GetArchivesByModuleId, number), parameters),
             func);
        }

        /// <summary>
        /// 获取指定栏目浏览次数最多的档案
        /// </summary>
        /// <returns></returns>
        public void GetArchivesByViewCount(int siteId, int[] catIdArray, int number, DataReaderFunc func)
        {
            String sql = DbSql.Archive_GetArchivesByViewCountDesc;
            sql = SQLRegex.Replace(sql, m =>
            {
                switch (m.Groups[1].Value)
                {
                    case "catIdArray":
                        return this.IntArrayToString(catIdArray);
                }
                return null;
            });

            IDictionary<String, object> paramters = new Dictionary<string, object>();
            paramters.Add("@siteId", siteId);
            SqlQuery query = new SqlQuery(String.Format(base.OptimizeSql(sql), number), paramters);
            base.ExecuteReader(query, func);
        }


        /// <summary>
        /// 获取指定模块浏览次数最多的档案
        /// </summary>
        /// <returns></returns>
        public void GetArchivesByViewCountAndModuleId(int siteId, int moduleId, int number, DataReaderFunc func)
        {
            base.ExecuteReader(
                new SqlQuery(String.Format(base.OptimizeSql(DbSql.Archive_GetArchivesByModuleIDAndViewCountDesc), number),
                     new object[,]{
                         {"@siteId",siteId},
                {"@ModuleId", moduleId}
                     }), func);
        }


        /// <summary>
        /// 获取指定数量和栏目的特殊文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catIdArray"></param>
        /// <param name="number"></param>
        /// <param name="skipSize"></param>
        /// <param name="func"></param>
        public void GetSpecialArchives(int siteId, int[] catIdArray, int number, int skipSize, DataReaderFunc func)
        {
            String sql = DbSql.Archive_GetSpecialArchiveList;
            sql = SQLRegex.Replace(sql, m =>
            {
                switch (m.Groups[1].Value)
                {
                    case "catIdArray":
                        return this.IntArrayToString(catIdArray);
                }
                return null;
            });

            IDictionary<String, object> paramters = new Dictionary<string, object>();
            paramters.Add("@siteId", siteId);
            SqlQuery query = new SqlQuery(String.Format(base.OptimizeSql(sql), skipSize, number), paramters);
            base.ExecuteReader(query, func);
        }

        /// <summary>
        /// 获取指定数量和模块的特殊文章
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="moduleId"></param>
        /// <param name="number"></param>
        /// <param name="func"></param>
        public void GetSpecialArchivesByModuleId(int siteId, int moduleId, int number, DataReaderFunc func)
        {
            base.ExecuteReader(
                        new SqlQuery(String.Format(base.OptimizeSql(DbSql.Archive_GetSpecialArchivesByModuleID), number),
                         new object[,]{
                            {"@siteId",siteId},
                            {"@ModuleId", moduleId}
                         }), func);
        }

        /// <summary>
        /// 获取指定栏目的第一篇特殊文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryId"></param>
        /// <param name="func"></param>
        public void GetFirstSpecialArchive(int siteId, int categoryId, DataReaderFunc func)
        {
            var pa = new object[,]{
                     {"@siteId", siteId},
                     {"@CategoryId", categoryId}
                     };
            var parameters = base.Db.CreateParametersFromArray(pa);

            base.ExecuteReader(
                base.NewQuery(DbSql.Archive_GetFirstSpecialArchiveByCategoryID, parameters),
                func
                );
        }


        /********* 上一篇和下一篇 **************/

        /// <summary>
        /// 获取上一篇文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="id"></param>
        /// <param name="sameCategory"></param>
        /// <param name="ingoreSpecial"></param>
        /// <param name="func"></param>
        public void GetPreviousArchive(int siteId, int id, bool sameCategory, bool ingoreSpecial, DataReaderFunc func)
        {
            var pa = new object[,]{
                         {"@siteId",siteId},
                         {"@id", id},
                        {"@sameCategory", sameCategory?1:0},
                        {"@special",ingoreSpecial?1:0},
                         };
            var parameters = base.Db.CreateParametersFromArray(pa);

            base.ExecuteReader(
                    base.NewQuery(DbSql.Archive_GetPreviousArchive, parameters), func
                    );
        }

        /// <summary>
        /// 获取下一篇文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="id"></param>
        /// <param name="ingoreSpecial"></param>
        /// <param name="func"></param>
        /// <param name="sameCategory"></param>
        public void GetNextArchive(int siteId, int id, bool sameCategory, bool ingoreSpecial, DataReaderFunc func)
        {
            var pa = new object[,]
                    {
                        {"@siteId", siteId},
                        {"@id", id},
                        {"@sameCategory", sameCategory?1:0},
                        {"@special",ingoreSpecial?1:0},
                    };
            var parameters = base.Db.CreateParametersFromArray(pa);

            base.ExecuteReader(
                base.NewQuery(DbSql.Archive_GetNextArchive, parameters),
                func
                );
        }



        /// <summary>
        /// 获取文档的前几篇
        /// </summary>
        /// <param name="id"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        /// 
        public DataTable GetPreviousArchives(string id, int number)
        {
            //
            //TODO:尚未实现
            //

            throw new NotImplementedException();
            //            return base.GetDataSet(@"select top " + number.ToString() + @" ID,Alias,Title from $PREFIX_archive,
            //                                 (select top 1 CID from $PREFIX_archive where ID=@id) as t
            //                                 where $PREFIX_archive.CID=t.CID and ID<@id order by ID desc",
            //                                 {"@id", id}).Tables[0];
        }

        /// <summary>
        /// 获取文档的后几篇
        /// </summary>
        /// <param name="id"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public DataTable GetNextArchives(string id, int number)
        {
            throw new NotImplementedException();

            //
            //TODO:尚未实现
            //

            //按ID计算上一篇
            //            return base.GetDataSet(@"select top " + number.ToString() + @" ID,Alias,Title from $PREFIX_archive,
            //                                 (select top 1 CID from $PREFIX_archive where ID=@id) as t
            //                                 where $PREFIX_archive.CID=t.CID and ID>@id",
            //                                 {"@id", id}).Tables[0];
        }


        /************ 分页 ****************/

        /// <summary>
        /// 获取栏目分页文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="lft"></param>
        /// <param name="pageSize"></param>
        /// <param name="skipSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="rgt"></param>
        /// <param name="recordCount"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        public DataTable GetPagedArchives(
            int siteId, int[] catIdArray,
            int pageSize, int skipSize, ref int currentPageIndex,
            out int recordCount, out int pages)
        {
            object[,] data = new object[,]{
                {"@siteId",siteId},
            };
            String catIdArrayString = this.IntArrayToString(catIdArray);

            String sql = SQLRegex.Replace(DbSql.ArchiveGetPagedArchivesCountSqlPagerqurey, m =>
            {
                switch (m.Groups[1].Value)
                {
                    case "catIdArray": return catIdArrayString;
                }
                return String.Empty;
            });
            //获取记录条数
            recordCount = int.Parse(base.ExecuteScalar(SqlQueryHelper.Format(sql, data)).ToString());

            pages = recordCount / pageSize;
            if (recordCount % pageSize != 0) pages++;

            //验证当前页数
            if (currentPageIndex > pages && currentPageIndex != 1) currentPageIndex = pages;
            if (currentPageIndex < 1) currentPageIndex = 1;

            //计算分页
            int skipCount = pageSize * (currentPageIndex - 1);

            //如果调过记录为0条，且为OLEDB时候，则用sql1
            sql = DbSql.ArchiveGetPagedArchivesByCategoryIdPagerquery;

            sql = SQLRegex.Replace(sql, m =>
            {
                switch (m.Groups[1].Value)
                {
                    case "catIdArray": return catIdArrayString;
                    case "skipsize": return (skipCount + skipSize).ToString();
                    case "pagesize": return pageSize.ToString();
                }
                return null;
            });
            return base.GetDataSet(SqlQueryHelper.Format(sql, data)).Tables[0];
        }

        /// <summary>
        /// 获取栏目分页文档
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="rgt"></param>
        /// <param name="siteId"></param>
        /// <param name="moduleId">参数暂时不使用为-1</param>
        /// <param name="publisherId"></param>
        /// <param name="includeChild"></param>
        /// <param name="flags"></param>
        /// <param name="keyword"></param>
        /// <param name="orderByField"></param>
        /// <param name="orderAsc"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        public DataTable GetPagedArchives(int siteId, int moduleId,
            int[] catIdArray, int publisherId, bool includeChild,
            string[,] flags, string keyword, string orderByField, bool orderAsc,
            int pageSize, int currentPageIndex,
            out int recordCount, out int pages)
        {
            //SQL Condition Template
            const string conditionTpl = "$[siteid]$[module]$[category]$[author_id]$[flags]$[keyword]";

            string condition,                                                             //SQL where condition
                    order = String.IsNullOrEmpty(orderByField) ? "a.sort_number" : orderByField,   //Order filed ( CreateDate | ViewCount | Agree | Disagree )
                    orderType = orderAsc ? "ASC" : "DESC";                                      //ASC or DESC

            string flag = ArchiveFlag.GetSQLString(flags);


            condition = SQLRegex.Replace(conditionTpl, match =>
            {
                switch (match.Groups[1].Value)
                {
                    case "siteid": return String.Format(" c.site_id={0}", siteId.ToString());

                    case "category":
                        if (catIdArray.Length > 0)
                        {
                            if (includeChild &&catIdArray.Length > 1)
                            {
                                return String.Format(" AND cat_id IN({0})", this.IntArrayToString(catIdArray));
                            }
                            return String.Format(" AND cat_id = {0}", catIdArray[0]);
                        }
                        return String.Empty;
                    case "module":
                        return moduleId <= 0 ? ""
             : String.Format(" AND m.id={0}", moduleId.ToString());

                    case "author_id":
                        return publisherId == 0 ? null
       : String.Format(" AND author_id='{0}'", publisherId);

                    case "flags": return String.IsNullOrEmpty(flag) ? "" : " AND " + flag;
                    case "keyword":
                        if (String.IsNullOrEmpty(keyword)) return "";
                        return String.Format(" AND (title LIKE '%{0}%' OR Content LIKE '%{0}%')", keyword);
                }
                return null;
            });

            // throw new Exception(base.OptimizeSql(String.Format(DbSql.Archive_GetpagedArchivesCountSql, condition)));

            //获取记录条数
            recordCount = int.Parse(base.ExecuteScalar(
                base.NewQuery(String.Format(DbSql.ArchiveGetpagedArchivesCountSql, condition), DalBase.EmptyParameter)).ToString());


            pages = recordCount / pageSize;
            if (recordCount % pageSize != 0) pages++;
            //验证当前页数

            if (currentPageIndex > pages && currentPageIndex != 1) currentPageIndex = pages;
            if (currentPageIndex < 1) currentPageIndex = 1;
            //计算分页
            int skipCount = pageSize * (currentPageIndex - 1);


            string sql = DbSql.Archive_GetPagedArchivesByCategoryId;

            sql = SQLRegex.Replace(sql, m =>
            {
                switch (m.Groups[1].Value)
                {
                    case "skipsize": return skipCount.ToString();
                    case "pagesize": return pageSize.ToString();
                    case "condition": return condition;
                    case "orderByField": return order;
                    case "orderASC": return orderType;
                }
                return null;
            });

            //throw new Exception(new SqlQuery(base.OptimizeSQL(sql));
            //throw new Exception(sql+"-"+DbHelper.DbType.ToString()+"-"+new SqlQuery(base.OptimizeSQL(SP.ToString());
            //System.Web.HttpContext.Current.Response.Write(sql);
            //throw new Exception(sql);
            return base.GetDataSet(base.NewQuery(sql,null)).Tables[0];
        }

        private String IntArrayToString(int[] catIdArray)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach(int catId in catIdArray)
            {
                if (i++ > 0) sb.Append(",");
                sb.Append(catId.ToString());
            }
            return sb.ToString();
        }


        #endregion


        #region 搜索相关

        /// <summary>
        /// 搜索关键词相关的内容
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryRgt"></param>
        /// <param name="onlyMatchTitle"></param>
        /// <param name="keyword"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <param name="orderby"></param>
        /// <param name="func"></param>
        /// <param name="categoryLft"></param>
        /// <returns></returns>
        public void SearchArchives(int siteId, int categoryLft, int categoryRgt, bool onlyMatchTitle,
            string keyword, int pageSize, int currentPageIndex, out int recordCount, out int pageCount,
            string orderby, DataReaderFunc func)
        {
            /*
            string condition = ArchiveFlag.GetSQLString(new string[,]{
                    {"st","0"},
                    {"v","1"}
                });
             */

            base.CheckSqlInject(keyword, orderby);

            StringBuilder sb = new StringBuilder(" flags LIKE '%st:''0''%' AND flags LIKE '%v:''1''%' ");
            if (siteId > 0)
            {
                sb.Append(" AND $PREFIX_category.site_id=").Append(siteId.ToString());
            }

            if (categoryLft > 0 && categoryRgt > 0)
            {
                sb.Append(" AND ($PREFIX_category.lft>=").Append(categoryLft.ToString())
                    .Append(" AND $PREFIX_category.rgt<=").Append(categoryRgt).Append(")");
            }

            if (onlyMatchTitle)
            {
                sb.Append(" AND title LIKE '%").Append(keyword).Append("%'");
            }
            else
            {
                sb.Append(" AND ( title LIKE '%").Append(keyword).Append("%' OR outline LIKE '%").Append(keyword).Append("%' OR content LIKE '%").Append(keyword).Append("%')");
            }

            string condition = sb.ToString();

            //排序规则
            if (String.IsNullOrEmpty(orderby)) orderby = String.Intern("ORDER BY $PREFIX_archive.sort_number DESC");


            //记录数
            recordCount = int.Parse(base.ExecuteScalar(SqlQueryHelper.Format(DbSql.Archive_GetSearchRecordCount, condition)).ToString());

            //页数
            pageCount = recordCount / pageSize;
            if (recordCount % pageSize != 0) pageCount++;

            //对当前页数进行验证
            if (currentPageIndex > pageCount && currentPageIndex != 1) currentPageIndex = pageCount;
            if (currentPageIndex < 1) currentPageIndex = 1;

            //跳过记录数
            int skipCount = pageSize * (currentPageIndex - 1);

            //如果调过记录为0条，且为OLEDB时候，则用sql1
            string sql = DbSql.Archive_GetPagedSearchArchives;

            sql = SQLRegex.Replace(sql, (match) =>
            {
                switch (match.Groups[1].Value)
                {
                    case "condition": return condition;
                    case "pagesize": return pageSize.ToString();
                    case "skipsize": return skipCount.ToString();
                    case "orderby": return orderby;
                }
                return null;
            });

            base.ExecuteReader(base.NewQuery(sql,null), func);
        }

        /// <summary>
        /// 在指定栏目下搜索关键词相关的内容
        /// </summary>
        /// <param name="categoryRgt"></param>
        /// <param name="keyword"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <param name="orderby"></param>
        /// <param name="siteId"></param>
        /// <param name="categoryLft"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public void SearchArchivesByCategory(int siteId,
            int categoryLft, int categoryRgt, string keyword,
            int pageSize, int currentPageIndex,
            out int recordCount, out int pageCount,
            string orderby, DataReaderFunc func)
        {

            const string condition = " flags LIKE '%st:''0''%'AND flags LIKE '%v:''1''%' ";


            if (String.IsNullOrEmpty(orderby)) orderby = String.Intern("ORDER BY $PREFIX_archive.sort_number DESC");

            object[,] data = new object[,]
            {
                {"@siteId",siteId},
                {"@lft",categoryLft},
                {"@rgt",categoryRgt}
            };

            //为第一页时
            const string sql1 = @"SELECT TOP $[pagesize] $PREFIX_archive.id AS ID,* 
                                  FROM $PREFIX_archive INNER JOIN $PREFIX_category 
                                  ON $PREFIX_archive.[CgID]=$PREFIX_category.id
                                  WHERE $[condition] AND $PREFIX_category.site_id=@siteId AND ($PREFIX_category.lft>=@lft
                                   AND $PREFIX_category.rgt<=@rgt) AND ([Title] LIKE 
                                  '%$[keyword]%' OR [Outline] LIKE '%$[keyword]%' 
                                   OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '%$[keyword]%')
                                   $[orderby],$PREFIX_archive.id";

            //记录数
            recordCount = int.Parse(base.ExecuteScalar(
                SqlQueryHelper.Format(DbSql.ArchiveGetSearchRecordCountByCategoryId,
                data,
                keyword,
                condition
                )).ToString());

            //页数
            pageCount = recordCount / pageSize;
            if (recordCount % pageSize != 0) pageCount++;

            //对当前页数进行验证
            if (currentPageIndex > pageCount && currentPageIndex != 1) currentPageIndex = pageCount;
            if (currentPageIndex < 1) currentPageIndex = 1;

            //跳过记录数
            int skipCount = pageSize * (currentPageIndex - 1);


            string sql = skipCount == 0 && base.DbType == DataBaseType.OLEDB ?
                sql1 :
                DbSql.ArchiveGetPagedSearchArchivesByCategoryId;

            sql = SQLRegex.Replace(sql, (match) =>
             {
                 switch (match.Groups[1].Value)
                 {
                     case "siteid": return siteId.ToString();
                     case "condition": return condition;
                     case "pagesize": return pageSize.ToString();
                     case "skipsize": return skipCount.ToString();
                     case "keyword": return keyword;
                     case "orderby": return orderby;
                 }
                 return null;
             });


            base.ExecuteReader(SqlQueryHelper.Format(sql, data), func);
        }

        /// <summary>
        /// 在指定模块下搜索关键词相关的内容
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="keyword"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public DataTable SearchByModule(int moduleId, string keyword, int pageSize, int currentPageIndex, out int recordCount, out int pageCount, string orderby)
        {

            const string condition = " flags LIKE '%st:''0''%'AND flags LIKE '%v:''1''%' ";

            if (String.IsNullOrEmpty(orderby)) orderby = String.Intern("ORDER BY $PREFIX_archive.sort_number DESC");

            //为第一页时
            const string sql1 = @"SELECT TOP $[pagesize] $PREFIX_archive.id AS ID,* FROM  $PREFIX_archive INNER JOIN $PREFIX_category ON $PREFIX_archive.[CID]=$PREFIX_category.id
                    WHERE $[condition] AND $PREFIX_category.[ModuleID]=$[module_id] AND ([Title] LIKE '%$[keyword]%' OR [Outline] LIKE '%$[keyword]%' OR [Content] LIKE '%$[keyword]%' OR [Tags] LIKE '%$[keyword]%') $[orderby],$PREFIX_archive.id";

            //记录数
            recordCount = int.Parse(base.ExecuteScalar(
                base.NewQuery(String.Format(DbSql.ArchiveGetSearchRecordCountByModuleId, moduleId,
                keyword, condition), DalBase.EmptyParameter)
                ).ToString());

            //页数
            pageCount = recordCount / pageSize;
            if (recordCount % pageSize != 0) pageCount++;

            //对当前页数进行验证
            if (currentPageIndex > pageCount && currentPageIndex != 1) currentPageIndex = pageCount;
            if (currentPageIndex < 1) currentPageIndex = 1;

            //跳过记录数
            int skipCount = pageSize * (currentPageIndex - 1);

            string sql = skipCount == 0 && base.DbType == DataBaseType.OLEDB ?
                         sql1 :
                        DbSql.ArchiveGetPagedSearchArchivesByModuleId;

            sql = SQLRegex.Replace(sql, (match) =>
            {
                switch (match.Groups[1].Value)
                {
                    case "condition": return condition;
                    case "pagesize": return pageSize.ToString();
                    case "skipsize": return skipCount.ToString();
                    case "keyword": return keyword;
                    case "module_id": return moduleId.ToString();
                    case "orderby": return orderby;
                }
                return null;
            });

            return base.GetDataSet(base.NewQuery(sql,null)).Tables[0];

        }

        #endregion


        /// <summary>
        /// 删除会员发布的文档
        /// </summary>
        /// <param name="id"></param>
        public void DeleteMemberArchives(int id)
        {
            var pa = new object[,]{
                {"@id", id}
                     };
            var parameters = base.Db.CreateParametersFromArray(pa);

            base.ExecuteNonQuery(
                base.NewQuery(DbSql.ArchiveDeleteMemberArchives, parameters) );
        }

        public int TransferPublisher(int userId, int toPublisherId)
        {
            var pa = new object[,]{
                        {"@toPublisherId", toPublisherId},
                        {"@publisherId", userId}
                     };
            var parameters = base.Db.CreateParametersFromArray(pa);

            return base.ExecuteNonQuery(
                 base.NewQuery(DbSql.ArchiveTransferPublisherId,parameters)
                    
                 );
        }

        public int GetCategoryArchivesCount(string id)
        {
            return int.Parse(base.ExecuteScalar(
                base.NewQuery(SQLRegex.Replace(DbSql.Archive_GetCategoryArchivesCount, (c) => { return id; }),null)).ToString());

        }



        public void GetSelftAndChildArchiveExtendValues(int siteId, int relationType,int[] catIdArray, int number, int skipSize, DataReaderFunc func)
        {
            String sql = DbSql.Archive_GetSelfAndChildArchiveExtendValues;
            sql = SQLRegex.Replace(sql, m =>
            {
                switch (m.Groups[1].Value)
                {
                    case "catIdArray":
                        return this.IntArrayToString(catIdArray);
                }
                return null;
            });

            IDictionary<String, object> paramters = new Dictionary<string, object>();
            paramters.Add("@siteId", siteId);
            paramters.Add("@relationType", relationType);
            SqlQuery query = new SqlQuery(String.Format(base.OptimizeSql(sql), skipSize, number), paramters);
            base.ExecuteReader(query, func);
        }

        public void GetArchivesExtendValues(int siteId, int relationType, string categoryTag, int number, int skipSize,
            DataReaderFunc func)
        {
            base.ExecuteReader(
                SqlQueryHelper.Format(DbSql.Archive_GetArchivesExtendValues,
                    new object[,]
                    {
                        {"@siteId", siteId},
                        {"@tag", categoryTag},
                        {"@relationType", relationType}
                    }, skipSize.ToString(),
                    number.ToString()
                    ), func);
        }


        public int GetMaxSortNumber(int siteId)
        {
            object obj = base.ExecuteScalar(
                SqlQueryHelper.Format(DbSql.ArchiveGetMaxSortNumber,
                    new object[,]
                    {
                        {"@siteId", siteId},
                    })).ToString();
            if (obj == DBNull.Value || (string)obj == "") return 0;
            return int.Parse(obj.ToString());
        }

        public void SaveSortNumber(int archiveId, int sortNumber)
        {
            base.ExecuteNonQuery(
                SqlQueryHelper.Format(DbSql.ArchiveUpdateSortNumber,
                    new object[,]
                    {
                        {"@archiveId", archiveId},
                        {"@sort_number",sortNumber},
                    }));
        }
    }
}