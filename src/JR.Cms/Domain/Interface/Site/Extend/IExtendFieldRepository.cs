using System.Collections.Generic;
using System.Data.Common;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Infrastructure;
using JR.Stand.Core.Extensions;

namespace JR.Cms.Domain.Interface.Site.Extend
{
    /// <summary>
    /// 
    /// </summary>
    public interface IExtendFieldRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="extendField"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IExtendValue CreateExtendValue(IExtendField extendField, int id, string value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        IExtendField CreateExtendField(int id, string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        IList<IExtendField> GetAllExtendsBySiteId(int siteId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="extendId"></param>
        /// <returns></returns>
        IExtendField GetExtendFieldById(int siteId, int extendId);

        IEnumerable<IExtendField> GetExtendFields(int siteId, int categoryId);

        Error SaveExtendField(int siteId, IExtendField extendField);

        bool DeleteExtendField(int fieldId, int extendFieldId);

        IList<IExtendValue> GetExtendFieldValues(IArchive archive);

        void UpdateArchiveRelationExtendValues(IArchive archive);

        IDictionary<int, IList<IExtendValue>> _GetExtendValuesFromDataReader(int siteId, DbDataReader rd);

        IDictionary<int, IList<IExtendValue>> GetExtendFieldValuesList(int siteId, ExtendRelationType type,
            IList<int> idList);

        int GetCategoryExtendRefrenceNum(ICategory category, int extendId);

        /// <summary>
        /// 更新栏目的扩展属性
        /// </summary>
        /// <param name="category"></param>
        void UpdateCategoryExtends(int catId, int[] extendIdArray);

        IExtendField GetExtendByName(int siteId, string name, string type);
    }
}