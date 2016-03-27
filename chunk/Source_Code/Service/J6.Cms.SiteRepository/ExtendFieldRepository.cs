using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using J6.Cms.Dal;
using J6.Cms.Domain.Implement.Site.Extend;
using J6.Cms.Domain.Interface.Content.Archive;
using J6.Cms.Domain.Interface.Site.Category;
using J6.Cms.Domain.Interface.Site.Extend;

namespace J6.Cms.ServiceRepository
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

                        if (!dicts.ContainsKey(inSiteId))
                        {
                            dicts.Add(inSiteId, new List<IExtendField>());
                        }

                        field = this.CreateExtendField(int.Parse(rd["id"].ToString()), rd["name"].ToString());
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

        public int SaveExtendField(int siteId, IExtendField extendField)
        {

            if (extendField.Id > 0)
            {
                _extendDal.UpdateExtendField(siteId, extendField);
            }
            else
            {
                extendField.Id = _extendDal.AddExtendField(siteId, extendField);
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

        public bool DeleteExtendField(int siteId, int extendFieldId)
        {
            return this._extendDal.DeleteExtendField(siteId, extendFieldId);
        }


        public IList<IExtendValue> GetExtendFieldValues(IArchive archive)
        {
            int extendId;
            int siteId = archive.Category.Site.Id;
            IDictionary<int, IExtendValue> extendValues = new Dictionary<int, IExtendValue>();
            foreach (IExtendField field in archive.Category.ExtendFields)
            {
                if (!extendValues.Keys.Contains(field.Id))
                {
                    extendValues.Add(field.Id, this.CreateExtendValue(field, -1, null));
                }
            }

            this._extendDal.GetExtendValues(siteId, (int)ExtendRelationType.Archive, archive.Id, rd =>
            {
                while (rd.Read())
                {
                    extendId = int.Parse(rd["field_id"].ToString());

                    if (extendValues.ContainsKey(extendId))
                    {
                        extendValues[extendId].Id = int.Parse(rd["id"].ToString());
                        extendValues[extendId].Value = rd["field_value"].ToString().Replace("\n", "<br />");
                    }

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

            this._extendDal.GetExtendValuesList(siteId, (int)ExtendRelationType.Archive, idList, rd =>
            {
                while (rd.Read())
                {
                    relationId = int.Parse(rd["relation_id"].ToString());
                    fieldId = int.Parse(rd["field_id"].ToString());
                    if (!extendValues.ContainsKey(relationId))
                    {
                        extendValues.Add(relationId, new List<IExtendValue>());
                    }
                    extendValues[relationId].Add(
                        new ExtendValue(int.Parse(rd["id"].ToString()),
                            this.GetExtendFieldById(siteId, fieldId),
                            rd["field_value"].ToString().Replace("\n", "<br />")
                            )

                        );
                }
            });
            return extendValues;
        }

        public IEnumerable<IExtendField> GetExtendFields(int siteId, int categoryId)
        {
            IList<IExtendField> list = this.GetAllExtendsBySiteId(siteId);
            int[] ids = this._extendDal.GetCategoryExtendIdList(siteId, categoryId);
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

            this._extendDal.InsertDataExtendFields(ExtendRelationType.Archive, archive.Id, extendValues);

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
                if (extendValues == null)
                {
                    extendValues = new Dictionary<int, IList<IExtendValue>>();
                }
                int relationId;
                if (!extendValues.ContainsKey(relationId = int.Parse(rd["relation_id"].ToString())))
                {
                    extendValues.Add(relationId, new List<IExtendValue>());
                }
                var extendId = int.Parse(rd["extendFieldId"].ToString());
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
            return this._extendDal.GetCategoryExtendRefrenceNum(category.Site.Id, category.Id, extendId);
        }


        public void UpdateCategoryExtends(ICategory category)
        {
            IList<int> extendIds = new List<int>();
            foreach (IExtendField field in category.ExtendFields)
            {
                if (!extendIds.Contains(field.Id))
                {
                    extendIds.Add(field.Id);
                }
            }
            this._extendDal.UpdateCategoryExtendsBind(category.Id, extendIds.ToArray());
        }


        public IExtendField GetExtendByName(int siteId, string name, string type)
        {
            IExtendField field = null;
            _extendDal.GetExtendFieldByName(siteId, name, type, rd =>
            {
                if (rd.Read())
                {
                    field = this.CreateExtendField(int.Parse(rd["id"].ToString()), name);
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
