using System;
using System.Collections.Generic;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Variable;

namespace JR.Cms.Domain.Site.Variable
{
    /// <summary>
    /// 站点变量管理器实现
    /// </summary>
    public class VariableManagerImpl:ISiteVariableManager
    {
        private readonly ISiteRepo _repo;
        private readonly ISite _site;
        private IList<SiteVariable> _cache;
        private IDictionary<String, SiteVariable> _mapper;

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="siteRepository"></param>
        /// <param name="site"></param>
        public VariableManagerImpl(ISiteRepo siteRepository, ISite site)
        {
            this._repo = siteRepository;
            this._site = site;
        }

        /// <summary>
        /// 保存变量
        /// </summary>
        /// <param name="v"></param>
        public void SaveVariable(SiteVariable v)
        {
            if (v.Id <= 0 && this.Get(v.Name) != null)
            {
                throw new ArgumentException("已存在相同的变量");
            }
            this._repo.SaveSiteVariable(this._site.GetAggregateRootId(), v);
            this._cache = null;
        }

        /// <summary>
        /// 删除变量
        /// </summary>
        /// <param name="varId"></param>
        public void DeleteVariable(int varId)
        {
            this._repo.DeleteSiteVariable(this._site.GetAggregateRootId(), varId);
            this._cache = null;
        }

        /// <summary>
        /// 获取所有的变量
        /// </summary>
        /// <returns></returns>
        public IList<SiteVariable> GetAll()
        {
            if (this._cache == null)
            {
                this._cache = this._repo.GetSiteVariables(this._site.GetAggregateRootId());
                this._mapper = new Dictionary<string, SiteVariable>();
                foreach (var v in this._cache) this._mapper.Add(v.Name, v);
            }

            return this._cache;
        }

        /// <summary>
        /// 根据名称获取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SiteVariable Get(string name)
        {
            this.GetAll();
            if(this._mapper.ContainsKey(name)) return this._mapper[name];
            return null;
        }
    }
}