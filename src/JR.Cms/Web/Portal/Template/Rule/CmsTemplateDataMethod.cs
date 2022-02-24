//
//  Name : 提供数据方法
//  修改说明：
//


using JR.Cms.Library.CacheService;
using JR.Stand.Abstracts.Web;

namespace JR.Cms.Web.Portal.Template.Rule
{
    public abstract class CmsTemplateDataMethod : CmsTemplateCore
    {
        protected CmsTemplateDataMethod(ICompatibleHttpContext context):base(context)
        {
        }
        
        //     protected string GetProvince()
        //     {
        //         string prv=this.Request("prv");
        //         if (Regex.IsMatch(prv, "^\\d+$"))
        //         {
        //             int id = int.Parse(prv);
        //             foreach (var t in Region.Provinces)
        //             {
        //                 if (t.ID == id) return t.Text;
        //             }
        //         }
        //         return "北京市";
        //     }
        //      
        //     protected string GetCity()
        //     {
        //         string cty = this.Request("cty");
        //         if (Regex.IsMatch(cty, "^\\d+$"))
        //         {
        //             int id = int.Parse(cty);
        //             foreach (var t in Region.Cities)
        //             {
        //                 if (t.ID == id) return t.Text;
        //             }
        //         }
        //         return "北京市";
        //     }

        /// <summary>
        /// 根据名称获取分类tag
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string GetCategory(string name)
        {
            var c = LocalService.Instance.SiteService.GetCategoryByName(SiteId, name);
            return c.ID <= 0 ? "" : c.Tag;
        }
    }
}