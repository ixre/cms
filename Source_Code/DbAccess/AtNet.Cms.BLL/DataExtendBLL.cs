/*
 * Copyright(C) 2010-2012 OPSoft Inc
 * 
 * File Name	: PropertyBLL
 * Author	: Newmin (new.min@msn.com)
 * Create	: 2012/9/30 10:30:20
 * Description	:
 *
 */

using Ops.Data;
using Spc.DAL;
using Spc.IDAL;
using Spc.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Spc.BLL
{

    /// <summary>
    /// 扩展字段逻辑
    /// </summary>
	public class DataExtendBLL :IDataExtend
	{
        private static IDataExtendDAL _dal;
        private IDataExtendDAL dal
        {
            get
            {
                return _dal??(_dal=new ExtendFieldDAL());
            }
        }

        public IEnumerable<DataExtend> GetExtends(SpecialEnum special)
        {
            foreach (DataExtend extend in WeakRefCache.DataExtends)
            {
                if ((int)special == extend.State)
                    yield return extend;
            }
        }


		/// <summary>
		/// 生成UI的Html
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public string GetUIHtml(DataExtendAttr pro,string value)
		{
			return "";
		}

		public string BuildUIString(PropertyUI ui,string key,params string[] data)
		{
			return "";
		}



		public bool AddExtendAttr(DataExtendAttr attr)
		{
			bool result= dal.AddExtendAttr(attr);
		    WeakRefCache.RebuiltDataExtendAttrs();
			return result;
		}

		public bool DeleteExtendAttr(int propertyID)
		{
            bool result = dal.DeleteExtendAttr(propertyID);
            WeakRefCache.RebuiltDataExtendAttrs();
			return result;
		}

		public bool UpdateExtendAttr(DataExtendAttr pro)
		{
            bool result = dal.UpdateExtendAttr(pro);
            WeakRefCache.RebuiltDataExtendAttrs();
			return result;
		}

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

		public DataExtendAttr GetExtendAttr(int extendID, string attrName)
		{
			foreach (DataExtendAttr p in WeakRefCache.DataExtendAttrs)
			{
                if (p.ExtendID == extendID && String.Compare(attrName, p.AttrName, true) == 0)
				{
					return p;
				}
			}
			return null;
		}

		public void RebuiltExtends()
		{
			WeakRefCache.RebuiltDataExtends();
		}

        public IList<DataExtendField> GetExtendFileds(int relationID)
        {
            IList<DataExtendField> fileds = null;

            dal.GetExtendFileds(relationID,rd =>
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
            if (module.ExtID1!=0)
            {
                ext = GetExtend(module.ExtID1);
                if (ext != null && ext.State==1)
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

        public DataExtend GetExtend(int id)
        {
        	IList<DataExtend> list=WeakRefCache.DataExtends;
        	
        	var i=list.Count;
        	;
            foreach (DataExtend e in WeakRefCache.DataExtends)
            {
                if (e.ID == id) return e;
            }
            return null;
        }


        public void UpdateExtendFileds(int relationID, IDictionary<int, string> extendData)
        {
            //============ 更新 ============
            foreach (DataExtendField f in  this.GetExtendFileds(relationID))
            {
                if (extendData.ContainsKey(f.AttrId))
                {
                    dal.UpdateDataExtendFieldValue(relationID,f.AttrId, extendData[f.AttrId]);

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
        }
    }
}
