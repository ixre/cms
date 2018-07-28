using System.Collections.Generic;
using System.Data.Common;
using T2.Cms.Domain.Interface.Content.Archive;
using T2.Cms.Domain.Interface.Site.Category;

namespace T2.Cms.Domain.Interface.Site.Extend
{
    public interface IExtendFieldRepository
    {
        IExtendValue CreateExtendValue(IExtendField extendField, int id, string value);

        IExtendField CreateExtendField(int id, string name);

        IList<IExtendField> GetAllExtendsBySiteId(int siteId);

        IExtendField GetExtendFieldById(int siteId, int extendId);

        IEnumerable<IExtendField> GetExtendFields(int siteId, int categoryId);

        int SaveExtendField(int siteId, IExtendField extendField);

        bool DeleteExtendField(int fieldId,int extendFieldId);

        IList<IExtendValue> GetExtendFieldValues(IArchive archive);

        void UpdateArchiveRelationExtendValues(IArchive archive);

        IDictionary<int, IList<IExtendValue>> _GetExtendValuesFromDataReader(int siteId,DbDataReader rd);

        IDictionary<int, IList<IExtendValue>> GetExtendFieldValuesList(int siteId, ExtendRelationType type, IList<int> idList);

        int GetCategoryExtendRefrenceNum(ICategory category, int extendId);

        /// <summary>
        /// 更新栏目的扩展属性
        /// </summary>
        /// <param name="category"></param>
        void UpdateCategoryExtends(ICategory category);

        IExtendField GetExtendByName(int siteId, string name, string type);
    }
}
