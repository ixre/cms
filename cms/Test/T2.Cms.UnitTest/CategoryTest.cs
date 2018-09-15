using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using T2.Cms.Infrastructure.Ioc;
using T2.Cms.Domain.Interface.Site.Category;
using T2.Cms.Domain.Interface.Site;
using T2.Cms.Models;
using T2.Cms.Infrastructure;
using T2.Cms.Domain.Interface.Site.Template;

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
            ISite ist = this.siteRepo.GetSiteById(siteId);
           ICategory ic = ist.GetCategoryByPath("root/cat1");
            if (ic == null)
            {
                Assert.Fail("no such category");
            }
            Console.WriteLine(this.Stringfy(ic.Get()));
        }

        [TestMethod]
        public void TestGetCategoryById()
        {
            ICategory ic = this.repo.GetCategory(siteId, 2);
            Console.WriteLine(this.Stringfy(ic.Get()));

        }

        /// <summary>
        /// 测试保存栏目
        /// </summary>
        [TestMethod]
        public void SaveCategory()
        {
            ISite ist = this.siteRepo.GetSiteById(siteId);
            CmsCategoryEntity cat = new CmsCategoryEntity
            {
                SiteId = siteId,
                Name = "一级分类",
                ParentId = 3,
                Tag = "cat1",
                Location= "http://baidu.com",
            };
            ICategory ic = ist.GetCategoryByPath("root/cat1");
            if (ic == null)
            {
                ic = this.repo.CreateCategory(cat);
            }
            Error err = ic.Set(cat);
            if(err == null)
            {
                TemplateBind[] arr = new TemplateBind[2];
                arr[0] = new TemplateBind(0, TemplateBindType.CategoryArchiveTemplate, "default/archive_1");
                arr[1] = new TemplateBind(0, TemplateBindType.CategoryTemplate, "default/category_1");
                err = ic.SetTemplates(arr);
                if (err == null)
                {
                    err = ic.Save();
                }
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

        /// <summary>
        /// 测试获取站点的栏目树形
        /// </summary>
        [TestMethod]
        public void TestGetCategoryTreeNode()
        {
            ISite ist = this.siteRepo.GetSiteById(siteId);
            this.Println(this.Stringfy(ist.GetCategoryTreeWithRootNode()));
        }
    }
}
