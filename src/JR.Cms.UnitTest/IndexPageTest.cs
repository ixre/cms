using System;
using System.Net.Http;
using NUnit.Framework;
using HttpClient = JR.Stand.Core.Framework.Net.HttpClient;

namespace JR.Cms.UnitTest
{
    public class IndexPageTests
    {
        [Test]
        public void Test66()
        {
            var t = DateTime.Now;
            HttpClient.Request("http://localhost:5000", "GET", null, null, 30000);
            Console.WriteLine((DateTime.Now - t).Milliseconds);
        }
    }
}