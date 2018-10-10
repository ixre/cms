using System;
using System.Collections.Generic;
using System.Linq;
using T2.Cms.Domain.Interface.Content;
using T2.Cms.Domain.Interface.Content.Archive;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Template;
using T2.Cms.Models;

namespace T2.Cms.Domain.Implement.Content
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
            this._catRepo = catRepo;
            this._archiveRep = archiveRep;
            this._tempRep = tempRep;
            this.SiteId = siteId;
        }


        public int GetAggregaterootId()
        {
            return this.Id;
        }

        public int Id
        {
            get { return this.SiteId; }
        }

        public int SiteId
        {
            get;
            private set;
        }

        public IArchive CreateArchive(CmsArchiveEntity value)
        {
            return _archiveRep.CreateArchive(value);
        }


        public IArchive GetArchiveByTag(string id)
        {
            return this._archiveRep.GetArchive(this.SiteId, id);
        }


        public IArchive GetArchiveById(int archiveId)
        {
            return this._archiveRep.GetArchiveById(this.SiteId, archiveId);
        }


        public IEnumerable<IArchive> GetArchivesByCategoryPath(string catPath,bool includeChild, int number, int skipSize)
        {
            ICategory ic = this._catRepo.GetCategoryByPath(this.SiteId, catPath);
            int[] catIdArray;
            if (includeChild)
            {
                catIdArray = this.GetCatArrayByPath(ic);
                if (catIdArray.Length == 0) return new List<IArchive>();
            }
            else
            {
                catIdArray = new int[] { ic.GetDomainId() };
            }
            return this._archiveRep.GetArchivesContainChildCategories(this.SiteId, catIdArray, number, skipSize);
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

        public IEnumerable<IArchive> GetArchivesByModuleId(int moduleId, int number)
        {
            return this._archiveRep.GetArchivesByModuleId(this.SiteId, moduleId, number);
        }
        
        public IEnumerable<IArchive> GetArchivesByViewCount(string catPath, bool includeChild, int number)
        {
            ICategory ic = this._catRepo.GetCategoryByPath(this.SiteId, catPath);
            int[] catIdArray;
            if (includeChild)
            {
                catIdArray = this.GetCatArrayByPath(ic);
            }
            else
            {
                catIdArray = new int[] { ic.GetDomainId() };
            }
            return this._archiveRep.GetArchivesByViewCount(this.SiteId, catIdArray, number);
        }

        public IEnumerable<IArchive> GetSpecialArchivesByModuleId(int moduleId, int number)
        {
            return this._archiveRep.GetSpecialArchivesByModuleId(this.SiteId, moduleId, number);
        }
        

        public IEnumerable<IArchive> GetSpecialArchives(string catPath,bool includeChild, int number, int skipSize)
        {
            ICategory ic = this._catRepo.GetCategoryByPath(this.SiteId, catPath);

            int[] catIdArray;
            if (includeChild)
            {
                catIdArray = this.GetCatArrayByPath(ic);
            }
            else
            {
                catIdArray = new int[] {ic.GetDomainId() };
            }
            return this._archiveRep.GetSpecialArchives(this.SiteId, catIdArray, number, skipSize);
        }

        public IEnumerable<IArchive> GetArchivesByViewCountByModuleId(int moduleId, int number)
        {
            return this._archiveRep.GetArchivesByViewCountByModuleId(this.SiteId, moduleId, number);
        }


        public IArchive GetPreviousSiblingArchive(int id)
        {
            return this._archiveRep.GetPreviousArchive(this.SiteId, id, true, false);
        }

        public IArchive GetNextSiblingArchive(int id)
        {
            return this._archiveRep.GetNextArchive(this.SiteId, id, true, false);
        }

        public void RefreshArchive(int archiveId)
        {
            this._archiveRep.RefreshArchive(this.SiteId, archiveId);
        }

        public bool DeleteArchive(int archiveId)
        {
            IArchive archive = this.GetArchiveById(archiveId);
            if (archive == null)
                return false;
            if (this.FlagAnd(archive.Get().Flag, BuiltInArchiveFlags.IsSystem)) {
                throw new NotSupportedException("系统文档，不允许删除,请先取消系统设置后再进行删除！");
            }
            bool result = this._archiveRep.DeleteArchive(this.SiteId, archive.GetAggregaterootId());

            if (result)
            {
                //删除模板绑定
                this._tempRep.RemoveBind(archive.GetAggregaterootId(),TemplateBindType.ArchiveTemplate);


                //
                //TODO:删除评论及点评
                //

                //删除评论
                // new CommentDAL().DeleteArchiveComments(archiveID);

                //删除点评
                //new CommentBLL().DeleteArchiveReviews(archiveID);

            }

            archive = null;

            return result;
        }

        private bool FlagAnd(int flag, BuiltInArchiveFlags b)
        {
            int x = (int)b;
            return (flag & x) == x;
        }

        public IEnumerable<IArchive> SearchArchivesByCategory(
            int categoryLft, int categoryRgt,
            string keyword, int pageSize,
            int pageIndex, out int records,
            out int pages, string orderBy)
        {
            return this._archiveRep.SearchArchivesByCategory(
                this.SiteId, categoryLft, categoryRgt, keyword, pageSize,
            pageIndex, out  records, out pages, orderBy);
        }

        public IEnumerable<IArchive> SearchArchives(
            int categoryLft, int categoryRgt,
            bool onlyMatchTitle,
            string keyword, int pageSize,
            int pageIndex, out int records,
            out int pages, string orderBy)
        {
            return this._archiveRep.SearchArchives(
                this.SiteId, categoryLft, categoryRgt, onlyMatchTitle, keyword, pageSize, pageIndex,
                 out  records, out pages, orderBy);
        }


        public void AddCountForArchive(int id, int count)
        {
            this._archiveRep.AddArchiveViewCount(this.SiteId, id, count);
        }



        public IBaseContent GetContent(string contentType, int contentId)
        {
            IBaseContent content = null;

            switch (contentType.ToLower())
            {
                case "1":
                case "archive":
                    //content = this._archiveRep.CreateArchive(contentId, null, -1, null);
                    content = this._archiveRep.GetArchiveById(this.SiteId, contentId);
                    break;
            }
            if (content == null)
            {
                throw new Exception("内容不存在");
            }
            return content;
        }

        public IArchive GetArchive(string path)
        {
            return this._archiveRep.GetArchiveByPath(this.SiteId, path);
        }
    }
}
