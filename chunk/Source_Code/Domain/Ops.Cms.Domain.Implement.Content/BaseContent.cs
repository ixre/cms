using Ops.Cms.Domain.Interface.Common;
using Ops.Cms.Domain.Interface.Content;
using Ops.Cms.Domain.Interface.Site.Category;
using Ops.Cms.Domain.Interface.Site.Extend;
using Ops.Cms.Domain.Interface.Site.Template;
using System;
using System.Collections.Generic;

namespace Ops.Cms.Domain.Implement.Content
{
    public abstract class BaseContent : IBaseContent
    {
        protected IExtendFieldRepository _extendRep;
        protected ITemplateRepository _templateRep;
        protected ICategoryRepository _categoryRep;

        protected IList<IExtendValue> _extendValues;
        protected ICategory _category;
        protected ITemplateBind _templateBind;
        protected IContentRepository _contentRep;
        private ILinkRepository _linkRep;
        private ILinkManager _linkManager;

        /// <summary>
        /// 内容模型标识
        /// </summary>
        public abstract int ContentModelIndent{get;}

        public BaseContent(
            IContentRepository contentRep,
            IExtendFieldRepository extendRep,
            ICategoryRepository categoryRep,
            ITemplateRepository templateRep,
            ILinkRepository linkRep,
            int id,
            int categoryId,
            string title)
        {
            this._contentRep = contentRep;
            this._linkRep = linkRep;
            this._extendRep = extendRep;
            this._categoryRep = categoryRep;
            this._templateRep = templateRep;

            this.ID = id;
            this._category = this._categoryRep.CreateCategory(categoryId, null);
            this.Title = title;
            this.ID = id;
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 栏目编号
        /// </summary>
        public ICategory Category
        {
            get
            {
                if (this._category.Site == null)
                {
                    this._category = this._categoryRep.GetCategoryById(this._category.ID);
                }
                return this._category;
            }
            set
            {
                this._category = value;
            }
        }

        /// <summary>
        /// 标签（关键词）
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 文档内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 显示次数
        /// </summary>
        public int ViewCount { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastModifyDate { get; set; }


        public abstract string Uri { get; set; }

        public string Author
        {
            get;
            set;
        }

        public int ID
        {
            get;
            protected set;
        }

        /// <summary>
        /// 保存内容，继承类应重写该类
        /// </summary>
        /// <returns></returns>
        public virtual int Save()
        {
            this.LinkManager.SaveLinks();
            return -1;
        }

        public ILinkManager LinkManager
        {
            get
            {
                return this._linkManager
                    ?? (this._linkManager =
                    new ContentLinkManager(this._linkRep, this.ContentModelIndent, this.ID));
            }
        }
    }
}
