using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using T2.Cms.Infrastructure.Ioc;
using T2.Cms.Domain.Interface.Site.Category;

namespace T2.Cms.UnitTest
{
    /// <summary>
    /// CategoryTest 的摘要说明
    /// </summary>
    [TestClass]
    public class CategoryTest:TestBase
    {
        ICategoryRepository repo;
        public CategoryTest()
        {
            repo = Ioc.GetInstance<ICategoryRepository>();
        }
        

        [TestMethod]
        public void TestGetCategory()
        {
            int siteId = 1;
            ICategory ic = this.repo.GetCategoryById(1);
            Console.WriteLine(this.Stringfy(ic.Get()));
        }
    }
}
