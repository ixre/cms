using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JR.Cms.CacheService;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Infrastructure.Ioc;
using JR.Cms.ServiceDto;

namespace JR.Cms.UnitTest
{
    [TestClass]
    public class SiteTest:TestBase
    {
        int siteId = 1;
        ISiteRepo siteRepo;
        public SiteTest()
        {
            siteRepo = Ioc.GetInstance<ISiteRepo>();
        }


        [TestMethod]
        public void TestGetSiteByUrl()
        {
            String host = "www.meizhuli.cn";
            String appPath = "/product";
            SiteDto site = ServiceCall.Instance.SiteService.GetSingleOrDefaultSite(host,appPath);
            this.Println(this.Stringfy(site));
        }
    }
}
