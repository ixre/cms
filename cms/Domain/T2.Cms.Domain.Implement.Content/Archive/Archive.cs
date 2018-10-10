using JR.DevFw.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using T2.Cms.Domain.Interface.Content;
using T2.Cms.Domain.Interface.Content.Archive;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Extend;
using T2.Cms.Domain.Interface.Site.Template;
using T2.Cms.Infrastructure;
using T2.Cms.Models;

//
// 2012-10-01 添加文档扩展属性
// 2013-06-09 14:00 newmin [+]: Thumbnail
//


namespace T2.Cms.Domain.Implement.Content.Archive
{
    /// <summary>
    /// 文档
    /// </summary>
    public sealed class Archive : BaseContent, IArchive
    {
        private ITemplateRepo _templateRep;
        private ICategoryRepo _catRepo;
        private TemplateBind _templateBind;
        private string _firstImageUrl;
        private IArchiveRepository _archiveRep;
        private string _uri;
        private CmsArchiveEntity _value;

        internal Archive(
            IContentRepository contentRep,
            IArchiveRepository archiveRep,
            IExtendFieldRepository extendRep,
            ICategoryRepo categoryRep,
            ITemplateRepo templateRep,
          CmsArchiveEntity value)
            : base(
                contentRep,
                extendRep,
                categoryRep,
                templateRep
                )
        {
            this._value = value;
            this._archiveRep = archiveRep;
            this._templateRep = templateRep;
            this._catRepo = categoryRep;
        }

        public override int ContentModelIndent
        {
            get { return 1; }
        }
        
        public TemplateBind Template
        {
            get
            {
                if (this._templateBind == null)
                {
                    //if (!this._templateBindLoaded)
                    //{
                    this._templateBind = this._templateRep.GetTemplateBind(
                        this.GetAggregaterootId(),
                        TemplateBindType.ArchiveTemplate);

                    //如果没有的话，则获取分类的模板
                    if (this._templateBind == null)
                    {
                        foreach (TemplateBind tplBind in this.Category.GetTemplates())
                        {
                            if (tplBind.BindType == TemplateBindType.CategoryArchiveTemplate)
                            {
                                this._templateBind = tplBind;
                                break;
                            }
                        }
                    }

                    //this._templateBindLoaded=true;
                }
                return this._templateBind;
            }
            set
            {
                _templateBind = value;
            }
        }

        public void SetTemplatePath(string templatePath)
        {
            bool templateIsNull = String.IsNullOrEmpty(templatePath);

            if (this.Template != null)
            {
                //处理自身的模板
                if (this.Template.BindRefrenceId == this.GetAggregaterootId())
                {
                    if (templateIsNull)
                    {
                        this.Template.TplPath = null;
                    }
                    else
                    {
                        this.Template.TplPath = templatePath;
                    }
                    return;
                }
            }

            //为文档新建模板绑定
            if (!templateIsNull)
            {
                this.Template = this._templateRep.CreateTemplateBind(
                       -1,
                       TemplateBindType.ArchiveTemplate,
                       templatePath);
                this.Template.BindRefrenceId = this.GetAggregaterootId();
            }
        }

        public IList<IExtendValue> GetExtendValues()
        {
            if (this._extendValues == null)
            {
                this._extendValues = new List<IExtendValue>(this._extendRep.GetExtendFieldValues(this));
            }
            return this._extendValues;
        }


        public Error SetExtendValue(IList<IExtendValue> extendValues)
        {
            this._extendValues = extendValues;
            return null;
        }

        public override Error Save()
        {
            this._value.SiteId = this.Category.Get().SiteId;
            this.UpdateArchivePath();
            if (this._value.SortNumber <= 0)
            {
                int sortNum = this._archiveRep.GetMaxSortNumber(this.Category.Site().GetAggregaterootId());
                this._value.SortNumber = sortNum + 1;
            }
            this._archiveRep.SaveArchive(this.Get());


            if (this._templateBind != null)
            {
            }
            //保存文档绑定的模板
            if (this._templateBind != null)
            {
                if (this._templateBind != null)
                {
                    this._templateBind.BindRefrenceId = this.GetAggregaterootId();
                }
                this._templateRep.SaveTemplateBind(this.GetAggregaterootId(), this._templateBind);
                if (this._templateBind.TplPath == null)
                {
                    this._templateBind = null;
                }
            }

            //保存扩展属性
            this._extendRep.UpdateArchiveRelationExtendValues(this);

            //保存其他
            return base.Save();

        }

        public override void MoveSortDown()
        {
            int siteId = this.Category.Site().GetAggregaterootId();
            IArchive prev = this._archiveRep.GetNextArchive(siteId, this.GetAggregaterootId(), true, true);
            this.SwapSortNumber(prev);
        }


        public override void MoveSortUp()
        {
            int siteId = this.Category.Site().GetAggregaterootId();
            IArchive next = this._archiveRep.GetPreviousArchive(siteId, this.GetAggregaterootId(), true, true);

            this.SwapSortNumber(next);
        }

        private Error SwapSortNumber(IArchive src)
        {
            if (src == null) return null;
            CmsArchiveEntity sv = src.Get();
            int sortN = sv.SortNumber;
            sv.SortNumber = this._value.SortNumber;
            Error err = src.Set(sv);
            if(err == null)
            {
               src.Save();
            }
            this._value.SortNumber = sortN;
            return this.Save();
           //src.SaveSortNumber();
            //this.SaveSortNumber();
        }

        public string FirstImageUrl
        {
            get
            {
                return "";
                const string imgTagRegPattern = "<img[^>]*\\bsrc=\"(?<imguri>[^\"]+)\"[^>]*>";
                const bool ignoreBase64 = true;

                //用""来表示为空
                if (this._firstImageUrl == null)
                {
                    Regex reg = new Regex(imgTagRegPattern);

                    if (reg.IsMatch(this.Get().Content))
                    {
                        //忽略base64格式的图片
                        if (ignoreBase64)
                        {
                            //匹配结果
                            string matchResult;
                            MatchCollection mcs = reg.Matches(this.Get().Content);
                            foreach (Match match in mcs)
                            {
                                matchResult = match.Groups["imguri"].Value;
                                if (!Regex.IsMatch(matchResult, "^data:image/[a-z]+;base64", RegexOptions.IgnoreCase))
                                {
                                    this._firstImageUrl = matchResult;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            this._firstImageUrl = reg.Match(this.Get().Content).Groups["imguri"].Value;
                        }
                    }

                    if (this._firstImageUrl == null) this._firstImageUrl = "";
                }

                return this._firstImageUrl == "" ? null : this._firstImageUrl;
            }
        }


        public string StrId
        {
            get;
            private set;
        }

        /// <summary>
        /// 更新文档路径
        /// </summary>
        public void UpdateArchivePath()
        {
            // 设置了跳转地址
            if (!String.IsNullOrEmpty(this._value.Location))
            {
                this._value.Path = this._value.Location;
                return;
            }
            String alias = String.IsNullOrEmpty(this._value.Alias) ? this.StrId : this._value.Alias;
            this._value.Path = String.Concat(this.Category.Get().Path,"/", alias);
        }

        public CmsArchiveEntity Get()
        {
            return this._value;
        }

        public Error Set(CmsArchiveEntity src)
        {
            this._value.ID = src.ID;
            this._value.StrId = src.StrId;
            this._value.SiteId = src.SiteId;
            this._value.Alias = src.Alias;
            this._value.CatId = src.CatId;
            this._value.Path = src.Path;
            this._value.Flag = src.Flag;
            this._value.AuthorId = src.AuthorId;
            this._value.Title = src.Title;
            this._value.SmallTitle = src.SmallTitle;
            this._value.Location = src.Location;
            this._value.Source = src.Source;
            this._value.Tags = src.Tags;
            this._value.Outline = src.Outline;
            this._value.Content = src.Content;
            this._value.ViewCount = src.ViewCount;
            this._value.Agree = src.Agree;
            this._value.Disagree = src.Disagree;
            this._value.UpdateTime = src.UpdateTime;
            this._value.CreateTime = src.CreateTime;
            this._value.Thumbnail = src.Thumbnail;
            int unix = TimeUtils.Unix(DateTime.Now);
            this._value.UpdateTime = unix;

            if (this.GetAggregaterootId() <= 0)
            {
                if(src.SiteId <= 0)
                {
                    return new Error("参数错误：SiteId");
                }
                if(this._value.CatId < 0)
                {
                    return new Error("参数错误:CatId");
                }
                this._value.SiteId = src.SiteId;
                this._value.CatId = src.CatId;
                this._value.CreateTime = unix;
                string strId;
                do
                {
                    strId = IdGenerator.GetNext(5);              //创建5位ID
                } while (this._archiveRep.CheckSidIsExist(this._value.SiteId, strId));
            }
            ICategory ic = this.Category;
            if (ic == null)
            {
                return new Error("栏目不存在");
            }
            // 如果设置了别名，检测路径是缶匹配
            if (!String.IsNullOrEmpty(src.Alias))
            {
                bool isMatch = this._archiveRep.CheckPathMatch(this._value.SiteId,
                    ic.Get().Path+"/"+src.Alias, this.GetAggregaterootId());
                if (!isMatch)
                {
                    return new Error("文档路径已存在");
                }
            }
            if (src.SortNumber > 0)
            {
                this._value.SortNumber = src.SortNumber;
            }
            return null;
        }

        public override int GetAggregaterootId()
        {
            return this._value.ID;
        }


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
                    this._category = this._catRepo.GetCategory(this._value.SiteId, this._value.CatId);
                }
                return this._category;
            }
            set
            {
                this._category = value;
            }
        }
    }
}
