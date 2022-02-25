/**
 * Copyright (C) 2009-2022 56X.NET, All rights reserved.
 *
 * name : CmsSearchEngineServiceImpl.cs
 * author : jarrysix
 * date : 2022/02/24 12:31:22
 * description :
 * history :
 */

using System;
using System.Collections.Generic;
using JR.Cms.Dao;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Repository;
using JR.Cms.ServiceContract;
using JR.Stand.Core.Extensions;

namespace JR.Cms.ServiceImpl
{
    /// <summary>
    /// 搜索引擎设置服务
    /// </summary>
    public class CmsSearchEngineServiceImpl : ICmsSearchEngineService
    {
        private readonly ICmsSearchEngineRepository _repo;

        /// <summary>
        /// 创建服务实例
        /// </summary>
        /// <param name="repo">仓储</param>
        public CmsSearchEngineServiceImpl(ICmsSearchEngineRepository repo)
        {
            this._repo = repo;
        }

        /// <summary>
        /// 查找搜索引擎设置
        /// </summary>
        /// <returns></returns>
        public CmsSearchEngineEntity FindSearchEngineById(long id)
        {
            return this._repo.FindById(id);
        }

        /// <summary>
        /// 查找全部搜索引擎设置
        /// </summary>
        /// <returns></returns>
        public IList<CmsSearchEngineEntity> FindAllSearchEngine()
        {
            return this._repo.FindAll();
        }

        /// <summary>
        /// 保存搜索引擎设置
        /// </summary>
        public Error SaveSearchEngine(CmsSearchEngineEntity e)
        {
            try
            {
                CmsSearchEngineEntity dst;
                if (e.Id > 0)
                {
                    dst = this._repo.FindById(e.Id);
                    if (dst == null) throw new Exception("no such data");
                }
                else
                {
                    dst = CmsSearchEngineEntity.CreateDefault();

                }

                dst.SiteId = e.SiteId;
                dst.SiteUrl = e.SiteUrl;
                dst.BaiduSiteToken = e.BaiduSiteToken;
                this._repo.Save(dst);
            }
            catch (Exception ex)
            {
                return new Error((ex.InnerException ?? ex).Message);
            }

            return null;
        }

        /// <summary>
        /// 删除搜索引擎设置
        /// </summary>
        public int DeleteSearchEngineById(long id)
        {
            return this._repo.DeleteById(id);
        }

        public CmsSearchEngineEntity FindSearchEngineBySiteId(int siteId)
        {
            return this._repo.FindBy($"site_id={siteId}");
        }
    }
}