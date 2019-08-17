
using JR.DevFw.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using JR.Cms.DataTransfer;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Infrastructure;
using JR.Cms.Models;
using JR.Cms.ServiceContract;
using JR.Cms.ServiceRepository.Query;

namespace JR.Cms.Service
{
    public class ArchiveService : IArchiveServiceContract
    {
        private readonly IContentRepository _contentRep;
        private readonly ISiteRepo _siteRep;
        private readonly ArchiveQuery _archiveQuery = new ArchiveQuery();
        private readonly IExtendFieldRepository _extendRep;
        private readonly ICategoryRepo _catRepo;

        public ArchiveService(
            IContentRepository contentRep,
            ISiteRepo siteRep,
            ICategoryRepo catRepo,
            IExtendFieldRepository extendRep
            )
        {
            this._contentRep = contentRep;
            this._siteRep = siteRep;
            this._extendRep = extendRep;
            this._catRepo = catRepo;
        }

        public ArchiveDto GetArchiveByIdOrAlias(int siteId, string id)
        {
            IContentContainer ic = this._contentRep.GetContent(siteId);
            IArchive archive = ic.GetArchive(id);
            if (archive == null) return default(ArchiveDto);

            return ArchiveDto.ConvertFrom(archive, true, true, true);
        }


        public ArchiveDto GetArchiveById(int siteId, int archiveId)
        {
            IContentContainer ic = this._contentRep.GetContent(siteId);
            IArchive archive = ic.GetArchiveById(archiveId);
            if (archive == null) return default(ArchiveDto);
            return ArchiveDto.ConvertFrom(archive, true, true, true);
        }


        public DataTransfer.Result SaveArchive(int siteId,int catId, ArchiveDto archiveDto)
        {
            CmsArchiveEntity value = archiveDto.ToArchiveEntity();
            value.CatId = catId;
            value.SiteId = siteId;
            IContentContainer ic = this._contentRep.GetContent(siteId);
            IArchive ia;
            if (archiveDto.Id <= 0)
            {
                ia = ic.CreateArchive(new CmsArchiveEntity());
            }
            else
            {
                ia = ic.GetArchiveById(archiveDto.Id);
            }

            Error err = ia.Set(value);
            if(err == null)
            {
                // 更新模板
                if (!String.IsNullOrEmpty(archiveDto.TemplatePath))
                {
                    ia.SetTemplatePath(archiveDto.TemplatePath);
                }
                // 设置扩展属性
                err = ia.SetExtendValue(archiveDto.ExtendValues);
                // 保存文档
                if(err == null)
                {
                    err = ia.Save();
                }
            }
            DataTransfer.Result r = new DataTransfer.Result();
            if(err == null)
            {
                r.Data = new Dictionary<String, String>();
                r.Data.Add("ArchiveId",ia.GetAggregaterootId().ToString());
            }
            else
            {
                r.ErrCode = 1;
                r.ErrMsg = err.Message;
            }
            return r;
        }


        public ArchiveDto GetSameCategoryPreviousArchive(int siteId, int archiveId)
        {
            IContentContainer ic = this._contentRep.GetContent(siteId);
            IArchive archive = ic.GetPreviousSiblingArchive(archiveId);
            if (archive == null) return default(ArchiveDto);

            return ArchiveDto.ConvertFrom(archive, true, false, false);
        }

        public ArchiveDto GetSameCategoryNextArchive(int siteId, int archiveId)
        {
            IContentContainer ic = this._contentRep.GetContent(siteId);
            IArchive archive = ic.GetNextSiblingArchive(archiveId);
            if (archive == null) return default(ArchiveDto);
            return ArchiveDto.ConvertFrom(archive, true, false, false);
        }

        private IEnumerable<ArchiveDto> GetArchiveEnumertor(IEnumerable<IArchive> archives)
        {
            IDictionary<int, CategoryDto> categories = new Dictionary<int, CategoryDto>();
            ArchiveDto archive;
            CategoryDto cateDto;
            int categoryId;
            foreach (IArchive ia in archives)
            {
                CmsArchiveEntity av = ia.Get();
                archive = new ArchiveDto
                {
                    StrId = av.StrId,
                    Id = ia.GetAggregaterootId(),
                    PublisherId = av.AuthorId,
                    Alias = av.Alias,
                    Agree = av.Agree,
                    Disagree = av.Disagree,
                    Content = av.Content,
                    CreateTime =TimeUtils.UnixTime(av.CreateTime),
                    Tags = av.Tags,
                    UpdateTime = TimeUtils.UnixTime(av.UpdateTime),
                    Source = av.Source,
                    Thumbnail = av.Thumbnail,
                    Title = av.Title,
                    SmallTitle = av.SmallTitle,
                    Location = av.Location,
                    ViewCount = av.ViewCount,
                    Outline = av.Outline,
                    //TemplateBind=null,
                    ExtendValues = ia.GetExtendValues()
                };

                //archive = new ArchiveDto().CloneData(ia);
                //archive.ID = ia.ID;

                if (!categories.TryGetValue(categoryId = ia.Category.GetDomainId(), out cateDto))
                {
                    cateDto = CategoryDto.ConvertFrom(ia.Category);
                    categories.Add(categoryId, cateDto);
                }
                archive.Category = cateDto;
                yield return archive;
            }
        }



        public ArchiveDto[] GetArchivesByCategoryPath(int siteId, string catPath,
            bool includeChild, int number, int skipSize)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            IEnumerable<IArchive> archives = content.GetArchivesByCategoryPath(catPath,
                includeChild, number, skipSize);
            return this.GetArchiveEnumertor(archives).ToArray();
        }


        public ArchiveDto[] GetArchivesByModuleId(int siteId, int moduleId, int number)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            IEnumerable<IArchive> archives = content.GetArchivesByModuleId(moduleId, number);

            return this.GetArchiveEnumertor(archives).ToArray();
        }

        public ArchiveDto[] GetArchivesByViewCount(int siteId, string catPath,bool includeChild, int number)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            IEnumerable<IArchive> archives = content.GetArchivesByViewCount(catPath,includeChild, number);

            return this.GetArchiveEnumertor(archives).ToArray();
        }

        public ArchiveDto[] GetSpecialArchivesByModuleId(int siteId, int moduleId, int number)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            IEnumerable<IArchive> archives = content.GetSpecialArchivesByModuleId(moduleId, number);

            return this.GetArchiveEnumertor(archives).ToArray();
        }
        

        public ArchiveDto[] GetSpecialArchives(int siteId, string catPath, bool includeChild, int number, int skipSize)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            IEnumerable<IArchive> archives = content.GetSpecialArchives(catPath,includeChild, number, skipSize);

            return this.GetArchiveEnumertor(archives).ToArray();
        }

        public ArchiveDto[] GetArchivesByViewCountByModuleId(int siteId, int moduleId, int number)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            IEnumerable<IArchive> archives = content.GetArchivesByViewCountByModuleId(moduleId, number);

            return this.GetArchiveEnumertor(archives).ToArray();
        }


        public DataTable GetPagedArchives(int siteId, int? categoryId, int publisherId, bool includeChild,
            int flag, string keyword,
            string orderByField, bool orderAsc, int pageSize, int currentPageIndex, out int recordCount, out int pages)
        {

            ICategory ic = this._catRepo.GetCategory(siteId, categoryId??0);

            int[] catIdArray;
            if (includeChild)
            {
               catIdArray =  this.GetCatArrayByPath(ic);
            }
            else
            {
                catIdArray = new int[] { categoryId ?? 0 };
            }

            return this._archiveQuery.GetPagedArchives(siteId,catIdArray, publisherId,
                includeChild, flag,keyword, orderByField, orderAsc, pageSize, currentPageIndex,
                out recordCount, out pages);
        }


        public DataTable GetPagedArchives(
            int siteId,
            String catPath,
            int pageSize,
            int skipSize,
            ref int pageIndex,
            out int records,
            out int pages,
            out IDictionary<int, IDictionary<string, string>> extendValues)
        {
            ICategory ic = this._catRepo.GetCategoryByPath(siteId, catPath);
            int[] catIdArray = this.GetCatArrayByPath(ic);
            //获取数据
            DataTable dt = this._archiveQuery.GetPagedArchives(siteId, catIdArray,
                 pageSize, skipSize, ref pageIndex, out records, out pages);

            IList<int> archiveIds = new List<int>();
            foreach (DataRow dr in dt.Rows)
                archiveIds.Add(int.Parse(dr["id"].ToString()));


            IDictionary<int, IList<IExtendValue>> dict = this._extendRep.GetExtendFieldValuesList(
                siteId,
                ExtendRelationType.Archive,
                archiveIds
                );

            extendValues = new Dictionary<int, IDictionary<string, string>>();
            foreach (int key in dict.Keys)
            {
                if (!extendValues.ContainsKey(key))
                {
                    extendValues.Add(key, new Dictionary<string, string>());
                }
                foreach (IExtendValue value in dict[key])
                {
                    // 避免重复键
                    if (!extendValues[key].ContainsKey(value.Field.Name))
                    {
                        extendValues[key].Add(value.Field.Name, value.Value);
                    }
                    else
                    {
                        extendValues[key][value.Field.Name] = value.Value;
                    }
                }
            }

            return dt;
        }

        /// <summary>
        /// 获取分类下的编号数组
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catPath"></param>
        /// <returns></returns>
        private int[] GetCatArrayByPath(ICategory ic)
        {
            if (ic == null) return new int[] { };
            IList<int> list = ic.Childs.Select(a => a.GetDomainId()).ToList();
            list.Insert(0, ic.GetDomainId());
            return list.ToArray();
        }

        public void DeleteArchive(int siteId, int archiveId)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            content.DeleteArchive(archiveId);
        }


        public void RepublishArchive(int siteId, int archiveId)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            content.RepublishArchive(archiveId);
        }


        public IEnumerable<ArchiveDto> SearchArchivesByCategory(int siteId, String catPath, string keyword, int pageSize,
            int pageIndex, out int records, out int pages,
            string orderBy)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            IEnumerable<IArchive> archives = content.SearchArchivesByCategory(
               catPath, keyword, pageSize,
                pageIndex, out records, out  pages, orderBy);
            return this.GetArchiveEnumertor(archives);
        }

        
        public IEnumerable<ArchiveDto> SearchArchives(int siteId,String catPath,
            bool onlyMatchTitle, string keyword, int pageSize, int pageIndex,out int records, 
            out int pages, string orderBy)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            IEnumerable<IArchive> archives = content.SearchArchives(catPath,
                onlyMatchTitle, keyword, pageSize, pageIndex, out records, out pages, orderBy);
            return this.GetArchiveEnumertor(archives);
        }


        public bool CheckArchiveAliasAvailable(int siteId, int archiveId, string alias)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            IArchive archive = content.GetArchiveByTag(alias);
            bool archiveIsNull = archive == null;

            if (archiveIsNull)
                return true;

            if (archiveId <= 0)
            {
                return archiveIsNull;
            }


            return archive.GetAggregaterootId() == archiveId;
        }


        public void AddCountForArchive(int siteId, int id, int count)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            content.AddCountForArchive(id, count);
        }



        public IEnumerable<ArchiveDto> GetRelatedArchives(int siteId, int contentId)
        {
            throw new NotImplementedException();
            //return this._archiveQuery.GetRelatedArchives(siteId, contentId);
        }


        public void MoveSortNumber(int siteId, int id, int direction)
        {
            IBaseContent archive = this._contentRep.GetContent(siteId).GetArchiveById(id);
            if (archive == null)
            {
                throw new ArgumentException("no such archive", "id");
            }

            if (direction == 1)
            {
                archive.MoveSortUp();
            }
            else if (direction == 2)
            {
                archive.MoveSortDown();
            }
        }


        public void BatchDelete(int siteId, int[] idArray)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            foreach (int id in idArray)
            {
                content.DeleteArchive(id);
            }
        }
        
    }
}
