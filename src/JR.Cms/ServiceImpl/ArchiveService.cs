using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Infrastructure;
using JR.Cms.Repository.Query;
using JR.Cms.ServiceContract;
using JR.Cms.ServiceDto;
using JR.Stand.Abstracts;
using JR.Stand.Core.Framework;

namespace JR.Cms.ServiceImpl
{
    public class ArchiveService : IArchiveServiceContract
    {
        private readonly IContentRepository _contentRep;
        private readonly ArchiveQuery _archiveQuery = new ArchiveQuery();
        private readonly IExtendFieldRepository _extendRep;
        private readonly ICategoryRepo _catRepo;

        public ArchiveService(
            IContentRepository contentRep,
            ICategoryRepo catRepo,
            IExtendFieldRepository extendRep
        )
        {
            _contentRep = contentRep;
            _extendRep = extendRep;
            _catRepo = catRepo;
        }

        public ArchiveDto GetArchiveByIdOrAlias(int siteId, string id)
        {
            var ic = _contentRep.GetContent(siteId);
            var archive = ic.GetArchive(id);
            if (archive == null) return default(ArchiveDto);

            return ArchiveDto.ConvertFrom(archive, true, true, true);
        }


        public ArchiveDto GetArchiveById(int siteId, int archiveId)
        {
            var ic = _contentRep.GetContent(siteId);
            var archive = ic.GetArchiveById(archiveId);
            if (archive == null) return default;
            return ArchiveDto.ConvertFrom(archive, true, true, true);
        }


        /// <summary>
        /// 保存文档
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catId"></param>
        /// <param name="archiveDto"></param>
        /// <returns></returns>
        public Result SaveArchive(int siteId, int catId, ArchiveDto archiveDto)
        {
            var value = archiveDto.ToArchiveEntity();
            value.CatId = catId;
            value.SiteId = siteId;
            var ic = _contentRep.GetContent(siteId);
            IArchive ia;
            if (archiveDto.Id <= 0)
            {
                ia = ic.CreateArchive(new CmsArchiveEntity());
            }
            else
            {
                ia = ic.GetArchiveById(archiveDto.Id);
            }

            var err = ia.Set(value);
            if (err == null)
            {
                // 更新模板
                ia.SetTemplatePath(archiveDto.TemplatePath);
                // 设置扩展属性
                err = ia.SetExtendValue(archiveDto.ExtendValues);
                // 保存文档
                if (err == null) err = ia.Save();
            }

            var r = new Result();
            if (err == null)
            {
                r.Data = new Dictionary<string, string>
                {
                    {"ArchiveId", ia.GetAggregateRootId().ToString()},
                    {"Alias",ia.Get().Alias}
                };
            }
            else
            {
                r.ErrCode = 1;
                r.ErrMsg = err.Message;
            }

            return r;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        public ArchiveDto GetSameCategoryPreviousArchive(int siteId, int archiveId)
        {
            var ic = _contentRep.GetContent(siteId);
            var archive = ic.GetPreviousSiblingArchive(archiveId);
            if (archive == null) return default(ArchiveDto);

            return ArchiveDto.ConvertFrom(archive, true, false, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="archiveId"></param>
        /// <returns></returns>
        public ArchiveDto GetSameCategoryNextArchive(int siteId, int archiveId)
        {
            var ic = _contentRep.GetContent(siteId);
            var archive = ic.GetNextSiblingArchive(archiveId);
            if (archive == null) return default(ArchiveDto);
            return ArchiveDto.ConvertFrom(archive, true, false, false);
        }

        private IEnumerable<ArchiveDto> GetArchiveEnumerator(IEnumerable<IArchive> archives)
        {
            IDictionary<int, CategoryDto> categories = new Dictionary<int, CategoryDto>();
            foreach (var ia in archives)
            {
                var av = ia.Get();
                var archive = new ArchiveDto
                {
                    StrId = av.StrId,
                    Id = ia.GetAggregateRootId(),
                    PublisherId = av.AuthorId,
                    Alias = av.Alias,
                    Agree = av.Agree,
                    Disagree = av.Disagree,
                    Path = av.Path,
                    Content = av.Content,
                    CreateTime = TimeUtils.UnixTime(av.CreateTime),
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

                int categoryId;
                if (!categories.TryGetValue(categoryId = ia.Category.GetDomainId(), out var cateDto))
                {
                    cateDto = CategoryDto.ConvertFrom(ia.Category);
                    categories.Add(categoryId, cateDto);
                }

                archive.Category = cateDto;
                yield return archive;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catPath"></param>
        /// <param name="includeChild"></param>
        /// <param name="number"></param>
        /// <param name="skipSize"></param>
        /// <returns></returns>
        public ArchiveDto[] GetArchivesByCategoryPath(int siteId, string catPath,
            bool includeChild, int number, int skipSize)
        {
            var content = _contentRep.GetContent(siteId);
            var archives = content.GetArchivesByCategoryPath(catPath,
                includeChild, number, skipSize);
            return GetArchiveEnumerator(archives).ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="moduleId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public ArchiveDto[] GetArchivesByModuleId(int siteId, int moduleId, int number)
        {
            var content = _contentRep.GetContent(siteId);
            var archives = content.GetArchivesByModuleId(moduleId, number);

            return GetArchiveEnumerator(archives).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catPath"></param>
        /// <param name="includeChild"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public ArchiveDto[] GetArchivesByViewCount(int siteId, string catPath, bool includeChild, int number)
        {
            var content = _contentRep.GetContent(siteId);
            var archives = content.GetArchivesByViewCount(catPath, includeChild, number);

            return GetArchiveEnumerator(archives).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="moduleId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public ArchiveDto[] GetSpecialArchivesByModuleId(int siteId, int moduleId, int number)
        {
            var content = _contentRep.GetContent(siteId);
            var archives = content.GetSpecialArchivesByModuleId(moduleId, number);

            return GetArchiveEnumerator(archives).ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catPath"></param>
        /// <param name="includeChild"></param>
        /// <param name="number"></param>
        /// <param name="skipSize"></param>
        /// <returns></returns>
        public ArchiveDto[] GetSpecialArchives(int siteId, string catPath, bool includeChild, int number, int skipSize)
        {
            var content = _contentRep.GetContent(siteId);
            var archives = content.GetSpecialArchives(
                catPath, includeChild, number, skipSize);

            return GetArchiveEnumerator(archives).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="moduleId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public ArchiveDto[] GetArchivesByViewCountByModuleId(int siteId, int moduleId, int number)
        {
            var content = _contentRep.GetContent(siteId);
            var archives = content.GetArchivesByViewCountByModuleId(moduleId, number);

            return GetArchiveEnumerator(archives).ToArray();
        }


        public DataTable GetPagedArchives(int siteId, int? categoryId, int publisherId, bool includeChild,
            int flag, string keyword,
            string orderByField, bool orderAsc, int pageSize, int currentPageIndex, out int recordCount, out int pages)
        {
            var ic = _catRepo.GetCategory(siteId, categoryId ?? 0);

            int[] catIdArray;
            if (includeChild)
                catIdArray = GetCatArrayByPath(ic);
            else
                catIdArray = new[] {categoryId ?? 0};

            return _archiveQuery.GetPagedArchives(siteId, catIdArray, publisherId,
                includeChild, flag, keyword, orderByField, orderAsc, pageSize, currentPageIndex,
                out recordCount, out pages);
        }


        public DataTable GetPagedArchives(
            int siteId,
            string catPath,
            int pageSize,
            int skipSize,
            ref int pageIndex,
            out int records,
            out int pages,
            out IDictionary<int, IDictionary<string, string>> extendValues)
        {
            var ic = _catRepo.GetCategoryByPath(siteId, catPath);
            var catIdArray = GetCatArrayByPath(ic);
            //获取数据
            var dt = _archiveQuery.GetPagedArchives(siteId, catIdArray,
                pageSize, skipSize, ref pageIndex, out records, out pages);

            IList<int> archiveIds = new List<int>();
            foreach (DataRow dr in dt.Rows)
                archiveIds.Add(int.Parse(dr["id"].ToString()));


            var dict = _extendRep.GetExtendFieldValuesList(
                siteId,
                ExtendRelationType.Archive,
                archiveIds
            );

            extendValues = new Dictionary<int, IDictionary<string, string>>();
            foreach (var key in dict.Keys)
            {
                if (!extendValues.ContainsKey(key)) extendValues.Add(key, new Dictionary<string, string>());
                foreach (var value in dict[key])
                    // 避免重复键
                    if (!extendValues[key].ContainsKey(value.Field.Name))
                        extendValues[key].Add(value.Field.Name, value.Value);
                    else
                        extendValues[key][value.Field.Name] = value.Value;
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
            IList<int> list = ic.Children.Select(a => a.GetDomainId()).ToList();
            list.Insert(0, ic.GetDomainId());
            return list.ToArray();
        }

        public void DeleteArchive(int siteId, int archiveId)
        {
            var content = _contentRep.GetContent(siteId);
            content.DeleteArchive(archiveId);
        }


        public void RepublishArchive(int siteId, int archiveId)
        {
            var content = _contentRep.GetContent(siteId);
            content.RepublishArchive(archiveId);
        }


        public IEnumerable<ArchiveDto> SearchArchivesByCategory(int siteId, string catPath, string keyword,
            int pageSize,
            int pageIndex, out int records, out int pages,
            string orderBy)
        {
            var content = _contentRep.GetContent(siteId);
            var archives = content.SearchArchivesByCategory(
                catPath, keyword, pageSize,
                pageIndex, out records, out pages, orderBy);
            return GetArchiveEnumerator(archives);
        }


        public IEnumerable<ArchiveDto> SearchArchives(int siteId, string catPath,
            bool onlyMatchTitle, string keyword, int pageSize, int pageIndex, out int records,
            out int pages, string orderBy)
        {
            var content = _contentRep.GetContent(siteId);
            var archives = content.SearchArchives(catPath,
                onlyMatchTitle, keyword, pageSize, pageIndex, out records, out pages, orderBy);
            return GetArchiveEnumerator(archives);
        }


        public bool CheckArchiveAliasAvailable(int siteId, int archiveId, string alias)
        {
            var content = _contentRep.GetContent(siteId);
            var archive = content.GetArchiveByTag(alias);
            var archiveIsNull = archive == null;

            if (archiveIsNull)
                return true;

            if (archiveId <= 0) return archiveIsNull;


            return archive.GetAggregateRootId() == archiveId;
        }


        public void AddCountForArchive(int siteId, int id, int count)
        {
            var content = _contentRep.GetContent(siteId);
            content.AddCountForArchive(id, count);
        }


        public IEnumerable<ArchiveDto> GetRelatedArchives(int siteId, int contentId)
        {
            throw new NotImplementedException();
            //return this._archiveQuery.GetRelatedArchives(siteId, contentId);
        }


        public void MoveSortNumber(int siteId, int id, int direction)
        {
            IBaseContent archive = _contentRep.GetContent(siteId).GetArchiveById(id);
            if (archive == null) throw new ArgumentException("no such archive", "id");

            if (direction == 1)
                archive.MoveSortUp();
            else if (direction == 2) archive.MoveSortDown();
        }


        public void BatchDelete(int siteId, int[] idArray)
        {
            var content = _contentRep.GetContent(siteId);
            foreach (var id in idArray) content.DeleteArchive(id);
        }

        public Error UpdateArchivePath( int siteId,  int archiveId)
        {
            var content = _contentRep.GetContent(siteId);
            var archive = content.GetArchiveById(archiveId);
            if (archive == null) return new Error("no such archive");
            var v = archive.Get();
            v.Path = "";
            return archive.Set(v) ?? archive.Save();
        }
    }
}