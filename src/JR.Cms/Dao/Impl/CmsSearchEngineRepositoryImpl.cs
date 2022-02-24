/**
 * Copyright (C) 2009-2022 56X.NET, All rights reserved.
 *
 * name : CmsSearchEngineRepositoryImpl.cs
 * author : jarrysix
 * date : 2022/02/24 12:31:22
 * description :
 * history :
 */

using System.Collections.Generic;
using System.Data;
using Dapper;
using JR.Cms.Domain.Interface.Models;
using JR.Stand.Core.Data.Provider;

namespace JR.Cms.Dao.Impl{
    /** 搜索引擎设置仓储接口 */
    public class CmsSearchEngineRepositoryImpl : ICmsSearchEngineRepository{
        private readonly IDbProvider _provider;
    
        /// <summary>
        /// 创建仓储对象
        /// </summary>
        public CmsSearchEngineRepositoryImpl(IDbProvider provider)
        {
            this._provider = provider;
        }
            
        /// <summary>
        /// 获取所有搜索引擎设置
        /// </summary>
        /// <returns></returns>
        public IList<CmsSearchEngineEntity> FindAll()
        {
           using (IDbConnection db = _provider.GetConnection())
           {
               return db.Query<CmsSearchEngineEntity>(_provider.FormatQuery("SELECT * FROM $PREFIX_search_engine")).AsList();
           } 
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
                           site_id,baidu_site_token
                        ) VALUES(
                          @SiteId,@BaiduSiteToken
                        )"),
                    e);
                    return e.Id;
                }

                db.Execute(
                    _provider.FormatQuery(
                    @"UPDATE $PREFIX_search_engine SET 
                     site_id = @SiteId, baidu_site_token = @BaiduSiteToken  
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
                return db.QueryFirst<CmsSearchEngineEntity>(_provider.FormatQuery("SELECT * FROM $PREFIX_search_engine WHERE id = @Id"),
                    new CmsSearchEngineEntity{
                      Id = id, 
                    });
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