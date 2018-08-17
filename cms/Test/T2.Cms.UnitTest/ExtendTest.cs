using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T2.Cms.CacheService;
using T2.Cms.DataTransfer;
using T2.Cms.Domain.Interface.Site.Extend;

namespace T2.Cms.UnitTest
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
            int i = ServiceCall.Instance.SiteService.SaveCategory(1, 0, cat);
            if(i == 0)
            {
                Assert.Fail("保存不成功");
            }
        }
    }
}
