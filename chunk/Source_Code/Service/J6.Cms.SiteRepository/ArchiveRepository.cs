using System;
using System.Collections.Generic;
using System.Data.Common;
using J6.Cms.DAL;
using J6.Cms.Domain.Implement.Content.Archive;
using J6.Cms.Domain.Interface.Common;
using J6.Cms.Domain.Interface.Content;
using J6.Cms.Domain.Interface.Content.Archive;
using J6.Cms.Domain.Interface.Site.Category;
using J6.Cms.Domain.Interface.Site.Extend;
using J6.Cms.Domain.Interface.Site.Template;
using J6.Cms.Infrastructure;
using J6.DevFw.Data.Extensions;
using J6.DevFw.Framework;

namespace J6.Cms.ServiceRepository
{
    public class ArchiveRepository : BaseArchiveRepository, IArchiveRepository
    {
        private IExtendFieldRepository _extendRep;
        private ITemplateRepository _templateRep;
        private ICategoryRepository _categoryRep;

        private ArchiveDal _dal = new ArchiveDal();
        private IContentRepository _contentRep;
        private ILinkRepository _linkRep;

        public ArchiveRepository(
            IContentRepository contentRep,
            IExtendFieldRepository extendRep,
            ITemplateRepository templateRep,
            ICategoryRepository categoryRep,
            ILinkRepository linkRep
            )
        {
            this._contentRep = contentRep;
            this._extendRep = extendRep;
            this._templateRep = templateRep;
            this._categoryRep = categoryRep;
            this._linkRep = linkRep;
        }


        public IArchive CreateArchive(int id, string strId, int categoryId, string title)
        {
            return base.CreateArchive(this._contentRep, this, this._extendRep, this._categoryRep, this._templateRep, this._linkRep, id,
                strId, categoryId, title);
        }

        #region helper


        private IndexOfHandler<String> GetIndexOfDataReaderColumnNameDelegate(String[] columns)
        {
            return columnName => Array.IndexOf<String>(columns, columnName.ToLower());
        }

        private IArchive CreateArchiveFromDataReader(DbDataReader rd, IndexOfHandler<String> indexOf)
        {
            IArchive archive;
            archive = this.CreateArchive(int.Parse(rd["id"].ToString()),
                rd["strid"].ToString(),
                int.Parse(rd["cid"].ToString()),
                rd["title"].ToString());
            archive.Alias = rd["alias"].ToString();
            if (indexOf("small_title")!=-1) archive.SmallTitle =( rd["small_title"] ?? "").ToString();
            if (indexOf("flags") != -1) archive.Flags = rd["flags"].ToString();
            if (indexOf("location") != -1)archive.Location = rd["location"].ToString();
            if (indexOf("sort_number") != -1) archive.SortNumber = int.Parse(rd["sort_number"].ToString());
            if (indexOf("outline") != -1) archive.Outline = (rd["outline"] ?? "").ToString();
            if (indexOf("publisher_id") != -1)
                archive.PublisherId = rd["publisher_id"] == DBNull.Value ? 0 : Convert.ToInt32(rd["publisher_id"]);
            if (indexOf("content") != -1) archive.Content = rd["content"].ToString();
            if (indexOf("source") != -1) archive.Source = (rd["source"] ?? "").ToString();
            if (indexOf("tags") != -1) archive.Tags = (rd["tags"] ?? "").ToString();
            if (indexOf("thumbnail") != -1) archive.Thumbnail = (rd["thumbnail"] ?? "").ToString();
            if (indexOf("createdate") != -1) archive.CreateDate = Convert.ToDateTime(rd["createdate"]);
            if (indexOf("lastmodifydate") != -1) archive.LastModifyDate = Convert.ToDateTime(rd["lastmodifydate"]);
            if (indexOf("viewcount") != -1) archive.ViewCount = int.Parse((rd["viewcount"] ?? "0").ToString());
            //archive.Agree = int.Parse((rd["agree"] ?? "0").ToString());
            //archive.Disagree = int.Parse((rd["disagree"] ?? "0").ToString());

            //rd.CopyToEntity(archive);
            return archive;

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
                idList.Add(a.Id);
            }

            //获取扩展信息
            extendValues = this._extendRep.GetExtendFieldValuesList(siteId, ExtendRelationType.Archive, idList);
            foreach (IArchive a in archives)
            {
                if (extendValues.ContainsKey(a.Id))
                    a.ExtendValues = extendValues[a.Id];
                yield return a;
            }

        }

        #endregion


        public int SaveArchive(IArchive archive)
        {
            int siteId = archive.Category.Site.Id;
            int categoryId = archive.Category.Id;

            if (archive.Thumbnail == null)
            {
                archive.Thumbnail = "";
            }

            if (archive.Id <= 0)
            {
                string strId;
                do
                {
                    strId = IdGenerator.GetNext(5);              //创建5位ID
                } while (_dal.CheckAliasIsExist(siteId, strId));



                _dal.Add(strId, archive.Alias, categoryId, archive.PublisherId, archive.Title,
                    archive.SmallTitle,archive.Source, archive.Thumbnail, archive.Outline, archive.Content, 
                    archive.Tags, archive.Flags,archive.Location,archive.SortNumber);

                return this.GetArchive(siteId, strId).Id;
            }
            else
            {
                //Update
                //archive.Thumbnail.IndexOf(CmsVariables.Archive_NoPhoto) != -1) 

                _dal.Update(archive.Id, categoryId, archive.Title,archive.SmallTitle, archive.Alias,
                    archive.Source, archive.Thumbnail, archive.Outline, archive.Content ?? "",
                    archive.Tags, archive.Flags,archive.Location,archive.SortNumber);
            }

            return archive.Id;
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

            _dal.GetArchive(siteId, alias, rd =>
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

            _dal.GetNextArchive(siteId, id, sameCategory,ingoreSpecial,rd =>
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

            _dal.GetPreviousArchive(siteId, id, sameCategory,ingoreSpecial, rd =>
            {
                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                if (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);
                }
            });

            return archive;

        }



        public IEnumerable<IArchive> GetArchivesByCategoryTag(int siteId, string categoryTag, int number)
        {

            IArchive archive;
            IList<IArchive> archives = new List<IArchive>();
            IList<IExtendValue> defaultValues = new List<IExtendValue>();

            IDictionary<int, IList<IExtendValue>> extendValues = new Dictionary<int, IList<IExtendValue>>();

            _dal.GetArchivesExtendValues(siteId, (int)ExtendRelationType.Archive, categoryTag, number, rd =>
            {
                extendValues = this._extendRep._GetExtendValuesFromDataReader(siteId, rd);
            });

            _dal.GetArchives(siteId, categoryTag, number, rd =>
            {

                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                while (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);
                    archive.ExtendValues = extendValues.ContainsKey(archive.Id) ?
                        extendValues[archive.Id] :
                        defaultValues;

                    archives.Add(archive);
                }
            });
            return archives;
        }


        public IEnumerable<IArchive> GetArchivesContainChildCategories(int siteId, int lft, int rgt, int number)
        {
            IList<IArchive> archives = new List<IArchive>();
            IList<IExtendValue> defaultValues = new List<IExtendValue>();

            IDictionary<int, IList<IExtendValue>> extendValues = new Dictionary<int, IList<IExtendValue>>();

            _dal.GetSelftAndChildArchiveExtendValues(siteId, (int)ExtendRelationType.Archive, lft, rgt, number, rd =>
            {
                extendValues = this._extendRep._GetExtendValuesFromDataReader(siteId, rd);
            });

            IArchive archive;
            _dal.GetSelftAndChildArchives(siteId, lft, rgt, number, rd =>
            {
                //DateTime dt = DateTime.Now;
                IndexOfHandler<String> dg = this.GetIndexOfDataReaderColumnNameDelegate(rd.GetColumns(true));
                while (rd.Read())
                {
                    archive = this.CreateArchiveFromDataReader(rd, dg);
                    archive.ExtendValues = extendValues.ContainsKey(archive.Id) ?
                        extendValues[archive.Id] :
                        defaultValues;

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

        public IEnumerable<IArchive> GetSpecialArchives(int siteId, string categoryTag, int number)
        {
            IList<IArchive> archives = new List<IArchive>();

            IArchive archive;
            _dal.GetSpecialArchives(siteId, categoryTag, number, rd =>
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

        public IEnumerable<IArchive> GetSpecialArchives(int siteId, int lft, int rgt, int number)
        {
            IList<IArchive> archives = new List<IArchive>();

            IArchive archive;
            _dal.GetSpecialArchives(siteId, lft, rgt, number, rd =>
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

        public IEnumerable<IArchive> GetArchivesByViewCount(int siteId, string categoryTag, int number)
        {
            IList<IArchive> archives = new List<IArchive>();

            IArchive archive;
            _dal.GetArchivesByViewCount(siteId, categoryTag, number, rd =>
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

        public IEnumerable<IArchive> GetArchivesByViewCount(int siteId, int lft, int rgt, int number)
        {
            IList<IArchive> archives = new List<IArchive>();

            IArchive archive;
            _dal.GetArchivesByViewCount(siteId, lft, rgt, number, rd =>
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


        public void RefreshArchive(int siteId, int archiveId)
        {
            this._dal.RePublish(siteId, archiveId);
        }


        public IEnumerable<IArchive> SearchArchivesByCategory(int siteId, int categoryLft, int categoryRgt, string keyword, int pageSize, int pageIndex, out int records, out int pages, string orderBy)
        {
            IArchive archive;
            IList<IArchive> archives = new List<IArchive>();
            _dal.SearchArchivesByCategory(siteId, categoryLft, categoryRgt, keyword,
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

        public IEnumerable<IArchive> SearchArchives(int siteId, string keyword, int pageSize, int pageIndex, out int records, out int pages, string orderBy)
        {
            IArchive archive;
            IList<IArchive> archives = new List<IArchive>();
            _dal.SearchArchives(siteId, keyword,
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
    }
}
