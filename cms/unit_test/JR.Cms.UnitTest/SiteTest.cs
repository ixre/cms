using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JR.Cms.CacheService;
using JR.Cms.DataTransfer;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Infrastructure.Ioc;

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
            String uri = "http://www.meizhuli.cn/product";
            SiteDto site = ServiceCall.Instance.SiteService.GetSingleOrDefaultSite(uri);
            this.Println(this.Stringfy(site));
        }
    }
}
