/**
 * Copyright (C) 2007-2024 fze.NET, All rights reserved.
 *
 * name: PageSiteModel.cs
 * author: jarrysix (jarrysix@gmail.com)
 * date: 2017-10-17 08:18:15
 * description:
 * history:
 */

using System.Collections.Generic;
using JR.Cms.Domain.Interface.Common.Language;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using JR.Stand.Core.Template.Impl;

namespace JR.Cms.Web.Portal.Template.Model
{
    /// <summary>
    /// 站点模型
    /// </summary>
    public class PageSiteModel : ITemplateVariableInstance
    {
        private IDictionary<string, string> _dict;

        /// <summary>
        /// 站点模型
        /// </summary>
        /// <param name="dto"></param>
        public PageSiteModel(SiteDto dto)
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
            Id = dto.SiteId;
            Tpl = dto.Tpl;
            BeianNo = dto.BeianNo;
            Language = dto.Language;
        }

        /// <summary>
        /// 站点标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 站点公告
        /// </summary>
        public string Notice { get; set; }

        /// <summary>
        /// 站点邮编
        /// </summary>
        public string Post { get; set; }

        /// <summary>
        /// 站点描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 站点关键词
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 站点电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 标语
        /// </summary>
        public string Slogan { get; set; }

        /// <summary>
        /// 站点微信/QQ
        /// </summary>
        public string Im { get; set; }

        /// <summary>
        /// 站点传真
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// 站点邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 站点电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 站点地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 站点全域名
        /// </summary>
        public string FullDomain { get; set; }

        /// <summary>
        /// 站点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 站点ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 模板
        /// </summary>
        public string Tpl { get; set; }
        /// <summary>
        /// 备案号
        /// </summary>
        public string BeianNo { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        public Languages Language { get; set; }

        /// <summary>
        /// 返回站点变量,使用${site.data(变量名)}获取
        /// </summary>
        public IDictionary<string, string> Data
        {
            get
            {
                if (_dict == null)
                {
                    _dict = new Dictionary<string, string>();
                    IList<SiteVariableDto> list = LocalService.Instance.SiteService.GetSiteVariables(this.Id);

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
