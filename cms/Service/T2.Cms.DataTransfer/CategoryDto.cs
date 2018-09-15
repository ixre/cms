using System.Collections.Generic;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Extend;
using T2.Cms.Domain.Interface.Site.Template;
using T2.Cms.Models;

namespace T2.Cms.DataTransfer
{
    public struct CategoryDto
    {
        public static CategoryDto ConvertFrom(ICategory ic)
        {
            if (ic == null) return default(CategoryDto);
            CmsCategoryEntity category = ic.Get();
            //int moduleId = category.ModuleId;
            CategoryDto dto = new CategoryDto
            {
                Id = category.ID,
                Keywords = category.Keywords,
                Description = category.Description,
                Tag = category.Tag,
                Icon = category.Icon,
                Name = category.Name,
                PageTitle = category.Title,
                SortNumber = category.SortNumber,
                ModuleId = category.ModuleId,
                Location = category.Location,
                ExtendFields = ic.ExtendFields,
                SiteId = ic.Site().GetAggregaterootId(),
                UriPath = ic.UriPath,
                //Lft = category.Lft,
                //Rgt = category.Rgt
                //ModuleID = moduleId
            };
            //dto.CloneData(category);



            foreach (TemplateBind tplBind in ic.GetTemplates())
            {
                switch (tplBind.BindType)
                {
                    case TemplateBindType.CategoryTemplate:
                        dto.CategoryTemplate = tplBind.TplPath;
                        break;
                    case TemplateBindType.CategoryArchiveTemplate:
                        dto.CategoryArchiveTemplate = tplBind.TplPath;
                        break;
                }
            }

            return dto;
        }

        public int Id
        {
            get;
            set;
        }

        public int SiteId
        {
            get;
            set;
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

        public int SortNumber
        {
            get;
            set;
        }

        public int ModuleId
        {
            get;
            set;
        }

        public string Tag
        {
            get;
            set;
        }

        public string Icon { get; set; }
        public string Name
        {
            get;
            set;
        }

        public string PageTitle
        {
            get;
            set;
        }

        public string Keywords
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Location { get; set; }

        public string UriPath { get; set; }

        public IList<IExtendField> ExtendFields { get; set; }

        public string CategoryTemplate { get; set; }

        public string CategoryArchiveTemplate { get; set; }

        /// <summary>
        /// 上级编号
        /// </summary>
        public int ParentId { get; set; }
    }
}
