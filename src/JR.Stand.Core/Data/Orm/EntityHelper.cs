using System;
using System.Reflection;
using JR.Stand.Core.Data.Orm.Mapping;

namespace JR.Stand.Core.Data.Orm
{
    public static class EntityHelper
    {
        public static IEntityManager<Entity> GetManager<Entity>(DataBaseAccess db) where Entity : class
        {
            return new EntityManager<Entity>(db);
        }

        public static PropertyInfo GetProperty<Entity>(string properName) where Entity : class
        {
            Type type = typeof (Entity);
            PropertyInfo p = type.GetProperty(properName);
            if (p == null) throw new DataMappingException("该实体:" + type.ToString() + "没有属性:" + properName);
            return p;
        }
    }
}