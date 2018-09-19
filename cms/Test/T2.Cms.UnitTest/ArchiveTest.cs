using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using T2.Cms.Domain.Interface.Content.Archive;
using T2.Cms.Infrastructure;
using T2.Cms.Infrastructure.Ioc;
using T2.Cms.Models;

namespace T2.Cms.UnitTest
{
    [TestClass]
    public class ArchiveTest:TestBase
    {
        private IArchiveRepository archiveRepo;

        public ArchiveTest()
        {
            this.archiveRepo = Ioc.GetInstance<IArchiveRepository>();
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
    }
}
