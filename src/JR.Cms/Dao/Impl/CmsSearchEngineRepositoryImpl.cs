/**
 * Copyright (C) 2009-2022 56X.NET, All rights reserved.
 *
 * name : CmsSearchEngineRepositoryImpl.cs
 * author : jarrysix
 * date : 2022/02/25 13:17:26
 * description :
 * history :
 */

using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using JR.Cms.Domain.Interface.Models;
using JR.Stand.Core.Data.Provider;

namespace JR.Cms.Dao.Impl
{
    /** 搜索引擎设置仓储接口 */
    public class CmsSearchEngineRepositoryImpl : ICmsSearchEngineRepository{
        private readonly IDbProvider _provider;
        private readonly String _fieldAliases = @"
            id as Id,
            site_id as SiteId,
            url_prefix as UrlPrefix,
            baidu_site_token as BaiduSiteToken
            ";
    
        /// <summary>
        /// 创建仓储对象
        /// </summary>
        public CmsSearchEngineRepositoryImpl(IDbProvider provider)
        {
            this._provider = provider;
        }
             
        /// <summary>
        /// 保存搜索引擎设置
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public long Save(CmsSearchEngineEntity e)
        {
            using (IDbConnection db = _provider.GetConnection())
            {
                if (e.Id == 0)
                {
                    int i = db.Execute(_provider.FormatQuery(
                        @"INSERT INTO $PREFIX_search_engine(
                           site_id,
                           url_prefix,
                           baidu_site_token
                           ) VALUES(
                          @SiteId,
                          @UrlPrefix,
                          @BaiduSiteToken
                          )"),
                    e);
                    return e.Id;
                }

                db.Execute(
                    _provider.FormatQuery(
                    @"UPDATE $PREFIX_search_engine SET 
                     site_id = @SiteId,
                     url_prefix = @UrlPrefix,
                     baidu_site_token = @BaiduSiteToken
                     WHERE id=@Id"),
                    e);
                return e.Id;
            }
        }

         /// <summary>
         /// 根据ID获取搜索引擎设置
         /// </summary>
         /// <param name="id"></param>
         /// <returns></returns>
         public CmsSearchEngineEntity FindById(long id)
         {
            using (IDbConnection db = _provider.GetConnection())
            {
                return db.QueryFirstOrDefault<CmsSearchEngineEntity>(_provider.FormatQuery($@"SELECT {_fieldAliases} FROM $PREFIX_search_engine WHERE id = @Id"),
                    new CmsSearchEngineEntity{
                      Id = id, 
                    });
            }  
         }
         
         /// <summary>
         /// 根据条件查找搜索引擎设置
         /// </summary>
         /// <param name="where"></param>
         /// <returns></returns>
         public CmsSearchEngineEntity FindBy(string where)
         {
             using (IDbConnection db = _provider.GetConnection())
             {
                 return db.QueryFirst<CmsSearchEngineEntity>(_provider.FormatQuery($@"SELECT {_fieldAliases} FROM $PREFIX_search_engine WHERE ${where}"));
             }  
         }         
       
        /// <summary>
        /// 获取所有搜索引擎设置
        /// </summary>
        /// <returns></returns>
        public IList<CmsSearchEngineEntity> FindAll()
        {
           using (IDbConnection db = _provider.GetConnection())
           {
               return db.Query<CmsSearchEngineEntity>(_provider.FormatQuery($@"SELECT {_fieldAliases} FROM $PREFIX_search_engine")).AsList();
           } 
        }
              
         /// <summary>
         /// 删除搜索引擎设置
         /// </summary>
         public int DeleteById(long id)
         {
            using (IDbConnection db = _provider.GetConnection())
            {
                 return db.Execute(
                     _provider.FormatQuery(
                         "DELETE FROM $PREFIX_search_engine WHERE id = @Id"),
                     new CmsSearchEngineEntity{
                       Id = id, 
                     });
            }
         }
    }
}