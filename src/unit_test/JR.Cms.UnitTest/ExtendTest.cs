using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using JR.Cms.CacheService;
using JR.Cms.Domain.Interface.Site.Extend;
using JR.Cms.ServiceDto;

namespace JR.Cms.UnitTest
{
    [TestClass]
    public class ExtendTest
    {
        private TestBase _base = new TestBase();
        [TestMethod]
        public void TestSaveCategoryExtendId()
        {
            _base.Boot();
            CategoryDto cat = ServiceCall.Instance.SiteService.GetCategory(1, 1);
            cat.ExtendFields = new List<IExtendField>();
            cat.ExtendFields.Add(new ExtendField(1, null));
            cat.ExtendFields.Add(new ExtendField(2, null));
            cat.ExtendFields.Add(new ExtendField(3, null));
            Result r = ServiceCall.Instance.SiteService.SaveCategory(1, 0, cat);
            if(r.ErrCode > 0)
            {
                Assert.Fail("保存不成功");
            }
        }
    }
}
