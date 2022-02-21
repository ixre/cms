using NHibernate;
using NHibernate.Cfg;
using Settings = JR.Cms.Conf.Settings;

namespace JR.Cms.Dao
{
    /// <summary>
    /// HBN数据源
    /// </summary>
    public class HibernateDataSourceImpl
    {
        private ISessionFactory _sessionFactory;
     
        private static ISessionFactory GetSessionFactory()
        {
                return (new Configuration()).Configure()
                    .DataBaseIntegration(db => { db.ConnectionString = Settings.DB_CONN; })
                    .BuildSessionFactory();
        }

        public ISession GetSession()
        {
            if (_sessionFactory == null)
            {
                _sessionFactory = this.GetSessionFactory();
            }
            return _sessionFactory.OpenSession();
        } 
    }
}