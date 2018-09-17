using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using T2.Cms.CacheService;
using T2.Cms.DataTransfer;
using T2.Cms.Domain.Interface.Site;
using T2.Cms.Infrastructure.Ioc;

namespace T2.Cms.UnitTest
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
            String uri = "http://www.meizhuli.cn/product";
            SiteDto site = ServiceCall.Instance.SiteService.GetSingleOrDefaultSite(uri);
            this.Println(this.Stringfy(site));
        }
    }
}
