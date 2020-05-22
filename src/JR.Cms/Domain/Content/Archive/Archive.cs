using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Infrastructure;
using JR.Stand.Core.Framework;

//
// 2012-10-01 添加文档扩展属性
// 2013-06-09 14:00 newmin [+]: Thumbnail
//


namespace JR.Cms.Domain.Content.Archive
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
            _value = value;
            _archiveRep = archiveRep;
            _templateRep = templateRep;
            _catRepo = categoryRep;
        }

        public override int ContentModelIndent => 1;

        public TemplateBind Template
        {
            get
            {
                if (_templateBind == null)
                {
                    //if (!this._templateBindLoaded)
                    //{
                    _templateBind = _templateRep.GetTemplateBind(
                        GetAggregateRootId(),
                        TemplateBindType.ArchiveTemplate);

                    //如果没有的话，则获取分类的模板
                    if (_templateBind == null)
                        foreach (var tplBind in Category.GetTemplates())
                            if (tplBind.BindType == TemplateBindType.CategoryArchiveTemplate)
                            {
                                _templateBind = tplBind;
                                break;
                            }

                    //this._templateBindLoaded=true;
                }

                return _templateBind;
            }
            set => _templateBind = value;
        }

        public void SetTemplatePath(string templatePath)
        {
            var templateIsNull = string.IsNullOrEmpty(templatePath);

            if (Template != null)
                //处理自身的模板
                if (Template.BindRefrenceId == GetAggregateRootId())
                {
                    if (templateIsNull)
                    {
                        Template.TplPath = null;
                    }
                    else
                    {
                        Template.TplPath = templatePath;
                    }
                    return;
                }

            //为文档新建模板绑定
            if (!templateIsNull)
            {
                Template = _templateRep.CreateTemplateBind(
                    -1,
                    TemplateBindType.ArchiveTemplate,
                    templatePath);
                Template.BindRefrenceId = GetAggregateRootId();
            }
        }

        public IList<IExtendValue> GetExtendValues()
        {
            if (_extendValues == null) _extendValues = new List<IExtendValue>(_extendRep.GetExtendFieldValues(this));
            return _extendValues;
        }


        public Error SetExtendValue(IList<IExtendValue> extendValues)
        {
            _extendValues = extendValues;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Error Save()
        {
            _value.SiteId = Category.Get().SiteId;
            this.CheckArchiveAlias();
            this.UpdateArchivePath();
            if (_value.SortNumber <= 0)
            {
                var sortNum = _archiveRep.GetMaxSortNumber(Category.Site().GetAggregateRootId());
                _value.SortNumber = sortNum + 1;
            }

            this._archiveRep.SaveArchive(Get());
            //保存文档绑定的模板
            if (this._templateBind != null)
            {
                if (_templateBind != null)
                {
                    this._templateBind.BindRefrenceId = GetAggregateRootId();
                }

                this._templateRep.SaveTemplateBind(GetAggregateRootId(), _templateBind);
                if (_templateBind.TplPath == null)
                {
                    this._templateBind = null;
                }
            }

            //保存扩展属性
            _extendRep.UpdateArchiveRelationExtendValues(this);

            //保存其他
            return base.Save();
        }

        private void CheckArchiveAlias()
        {
            if (String.IsNullOrEmpty(this._value.Alias))
            {
                // 生成5位随机ID
                string strId;
                do
                {
                    strId = IdGenerator.GetNext(5); //创建5位ID
                } while (_archiveRep.CheckSidIsExist(_value.SiteId, strId));

                this._value.Alias = strId;
            }
        }

        /// <inheritdoc />
        public override void MoveSortDown()
        {
            var siteId = Category.Site().GetAggregateRootId();
            var prev = _archiveRep.GetNextArchive(siteId, GetAggregateRootId(), true, true);
            SwapSortNumber(prev);
        }


        /// <inheritdoc />
        public override void MoveSortUp()
        {
            var siteId = Category.Site().GetAggregateRootId();
            var next = _archiveRep.GetPreviousArchive(siteId, GetAggregateRootId(), true, true);
            SwapSortNumber(next);
        }

        private Error SwapSortNumber(IArchive src)
        {
            if (src == null) return null;
            var sv = src.Get();
            var sortN = sv.SortNumber;
            sv.SortNumber = _value.SortNumber;
            var err = src.Set(sv);
            if (err == null) src.Save();
            _value.SortNumber = sortN;
            return Save();
        }

        public string FirstImageUrl
        {
            get
            {
                return "";
                const string imgTagRegPattern = "<img[^>]*\\bsrc=\"(?<imguri>[^\"]+)\"[^>]*>";
                const bool ignoreBase64 = true;

                //用""来表示为空
                if (_firstImageUrl == null)
                {
                    var reg = new Regex(imgTagRegPattern);

                    if (reg.IsMatch(Get().Content))
                    {
                        //忽略base64格式的图片
                        if (ignoreBase64)
                        {
                            //匹配结果
                            string matchResult;
                            var mcs = reg.Matches(Get().Content);
                            foreach (Match match in mcs)
                            {
                                matchResult = match.Groups["imguri"].Value;
                                if (!Regex.IsMatch(matchResult, "^data:image/[a-z]+;base64", RegexOptions.IgnoreCase))
                                {
                                    _firstImageUrl = matchResult;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            _firstImageUrl = reg.Match(Get().Content).Groups["imguri"].Value;
                        }
                    }

                    if (_firstImageUrl == null) _firstImageUrl = "";
                }

                return _firstImageUrl == "" ? null : _firstImageUrl;
            }
        }


        /// <summary>
        /// 组合文档路径
        /// </summary>
        /// <returns></returns>
        private string CombineArchivePath()
        {
            var alias = string.IsNullOrEmpty(_value.Alias) ? _value.StrId : _value.Alias;
            return string.Concat(Category.Get().Path, "/", alias);
        }

        /// <summary>
        /// 更新文档路径
        /// </summary>
        private void UpdateArchivePath()
        {
            // 设置了跳转地址
            if (!string.IsNullOrEmpty(_value.Location))
            {
                _value.Path = _value.Location;
                return;
            }

            _value.Path = CombineArchivePath();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CmsArchiveEntity Get()
        {
            return _value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Error Set(CmsArchiveEntity src)
        {
            _value.ID = src.ID;
            _value.StrId = src.StrId;
            _value.SiteId = src.SiteId;
            _value.CatId = src.CatId;
            _value.Path = src.Path;
            _value.Flag = src.Flag;
            _value.AuthorId = src.AuthorId;
            _value.Title = src.Title;
            _value.SmallTitle = src.SmallTitle;
            _value.Location = src.Location;
            _value.Source = src.Source;
            _value.Tags = src.Tags;
            _value.Outline = src.Outline;
            _value.Content = src.Content;
            _value.ViewCount = src.ViewCount;
            _value.Agree = src.Agree;
            _value.Disagree = src.Disagree;
            _value.UpdateTime = src.UpdateTime;
            _value.CreateTime = src.CreateTime;
            _value.Thumbnail = src.Thumbnail;
            var unix = TimeUtils.Unix(DateTime.Now);
            _value.UpdateTime = unix;

            if (GetAggregateRootId() <= 0)
            {
                if (src.SiteId <= 0) return new Error("参数错误：SiteId");
                if (_value.CatId < 0) return new Error("参数错误:CatId");
                _value.SiteId = src.SiteId;
                _value.CatId = src.CatId;
                _value.CreateTime = unix;
            }

            var ic = Category;
            if (ic == null) return new Error("栏目不存在");
            // 如果设置了别名，检测路径是缶匹配
            if (!string.IsNullOrEmpty(src.Alias))
            {
                var path = CombineArchivePath();
                var isMatch = _archiveRep.CheckPathMatch(_value.SiteId, path, GetAggregateRootId());
                if (!isMatch) return new Error("文档路径已存在");
                this._value.Alias = src.Alias;
            }
            if (src.SortNumber > 0) _value.SortNumber = src.SortNumber;
            return null;
        }

        /// <inheritdoc />
        public override int GetAggregateRootId()
        {
            return _value.ID;
        }


        /// <summary>
        /// 栏目编号
        /// </summary>
        public ICategory Category
        {
            get
            {
                if (_category == null)
                    //todo: 获取站点
                    _category = _catRepo.GetCategory(_value.SiteId, _value.CatId);
                return _category;
            }
            set => _category = value;
        }
    }
}