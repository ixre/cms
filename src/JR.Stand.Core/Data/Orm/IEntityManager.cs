using System.Collections.Generic;

namespace JR.Stand.Core.Data.Orm
{
    public interface IEntityManager<Entity> where Entity : class
    {
        void Insert(Entity entity);
        void Delete(FieldValueDictionary field);
        void Update(PrimaryKeyValueDictionary parmaryField, params FieldValueDictionary[] filds);
        Entity Get(PrimaryKeyValueDictionary parmaryField);
        Entity Get(FieldValueDictionary parmaryField);
        IEnumerable<Entity> GetEntityList(params FieldValueDictionary[] fileds);
    }
}