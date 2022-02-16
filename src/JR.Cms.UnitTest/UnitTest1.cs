using System;
using System.Text.RegularExpressions;
using JR.Stand.Core.Framework.Security;
using NUnit.Framework;

namespace JR.Cms.UnitTest
{
    public class RsaTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var key = RSA.CreateKey();
            Console.WriteLine(key.PrivateKey);
            Match mc = Regex.Match(key.PrivateKey, "<Modulus>(.+)</Modulus>");
            Console.WriteLine("--"+mc.Groups[1].Value);

            Assert.Pass();
        }
    }
}