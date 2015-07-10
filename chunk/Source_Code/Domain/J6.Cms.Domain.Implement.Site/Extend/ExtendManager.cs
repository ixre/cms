using System.Collections.Generic;
using J6.Cms.Domain.Interface.Site.Extend;

namespace J6.Cms.Domain.Implement.Site.Extend
{
    internal class ExtendManager:IExtendManager
    {
        private IExtendFieldRepository _resp;

        public ExtendManager(IExtendFieldRepository resp, int siteId)
        {
            this.SiteId = siteId;
            this._resp = resp;
        }
        public int SiteId
        {
            get;
            private set;
        }

        public IList<IExtendField> GetAllExtends()
        {
            return _resp.GetAllExtendsBySiteId(this.SiteId);
        }

        /*
        public IEnumerable<IExtendField> GetExtends(bool enabled)
        {
            foreach (IExtendField extend in GetAllExtends())
            {
                if (enabled && extend.Enabled)
                    yield return extend;
            }
        }*/

        public int SaveExtendField(IExtendField extendField)
        {
            return _resp.SaveExtendField(this.SiteId, extendField);
        }

        public bool DeleteExtendAttr(int extendFieldId)
        {
            return _resp.DeleteExtendField(this.SiteId,extendFieldId);

        }


        public IExtendField GetExtendField(int extendId)
        {
            IList<IExtendField> list = this.GetAllExtends();

            var i = list.Count;
            foreach (IExtendField e in list)
            {
                if (e.Id == extendId) return e;
            }
            return null;
        }

        public IExtendField GetExtendFieldByName(string extendName)
        {
            IList<IExtendField> list = this.GetAllExtends();

            var i = list.Count;
            foreach (IExtendField e in list)
            {
                if (extendName.Equals(e.Name)) return e;
            }
            return null;
        }


        /*
        public IEnumerable<DataExtendAttr> GetExtendAttrs(int extID)
        {
            foreach (DataExtendAttr p in WeakRefCache.DataExtendAttrs)
            {
                if (p.ExtendID == extID && p.Enabled)
                {
                    yield return p;
                }
            }
        }
        */

        /*
        public IList<DataExtendField> GetExtendFileds(int relationID)
        {
            IList<DataExtendField> fileds = null;

            dal.GetExtendFileds(relationID, rd =>
            {
                if (rd.HasRows)
                {
                    fileds = rd.ToEntityList<DataExtendField>();
                }
            });

            return fileds ?? new List<DataExtendField>();
        }

        public DataTable GetExtendFiledsTable(int relationID)
        {
            return dal.GetExtendFieldsTable(relationID);
        }

        public IEnumerable<DataExtend> GetExtendsByModule(Module module)
        {
            DataExtend ext;
            if (module.ExtID1 != 0)
            {
                ext = GetExtend(module.ExtID1);
                if (ext != null && ext.State == 1)
                    yield return ext;
            }


            if (module.ExtID2 != 0)
            {
                ext = GetExtend(module.ExtID2);
                if (ext != null && ext.State == 1)
                    yield return ext;
            }


            if (module.ExtID3 != 0)
            {
                ext = GetExtend(module.ExtID3);
                if (ext != null && ext.State == 1)
                    yield return ext;
            }


            if (module.ExtID3 != 0)
            {
                ext = GetExtend(module.ExtID3);
                if (ext != null && ext.State == 1)
                    yield return ext;
            }
        }
        */

        /*
        public void UpdateExtendFileds(int relationID, IDictionary<int, string> extendData)
        {
            //============ 更新 ============
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
        }


        public IDictionary<string, string> GetExtendFiledDictionary(int relationID)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (DataExtendField d in this.GetExtendFileds(relationID))
            {
                dict.Add(d._AttrName, d.AttrVal);
            }
            return dict;
        }*/

    }
}
