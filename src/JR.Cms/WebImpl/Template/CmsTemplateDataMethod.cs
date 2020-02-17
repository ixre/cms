//
//  Name : 提供数据方法
//  修改说明：
//


using System.Drawing;
using System.Text.RegularExpressions;
using JR.Cms.CacheService;
using JR.Cms.ServiceDto;

namespace JR.Cms.WebImpl.Template
{
    public abstract class CmsTemplateDataMethod : CmsTemplateCore
    {
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
          CategoryDto c = ServiceCall.Instance.SiteService.GetCategoryByName(this.SiteId, name);
           return c.ID<=0 ? "" : c.Tag;
        }
    }
}
