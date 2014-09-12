using Ops.Cms.DAL;
using Ops.Cms.Domain.Implement.Site.Extend;
using Ops.Cms.Domain.Interface.Content.Archive;
using Ops.Cms.Domain.Interface.Site.Category;
using Ops.Cms.Domain.Interface.Site.Extend;
using Ops.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Ops.Cms.ServiceRepository
{
    public class ExtendFieldRepository : BaseExtendFieldRepository, IExtendFieldRepository
    {
        private ExtendFieldDAL extendDAL = new ExtendFieldDAL();


        //缓存数据
        private IDictionary<int, IList<IExtendField>> dicts;

        public IList<IExtendField> GetAllExtendsBySiteId(int siteId)
        {

            if (dicts == null)
            {

                dicts = new Dictionary<int, IList<IExtendField>>();
                IExtendField field;
                int _siteId;

                extendDAL.GetAllExtendFields(rd =>
                {
                    while (rd.Read())
                    {
                        _siteId = int.Parse(rd["siteId"].ToString());

                        if (!dicts.ContainsKey(_siteId))
                        {
                            dicts.Add(_siteId, new List<IExtendField>());
                        }

                        field = this.CreateExtendField(int.Parse(rd["id"].ToString()), rd["name"].ToString());
                        rd.CopyToEntity(field);

                        dicts[_siteId].Add(field);
                    }
                });
            }

            return dicts.ContainsKey(siteId) ? dicts[siteId] : new List<IExtendField>();

        }

        public int SaveExtendField(int siteId, IExtendField extendField)
        {

            if (extendField.ID > 0)
            {
                extendDAL.UpdateExtendField(siteId, extendField);
            }
            else
            {
                extendDAL.AddExtendField(siteId, extendField);
            }

            //清理
            this.dicts = null;

            return extendField.ID;
        }

        public IExtendField GetExtendFieldById(int siteId, int extendId)
        {
            IList<IExtendField> list = this.GetAllExtendsBySiteId(siteId);
            foreach (IExtendField extend in list)
            {
                if (extend.ID == extendId) return extend;
            }
            return null;
        }

        public bool DeleteExtendField(int siteId,int extendFieldId)
        {
            return this.extendDAL.DeleteExtendField(siteId, extendFieldId);
        }


        public IList<IExtendValue> GetExtendFieldValues(IArchive archive)
        {
            int extendId;
            int siteId = archive.Category.Site.ID;
            IDictionary<int, IExtendValue> extendValues =
                new Dictionary<int, IExtendValue>();
            foreach (IExtendField field in archive.Category.ExtendFields)
            {
                extendValues.Add(field.ID, this.CreateExtendValue(field, -1, null));
            }

            this.extendDAL.GetExtendValues(siteId,(int)ExtendRelationType.Archive, archive.ID, rd =>
            {
                while (rd.Read())
                {
                    extendId = int.Parse(rd["fieldId"].ToString());

                    if (extendValues.ContainsKey(extendId))
                    {
                        extendValues[extendId].ID = int.Parse(rd["id"].ToString());
                        extendValues[extendId].Value = rd["fieldValue"].ToString();
                    }

                }
            });

            return new List<IExtendValue>(extendValues.Values);
        }

        public IDictionary<int, IList<IExtendValue>> GetExtendFieldValuesList(int siteId, ExtendRelationType type, IList<int> idList)
        {
            int fieldId;
            int relationId;
            IDictionary<int, IList<IExtendValue>> extendValues = new Dictionary<int, IList<IExtendValue>>();

            this.extendDAL.GetExtendValuesList(siteId, (int)ExtendRelationType.Archive, idList, rd =>
            {
                while (rd.Read())
                {
                    relationId = int.Parse(rd["relationId"].ToString());
                    fieldId = int.Parse(rd["fieldId"].ToString());
                    if (!extendValues.ContainsKey(relationId))
                    {
                        extendValues.Add(relationId, new List<IExtendValue>());
                    }
                    extendValues[relationId].Add(
                        new ExtendValue(int.Parse(rd["id"].ToString()),
                            this.GetExtendFieldById(siteId, fieldId),
                            rd["fieldValue"].ToString()
                        )

                        );
                }
            });
            return extendValues;
        }

        public IEnumerable<IExtendField> GetExtendFields(int siteId, int categoryId)
        {
            IList<IExtendField> list = this.GetAllExtendsBySiteId(siteId);
            int[] ids = this.extendDAL.GetCategoryExtendIdList(siteId, categoryId);
            foreach (IExtendField field in list)
            {
                if (Array.Exists(ids, a => a == field.ID))
                    yield return field;
            }
        }


        public void UpdateArchiveRelationExtendValues(IArchive archive)
        {

            int siteId = archive.Category.Site.SiteId;
            //============ 更新 ============
            IDictionary<int, string> extendValues = new Dictionary<int, string>();
            IExtendField field;
            foreach (IExtendValue value in archive.ExtendValues)
            {
                field = this.GetExtendFieldById(siteId, value.Field.ID);

                if (value.Value != null)
                    extendValues.Add(value.Field.ID,
                    value.Value == field.DefaultValue ? String.Empty : value.Value);
            }

            this.extendDAL.InsertDataExtendFields(ExtendRelationType.Archive, archive.ID, extendValues);

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
        public IDictionary<int, IList<IExtendValue>> _GetExtendValuesFromDataReader(int siteId,DbDataReader rd)
        {
            IDictionary<int, IList<IExtendValue>> extendValues = new Dictionary<int, IList<IExtendValue>>();

            int extendId;
            int relationId;

            while (rd.Read())
            {
                if (!extendValues.ContainsKey(relationId = int.Parse(rd["relationId"].ToString())))
                {
                    extendValues.Add(relationId, new List<IExtendValue>());
                }
                extendId = int.Parse(rd["extendFieldId"].ToString());
                extendValues[relationId].Add(
                    new ExtendValue(int.Parse(rd["id"].ToString()),
                    this.GetExtendFieldById(siteId, extendId),
                    (rd["extendFieldValue"] ?? "").ToString()
                    ));
            }

            return extendValues;
        }



        public int GetCategoryExtendRefrenceNum(ICategory category, int extendId)
        {
            return this.extendDAL.GetCategoryExtendRefrenceNum(category.Site.ID, category.ID, extendId);
        }


        public void UpdateCategoryExtends(ICategory category)
        {
            int[] extendIds = new int[category.ExtendFields.Count];
            int i=0;
            foreach(IExtendField field in category.ExtendFields)
            {
                extendIds[i++] = field.ID;
            }
            this.extendDAL.UpdateCategoryExtendsBind(category.ID, extendIds);
        }
    }
}
