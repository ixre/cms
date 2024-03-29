﻿using System.Collections.Generic;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Template;

namespace JR.Cms.ServiceDto
{
    public struct CategoryDto
    {
        public static CategoryDto ConvertFrom(ICategory ic)
        {
            if (ic == null) return default(CategoryDto);
            var category = ic.Get();
            //int moduleId = category.ModuleId;
            var dto = new CategoryDto
            {
                ID = category.ID,
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
                SiteId = ic.Site().GetAggregateRootId(),
                Path = category.Path,
                ParentId = category.ParentId,
                //Lft = category.Lft,
                //Rgt = category.Rgt
                //ModuleID = moduleId
            };
            //dto.CloneData(category);


            foreach (var tplBind in ic.GetTemplates())
                switch (tplBind.BindType)
                {
                    case TemplateBindType.CategoryTemplate:
                        dto.CategoryTemplate = tplBind.TplPath;
                        break;
                    case TemplateBindType.CategoryArchiveTemplate:
                        dto.CategoryArchiveTemplate = tplBind.TplPath;
                        break;
                }

            return dto;
        }

        public int ID { get; set; }

        public int SiteId { get; set; }


        public int SortNumber { get; set; }

        public int ModuleId { get; set; }

        public string Tag { get; set; }

        public string Icon { get; set; }
        public string Name { get; set; }

        public string PageTitle { get; set; }

        public string Keywords { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public string Path { get; set; }

        public IList<IExtendField> ExtendFields { get; set; }

        public string CategoryTemplate { get; set; }

        public string CategoryArchiveTemplate { get; set; }

        /// <summary>
        /// 上级编号
        /// </summary>
        public int ParentId { get; set; }
    }
}