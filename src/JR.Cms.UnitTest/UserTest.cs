using System;
using JR.Cms.Infrastructure.Domain;
using JR.Stand.Core.Framework.Extensions;
using NUnit.Framework;

namespace JR.Cms.UnitTest
{
    [TestFixture]
    public class UserTest
    {
        [Test]
        public void TestSha1Pwd()
        {
            var md5 = "123456".Md5();
            Console.WriteLine(Generator.CreateUserPwd(md5));
        }
    }
}