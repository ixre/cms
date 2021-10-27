using System.Collections.Generic;
using JR.Cms.Domain.Interface.Common.Language;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using JR.Stand.Core.Template.Impl;

namespace JR.Cms.Web.Portal.Template.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class PageSite:ITemplateVariableInstance
    {
        private IDictionary<string, string> _dict;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        public PageSite(SiteDto dto)
        {
            Name = dto.Name;
            FullDomain = dto.FullDomain;
            Address = dto.ProAddress;
            Tel = dto.ProTel;
            Email = dto.ProEmail;
            Fax = dto.ProFax;
            Im = dto.ProIm;
            Notice = dto.ProNotice;
            Slogan = dto.ProSlogan;
            Post = dto.ProPost;
            Phone = dto.ProPhone;
            Title = dto.SeoTitle;
            Keywords = dto.SeoKeywords;
            Description = dto.SeoDescription;
            SiteId = dto.SiteId;
            Tpl = dto.Tpl;
            Language = dto.Language;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Notice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Post { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Keywords { get; set; }

        public string Phone { get; set; }

        public string Slogan { get; set; }

        public string Im { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        public string Tel { get; set; }

        public string Address { get; set; }

        public string FullDomain { get; set; }

        public string Name { get; set; }

        public int SiteId { get; set; }
        public string Tpl { get; set; }
        public Languages Language { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IDictionary<string, string> Data
        {
            get
            {
                if (_dict == null)
                {
                    _dict = new Dictionary<string, string>();
                    IList<SiteVariableDto> list = ServiceCall.Instance.SiteService.GetSiteVariables(this.SiteId);

                    foreach (var value in list) _dict.Add(value.Name, value.Value);
                }

                return _dict;
            }
        }

        /// <summary>
        /// 添加变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void AddData(string key, string data)
        {
            _dict.Add(key, data);
        }

        /// <summary>
        /// 删除变量
        /// </summary>
        /// <param name="key"></param>
        public void RemoveData(string key)
        {
            _dict.Remove(key);
        }
    }
}