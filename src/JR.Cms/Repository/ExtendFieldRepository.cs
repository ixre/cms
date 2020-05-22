using System;
using System.Collections.Generic;
using System.Data.Common;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Site.Extend;
using JR.Cms.Infrastructure;
using JR.Cms.Library.DataAccess.DAL;

namespace JR.Cms.Repository
{
    public class ExtendFieldRepository : BaseExtendFieldRepository, IExtendFieldRepository
    {
        private readonly ExtendFieldDal _extendDal = new ExtendFieldDal();

        //缓存数据
        private IDictionary<int, IList<IExtendField>> dicts;

        public IList<IExtendField> GetAllExtendsBySiteId(int siteId)
        {
            if (dicts == null)
            {
                dicts = new Dictionary<int, IList<IExtendField>>();
                IExtendField field;
                int inSiteId;

                _extendDal.GetAllExtendFields(rd =>
                {
                    while (rd.Read())
                    {
                        inSiteId = int.Parse(rd["site_id"].ToString());

                        if (!dicts.ContainsKey(inSiteId)) dicts.Add(inSiteId, new List<IExtendField>());

                        field = CreateExtendField(int.Parse(rd["id"].ToString()), rd["name"].ToString());
                        field.Message = Convert.ToString(rd["message"]);
                        field.DefaultValue = Convert.ToString(rd["default_value"]);
                        field.Regex = Convert.ToString(rd["regex"]);
                        field.Type = Convert.ToString(rd["type"]);

                        dicts[inSiteId].Add(field);
                    }
                });
            }

            return dicts.ContainsKey(siteId) ? dicts[siteId] : new List<IExtendField>();
        }

        public Error SaveExtendField(int siteId, IExtendField extendField)
        {
            try
            {
                if (extendField.GetDomainId() > 0)
                    _extendDal.UpdateExtendField(siteId, extendField);
                else
                    _extendDal.AddExtendField(siteId, extendField);
                dicts = null;
            }
            catch (Exception ex)
            {
                return new Error(ex.Message);
            }

            return null;
        }

        public IExtendField GetExtendFieldById(int siteId, int extendId)
        {
            var list = GetAllExtendsBySiteId(siteId);
            foreach (var extend in list)
                if (extend.GetDomainId() == extendId)
                    return extend;
            return null;
        }

        public bool DeleteExtendField(int siteId, int extendFieldId)
        {
            return _extendDal.DeleteExtendField(siteId, extendFieldId);
        }


        public IList<IExtendValue> GetExtendFieldValues(IArchive archive)
        {
            int extendId;
            var siteId = archive.Category.Site().GetAggregateRootId();
            IDictionary<int, IExtendValue> extendValues = new Dictionary<int, IExtendValue>();
            foreach (var field in archive.Category.ExtendFields)
                if (!extendValues.Keys.Contains(field.GetDomainId()))
                    extendValues.Add(field.GetDomainId(), CreateExtendValue(field, -1, null));

            _extendDal.GetExtendValues(siteId, (int) ExtendRelationType.Archive, archive.GetAggregateRootId(), rd =>
            {
                while (rd.Read())
                {
                    extendId = int.Parse(rd["field_id"].ToString());

                    if (extendValues.ContainsKey(extendId))
                        //extendValues[extendId].Id = int.Parse(rd["id"].ToString());  //todo:fix
                        extendValues[extendId].Value = rd["field_value"].ToString().Replace("\n", "<br />");
                }
            });

            return new List<IExtendValue>(extendValues.Values);
        }

        public IDictionary<int, IList<IExtendValue>> GetExtendFieldValuesList(int siteId, ExtendRelationType type,
            IList<int> idList)
        {
            int fieldId;
            int relationId;
            IDictionary<int, IList<IExtendValue>> extendValues = new Dictionary<int, IList<IExtendValue>>();

            _extendDal.GetExtendValuesList(siteId, (int) ExtendRelationType.Archive, idList, rd =>
            {
                while (rd.Read())
                {
                    relationId = int.Parse(rd["relation_id"].ToString());
                    fieldId = int.Parse(rd["field_id"].ToString());
                    if (!extendValues.ContainsKey(relationId)) extendValues.Add(relationId, new List<IExtendValue>());
                    extendValues[relationId].Add(
                        new ExtendValue(int.Parse(rd["id"].ToString()),
                            GetExtendFieldById(siteId, fieldId),
                            rd["field_value"].ToString().Replace("\n", "<br />")
                        )
                    );
                }
            });
            return extendValues;
        }

        public IEnumerable<IExtendField> GetExtendFields(int siteId, int categoryId)
        {
            var list = GetAllExtendsBySiteId(siteId);
            var ids = _extendDal.GetCategoryExtendIdList(siteId, categoryId);
            foreach (var field in list)
                if (field != null && Array.Exists(ids, a => a == field.GetDomainId()))
                    yield return field;
        }


        public void UpdateArchiveRelationExtendValues(IArchive archive)
        {
            var siteId = archive.Category.Site().GetAggregateRootId();
            //============ 更新 ============
            IDictionary<int, string> extendValues = new Dictionary<int, string>();
            IExtendField field;
            foreach (var value in archive.GetExtendValues())
            {
                field = GetExtendFieldById(siteId, value.Field.GetDomainId());

                //如果为默认数据，也要填写进去,以免模板获取不到
                if (value.Value != null)
                    extendValues.Add(value.Field.GetDomainId(),
                        //value.Value == field.DefaultValue ? String.Empty : value.Value);
                        value.Value);
            }

            _extendDal.InsertDataExtendFields(ExtendRelationType.Archive, archive.GetAggregateRootId(), extendValues);

            /*
            foreach (DataExtendField f in this.GetExtendFileds(relationID))
            {
                if (extendData.ContainsKey(f.AttrId))
                {
                    dal.UpdateDataExtendFieldValue(relationID, f.AttrId, extendData[f.AttrId]);

                    extendData.Remove(f.AttrId);
                }
            }

            //=========== 增加 ===========
            dal.InsertDataExtendFields(relationID, extendData);
           */
        }

        [Obsolete]
        public IDictionary<int, IList<IExtendValue>> _GetExtendValuesFromDataReader(int siteId, DbDataReader rd)
        {
            IDictionary<int, IList<IExtendValue>> extendValues = null;

            while (rd.Read())
            {
                if (extendValues == null) extendValues = new Dictionary<int, IList<IExtendValue>>();
                int relationId;
                if (!extendValues.ContainsKey(relationId = int.Parse(rd["relation_id"].ToString())))
                    extendValues.Add(relationId, new List<IExtendValue>());
                var extendId = int.Parse(rd["extendFieldId"].ToString());
                extendValues[relationId].Add(
                    new ExtendValue(int.Parse(rd["id"].ToString()),
                        GetExtendFieldById(siteId, extendId),
                        (rd["extendFieldValue"] ?? "").ToString()
                    ));
            }

            return extendValues;
        }


        public int GetCategoryExtendRefrenceNum(ICategory category, int extendId)
        {
            return _extendDal.GetCategoryExtendRefrenceNum(category.Site().GetAggregateRootId(), category.GetDomainId(),
                extendId);
        }


        public void UpdateCategoryExtends(int catId, int[] extendIdArray)
        {
            _extendDal.UpdateCategoryExtendsBind(catId, extendIdArray);
        }


        public IExtendField GetExtendByName(int siteId, string name, string type)
        {
            IExtendField field = null;
            _extendDal.GetExtendFieldByName(siteId, name, type, rd =>
            {
                if (rd.Read())
                {
                    field = CreateExtendField(int.Parse(rd["id"].ToString()), name);
                    field.Message = Convert.ToString(rd["message"]);
                    field.DefaultValue = Convert.ToString(rd["default_value"]);
                    field.Regex = Convert.ToString(rd["regex"]);
                    field.Type = type;
                }
            });
            return field;
        }
    }
}