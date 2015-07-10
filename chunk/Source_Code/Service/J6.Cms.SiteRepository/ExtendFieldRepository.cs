using System;
using System.Collections.Generic;
using System.Data.Common;
using J6.Cms.DAL;
using J6.Cms.Domain.Implement.Site.Extend;
using J6.Cms.Domain.Interface.Content.Archive;
using J6.Cms.Domain.Interface.Site.Category;
using J6.Cms.Domain.Interface.Site.Extend;
using J6.DevFw.Data.Extensions;

namespace J6.Cms.ServiceRepository
{
    public class ExtendFieldRepository : BaseExtendFieldRepository, IExtendFieldRepository
    {
        private ExtendFieldDal extendDAL = new ExtendFieldDal();

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

            if (extendField.Id > 0)
            {
                extendDAL.UpdateExtendField(siteId, extendField);
            }
            else
            {
                extendDAL.AddExtendField(siteId, extendField);
            }

            //清理
            this.dicts = null;

            return extendField.Id;
        }

        public IExtendField GetExtendFieldById(int siteId, int extendId)
        {
            IList<IExtendField> list = this.GetAllExtendsBySiteId(siteId);
            foreach (IExtendField extend in list)
            {
                if (extend.Id == extendId) return extend;
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
            int siteId = archive.Category.Site.Id;
            IDictionary<int, IExtendValue> extendValues =
                new Dictionary<int, IExtendValue>();
            foreach (IExtendField field in archive.Category.ExtendFields)
            {
                extendValues.Add(field.Id, this.CreateExtendValue(field, -1, null));
            }

            this.extendDAL.GetExtendValues(siteId,(int)ExtendRelationType.Archive, archive.Id, rd =>
            {
                while (rd.Read())
                {
                    extendId = int.Parse(rd["fieldId"].ToString());

                    if (extendValues.ContainsKey(extendId))
                    {
                        extendValues[extendId].Id = int.Parse(rd["id"].ToString());
                        extendValues[extendId].Value = rd["fieldValue"].ToString().Replace("\n", "<br />");
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
                            rd["fieldValue"].ToString().Replace("\n","<br />")
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
                if (Array.Exists(ids, a => a == field.Id))
                    yield return field;
            }
        }


        public void UpdateArchiveRelationExtendValues(IArchive archive)
        {

            int siteId = archive.Category.Site.Id;
            //============ 更新 ============
            IDictionary<int, string> extendValues = new Dictionary<int, string>();
            IExtendField field;
            foreach (IExtendValue value in archive.ExtendValues)
            {
                field = this.GetExtendFieldById(siteId, value.Field.Id);

                //如果为默认数据，也要填写进去,以免模板获取不到
                if (value.Value != null)
                    extendValues.Add(value.Field.Id,
                    //value.Value == field.DefaultValue ? String.Empty : value.Value);
                    value.Value);
            }

            this.extendDAL.InsertDataExtendFields(ExtendRelationType.Archive, archive.Id, extendValues);

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
            return this.extendDAL.GetCategoryExtendRefrenceNum(category.Site.Id, category.Id, extendId);
        }


        public void UpdateCategoryExtends(ICategory category)
        {
            int[] extendIds = new int[category.ExtendFields.Count];
            int i=0;
            foreach(IExtendField field in category.ExtendFields)
            {
                extendIds[i++] = field.Id;
            }
            this.extendDAL.UpdateCategoryExtendsBind(category.Id, extendIds);
        }
    }
}
