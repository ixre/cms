using System;
using System.Collections.Generic;
using System.IO;
using JR.DevFw.Data;
using JR.DevFw.Toolkit.Data.Export;

namespace JR.Cms.ServiceRepository.Export
{
    public class ExportManager
    {
        private static readonly IDictionary<string, IDataExportPortal> ExportPortals;
        
        private  static readonly IDictionary<string,ExportItemConfig> ConfigDict = new Dictionary<string, ExportItemConfig>();
        private static DataBaseAccess _dao;

        public static string BaseDirectory { get; set; }

        static ExportManager()
        {
            ExportPortals = new Dictionary<string, IDataExportPortal>();
        }

        public static ExportItemConfig GetConfigByQueryName(string queryName)
        {
            if (!ConfigDict.ContainsKey(queryName))
            {
                string filePath = BaseDirectory + queryName + ".config";

                if (!File.Exists(filePath))
                {
                    throw new Exception("不包含查询:" + queryName);
                }
                ExportItemConfig cfg = ExportUtil.GetExportItemFormXml(
                    File.ReadAllText(filePath)
                    , null);

                ConfigDict.Add(queryName, cfg);
            }
            return ConfigDict[queryName];
        }

        public static string GetColumnMappingString(string queryName)
        {
            return GetConfigByQueryName(queryName).ColumnMappingString;
        }

        public static IDataExportPortal GetPortal(string queryName)
        {
            if (!ExportPortals.Keys.Contains(queryName))
            {
                DataColumnMapping[] columns =
                    ExportUtil.GetColumnMappings(GetColumnMappingString(queryName));

                ExportPortals.Add(queryName,new ExportItem(queryName,columns));
            }
            return ExportPortals[queryName];
        }

        public static void Initialize(string dir,DataBaseAccess dao)
        {
            BaseDirectory = dir;
            _dao = dao;
        }

        public static DataBaseAccess GetDao()
        {
            return _dao;
        }
    }
}