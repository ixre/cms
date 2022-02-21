using NHibernate;

namespace JR.Cms.Core.Hibernate
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseDatabaseSession
    {
        /// <summary>
        /// 会话工厂
        /// </summary>
        private readonly ISessionFactory _sessionFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        protected BaseDatabaseSession(ISessionFactory factory)
        {
            this._sessionFactory = factory;
        }
    }
}