using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using J6.Cms.Infrastructure.Domain;

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
            String pwd = Generator.Md5Pwd("lms87319225", null);
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
    }
}
