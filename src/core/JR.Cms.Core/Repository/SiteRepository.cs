using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using JR.Cms.Dal;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.Domain.Interface.Site.Link;
using JR.Cms.Domain.Interface.Site.Template;
using JR.Cms.Domain.Interface.User;
using JR.Cms.Domain.Site;
using JR.Cms.Infrastructure;
using JR.Cms.Models;

namespace JR.Cms.Repository
{
    public class SiteRepository:BaseSiteRepository,ISiteRepo
    {
        private static readonly SiteDal siteDal = new SiteDal();
        private static readonly LinkDal linkDal = new LinkDal();

        private readonly IExtendFieldRepository _extendFieldRepository;
        private readonly ICategoryRepo _categoryRep;
        private readonly ITemplateRepo _tempRep;
        private readonly IUserRepository _userRep;

        public SiteRepository(
            IExtendFieldRepository extendFieldRepository,
            ICategoryRepo categoryRepository,
            ITemplateRepo tempRep,
            IUserRepository userRep
            )
        {
            this._extendFieldRepository = extendFieldRepository;
            this._categoryRep = categoryRepository;
            this._tempRep = tempRep;
            this._userRep = userRep;
        }

        public ISite CreateSite(CmsSiteEntity site)
        {
            return base.CreateSite(this,
                this._extendFieldRepository,
                this._categoryRep,
                this._tempRep,
                this._userRep,
                site);
        }
        public int SaveSite(ISite site)
        {
            int siteId = site.GetAggregaterootId();
            if (site.GetAggregaterootId() <= 0)
            {
                siteId=siteDal.CreateSite(site);
                if (siteId <= 0) throw new ArgumentException("创建站点失败");
            }
            else
            {
                if (siteDal.UpdateSite(site) != 1)
                {
                    throw new ArgumentException("站点不存在，保存失败");
                }
            }
            //清理缓存
            RepositoryDataCache._siteDict = null;
            RepositoryDataCache._categories = null;

            return siteId;
        }

        public IList<ISite> GetSites()
        {
            if (RepositoryDataCache._siteDict == null)
            {
                RepositoryDataCache._siteDict = new Dictionary<int, ISite>();
                siteDal.ReadSites(rd =>
               {
                   while (rd.Read())
                   {
                       CmsSiteEntity site = new CmsSiteEntity();

                       site.SiteId = Convert.ToInt32(rd["site_id"]);
                       site.Name = rd["name"].ToString();
                       //ISite site = this.CreateSite(Convert.ToInt32(rd["site_id"]), );

                       //rd.CopyToEntity<ISite>(site);
                       site.AppPath = rd["app_name"].ToString();
                       site.Tpl = rd["tpl"].ToString();
                       site.State =int.Parse(rd["state"].ToString());
                       site.Location = rd["location"].ToString();
                       if (site.Location != null && site.Location.Trim() == "")
                       {
                           site.Location = "";
                       }
                       site.Domain = rd["domain"].ToString();
                       site.ProAddress = rd["pro_address"].ToString();
                       site.ProEmail = rd["pro_email"].ToString();
                       site.ProFax = rd["pro_fax"].ToString();
                       site.ProPost = rd["pro_post"].ToString();
                       site.Note = rd["note"].ToString();
                       site.ProNotice = rd["pro_notice"].ToString();
                       site.ProPhone = rd["pro_phone"].ToString();
                       site.ProIm = rd["pro_im"].ToString();
                       site.SeoTitle = rd["seo_title"].ToString();
                       site.SeoKeywords = rd["seo_keywords"].ToString();
                       site.SeoDescription = rd["seo_description"].ToString();
                       site.ProSlogan = rd["pro_slogan"].ToString();
                       site.ProTel = rd["pro_tel"].ToString();
                       site.Language = int.Parse(rd["language"].ToString());
                       ISite ist = this.CreateSite(site);
                       RepositoryDataCache._siteDict.Add(site.SiteId, ist);
                   }
               });

            }
            return RepositoryDataCache._siteDict.Values.ToList();
        }
        

        public ISite GetSiteById(int siteId)
        {
            IList<ISite> sites = this.GetSites();
            if (sites.Count == 0) throw new Exception("Missing site");
            return BinarySearch.IntSearch(sites, 0, sites.Count, siteId, a => a.GetAggregaterootId());
        }

        public ISite GetSingleOrDefaultSite(String host, String appPath)
        {
            ISite site = this.GetSiteByUri(host,appPath);
            if (site != null)
            {
                return site;
            }

            IList<ISite> sites = this.GetSites();
            if (sites.Count == 0) throw new Exception("Missing site");

            //获取host和dir均为空的站点
            foreach (ISite _site in sites)
            {
                //if (_site.Id == 7)
                //{
                //   return _site;
                //}
                if (_site.Get().Domain == "" && _site.Get().AppPath == "")
                {
                    return _site;
                }
            }

            return sites[0];
        }

        public ISite GetSiteByUri(String host,String appPath)
        {
            IList<ISite> sites = this.GetSites();
            int i = host.IndexOf(":");
            if(i != -1) host = host.Substring(0, i);
            String appName = appPath != null && appPath.StartsWith("/") ? appPath.Substring(1) : appPath;
            //todo:
            // site = sites[0];
            //return sites;

            //获取使用域名标识的网站
            string _hostName = String.Concat(
            "^", host.Replace(".", "\\."),
            "$|\\s+", host.Replace(".", "\\."),
            "\\s*|\\s*", host.Replace(".", "\\."), "\\s+");

            ISite curr = null;
            foreach (ISite s in sites)
            {
                if (String.IsNullOrEmpty(s.Get().Domain)) continue;
                // 判断域名相同
                if (Regex.IsMatch(s.Get().Domain, _hostName, RegexOptions.IgnoreCase))
                {
                    s.SetRunType(SiteRunType.Stand);
                    if (String.IsNullOrEmpty(appName)) return s;
                    // 判断应用名称相同
                    if (String.Compare(s.Get().AppPath, appName, true) == 0)
                    {
                        s.SetRunType(SiteRunType.VirtualDirectory);
                        return s;
                    }
                    curr = s;
                }
            }
            if (curr != null) return curr;
            //获取使用目录绑定的网站
            if (!String.IsNullOrEmpty(appName))
            {
                foreach (ISite s in sites)
                {
                    if (String.Compare(s.Get().AppPath, appName, true) == 0)
                    {
                        s.SetRunType(SiteRunType.VirtualDirectory);
                        return s;
                    }
                }
            }
            return null;
        }


        public ISiteLink CreateLink(ISite site, int id, string text)
        {
            return base.CreateLink(this, site, id, text);
        }


        public int SaveSiteLink(int siteId, ISiteLink link)
        {
            if(link.GetDomainId() <=0 )
            {
               return linkDal.AddSiteLink(siteId, link);
            }

            return linkDal.UpdateSiteLink(siteId, link);
        }

        public ISiteLink ConvertToILink(int siteId, DbDataReader reader)
        {
            ISiteLink link = this.CreateLink(
                       this.GetSiteById(siteId),
                       int.Parse(reader["id"].ToString()),
                       reader["text"].ToString()
                       );

            link.Bind = reader["bind"].ToString();
            link.ImgUrl = reader["img_url"].ToString();
            link.SortNumber = int.Parse(reader["sort_number"].ToString());
            link.Pid = int.Parse(reader["pid"].ToString());
            link.Target = reader["target"].ToString();
            link.Type = (SiteLinkType)int.Parse(reader["type"].ToString());
            link.Uri = reader["uri"].ToString();
            link.Visible = Convert.ToBoolean(reader["visible"]);


            return link;
        }

        public bool DeleteSiteLink(int siteId, int linkId)
        {
            return linkDal.DeleteSiteLink(siteId, linkId) == 1;
        }


        public ISiteLink GetSiteLinkById(int siteId, int linkId)
        {
            ISiteLink link = null;
            linkDal.GetSiteLinkById(siteId, linkId, rd =>
            {
                if (rd.Read())
                {
                    link = this.ConvertToILink(siteId, rd);
                }
            });

            return link;
        }

        public IEnumerable<ISiteLink> GetSiteLinks(int siteId, SiteLinkType type)
        {
            IList<ISiteLink> links = new List<ISiteLink>();
            linkDal.GetAllSiteLinks(siteId, type, rd =>
            {
                while(rd.Read())
                {
                    links.Add( this.ConvertToILink(siteId,rd));
                }
            });
            return links;
        }
        
    }
}
