using System;
using System.Collections.Generic;
using System.Linq;
using T2.Cms.CacheService;
using T2.Cms.DataTransfer;
using T2.Cms.Domain.Interface.Site.Extend;
using T2.Cms.WebManager;
using JR.DevFw.Framework.Automation;
using JR.DevFw.Framework.Extensions;

namespace T2.Cms.Web.WebManager.Handle
{
    public class ExtendC : BasePage
    {
        public void SetProperty_GET()
        {
            base.RenderTemplate(ResourceMap.SetProperties, new
            {
                init = ""
            });
        }

        public string Fields_GET()
        {

            string form = EntityForm.Build<ExtendFieldDto>(new ExtendFieldDto());
            ViewData["form"] = form;
            return base.RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Site_Extend_List));
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
            int categoryId = int.Parse(base.Request["category_id"]);
            IList<int> extendIds = new List<int>();
            IList<ExtendFieldDto> list = ExtendFieldCacheManager.GetExtendFields(base.CurrentSite.SiteId);
            CategoryDto category = ServiceCall.Instance.SiteService.GetCategory(this.SiteId, categoryId);

            foreach (IExtendField extend in category.ExtendFields)
            {
                extendIds.Add(extend.Id);
            }

            string json = JsonSerializer.Serialize(list);
            category.ExtendFields = null;
            string categoryJson = JsonSerializer.Serialize(new
            {
                ID = category.Id,
                Lft = category.Lft,
                Name = category.Name,
                ExtendIds = extendIds
            });

            base.RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Site_Extend_Category_Check), new
            {
                url = base.Request.RawUrl,
                json = json,
                category = categoryJson
            });
        }

        public string Category_Check_POST()
        {
            int categoryId = int.Parse(base.Request["category_id"]);
            string extendIdstr = base.Request["extendIds"];

            int[] extendIds = null;

            if (!String.IsNullOrEmpty(extendIdstr))
            {
                extendIds = Array.ConvertAll<string, int>(
                extendIdstr.Split(',')
                , a => int.Parse(a));
            }

            CategoryDto category = ServiceCall.Instance.SiteService.GetCategory(
                this.SiteId, categoryId);

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
                throw exc;
                return base.ReturnError(exc.Message);
            }
            return base.ReturnSuccess("保存成功!");

        }


        public string SaveExtendField_POST()
        {
            try
            {
                ExtendFieldDto extend = this.Request.Form.ConvertToEntity<ExtendFieldDto>();
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
