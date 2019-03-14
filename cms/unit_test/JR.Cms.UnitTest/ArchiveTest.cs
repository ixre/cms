using System;
using JR.DevFw.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JR.Cms.CacheService;
using JR.Cms.DataTransfer;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site.Category;
using JR.Cms.Infrastructure;
using JR.Cms.Infrastructure.Ioc;
using JR.Cms.Models;

namespace JR.Cms.UnitTest
{
    [TestClass]
    public class ArchiveTest:TestBase
    {
        private IArchiveRepository archiveRepo;
        private ICategoryRepo catRepo;

        public ArchiveTest()
        {
            this.archiveRepo = Ioc.GetInstance<IArchiveRepository>();
            this.catRepo = Ioc.GetInstance<ICategoryRepo>();
        }

        [TestMethod]
        public void TestSaveArchive()
        {
            IArchive ia = this.archiveRepo.GetArchiveById(1, 3);
            CmsArchiveEntity v = ia.Get();
            Error err = ia.Set(v);
            if(err == null)
            {
                ia.SetTemplatePath("archive");
                err = ia.Save();
            }
            if(err != null)
            {
                Assert.Fail(err.Message);
            }
            else
            {
                this.Println(this.Stringfy(ia.Get()));
            }
        }

        [TestMethod]
        public void TestSwapArchiveSortNumber()
        {
            IArchive src = this.archiveRepo.GetArchiveById(1, 4);
            //src.MoveSortUp();
            src.MoveSortDown();
            Console.WriteLine("--flag=" + src.Get().Flag);
        }

        [TestMethod]
        public void TestGetArchive()
        {
            String path = "video_res/SP-001";
            IArchive ia = this.archiveRepo.GetArchiveByPath(1, path);
            if(ia == null)
            {
                Assert.Fail("no such archive");
            }
            else
            {
                this.Println(this.Stringfy(ia.Get()));
            }
        }

        [TestMethod]
        public void TestGetArchivesByCatPath()
        {
            int siteId = 1;
            String catPath = "cms";
           ArchiveDto[] array = ServiceCall.Instance.ArchiveService
                .GetArchivesByCategoryPath(siteId, catPath, true, 10,0);
            this.Println(this.Stringfy(array));
        }

        [TestMethod]
        public void TestUnixTime()
        {
            DateTime dt = TimeUtils.UnixTime(1538062009);
            this.Println(dt.ToString());
        }
    }
}
