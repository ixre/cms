using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T2.Cms.Domain.Interface.Site;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Extend;
using T2.Cms.Domain.Interface.Site.Template;
using T2.Cms.Infrastructure;
using T2.Cms.Models;

namespace T2.Cms.Domain.Implement.Site.Category
{
    public class Category : ICategory
    {
        private ICategoryRepo _repo;
        private IExtendFieldRepository _extendRep;

        private IList<IExtendField> _extendFields;
        private ICategory _parent;
        private IEnumerable<ICategory> _childs;
        private ICategory _next;
        private ICategory _previous;
        private IEnumerable<ICategory> _nextLevelChilds;
        private IList<TemplateBind> _templates;
        private CmsCategoryEntity value;
        private ISite site;
        private ISiteRepo siteRepo;
        private readonly ITemplateRepo _tempRep;
        private bool _parentChanged = false;

        internal Category(ICategoryRepo rep, ISiteRepo siteRepo,
            IExtendFieldRepository extendRep, ITemplateRepo tmpRep, CmsCategoryEntity value)
        {
            this.value = value;
            this._repo = rep;
            this._extendRep = extendRep;
            this._tempRep = tmpRep;
            this.siteRepo = siteRepo;
        }

        public int Lft
        {
            get;
            set;
        }

        public int Rgt
        {
            get;
            set;
        }


        public Error Set(CmsCategoryEntity src)
        {
            if (src.ParentId != 0 && src.ParentId == this.value.ID)
            {
                return new Error("父级栏目有误:与子栏目相同");
            }

            if (this.value.ID <= 0)
            {
                if (src.SiteId <= 0)
                {
                    return new Error("参数错误:SiteId");
                }
                if (String.IsNullOrEmpty(src.Tag))
                {
                    return new Error("缺少参数:Tag");
                }
                if (String.IsNullOrEmpty(src.Name))
                {
                    return new Error("栏目名称不能为空");
                }
                this.value.SiteId = src.SiteId;
                this.value.Code = src.Code ?? "";
                this.value.Icon = "";
                this.value.Path = "";
                this.value.Flag = 0; //todo: 初始化flag
                int maxSortNumber = this._repo.GetMaxSortNumber(this.value.SiteId);
                if (maxSortNumber == 0) maxSortNumber = 1;
                this.value.SortNumber = maxSortNumber;
                this._parentChanged = true;
            }
            if(src.Tag == "-")
            {
                return new Error("不允许使用栏目保留Tag");
            }
            
            if (this.value.ParentId != src.ParentId)
            {
                if (src.ParentId > 0)
                {
                    ICategory ip = this._repo.GetCategory(this.value.SiteId, src.ParentId);
                    if (ip == null || ip.Get().SiteId != this.value.SiteId)
                    {
                        return new Error("上级分类不存在");
                    }
                }
                this.value.ParentId = src.ParentId;
                this._parentChanged = true;
            }
            if (!this._repo.CheckTagMatch(this.value.SiteId, this.value.ParentId,src.Tag, this.value.ID))
            {
                return new Error("分类TAG已存在");
            }
            this.value.Flag = src.Flag;
            this.value.ModuleId = src.ModuleId;
            this.value.Name = src.Name ?? "";
            this.value.Icon = src.Icon ?? "";
            this.value.Title = src.Title ?? "";
            this.value.Keywords = src.Keywords ?? "";
            this.value.Description = src.Description ?? "";
            this.value.Location = src.Location ?? "";
            if (String.IsNullOrEmpty(src.Location))
            {
                this.value.Flag ^= (int)CategoryFlag.Redirect;
            }
            else
            {
                this.value.Flag |= (int)CategoryFlag.Redirect;
            }
            return null;
        }


        public Error Save()
        {
            if (this.GetDomainId() <= 0)
            {
                this.SetAutoSortNumber();
            }
            if (this._parentChanged)
            {
                this._parent = null;
                this.UpdateCategoryPath();
                this._parentChanged = false;
            }


            Error err = this._repo.SaveCategory(this.value);
            if (err == null)
            {
                if (this._templateChanged)
                {
                    err = this.SaveTemplateBinds();
                    this._templateChanged = false;
                }


                #region 保存扩展属性

                this._extendRep.UpdateCategoryExtends(this);

                #endregion

                this.Site().ClearSelf();
                this.ClearSelf();
            }
            return err;
        }


        // 更新分类的路径
        private void UpdateCategoryPath()
        {
            string path = this.value.Tag;
            ICategory parent = this;
            while (parent != null && (parent = parent.Parent) != null)
            {
                path = String.Concat(parent.Get().Tag, "/", path);
            }
            this.value.Path = path;
        }

        private void SetAutoSortNumber()
        {
            if (this.Parent == null)
            {
                this.value.SortNumber = 1;
                return;
            }
            if (this.Parent.Childs != null && this.Parent.Childs.Any())
            {
                this.value.SortNumber = this.Parent.Childs.Max(a => a.Get().SortNumber) + 1;
            }
            else
            {
                this.value.SortNumber = 1;
            }
        }


        public IList<IExtendField> ExtendFields
        {
            get
            {
                return _extendFields ?? (_extendFields = new List<IExtendField>(
                    this._extendRep.GetExtendFields(this.Site().GetAggregaterootId(), this.GetDomainId())));
            }
            set
            {

                IList<IExtendField> delList = new List<IExtendField>();
                IList<int> addList = new List<int>();
                IList<IExtendField> usedList = new List<IExtendField>();


                bool isExists;

                #region 计算删除的扩展属性
                foreach (IExtendField extend in this.ExtendFields)
                {
                    isExists = false;

                    foreach (IExtendField valueExtend in value)
                    {
                        if (extend.GetDomainId() == valueExtend.GetDomainId())
                        {
                            isExists = true;
                            break;
                        }
                    }

                    if (!isExists)
                    {
                        delList.Add(extend);

                        //验证是否被占用
                        int usedNum = this._extendRep.GetCategoryExtendRefrenceNum(this, extend.GetDomainId());
                        if (usedNum > 0)
                        {
                            usedList.Add(extend);
                        }
                    }
                }

                if (usedList.Count != 0)
                {
                    StringBuilder strBuilder = new StringBuilder();
                    strBuilder.Append("扩展属性：");
                    int i = 0;
                    foreach (IExtendField extend in usedList)
                    {
                        if (i++ != 0) strBuilder.Append("、");
                        strBuilder.Append(extend.Name);
                    }
                    strBuilder.Append("已使用该扩展，请先清理后再删除绑定！");

                    throw new Exception(strBuilder.ToString());
                }


                foreach (IExtendField extend in delList)
                {
                    this.ExtendFields.Remove(extend);
                }

                #endregion

                #region 添加新增的扩展属性
                foreach (IExtendField valueExtend in value)
                {
                    isExists = false;
                    foreach (IExtendField extend in this.ExtendFields)
                    {
                        if (extend.GetDomainId() == valueExtend.GetDomainId())
                        {
                            isExists = true;
                            break;
                        }
                    }

                    if (!isExists)
                    {
                        addList.Add(valueExtend.GetDomainId());
                    }
                }

                foreach (int extendId in addList)
                {
                    this.ExtendFields.Add(this._extendRep.GetExtendFieldById(this.Site().GetAggregaterootId(), extendId));
                }

                #endregion


                _extendFields = value;
            }
        }

        public ICategory Parent
        {
            get
            {
                if (this._parent == null && this.value.ParentId > 0)
                {
                    this._parent = this._repo.GetCategory(this.value.SiteId, this.value.ParentId);
                }
                return this._parent;
            }
            set { this._parent = value; }
        }

        public IEnumerable<ICategory> Childs
        {
            get
            {
                return _childs ?? (_childs ?? this._repo.GetChilds(this));
            }
        }

        public IEnumerable<ICategory> NextLevelChilds
        {
            get
            {
                if (this._nextLevelChilds == null)
                {
                    this._nextLevelChilds = this._repo.GetNextLevelChilds(this).OrderBy(a => a.Get().SortNumber);
                }
                return this._nextLevelChilds;
            }
        }


        public ICategory Next
        {
            get { return this._next ?? (this._next = this._repo.GetNext(this)); }
        }

        public ICategory Previous
        {
            get { return this._previous ?? (this._previous = this._repo.GetPrevious(this)); }
        }


        public void ClearSelf()
        {
            foreach (ICategory category in this.Childs)
            {
                category.ClearSelf();
            }

            this._childs = null;
            this._parent = null;
            this._next = null;
            this._previous = null;
            this._extendFields = null;
            this._nextLevelChilds = null;
            this._templates = null;

        }

        /// <summary>
        /// 向上移动排序
        /// </summary>
        public void MoveSortUp()
        {
            if (this.Parent != null)
            {
                ICategory[] list = this.Parent.Childs.OrderBy(a => a.Get().SortNumber).ToArray();
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].GetDomainId() == this.GetDomainId() && i != 0)
                    {
                        this.SwapSortNumber(list[i - 1], true);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 向下移动排序
        /// </summary>
        public void MoveSortDown()
        {
            if (this.Parent != null)
            {
                ICategory[] list = this.Parent.Childs.OrderBy(a => a.Get().SortNumber).ToArray();
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].GetDomainId() == this.GetDomainId() && i < list.Length - 1)
                    {
                        this.SwapSortNumber(list[i + 1], false);
                        break;
                    }
                }
            }
        }

        private void SwapSortNumber(ICategory c, bool up)
        {
            //todo: 应该是更改
            if (c == null) return;
            int sortN = c.Get().SortNumber;
            c.Get().SortNumber = this.value.SortNumber;
            if (this.value.SortNumber == sortN || sortN < 0 || this.value.SortNumber < 0) //为了兼容旧有数据
            {
                if (sortN < 0 && !up)
                {
                    this.value.SortNumber = 0;
                    c.Get().SortNumber += 1;
                    if (c.Get().SortNumber <= 0)
                    {
                        c.Get().SortNumber = 1;
                    }
                }
                else
                {
                    this.value.SortNumber = sortN + (up ? 1 : -1);
                }
            }
            else
            {
                // 以上均为兼容
                this.value.SortNumber = sortN;
            }
            c.SaveSortNumber();
            this.SaveSortNumber();
        }

        /// <summary>
        /// 保存排序号码
        /// </summary>
        public void SaveSortNumber()
        {
            this._repo.SaveCategorySortNumber(this.GetDomainId(), this.value.SortNumber);
        }

        public CmsCategoryEntity Get()
        {
            return this.value;

        }




        public ISite Site()
        {
            if (this.site == null && this.value.SiteId > 0)
            {
                this.site = this.siteRepo.GetSiteById(this.value.SiteId);
            }
            return this.site;
        }

        public int GetDomainId()
        {
            return this.value.ID;
        }

        private bool _templateChanged = false;
        private TemplateBind[] _newTemplates;

        public Error SetTemplates(TemplateBind[] arr)
        {
            this._templateChanged = true;
            this._newTemplates = arr;
            return null;
        }

        public IList<TemplateBind> GetTemplates()
        {
            if (this._templates == null)
            {
                IEnumerable<TemplateBind> ie = this._tempRep.GetTemplateBindsForCategory(this);
                this._templates = new List<TemplateBind>(ie);
            }
            return this._templates;
        }


        private Error SaveTemplateBinds()
        {
            IList<TemplateBind> delList = new List<TemplateBind>();
            IList<TemplateBind> origin = this.GetTemplates();
            this._newTemplates = this._newTemplates ?? new TemplateBind[0];
            IDictionary<TemplateBindType, TemplateBind> tplMap = new Dictionary<TemplateBindType, TemplateBind>();
            foreach(TemplateBind b in this._newTemplates)
            {
                if (b == null) return new Error("参数包含空的模板");
                tplMap.Add(b.BindType, b);
            }
            foreach (TemplateBind templateBind in origin)
            {
                TemplateBind b;
                tplMap.TryGetValue(templateBind.BindType,out b);
                if(b == null || String.IsNullOrEmpty(b.TplPath))
                {
                    delList.Add(templateBind);
                }
                else if(b.ID <= 0)
                {
                    b.ID = templateBind.ID;
                }
            }
            Error err = this._tempRep.RemoveBinds(this.GetDomainId(), delList.ToArray());
            if(err == null)
            {
                err = this._tempRep.SaveTemplateBinds(this.GetDomainId(), this._newTemplates);
                if(err == null)
                {
                    this._templates = null;
                }
            }
            return err;
        }
    }
}
