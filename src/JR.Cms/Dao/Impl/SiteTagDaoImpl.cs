using System.Collections.Generic;
using JR.Cms.Core.Hibernate;
using JR.Cms.Domain.Interface.Models;
using JR.Cms.Infrastructure;
using NHibernate;

namespace JR.Cms.Dao.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteTagDaoImpl:BaseDatabaseSession,ISiteTagDao
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<SiteTag> GetTags()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Error SaveTag(SiteTag tag)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Error DeleteTag(SiteTag tag)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        public SiteTagDaoImpl(ISessionFactory factory) : base(factory)
        {
        }
    }
}