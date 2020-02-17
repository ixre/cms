using System.Collections.Generic;
using JR.Cms.Domain.Content;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Infrastructure;

namespace JR.Cms.Domain.Implement.Content
{
    public abstract class BaseContent : IBaseContent
    {
        protected readonly IExtendFieldRepository _extendRep;
        private readonly ITemplateRepo _templateRep;
        private readonly ICategoryRepo _categoryRep;

        protected IList<IExtendValue> _extendValues;
        protected  ICategory _category;
        private readonly IContentRepository _contentRep;
        private  IContentLinkManager _linkManager;
        protected int _categoryId;


        public abstract int GetAggregaterootId();

        /// <summary>
        /// 内容模型标识
        /// </summary>
        public abstract int ContentModelIndent { get; }

        public BaseContent(
            IContentRepository contentRep,
            IExtendFieldRepository extendRep,
            ICategoryRepo categoryRep,
            ITemplateRepo templateRep)
        {
            this._contentRep = contentRep;
            this._extendRep = extendRep;
            this._categoryRep = categoryRep;
            this._templateRep = templateRep;
        }
       
        

        /// <summary>
        /// 保存内容，继承类应重写该类
        /// </summary>
        /// <returns></returns>
        public virtual Error Save()
        {
            this.LinkManager.SaveRelatedLinks();
            return null;
        }

        public IContentLinkManager LinkManager
        {
            get
            {
                return this._linkManager
                    ?? (this._linkManager =
                    new ContentLinkManager(this._contentRep,((ContentTypeIndent) this.ContentModelIndent).ToString().ToLower(), this.GetAggregaterootId()));
            }
        }

        /// <summary>
        /// 下移排序
        /// </summary>
        public abstract void MoveSortDown();


        /// <summary>
        /// 上移排序
        /// </summary>
        public abstract void MoveSortUp();
    }
}
