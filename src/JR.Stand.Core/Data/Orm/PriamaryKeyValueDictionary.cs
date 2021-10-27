using System.Reflection;

namespace JR.Stand.Core.Data.Orm
{
    public class PrimaryKeyValueDictionary : FieldValueDictionary
    {
        public PrimaryKeyValueDictionary()
        {
        }

        public PrimaryKeyValueDictionary(PropertyInfo proper, object value)
        {
            base.Field = proper;
            base.Value = value;
        }

        public override bool IsPrimaryKeyField
        {
            get
            {
                if (!base.IsPrimaryKeyField)
                    throw new FieldValueException("不是主键不能用做选取和删除操作!");
                return true;
            }
        }
    }
}