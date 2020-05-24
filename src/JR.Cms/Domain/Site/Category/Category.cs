using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Infrastructure;

namespace JR.Cms.Domain.Site.Category
{
    public class Category : ICategory
    {
        private readonly ICategoryRepo _repo;
        private readonly IExtendFieldRepository _extendRep;

        private IList<IExtendField> _extendFields;
        private ICategory _parent;
        private IEnumerable<ICategory> _childs;
        private ICategory _next;
        private ICategory _previous;
        private IEnumerable<ICategory> _nextLevelChilds;
        private IList<TemplateBind> _templates;
        private readonly CmsCategoryEntity value;
        private ISite site;
        private readonly ISiteRepo siteRepo;
        private readonly ITemplateRepo _tempRep;
        private bool _pathChanged;
        private string _oldPath;

        private readonly string[] errTags = {"public", "uploads", "config", "plugins", "bin", "data", "tmp", "install"};

        internal Category(ICategoryRepo rep, ISiteRepo siteRepo,
            IExtendFieldRepository extendRep, ITemplateRepo tmpRep,
            CmsCategoryEntity value)
        {
            this.value = value;
            _repo = rep;
            _extendRep = extendRep;
            _tempRep = tmpRep;
            this.siteRepo = siteRepo;
        }

        public Error Set(CmsCategoryEntity src)
        {
            if (src.ParentId != 0 && src.ParentId == value.ID) return new Error("父级栏目有误:与子栏目相同");

            if (value.ID <= 0)
            {
                if (src.SiteId <= 0) return new Error("参数错误:SiteId");
                if (string.IsNullOrEmpty(src.Tag)) return new Error("缺少参数:Tag");
                if (string.IsNullOrEmpty(src.Name)) return new Error("栏目名称不能为空");
                value.SiteId = src.SiteId;
                value.Code = src.Code ?? "";
                value.Tag = src.Tag ?? "";
                value.Icon = "";
                value.Path = "";
                value.Flag = 0; //todo: 初始化flag
                var maxSortNumber = this._repo.GetMaxSortNumber(value.SiteId);
                if (maxSortNumber == 0) maxSortNumber = 1;
                value.SortNumber = maxSortNumber;
                _pathChanged = true;
            }

            if (src.Tag == "-") return new Error("不允许使用栏目保留Tag");
            if (src.ParentId == 0 && errTags.Contains(src.Tag)) return new Error("不允许使用保留栏目Tag");

            if (value.ParentId != src.ParentId)
            {
                if (src.ParentId > 0)
                {
                    var ip = _repo.GetCategory(value.SiteId, src.ParentId);
                    if (ip == null || ip.Get().SiteId != value.SiteId) return new Error("上级分类不存在");
                }

                value.ParentId = src.ParentId;
                _pathChanged = true;
            }

            if (string.IsNullOrEmpty(src.Tag)) return new Error("栏目tag不能为空");
            if (!_repo.CheckTagMatch(value.SiteId, value.ParentId, src.Tag, value.ID)) return new Error("分类TAG已存在");
            if (value.Tag != src.Tag) _pathChanged = true;
            value.Tag = src.Tag;
            value.Flag = src.Flag;
            value.ModuleId = src.ModuleId;
            value.Name = src.Name ?? "";
            value.Icon = src.Icon ?? "";
            value.Title = src.Title ?? "";
            value.Keywords = src.Keywords ?? "";
            value.Description = src.Description ?? "";
            value.Location = src.Location ?? "";
            if (string.IsNullOrEmpty(src.Location))
                value.Flag ^= (int) CategoryFlag.Redirect;
            else
                value.Flag |= (int) CategoryFlag.Redirect;
            return null;
        }


        public Error Save()
        {
            if (GetDomainId() <= 0) SetAutoSortNumber();

            // 更新栏目路径
            var pathRenew = _pathChanged;
            if (pathRenew || string.IsNullOrEmpty(value.Path))
            {
                _parent = null;
                UpdateCategoryPath();
                _pathChanged = false;
            }

            var err = _repo.SaveCategory(value);
            if (err == null)
            {
                // 更新子栏目的Tag
                if (pathRenew)
                {
                    foreach (var ic in NextLevelChildren)ic.ForceUpdatePath();
                    // 替换文档路径
                    _repo.ReplaceArchivePath(value.SiteId, _oldPath, value.Path);
                }

                // 保存模板
                if (_templateChanged)
                {
                    err = SaveTemplateBinds();
                    _templateChanged = false;
                }


                #region 保存扩展属性

                if (ExtendFields != null)
                {
                    IList<int> extendIds = new List<int>();
                    foreach (var field in ExtendFields)
                        if (!extendIds.Contains(field.GetDomainId()))
                            extendIds.Add(field.GetDomainId());
                    _extendRep.UpdateCategoryExtends(GetDomainId(), extendIds.ToArray());
                }

                #endregion


                ClearSelf();
                Site().ClearSelf();
            }

            return err;
        }


        // 更新分类的路径
        private void UpdateCategoryPath()
        {
            // 旧的路径
            _oldPath = value.Path;
            // 计算新的路径
            var path = value.Tag;
            ICategory parent = this;
            while (parent != null && (parent = parent.Parent) != null)
            {
                path = string.Concat(parent.Get().Tag, "/", path);
            }
            value.Path = path;
        }

        private void SetAutoSortNumber()
        {
            if (Parent == null)
            {
                value.SortNumber = 1;
                return;
            }

            if (Parent.Children != null && Parent.Children.Any())
                value.SortNumber = Parent.Children.Max(a => a.Get().SortNumber) + 1;
            else
                value.SortNumber = 1;
        }


        public IList<IExtendField> ExtendFields
        {
            get
            {
                return _extendFields ?? (_extendFields = new List<IExtendField>(
                    _extendRep.GetExtendFields(Site().GetAggregateRootId(), GetDomainId())));
            }
            set
            {
                IList<IExtendField> delList = new List<IExtendField>();
                IList<int> addList = new List<int>();
                IList<IExtendField> usedList = new List<IExtendField>();


                bool isExists;

                #region 计算删除的扩展属性

                foreach (var extend in ExtendFields)
                {
                    isExists = false;

                    foreach (var valueExtend in value)
                        if (extend.GetDomainId() == valueExtend.GetDomainId())
                        {
                            isExists = true;
                            break;
                        }

                    if (!isExists)
                    {
                        delList.Add(extend);

                        //验证是否被占用
                        var usedNum = _extendRep.GetCategoryExtendRefrenceNum(this, extend.GetDomainId());
                        if (usedNum > 0) usedList.Add(extend);
                    }
                }

                if (usedList.Count != 0)
                {
                    var strBuilder = new StringBuilder();
                    strBuilder.Append("扩展属性：");
                    var i = 0;
                    foreach (var extend in usedList)
                    {
                        if (i++ != 0) strBuilder.Append("、");
                        strBuilder.Append(extend.Name);
                    }

                    strBuilder.Append("已使用该扩展，请先清理后再删除绑定！");

                    throw new Exception(strBuilder.ToString());
                }


                foreach (var extend in delList) ExtendFields.Remove(extend);

                #endregion

                #region 添加新增的扩展属性

                foreach (var valueExtend in value)
                {
                    isExists = false;
                    foreach (var extend in ExtendFields)
                        if (extend.GetDomainId() == valueExtend.GetDomainId())
                        {
                            isExists = true;
                            break;
                        }

                    if (!isExists) addList.Add(valueExtend.GetDomainId());
                }

                foreach (var extendId in addList)
                    ExtendFields.Add(_extendRep.GetExtendFieldById(Site().GetAggregateRootId(), extendId));

                #endregion


                _extendFields = value;
            }
        }

        public ICategory Parent
        {
            get
            {
                if (_parent == null && value.ParentId > 0) _parent = _repo.GetCategory(value.SiteId, value.ParentId);
                return _parent;
            }
            set => _parent = value;
        }

        public IEnumerable<ICategory> Children
        {
            get
            {
                if (_childs == null) return _repo.GetChilds(value.SiteId, value.Path);
                return _childs;
            }
        }

        /// <inheritdoc />
        public IEnumerable<ICategory> NextLevelChildren
        {
            get
            {
                if (this._nextLevelChilds == null)
                {
                    this._nextLevelChilds = _repo.GetNextLevelChildren(this)
                        .OrderBy(a => a.Get().SortNumber);
                }

                return this._nextLevelChilds;
            }
        }


        public ICategory Next => _next ?? (_next = _repo.GetNext(this));

        public ICategory Previous => _previous ?? (_previous = _repo.GetPrevious(this));


        public void ClearSelf()
        {
            foreach (var category in Children) category.ClearSelf();

            _childs = null;
            _parent = null;
            _next = null;
            _previous = null;
            _extendFields = null;
            _nextLevelChilds = null;
            _templates = null;
        }

        /// <summary>
        /// 向上移动排序
        /// </summary>
        public void MoveSortUp()
        {
            if (Parent != null)
            {
                var list = Parent.Children.OrderBy(a => a.Get().SortNumber).ToArray();
                for (var i = 0; i < list.Length; i++)
                    if (list[i].GetDomainId() == GetDomainId() && i != 0)
                    {
                        SwapSortNumber(list[i - 1], true);
                        break;
                    }
            }
        }

        /// <summary>
        /// 向下移动排序
        /// </summary>
        public void MoveSortDown()
        {
            if (Parent != null)
            {
                var list = Parent.Children.OrderBy(a => a.Get().SortNumber).ToArray();
                for (var i = 0; i < list.Length; i++)
                    if (list[i].GetDomainId() == GetDomainId() && i < list.Length - 1)
                    {
                        SwapSortNumber(list[i + 1], false);
                        break;
                    }
            }
        }

        private void SwapSortNumber(ICategory c, bool up)
        {
            //todo: 应该是更改
            if (c == null) return;
            var sortN = c.Get().SortNumber;
            c.Get().SortNumber = value.SortNumber;
            if (value.SortNumber == sortN || sortN < 0 || value.SortNumber < 0) //为了兼容旧有数据
            {
                if (sortN < 0 && !up)
                {
                    value.SortNumber = 0;
                    c.Get().SortNumber += 1;
                    if (c.Get().SortNumber <= 0) c.Get().SortNumber = 1;
                }
                else
                {
                    value.SortNumber = sortN + (up ? 1 : -1);
                }
            }
            else
            {
                // 以上均为兼容
                value.SortNumber = sortN;
            }

            c.SaveSortNumber();
            SaveSortNumber();
        }

        /// <summary>
        /// 保存排序号码
        /// </summary>
        public void SaveSortNumber()
        {
            _repo.SaveCategorySortNumber(GetDomainId(), value.SortNumber);
        }

        public CmsCategoryEntity Get()
        {
            return value;
        }


        public ISite Site()
        {
            if (site == null && value.SiteId > 0) site = siteRepo.GetSiteById(value.SiteId);
            return site;
        }

        public int GetDomainId()
        {
            return value.ID;
        }

        private bool _templateChanged = false;
        private TemplateBind[] _newTemplates;

        public Error SetTemplates(TemplateBind[] arr)
        {
            _templateChanged = true;
            _newTemplates = arr;
            return null;
        }

        public IList<TemplateBind> GetTemplates()
        {
            if (_templates == null)
            {
                var ie = _tempRep.GetTemplateBindsForCategory(this);
                _templates = new List<TemplateBind>(ie);
            }

            return _templates;
        }


        private Error SaveTemplateBinds()
        {
            IList<TemplateBind> delList = new List<TemplateBind>();
            var origin = GetTemplates();
            _newTemplates = _newTemplates ?? new TemplateBind[0];
            IDictionary<TemplateBindType, TemplateBind> tplMap = new Dictionary<TemplateBindType, TemplateBind>();
            foreach (var b in _newTemplates)
            {
                if (b == null) return new Error("参数包含空的模板");
                tplMap.Add(b.BindType, b);
            }

            foreach (var templateBind in origin)
            {
                TemplateBind b;
                tplMap.TryGetValue(templateBind.BindType, out b);
                if (b == null || string.IsNullOrEmpty(b.TplPath))
                    delList.Add(templateBind);
                else if (b.ID <= 0) b.ID = templateBind.ID;
            }

            var err = _tempRep.RemoveBinds(GetDomainId(), delList.ToArray());
            if (err == null)
            {
                err = _tempRep.SaveTemplateBinds(GetDomainId(), _newTemplates);
                if (err == null) _templates = null;
            }

            return err;
        }

        public void ForceUpdatePath()
        {
            UpdateCategoryPath();
            var err = Save();
            if (err == null)
            {
                foreach (var ic in NextLevelChildren)
                {
                    ic.ForceUpdatePath();
                }
            }
        }
    }
}