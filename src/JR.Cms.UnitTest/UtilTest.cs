using System;
using JR.Cms.Web.Resource;
using NUnit.Framework;

namespace JR.Cms.UnitTest
{
    [TestFixture]
    public class UtilTest
    {
        [Test]
        public void TestCompressJS()
        {
            var code = " if(a.length>0&&a[0]!='/'&&a.indexOf('//')!=0&&a.indexOf('https://')==-1&&a.indexOf('http://')==-1)";
            var result = ResourceUtility.CompressHtml(code);
            Console.WriteLine(result);
        }
    }
}