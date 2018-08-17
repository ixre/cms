using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using T2.Cms.Dal;
using T2.Cms.Domain.Implement.Site;
using T2.Cms.Domain.Interface.Common.Language;
using T2.Cms.Domain.Interface.Site;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site.Extend;
using T2.Cms.Domain.Interface.Site.Link;
using T2.Cms.Domain.Interface.Site.Template;
using T2.Cms.Domain.Interface.User;
using T2.Cms.Infrastructure;
using T2.Cms.Models;

namespace T2.Cms.ServiceRepository
{
    public class SiteRepository:BaseSiteRepository,ISiteRepository
    {
        private static readonly SiteDal siteDal = new SiteDal();
        private static readonly LinkDal linkDal = new LinkDal();

        private readonly IExtendFieldRepository _extendFieldRepository;
        private readonly ICategoryRepository _categoryRep;
        private readonly ITemplateRepository _tempRep;
        private readonly IUserRepository _userRep;

        public SiteRepository(
            IExtendFieldRepository extendFieldRepository,
            ICategoryRepository categoryRepository,
            ITemplateRepository tempRep,
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
            if (site.Id == 0)
            {
                int siteId=siteDal.CreateSite(site);
               int  rootCategoryId = this._categoryRep.GetNewCategoryId(siteId);
                if (siteId <= 0)
                {
                    throw new ArgumentException("创建站点失败");
                }
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

            return site.Id;
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
                       site.AppName = rd["app_name"].ToString();
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


        public ISite GetSiteByUri(Uri uri)
        {
            ISite site = null;
            GetSiteByUri(uri, ref site);
            return site;
        }

        public ISite GetSiteById(int siteId)
        {
            IList<ISite> sites = this.GetSites();
            if (sites.Count == 0) throw new Exception("Missing site");
            return BinarySearch.IntSearch(sites, 0, sites.Count, siteId, a => a.Id);
        }

        public ISite GetSingleOrDefaultSite(Uri uri)
        {
            ISite site = null;
            IList<ISite> sites = GetSiteByUri(uri, ref site);
            if (site != null)
            {
                return site;
            }

            if (sites.Count == 0) throw new Exception("Missing site");

            //获取host和dir均为空的站点
            foreach (ISite _site in sites)
            {
                //if (_site.Id == 7)
                //{
                //   return _site;
                //}
                if (_site.Get().Domain == "" && _site.Get().AppName == "")
                {
                    return _site;
                }
            }

            return sites[0];
        }

        private IList<ISite> GetSiteByUri(Uri uri, ref ISite site)
        {

            string hostName = uri.Host;
            IList<ISite> sites = this.GetSites();

            //todo:
           // site = sites[0];
            //return sites;

            //获取使用域名标识的网站
            string _hostName = String.Concat(
                "^",
                hostName.Replace(".", "\\."),
                "$|\\s+",
                hostName.Replace(".", "\\."),
                "\\s*|\\s*",
                hostName.Replace(".", "\\."),
                "\\s+");

            foreach (ISite s in sites)
            {
                if (!String.IsNullOrEmpty(s.Get().Domain))
                {
                    if (Regex.IsMatch(s.Get().Domain, _hostName, RegexOptions.IgnoreCase))
                    {
                        site = s;
                        site.SetRunType(SiteRunType.Stand);
                        break;
                    }
                }

            }

            //获取使用目录绑定的网站
            if (site == null)
            {
                string[] segments = uri.Segments;
                if (segments.Length >= 2)
                {
                    string dirName = segments[1].Replace("/", "");
                    foreach (ISite s in sites)
                    {
                        if (String.Compare(s.Get().AppName, dirName,true,CultureInfo.InvariantCulture) == 0)
                        {
                            site = s;
                            site.SetRunType(SiteRunType.VirtualDirectory);
                            break;
                        }
                    }
                }
            }
            return sites;
        }


        public ISiteLink CreateLink(ISite site, int id, string text)
        {
            return base.CreateLink(this, site, id, text);
        }


        public int SaveSiteLink(int siteId, ISiteLink link)
        {
            if(link.Id <=0 )
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
