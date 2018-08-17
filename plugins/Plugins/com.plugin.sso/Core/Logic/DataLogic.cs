/**
 * Copyright (C) 2007-2015 Z3Q.NET,All rights reseved.
 * Get more infromation of this software,please visit site http://k3f.net/cms
 * 
 * name : DataLogic.cs
 * author : newmin (new.min@msn.com)
 * date : 2012/12/01 23:00:00
 * description : 
 * history : 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using com.plugin.sso.Core.ILogic;
using JR.DevFw.Data;
using JR.DevFw.Framework.Extensions;
using JR.DevFw.Toolkit.Data.Export;

namespace com.plugin.sso.Core.Logic
{
    internal class DataLogic : IDataLogic
    {
        private IDictionary<string, ExportItemConfig> configDict;
        private readonly string _baseDirectory;
        private static Regex reg = new Regex("(DROP|SELECT|DELETE|UPDATE)\\s",RegexOptions.IgnoreCase);

        public DataLogic()
        {
            this.configDict = new Dictionary<string, ExportItemConfig>();
            _baseDirectory = Config.PluginAttr.WorkSpace;
        }

        private ExportItemConfig GetConfigByQueryName(string queryName)
        {
            if (!this.configDict.ContainsKey(queryName))
            {
                string filePath = _baseDirectory + "query/" + queryName + ".config";

                if (!File.Exists(filePath))
                {
                    throw new Exception("不包含查询:" + queryName);
                }
                ExportItemConfig cfg = ExportUtil.GetExportItemFormXml(
                    File.ReadAllText(filePath)
                    , null);

                this.configDict.Add(queryName, cfg);
            }
            return this.configDict[queryName];
        }

        public DataTable GetQueryView(string queryName, Hashtable hash, int pageSize, int currentPageIndex,
            out int totalCount)
        {
            DataBaseAccess db = IocObject.GetDao();

            string query = this.GetConfigByQueryName(queryName).Query;
            string queryTotal = this.GetConfigByQueryName(queryName).Total;

            //添加分页参数
            if (hash != null)
            {
                foreach (DictionaryEntry o in hash)
                {
                    if (reg.IsMatch(o.Value.ToString()))
                    {
                        throw  new ArgumentException("含有不安全的查询!");
                    }
                    
                }
                hash.Add("page_start", currentPageIndex<=0?0:(currentPageIndex - 1) * pageSize);
                hash.Add("page_end", (currentPageIndex) * pageSize);
                hash.Add("page_size", pageSize);


                //格式化
                query = query.Template(hash);
               // throw new Exception(query + "/" + currentPageIndex+"/"+pageSize);
                if (!String.IsNullOrEmpty(queryTotal))
                {
                    queryTotal = queryTotal.Template(hash);
                }
            }


            //获取分页结果
            DataTable dataTable = db.GetDataSet(query, hash).Tables[0];

            //获取统计数据
            if (!String.IsNullOrEmpty(queryTotal))
            {
                object data = db.ExecuteScalar(queryTotal, hash);
                int.TryParse(data.ToString(), out totalCount);
            }
            else
            {
                totalCount = dataTable.Rows.Count;
            }

            return dataTable;
        }

        public string GetColumnMappingString(string queryName)
        {
            return this.GetConfigByQueryName(queryName).ColumnMappingString;
        }

        public DataRow GetTotalView(string queryName, Hashtable hash)
        {
            throw new NotImplementedException();
        }
    }
}
