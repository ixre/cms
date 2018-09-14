using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using T2.Cms.Infrastructure.Ioc;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site;
using T2.Cms.Models;
using T2.Cms.Infrastructure;

namespace T2.Cms.UnitTest
{
    /// <summary>
    /// CategoryTest 的摘要说明
    /// </summary>
    [TestClass]
    public class CategoryTest:TestBase
    {
        int siteId = 1;
        ICategoryRepo repo;
        ISiteRepo siteRepo;
        public CategoryTest()
        {
            repo = Ioc.GetInstance<ICategoryRepo>();
            siteRepo = Ioc.GetInstance<ISiteRepo>();
        }
        

        [TestMethod]
        public void TestGetCategory()
        {
            ICategory ic = this.repo.GetCategory(siteId,1);
            Console.WriteLine(this.Stringfy(ic.Get()));
        }

        [TestMethod]
        public void SaveCategory()
        {
            ISite ist = this.siteRepo.GetSiteById(siteId);
            CmsCategoryEntity cat = new CmsCategoryEntity
            {
                SiteId = siteId,
                Name = "一级分类",
                ParentId = 1,
                Tag = "cat1",
            };
            ICategory ic = ist.GetCategoryByTag("root/cat1");
            if (ic == null)
            {
                ic = this.repo.CreateCategory(cat);
            }
            Error err = ic.Set(cat);
            if(err == null)
            {
                err = ic.Save();
            }
            if(err != null)
            {
                Assert.Fail(err.Message);
            }
            else
            {
                this.Println(this.Stringfy(ic.Get()));
            }
        }

    }
}
