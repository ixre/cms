using System;
using System.Collections.Generic;
using System.Data.Common;
using JR.Cms.Domain.Implement.Content.Archive;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Infrastructure;
using JR.DevFw.Data.Extensions;
using JR.DevFw.Framework;
using JR.Cms.Models;

namespace JR.Cms.ServiceRepository
{
    public class ArchiveRepository : BaseArchiveRepository, IArchiveRepository
    {
        private IExtendFieldRepository _extendRep;
        private ITemplateRepo _templateRep;
        private ICategoryRepo _categoryRep;

        private ArchiveDal _dal = new ArchiveDal();
        private IContentRepository _contentRep;

        public ArchiveRepository(
            IContentRepository contentRep,
            IExtendFieldRepository extendRep,
            ITemplateRepo templateRep,
            ICategoryRepo categoryRep
            )
        {
            this._contentRep = contentRep;
            this._extendRep = extendRep;
            this._templateRep = templateRep;
            this._categoryRep = categoryRep;
        }


        public IArchive CreateArchive(CmsArchiveEntity value)
        {
            return base.CreateArchive(this._contentRep, this, this._extendRep, this._categoryRep, this._templateRep,value);
        }

        #region helper


        private IndexOfHandler<String> GetIndexOfDataReaderColumnNameDelegate(String[] columns)
        {
            return columnName => Array.IndexOf<String>(columns, columnName.ToLower());
        }

        private IArchive CreateArchiveFromDataReader(DbDataReader rd, IndexOfHandler<String> indexOf)
        {
            CmsArchiveEntity archive = new CmsArchiveEntity();
            if (indexOf("id") != -1) archive.ID = Convert.ToInt32(rd["id"]??"0");
            if (indexOf("cat_id") != -1) archive.CatId = Convert.ToInt32(rd["cat_id"]);
            if (indexOf("title") != -1) archive.Title = (rd["title"]??"").ToString();
            if (indexOf("flag") != -1) archive.Flag = Convert.ToInt32(rd["flag"] ?? "0");
            if (indexOf("path") != -1) archive.Path = (rd["path"] ?? "").ToString();
            if (indexOf("alias") != -1) archive.Alias = (rd["alias"] ?? "").ToString();
            if (indexOf("str_id") != -1) archive.StrId = (rd["str_id"] ?? "").ToString();
            if (indexOf("site_id") != -1) archive.SiteId = Convert.ToInt32(rd["site_id"]);
            if (indexOf("small_title") != -1) archive.SmallTitle = (rd["small_title"] ?? "").ToString();
            if (indexOf("location") != -1) archive.Location = rd["location"].ToString();
            if (indexOf("sort_number") != -1) archive.SortNumber = int.Parse(rd["sort_number"].ToString());
            if (indexOf("outline") != -1) archive.Outline = (rd["outline"] ?? "").ToString();
            if (indexOf("author_id") != -1)
                archive.AuthorId = rd["author_id"] == DBNull.Value ? 0 : Convert.ToInt32(rd["author_id"]);
            if (indexOf("content") != -1) archive.Content = rd["content"].ToString();
            if (indexOf("source") != -1) archive.Source = (rd["source"] ?? "").ToString();
            if (indexOf("tags") != -1) archive.Tags = (rd["tags"] ?? "").ToString();
            if (indexOf("thumbnail") != -1) archive.Thumbnail = (rd["thumbnail"] ?? "").ToString();
            if (indexOf("create_time") != -1) archive.CreateTime = Convert.ToInt32(rd["create_time"]);
            if (indexOf("update_time") != -1) archive.UpdateTime = Convert.ToInt32(rd["update_time"]);
            if (indexOf("view_count") != -1) archive.ViewCount = int.Parse((rd["view_count"] ?? "0").ToString());
            //archive.Agree = int.Parse((rd["agree"] ?? "0").ToString());
            //archive.Disagree = int.Parse((rd["disagree"] ?? "0").ToString());

            //rd.CopyToEntity(archive);
            return this.CreateArchive(archive);

        }

        /// <summary>
        /// 获取包含扩展属性的文档列表
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="archives"></param>
        /// <returns></returns>
        private IEnumerable<IArchive> GetContainExtendValueArchiveList(int siteId, IEnumerable<IArchive> archives)
        {
            IDictionary<int, IList<IExtendValue>> extendValues = new Dictionary<int, IList<IExtendValue>>();

            IList<int> idList = new List<int>();
            foreach (IArchive a in archives)
            {
                idList.Add(a.GetAggregaterootId());
            }

            //获取扩展信息
            extendValues = this._extendRep.GetExtendFieldValuesList(siteId, ExtendRelationType.Archive, idList);
            foreach (IArchive a in archives)
            {
                if (extendValues.ContainsKey(a.GetAggregaterootId()))
                {
                    a.SetExtendValue(extendValues[a.GetAggregaterootId()]);
                }
                yield return a;
            }

        }

        #endregion


        public Error SaveArchive(CmsArchiveEntity a)
        {
            if (a.ID <= 0)
            {
                _dal.Add(a);
                a.ID = _dal.GetMaxArchiveId(a.SiteId);
            }
            else
            {
                _dal.Update(a);
            }
            return null;
        }


        public IArchive GetArchiveById(int siteId, int archiveId)
        {
            IArchive archive = null;

            _dal.GetArchiveById(siteId, archiveId, rd =>
            {
                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                if (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);
                }
            });

            return archive;
        }

        public IArchive GetArchive(int siteId, string alias)
        {
            IArchive archive = null;

            _dal.GetArchiveByPath(siteId, alias, rd =>
            {
                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                if (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);
                }
            });

            return archive;
        }

        public IArchive GetNextArchive(int siteId, int id, bool sameCategory, bool ingoreSpecial)
        {
            IArchive archive = null;

            _dal.GetNextArchive(siteId, id, sameCategory, ingoreSpecial, rd =>
            {
                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));

                if (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);
                }
            });

            return archive;

        }

        public IArchive GetPreviousArchive(int siteId, int id, bool sameCategory, bool ingoreSpecial)
        {
            IArchive archive = null;

            _dal.GetPreviousArchive(siteId, id, sameCategory, ingoreSpecial, rd =>
            {
                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                if (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);
                }
            });

            return archive;

        }


        
        public IEnumerable<IArchive> GetArchivesContainChildCategories(int siteId, int[] catIdArray, int number, int skipSize)
        {
            IList<IArchive> archives = new List<IArchive>();
            IList<IExtendValue> defaultValues = new List<IExtendValue>();

            IDictionary<int, IList<IExtendValue>> extendValues = null;

            _dal.GetSelftAndChildArchiveExtendValues(siteId, (int)ExtendRelationType.Archive, catIdArray, number, skipSize, rd =>
            {
                extendValues = this._extendRep._GetExtendValuesFromDataReader(siteId, rd);
            });

            if (extendValues == null) extendValues = new Dictionary<int, IList<IExtendValue>>();

            IArchive archive;
            _dal.GetArchiveList(siteId, catIdArray, number, skipSize, rd =>
            {
                //DateTime dt = DateTime.Now;
                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                while (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);
                    if (extendValues.ContainsKey(archive.GetAggregaterootId()))
                    {
                        archive.SetExtendValue(extendValues[archive.GetAggregaterootId()]);
                    }
                    else
                    {
                        archive.SetExtendValue(defaultValues);
                    }
                    archives.Add(archive);
                }
                //throw new Exception((DateTime.Now - dt).TotalMilliseconds.ToString());
            });
            return archives;
        }

        public IEnumerable<IArchive> GetArchivesByModuleId(int siteId, int moduleId, int number)
        {
            IList<IArchive> archives = new List<IArchive>();

            IArchive archive;
            _dal.GetArchivesByModuleId(siteId, moduleId, number, rd =>
            {
                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                while (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);
                    archives.Add(archive);
                }
            });

            return this.GetContainExtendValueArchiveList(siteId, archives);
        }


        public IEnumerable<IArchive> GetArchivesByViewCountByModuleId(int siteId, int moduleId, int number)
        {
            IList<IArchive> archives = new List<IArchive>();

            IArchive archive;
            _dal.GetArchivesByViewCountAndModuleId(siteId, moduleId, number, rd =>
            {
                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                while (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);
                    archives.Add(archive);
                }
            });
            return this.GetContainExtendValueArchiveList(siteId, archives);
        }

        public IEnumerable<IArchive> GetSpecialArchives(int siteId, int[] catIdArray, int number, int skipSize)
        {
            IList<IArchive> archives = new List<IArchive>();

            IArchive archive;
            _dal.GetSpecialArchives(siteId, catIdArray, number, skipSize, rd =>
            {
                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                while (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);
                    archives.Add(archive);
                }
            });
            return this.GetContainExtendValueArchiveList(siteId, archives);
        }
        
        public IEnumerable<IArchive> GetSpecialArchivesByModuleId(int siteId, int moduleId, int number)
        {
            IList<IArchive> archives = new List<IArchive>();

            IArchive archive;
            _dal.GetSpecialArchivesByModuleId(siteId, moduleId, number, rd =>
            {
                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                while (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);
                    archives.Add(archive);
                }
            });
            return this.GetContainExtendValueArchiveList(siteId, archives);
        }

        public IEnumerable<IArchive> GetArchivesByViewCount(int siteId, int[] catIdArray, int number)
        {
            IList<IArchive> archives = new List<IArchive>();

            IArchive archive;
            _dal.GetArchivesByViewCount(siteId, catIdArray, number, rd =>
            {
                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                while (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);
                    archives.Add(archive);
                }
            });
            return this.GetContainExtendValueArchiveList(siteId, archives);
        }

       
        public bool DeleteArchive(int siteId, int archiveId)
        {

            //删除文章
            this._dal.Delete(siteId, archiveId);

            return true;
        }


        public void RepublishArchive(int siteId, int archiveId)
        {
            this._dal.RePublish(siteId, archiveId);
        }


        public IEnumerable<IArchive> SearchArchivesByCategory(int siteId,String catPath, string keyword, int pageSize, int pageIndex, out int records, out int pages, string orderBy)
        {
            IArchive archive;
            IList<IArchive> archives = new List<IArchive>();
            _dal.SearchArchivesByCategory(siteId,catPath, keyword,
                pageSize, pageIndex, out records, out pages
                , orderBy, rd =>
            {
                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                while (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);

                    archives.Add(archive);
                }
            });
            return archives;
        }

        public IEnumerable<IArchive> SearchArchives(int siteId, String catPath, bool onlyMatchTitle, string keyword, int pageSize, int pageIndex, out int records, out int pages, string orderBy)
        {
            IArchive archive;
            IList<IArchive> archives = new List<IArchive>();
            _dal.SearchArchives(siteId, catPath, onlyMatchTitle, keyword,
                pageSize, pageIndex, out records, out pages
                , orderBy, rd =>
                {
                    IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                    while (rd.Read())
                    {
                        archive = this.CreateArchiveFromDataReader(rd, dg);

                        archives.Add(archive);
                    }
                });
            return archives;
        }


        public void AddArchiveViewCount(int siteId, int id, int count)
        {
            this._dal.AddViewCount(siteId, id, count);
        }


        public int GetMaxSortNumber(int siteId)
        {
            return this._dal.GetMaxSortNumber(siteId);
        }



        public void SaveSortNumber(int archiveId, int sortNumber)
        {
            this._dal.SaveSortNumber(archiveId, sortNumber);
        }


        public int TransferArchives(int userId, int toUserId)
        {
            return this._dal.TransferPublisher(userId, toUserId);
        }
        

        public bool CheckSidIsExist(int siteId, string strId)
        {
            return this._dal.CheckAliasIsExist(siteId, strId);
        }

        public bool CheckPathMatch(int siteId, string path, int archiveId)
        {
            return this._dal.CheckArchivePathMatch(siteId, path,archiveId);
        }

        public IArchive GetArchiveByPath(int siteId, string path)
        {
            IArchive archive = null;
            this._dal.GetArchiveByPath(siteId, path, rd =>
            {
                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                if (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);
                }
            });
            return archive;
        }

    }
}
