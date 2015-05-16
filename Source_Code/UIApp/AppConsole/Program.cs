using System;
using System.Collections.Generic;
using System.Text;
using AtNet.DevFw.Data;
using System.Net;

namespace AtNet.DevFw.Data.Test
{
    class Program
    {
        internal enum TestEnum { A, B };
        internal class A { }
        internal class B : A { }

        private class Word
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public static string ID2 { get; set; }
        }

        static void Main(string[] args)
        {
            //TestEnumMethod();

            do
            {
                DateTime dt = DateTime.Now;
                Console.WriteLine("===== testing ======");
                TestSite();
                TimeSpan ts = (DateTime.Now - dt);
                Console.WriteLine("total use time:{0}.{1},enter retesting!",ts.TotalSeconds.ToString(),ts.Milliseconds.ToString());

            } while (Console.ReadKey().Key == ConsoleKey.Enter);

        }

        static void TestEnumMethod()
        {
            Console.WriteLine(Enum.Parse(typeof(TestEnum),"B"));
        }

        static void TestSite()
        {
            const int times = 100;
            string url="http://localhost:8000/";
            Console.WriteLine("test {0} ,{1} times avg response time:{2}", url, times.ToString(), TestUrl(url, times).ToString());

            url = "http://localhost:8000/news/";
            Console.WriteLine("test {0} ,{1} times avg response time:{2}", url, times.ToString(), TestUrl(url, times).ToString());

            url = "http://localhost:8000/news/news.html";
            Console.WriteLine("test {0} ,{1} times avg response time:{2}", url, times.ToString(), TestUrl(url, times).ToString());

        }

        static int TestUrl(string url,int times)
        {
            WebClient client = new WebClient();
            int totalMiniSeconds = 0;
            for (int i = 0; i < times; i++)
            {
                DateTime dt = DateTime.Now;
                client.DownloadString(url);
                totalMiniSeconds += (DateTime.Now - dt).Milliseconds;
            }

            return totalMiniSeconds / times;
        }
    }
}
