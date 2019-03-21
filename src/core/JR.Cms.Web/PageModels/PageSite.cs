using System;
using JR.Cms.DataTransfer;
using JR.Cms.Domain.Interface.Common.Language;

namespace JR.Cms.Web.PageModels
{
    public class PageSite
    {
        public PageSite(SiteDto dto)
        {
            this.Name = dto.Name;
            this.FullDomain = dto.FullDomain;
            this.Address = dto.ProAddress;
            this.Tel = dto.ProTel;
            this.Email = dto.ProEmail;
            this.Fax = dto.ProFax;
            this.Im = dto.ProIm;
            this.Notice = dto.ProNotice;
            this.Slogan = dto.ProSlogan;
            this.Post = dto.ProPost;
            this.Phone = dto.ProPhone;
            this.Title = dto.SeoTitle;
            this.Keywords = dto.SeoKeywords;
            this.Description = dto.SeoDescription;
            this. SiteId= dto.SiteId;
            this.Tpl = dto.Tpl;
            this.Language = dto.Language;
        }

        public string Title { get; set; }

        public string Notice { get; set; }

        public string Post { get; set; }

        public string Description { get; set; }

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
        public String Tpl { get; set; }
        public Languages Language { get; set; }
    }
}
