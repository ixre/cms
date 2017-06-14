using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using T2.Cms.Infrastructure.Domain;
using T2.Cms.Infrastructure;
using T2.Cms;
using System.Net;
using System.IO;

namespace UnitTest
{
    public class TempA {
        public TempA(int id)
        {
            this.ID = id;
        }
        public int ID { get; private set; }
    }

    public class A
    {
        public static IList<TempA> dict;
        
        static A()
        {
            dict = new List<TempA>();

            dict.Add(new TempA(1));
            dict.Add(new TempA(2));
            dict.Add(new TempA(3));
        }

        public static void Clear()
        {
            for (int i = 0; i < dict.Count;i++ )
            {
                dict[i] = null;
            }
            dict = null;
        }

    }



    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            String pwd = Generator.Sha1Pwd("289941858", Generator.Offset);
            //pwd = Generator.Sha1Pwd("123456", Generator.Offset);// 316ed7406e674011bf326cbe3c7167b6fc86e026
            Assert.Inconclusive(pwd);
        }

        [TestMethod]    
        public void TestSharpRefrence()
        {
            TempA a = A.dict[0];
            A.Clear();
            //a = null;
            Assert.IsTrue(a == null);
        }
        [TestMethod]
        public void TestKvDb()
        {
            Kvdb.SetPath("");
            Kvdb.Put("abc:test", "123");
            var x = Kvdb.Get("abc:test");
            Assert.AreEqual(x, "123");
        }
        [TestMethod]
        public void TestDownloadRedirect()
        {
            string remoteVersionDescript =  "http://hk1001.ns.to2.net/cms/patch/latest/upgrade.xml";
            WebRequest wr = WebRequest.Create(remoteVersionDescript);

            HttpWebResponse rsp = wr.GetResponse() as HttpWebResponse;
            Stream rspStream = null;
            if (rsp == null || rsp.ContentLength <= 0 || (rspStream = rsp.GetResponseStream()) == null)
            {
                Console.WriteLine("链接失败");
            }

            Console.WriteLine(rsp.StatusCode.ToString());
            StreamReader rd = new StreamReader(rspStream);
            Console.WriteLine(rd.ReadToEnd());
            rd.Close();
           
        }

    }
}
