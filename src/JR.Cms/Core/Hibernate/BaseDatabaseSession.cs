using System.Collections.Generic;
using System.Linq;
using JR.Cms.Domain.Interface.Models;
using NHibernate;
using NHibernate.Cfg;
using Settings = JR.Cms.Conf.Settings;


namespace JR.Cms.Core.Hibernate
{
    /// <summary>
    /// 会话工厂
    /// </summary>
    public interface IDatabaseSessionFactory
    {
        /// <summary>
        /// 获取Session工厂
        /// </summary>
        /// <returns></returns>
        ISessionFactory GetFactory();
    }

    /// <summary>
    /// 
    /// </summary>
    public class DatabaseSessionFactoryImpl : IDatabaseSessionFactory
    {
        /// <summary>
        /// 会话工厂
        /// </summary>
        private  ISessionFactory _sessionFactory;
        /// <summary>
        /// 获取Session工厂
        /// </summary>
        /// <returns></returns>
        public ISessionFactory GetFactory()
        {
            return this._sessionFactory ?? (this._sessionFactory = (new Configuration()).Configure()
                .DataBaseIntegration(db => { db.ConnectionString = Settings.DB_CONN; })
                .BuildSessionFactory());
        }
    }
    
    
    /// <summary>
    /// 
    /// </summary>
    public abstract class DatabaseObjectAccess<T>
    {
        private readonly IDatabaseSessionFactory _factory;

        /// <summary>
        /// 
        /// </summary>
        protected DatabaseObjectAccess(IDatabaseSessionFactory factory)
        {
            this._factory = factory;
        }

        protected List<T> FindAll()
        {
            ISession session = this._factory.GetFactory().OpenSession();
           return session.Query<T>();
        }
    }
}