using System;
using System.Collections.Generic;
using System.Linq;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using JR.Cms.Web.Util;
using JR.Stand.Core.Framework.Automation;

namespace JR.Cms.Web.Manager.Handle
{
    public class ExtendHandler : BasePage
    {
        public void SetProperty()
        {
            RenderTemplate(ResourceMap.SetProperties, new
            {
                init = ""
            });
        }

        public string Fields()
        {
            var form = EntityForm.Build<ExtendFieldDto>(new ExtendFieldDto());
            ViewData["form"] = form;
            return RequireTemplate(ResourceMap.GetPageContent(ManagementPage.Site_Extend_List));
        }

        public void GetExtendFields_POST()
        {
            var list = ServiceCall.Instance.SiteService.GetExtendFields(SiteId);
            PagerJson(list, "共" + list.Count().ToString() + "条");
        }

        /// <summary>
        /// 分类扩展信息设置
        /// </summary>
        /// <returns></returns>
        public void Category_Check()
        {
            var categoryId = int.Parse(Request.Query("category_id"));
            IList<int> extendIds = new List<int>();
            var list = ExtendFieldCacheManager.GetExtendFields(CurrentSite.SiteId);
            var category = ServiceCall.Instance.SiteService.GetCategory(SiteId, categoryId);

            foreach (var extend in category.ExtendFields) extendIds.Add(extend.GetDomainId());

            var json = JsonSerializer.Serialize(list);
            category.ExtendFields = null;
            var categoryJson = JsonSerializer.Serialize(new
            {
                ID = category.ID,
                Path = category.Path,
                Name = category.Name,
                ExtendIds = extendIds
            });

            RenderTemplate(ResourceMap.GetPageContent(ManagementPage.Site_Extend_Category_Check), new
            {
                url = Request.GetEncodedUrl(),
                json = json,
                category = categoryJson
            });
        }

        public string Category_Check_POST()
        {
            var categoryId = int.Parse(Request.Query("category_id"));
            string extendIdstr = Request.Query("extendIds");

            int[] extendIds = null;

            if (!string.IsNullOrEmpty(extendIdstr))
                extendIds = Array.ConvertAll<string, int>(
                    extendIdstr.Split(',')
                    , a => int.Parse(a));

            var category = ServiceCall.Instance.SiteService.GetCategory(
                SiteId, categoryId);

            //重新设置扩展信息
            category.ExtendFields = new List<IExtendField>();
            if (extendIds != null)
                Array.ForEach(extendIds, id => { category.ExtendFields.Add(new ExtendField(id, null)); });


            //设置并保存
            try
            {
                ServiceCall.Instance.SiteService.SaveCategory(SiteId, category.ParentId, category);
            }
            catch (Exception exc)
            {
                throw exc;
                return ReturnError(exc.Message);
            }

            return ReturnSuccess("保存成功!");
        }


        public string SaveExtendField_POST()
        {
            var extend = Request.ParseFormToEntity<ExtendFieldDto>();
            var r = ServiceCall.Instance.SiteService.SaveExtendField(SiteId, extend);
            if (r.ErrCode > 0) return ReturnError(r.ErrMsg);
            return ReturnSuccess();
        }
    }
}