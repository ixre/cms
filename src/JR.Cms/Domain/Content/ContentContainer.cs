using System;
using System.Collections.Generic;
using System.Linq;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Template;

namespace JR.Cms.Domain.Content
{
    public class ContentContainer : IContentContainer
    {
        private ICategoryRepo _catRepo;
        private IArchiveRepository _archiveRep;
        private ITemplateRepo _tempRep;

        internal ContentContainer(
            ICategoryRepo catRepo,
            IArchiveRepository archiveRep,
            ITemplateRepo tempRep,
            int siteId)
        {
            _catRepo = catRepo;
            _archiveRep = archiveRep;
            _tempRep = tempRep;
            SiteId = siteId;
        }


        public int GetAggregaterootId()
        {
            return Id;
        }

        public int Id => SiteId;

        public int SiteId { get; private set; }

        public IArchive CreateArchive(CmsArchiveEntity value)
        {
            return _archiveRep.CreateArchive(value);
        }


        public IArchive GetArchiveByTag(string id)
        {
            return _archiveRep.GetArchive(SiteId, id);
        }


        public IArchive GetArchiveById(int archiveId)
        {
            return _archiveRep.GetArchiveById(SiteId, archiveId);
        }


        public IEnumerable<IArchive> GetArchivesByCategoryPath(string catPath, bool includeChild, int number,
            int skipSize)
        {
            var ic = _catRepo.GetCategoryByPath(SiteId, catPath);
            int[] catIdArray;
            if (includeChild)
            {
                catIdArray = GetCatArrayByPath(ic);
                if (catIdArray.Length == 0) return new List<IArchive>();
            }
            else
            {
                catIdArray = new[] {ic.GetDomainId()};
            }

            return _archiveRep.GetArchivesContainChildCategories(SiteId, catIdArray, number, skipSize);
        }

        /// <summary>
        /// 获取分类下的编号数组
        /// </summary>
        /// <returns></returns>
        private int[] GetCatArrayByPath(ICategory ic)
        {
            if (ic == null) return new int[] { };
            IList<int> list = ic.Childs.Select(a => a.GetDomainId()).ToList();
            list.Insert(0, ic.GetDomainId());
            return list.ToArray();
        }

        public IEnumerable<IArchive> GetArchivesByModuleId(int moduleId, int number)
        {
            return _archiveRep.GetArchivesByModuleId(SiteId, moduleId, number);
        }

        public IEnumerable<IArchive> GetArchivesByViewCount(string catPath, bool includeChild, int number)
        {
            var ic = _catRepo.GetCategoryByPath(SiteId, catPath);
            int[] catIdArray;
            if (includeChild)
                catIdArray = GetCatArrayByPath(ic);
            else
                catIdArray = new int[] {ic.GetDomainId()};
            return _archiveRep.GetArchivesByViewCount(SiteId, catIdArray, number);
        }

        public IEnumerable<IArchive> GetSpecialArchivesByModuleId(int moduleId, int number)
        {
            return _archiveRep.GetSpecialArchivesByModuleId(SiteId, moduleId, number);
        }


        public IEnumerable<IArchive> GetSpecialArchives(string catPath, bool includeChild, int number, int skipSize)
        {
            var ic = _catRepo.GetCategoryByPath(SiteId, catPath);

            int[] catIdArray;
            if (includeChild)
                catIdArray = GetCatArrayByPath(ic);
            else
                catIdArray = new int[] {ic.GetDomainId()};
            return _archiveRep.GetSpecialArchives(SiteId, catIdArray, number, skipSize);
        }

        public IEnumerable<IArchive> GetArchivesByViewCountByModuleId(int moduleId, int number)
        {
            return _archiveRep.GetArchivesByViewCountByModuleId(SiteId, moduleId, number);
        }


        public IArchive GetPreviousSiblingArchive(int id)
        {
            return _archiveRep.GetPreviousArchive(SiteId, id, true, false);
        }

        public IArchive GetNextSiblingArchive(int id)
        {
            return _archiveRep.GetNextArchive(SiteId, id, true, false);
        }

        public void RepublishArchive(int archiveId)
        {
            _archiveRep.RepublishArchive(SiteId, archiveId);
        }

        public bool DeleteArchive(int archiveId)
        {
            var archive = GetArchiveById(archiveId);
            if (archive == null)
                return false;
            if (FlagAnd(archive.Get().Flag, BuiltInArchiveFlags.IsSystem))
                throw new NotSupportedException("系统文档，不允许删除,请先取消系统设置后再进行删除！");
            var result = _archiveRep.DeleteArchive(SiteId, archive.GetAggregaterootId());

            if (result)
                //删除模板绑定
                _tempRep.RemoveBind(archive.GetAggregaterootId(), TemplateBindType.ArchiveTemplate);

            //
            //TODO:删除评论及点评
            //

            //删除评论
            // new CommentDAL().DeleteArchiveComments(archiveID);

            //删除点评
            //new CommentBLL().DeleteArchiveReviews(archiveID);

            archive = null;

            return result;
        }

        private bool FlagAnd(int flag, BuiltInArchiveFlags b)
        {
            var x = (int) b;
            return (flag & x) == x;
        }

        public IEnumerable<IArchive> SearchArchivesByCategory(
            string catPath,
            string keyword, int pageSize,
            int pageIndex, out int records,
            out int pages, string orderBy)
        {
            return _archiveRep.SearchArchivesByCategory(
                SiteId, catPath, keyword, pageSize,
                pageIndex, out records, out pages, orderBy);
        }

        public IEnumerable<IArchive> SearchArchives(
            string catPath,
            bool onlyMatchTitle,
            string keyword, int pageSize,
            int pageIndex, out int records,
            out int pages, string orderBy)
        {
            return _archiveRep.SearchArchives(
                SiteId, catPath, onlyMatchTitle, keyword, pageSize, pageIndex,
                out records, out pages, orderBy);
        }


        public void AddCountForArchive(int id, int count)
        {
            _archiveRep.AddArchiveViewCount(SiteId, id, count);
        }


        public IBaseContent GetContent(string contentType, int contentId)
        {
            IBaseContent content = null;

            switch (contentType.ToLower())
            {
                case "1":
                case "archive":
                    //content = this._archiveRep.CreateArchive(contentId, null, -1, null);
                    content = _archiveRep.GetArchiveById(SiteId, contentId);
                    break;
            }

            if (content == null) throw new Exception("内容不存在");
            return content;
        }

        public IArchive GetArchive(string path)
        {
            return _archiveRep.GetArchiveByPath(SiteId, path);
        }
    }
}