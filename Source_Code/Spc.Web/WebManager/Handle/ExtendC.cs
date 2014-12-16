using Ops.Cms.CacheService;
using Ops.Cms.DataTransfer;
using Ops.Cms.Domain.Interface.Site.Extend;
using Ops.Cms.Web;
using Ops.Framework.Automation;
using Ops.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ops.Cms.WebManager
{
    public class ExtendC:BasePage
    {
        public void SetProperty_GET()
        {
            base.RenderTemplate(ResourceMap.SetProperties, new
            {
                init = ""
            });
        }

        public void Fields_GET()
        {

            string form = EntityForm.Build<ExtendFieldDto>(new ExtendFieldDto(), false, "btn");
            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Site_Extend_List), new
            {
                form = form
            });
        }

        public void GetExtendFields_POST()
        {
            IEnumerable<ExtendFieldDto> list = ServiceCall.Instance.SiteService.GetExtendFields(this.SiteId);
            base.PagerJson(list, "共" + list.Count().ToString() + "条");
        }

        /// <summary>
        /// 分类扩展信息设置
        /// </summary>
        /// <returns></returns>
        public void Category_Check_GET()
        {
            int categoryLft = int.Parse(base.Request["lft"]);
            IList<int> extendIds = new List<int>();
            IList<ExtendFieldDto> list = ExtendFieldCacheManager.GetExtendFields(base.CurrentSite.SiteId);
            CategoryDto category = ServiceCall.Instance.SiteService.GetCategoryByLft(this.SiteId, categoryLft);

            foreach (IExtendField extend in category.ExtendFields)
            {
                extendIds.Add(extend.Id);
            }

            string json = JsonSerializer.Serialize(list);
            category.ExtendFields = null;
            string categoryJson = JsonSerializer.Serialize(new {
                ID=category.Id,
                Lft=category.Lft,
                Name = category.Name, 
                ExtendIds = extendIds
            });

            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Site_Extend_Category_Check), new
            {
                url=base.Request.RawUrl,
                json = json,
                category = categoryJson
            });
        }

        public string Category_Check_POST()
        {
            int categoryLft = int.Parse(base.Request["lft"]);
            string extendIdstr = base.Request["extendIds"];

            int[] extendIds = null;

            if(!String.IsNullOrEmpty(extendIdstr))
            {
                extendIds = Array.ConvertAll<string, int>(
                extendIdstr.Split(',')
                ,a=>int.Parse(a));
            }

            CategoryDto category = ServiceCall.Instance.SiteService.GetCategoryByLft(
                this.SiteId,
                categoryLft);

            //重新设置扩展信息
            category.ExtendFields = new List<IExtendField>();
            if (extendIds != null)
            {
                Array.ForEach(extendIds, id =>
                {
                    category.ExtendFields.Add(new ExtendField(id, null));
                });
            }


            //设置并保存
            try
            {
                ServiceCall.Instance.SiteService.SaveCategory(this.SiteId, -1, category);
            }
            catch (Exception exc)
            {
                return base.ReturnError(exc.Message);
            }
            return base.ReturnSuccess("保存成功!");

        }

        
        public string SaveExtendField_POST()
        {
            try
            {
                ExtendFieldDto extend = ICollectionExtensions.ConvertToEntity<ExtendFieldDto>(this.Request.Form);
                int result = ServiceCall.Instance.SiteService.SaveExtendField(this.SiteId, extend);
                return base.ReturnSuccess();
            }
            catch (Exception exc)
            {
                return base.ReturnError(exc.Message);
            }
        }
    }
}
