using System;
using System.Collections.Generic;
using T2.Cms.Domain.Interface.Common;
using T2.Cms.Domain.Interface.Content;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Extend;
using T2.Cms.Domain.Interface.Site.Template;

namespace T2.Cms.Domain.Implement.Content
{
    public abstract class BaseContent : IBaseContent
    {
        protected readonly IExtendFieldRepository _extendRep;
        private readonly ITemplateRepository _templateRep;
        private readonly ICategoryRepo _categoryRep;

        protected IList<IExtendValue> _extendValues;
        protected  ICategory _category;
        private readonly ITemplateBind _templateBind;
        private readonly IContentRepository _contentRep;
        private  IContentLinkManager _linkManager;
        protected int _categoryId;


        public int GetAggregaterootId()
        {
            return this.Id;
        }

        /// <summary>
        /// 内容模型标识
        /// </summary>
        public abstract int ContentModelIndent { get; }

        public BaseContent(
            IContentRepository contentRep,
            IExtendFieldRepository extendRep,
            ICategoryRepo categoryRep,
            ITemplateRepository templateRep,
            int id,
            int categoryId,
            string title)
        {
            this._contentRep = contentRep;
            this._extendRep = extendRep;
            this._categoryRep = categoryRep;
            this._templateRep = templateRep;

            this.Id = id;
            this.Title = title;
            this._categoryId = categoryId;
            this.Id = id;
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 小标题
        /// </summary>
        public String SmallTitle { get; set; }

        /// <summary>
        /// 栏目编号
        /// </summary>
        public ICategory Category
        {
            get
            {
                if (this._category == null)
                {
                    //todo: 获取站点
                    this._category = this._categoryRep.GetCategory(0,this._categoryId);
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

        /// <summary>
        /// 排序序号
        /// </summary>
        public int SortNumber { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastModifyDate { get; set; }


        public abstract string Uri { get; set; }

        public string Location { get; set; }

        public int PublisherId
        {
            get;
            set;
        }

        public int Id
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
            this.LinkManager.SaveRelatedLinks();
            return -1;
        }

        public IContentLinkManager LinkManager
        {
            get
            {
                return this._linkManager
                    ?? (this._linkManager =
                    new ContentLinkManager(this._contentRep,((ContentTypeIndent) this.ContentModelIndent).ToString().ToLower(), this.Id));
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
