using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using T2.Cms.DataTransfer;
using T2.Cms.Domain.Interface.Content;
using T2.Cms.Domain.Interface.Content.Archive;
using T2.Cms.Domain.Interface.Site;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Extend;
using T2.Cms.ServiceContract;
using T2.Cms.ServiceRepository.Query;

namespace T2.Cms.Service
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


        public int SaveArchive(int siteId, ArchiveDto archiveDto)
        {
            IContentContainer ic = this._contentRep.GetContent(siteId);
            IArchive archive;

            if (archiveDto.Id <= 0)
            {
                archive = ic.CreateArchive(-1, archiveDto.StrId, archiveDto.Category.ID, archiveDto.Title);
            }
            else
            {
                archive = ic.GetArchiveById(archiveDto.Id);

                //修改栏目
                if (archiveDto.Category.ID != archive.Category.GetDomainId())
                {
                    ISite site = this._siteRep.GetSiteById(siteId);
                    archive.Category = site.GetCategory(archiveDto.Category.ID);
                }
            }


            archive.LastModifyDate = DateTime.Now;
            archive.Content = archiveDto.Content;
            archive.Alias = archiveDto.Alias;
            archive.PublisherId = archiveDto.PublisherId;
            archive.Flags = archiveDto.Flags;
            archive.Outline = archiveDto.Outline;
            archive.Source = archiveDto.Source;
            archive.Tags = archiveDto.Tags;
            archive.Thumbnail = archiveDto.Thumbnail;
            archive.Title = archiveDto.Title;
            archive.SmallTitle = archiveDto.SmallTitle;
            archive.Location = archiveDto.Location;
            archive.ExtendValues = archiveDto.ExtendValues;

            //只更新自己的模板
            if (archiveDto.IsSelfTemplate
                || (archive.GetAggregaterootId() == -1 && !String.IsNullOrEmpty(archiveDto.TemplatePath))
                ){
                archive.SetTemplatePath(archiveDto.TemplatePath);
            }

            return archive.Save();
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
                archive = new ArchiveDto
                {
                    StrId = ia.StrId,
                    Id = ia.GetAggregaterootId(),
                    PublisherId = ia.PublisherId,
                    Alias = ia.Alias,
                    Agree = ia.Agree,
                    Disagree = ia.Disagree,
                    Content = ia.Content,
                    CreateDate = ia.CreateDate,
                    // FirstImageUrl=ia.FirstImageUrl,
                    Flags = ia.Flags,
                    Tags = ia.Tags,
                    LastModifyDate = ia.LastModifyDate,
                    Source = ia.Source,
                    Thumbnail = ia.Thumbnail,
                    Title = ia.Title,
                    SmallTitle = ia.SmallTitle,
                    Location = ia.Location,
                    ViewCount = ia.ViewCount,
                    Outline = ia.Outline,
                    //TemplateBind=null,
                    ExtendValues = ia.ExtendValues
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
            string[,] flags, string keyword,
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
                includeChild, flags,keyword, orderByField, orderAsc, pageSize, currentPageIndex,
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


        public void RefreshArchive(int siteId, int archiveId)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            content.RefreshArchive(archiveId);

        }


        public IEnumerable<ArchiveDto> SearchArchivesByCategory(int siteId, int categoryLft,
            int categoryRgt, string keyword, int pageSize,
            int pageIndex, out int records, out int pages,
            string orderBy)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            IEnumerable<IArchive> archives = content.SearchArchivesByCategory(
                categoryLft, categoryRgt, keyword, pageSize,
                pageIndex, out records, out  pages, orderBy);
            return this.GetArchiveEnumertor(archives);
        }

        
        public IEnumerable<ArchiveDto> SearchArchives(int siteId, int categoryLft, int categoryRgt,
            bool onlyMatchTitle, string keyword, int pageSize, int pageIndex,out int records, 
            out int pages, string orderBy)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            IEnumerable<IArchive> archives = content.SearchArchives(categoryLft,categoryRgt,
                onlyMatchTitle, keyword, pageSize, pageIndex, out records, out pages, orderBy);
            return this.GetArchiveEnumertor(archives);
        }


        public bool CheckArchiveAliasAvailable(int siteId, int archiveId, string alias)
        {
            IContentContainer content = this._contentRep.GetContent(siteId);
            IArchive archive = content.GetArchive(alias);
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
