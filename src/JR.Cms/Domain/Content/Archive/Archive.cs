using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JR.Cms.Domain.Interface.Content;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Infrastructure;
using JR.Stand.Core.Extensions;
using JR.Stand.Core.Framework;
using Org.BouncyCastle.Utilities;

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
        private IArchiveRepository _archiveRepo;
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
            _archiveRepo = archiveRep;
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

        /// <inheritdoc />
        public void SetTemplatePath(string templatePath)
        {
            var templateIsNull = string.IsNullOrEmpty(templatePath);
            if (Template != null)
            {
                //处理自身的模板
                if (Template.BindRefrenceId == GetAggregateRootId())
                {
                    Template.TplPath = templateIsNull ? null : templatePath;
                    return;
                }
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


        /// <inheritdoc />
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
            Error err = this.UpdateArchivePath();
            if (err != null)
            {
                return err;
            }
            if (_value.SortNumber <= 0)
            {
                var sortNum = _archiveRepo.GetMaxSortNumber(Category.Site().GetAggregateRootId());
                _value.SortNumber = sortNum + 1;
            }

            this._archiveRepo.SaveArchive(Get());
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
                } while (_archiveRepo.CheckSidIsExist(_value.SiteId, strId));

                this._value.Alias = strId;
            }
        }

        /// <inheritdoc />
        public override void MoveSortDown()
        {
            var siteId = Category.Site().GetAggregateRootId();
            var prev = _archiveRepo.GetNextArchive(siteId, GetAggregateRootId(), true, true);
            SwapSortNumber(prev);
        }


        /// <inheritdoc />
        public override void MoveSortUp()
        {
            var siteId = Category.Site().GetAggregateRootId();
            var next = _archiveRepo.GetPreviousArchive(siteId, GetAggregateRootId(), true, true);
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

        private static bool FlagAnd(int flag, BuiltInArchiveFlags b)
        {
            var x = (int)b;
            return (flag & x) == x;
        }

        /// <summary>
        /// 获取文档路径
        /// </summary>
        private String GetArchivePath()
        {
            // 设置了跳转地址
            if (!string.IsNullOrEmpty(_value.Location))
            {
                // 设置了自定义路径
                return _value.Location;
            }
            if (FlagAnd(_value.Flag, BuiltInArchiveFlags.AsPage))
            {
                // 单页面
                return string.IsNullOrEmpty(_value.Alias) ? _value.StrId : _value.Alias;
            }
            return CombineArchivePath();
        }

        /// <summary>
        /// 更新文档路径
        /// </summary>
        private Error UpdateArchivePath()
        {
            String path = GetArchivePath();
            Boolean isMatch = this._archiveRepo.CheckPathMatch(_value.SiteId, path, GetAggregateRootId());
            if (!isMatch)
            {
                return new Error("文档路径已存在");
            }
            _value.Path = path;
            return null;
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
            if (String.IsNullOrEmpty(this._value.Title))
            {
                return new Error("标题不能为空");
            }
            // 如果设置了别名，检测路径是否匹配
            if (!string.IsNullOrEmpty(src.Alias))
            {
                // var path = CombineArchivePath();
                // var isMatch = _archiveRepo.CheckPathMatch(_value.SiteId, path, GetAggregateRootId());
                // if (!isMatch) return new Error("文档路径已存在");
                this._value.Alias = src.Alias;
            }
            if (src.SortNumber > 0) _value.SortNumber = src.SortNumber;

            // 处理定时发送
            if (src.ScheduleTime > 0)
            {
                if (src.ScheduleTime <= TimeUtils.Unix())
                {
                    return new Error("定时发布时间不能小于当前时间");
                }
                // 定时发送
                _value.ScheduleTime = src.ScheduleTime;
                _value.CreateTime = 0;
            }
            else
            {
                _value.ScheduleTime = 0;
                if (_value.CreateTime <= 0)
                {
                    this._value.CreateTime = TimeUtils.Unix();
                }
            }
            return null;
        }

        /// <inheritdoc />
        public override int GetAggregateRootId()
        {
            return _value.ID;
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="refresh"></param>
        /// <returns></returns>
        public Error Publish(bool refresh = false)
        {
            if (this.GetAggregateRootId() <= 0)
            {
                throw new Error("文章未保存");
            }

            if (this._value.ScheduleTime == 0 && !refresh)
            {
                return new Error("文章已发布");
            }

            long now = TimeUtils.Unix(DateTime.Now);
            if (!refresh && this._value.ScheduleTime > 0)
            {
                // 定时发布
                if (this._value.ScheduleTime > now)
                {
                    return new Error("文章未到发布时间");
                }
            }

            this._value.ScheduleTime = 0;
            this._value.CreateTime = now;
            return this.Save();
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