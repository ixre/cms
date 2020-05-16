using System;
using System.Collections.Generic;
using JR.DevFw.Toolkit.NetCrawl;
using JR.Stand.Core.Data;
using JR.Stand.Core.Framework;

namespace Ops.Collection.TestApp
{
    class Program
    {
        static Collector director = Collector.Create(AppDomain.CurrentDomain.BaseDirectory + "default.conf");
        static DataBaseAccess db { get { return new DataBaseAccess(DataBaseType.SQLite, "data source=test.db"); } }

        static void Main(string[] args)
        {
        	
        	
        	//ToEntity.TestToEntity.Test();
        	
           // Show_ProjectsInfo();

          Download_Project1();

          //  CreateNewProject();

           // invoke_SinglePage();

          Console.ReadKey();

          //  RecordLog();

           // Test_tags();
            //Console.ReadLine();

        }

        private static void RecordLog()
        {
            LogFile log = new LogFile("log.txt",true);
            log.Println("xxxx");
            log.Println("记录第二条\r\n");
            Console.WriteLine("完毕");
        }

        private static void invoke_SinglePage()
        {
            //uri:http://news.163.com/11/0904/09/7D3M4LNK00011229.html
            Project pro = director.GetProject("guolan");
            pro.InvokeSingle("/news/24779.html", dp =>
            {
                Console.WriteLine(dp["content"]);
            });

            Console.WriteLine(pro.Name+"/"+pro.State.SuccessCount + "/" + pro.State.FailCount);
            pro.ResetState();

        }





        private static void Download_Project1()
        {
            DateTime dt = DateTime.Now;
            Project pro = director.GetProject("ifengmainland");
            pro.UseMultiThread = true;
            if (pro == null)
            {
                Console.WriteLine("项目不存在!"); return;
            }

            Console.WriteLine("项目:{0}开始下载数据!", pro.Name);

            int i = 0;
            DataPackFunc func = pk =>
            {
                /*
                db.ExecuteNonQuery("INSERT INTO test([title],[content],[createDate]) VALUES(@Title,@Content,@SubmitDate)",
                    db.NewParameter("@Title", pk["title"]),
                    db.NewParameter("@Content", pk["content"]),
                    db.NewParameter("@SubmitDate", string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now)));
                */
                ++i;

                Console.WriteLine("入库第{0}条->{1} [总用时：{2}s]", i, pk["title"], (DateTime.Now - dt).Seconds.ToString());
            };
            /*
            DataPack pack = new DataPack(pro.Rules);
            pack["title"] = "1";
            pack["content"]="content";
            func(pack);*/

           // pro.InvokeList(1, func);
           //pro.InvokeList(2, func);
           for (var j = 0; j< 100; j++)
           {
               pro.InvokeList(j+1, false,func);

           }
            //Console.WriteLine(pro.Collect("2").Count.ToString());


        }

        static void Show_ProjectsInfo()
        {
            Project[] projects = director.GetProjects();

            Console.WriteLine("共用{0}个项目\r\n", projects.Length);
            foreach (Project pro in projects)
            {
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("项目编号：{0}\r\n项目名称:{1}\r\n列表规则：{2}\r\n列表块规则：{3}\r\n页面地址规则：{4}\r\n词语过滤规则：{5}\r\n其他规则：\r\n",
                    pro.Id, pro.Name, pro.ListUriRule, pro.ListBlockRule, pro.PageUriRule,pro.FilterWordsRule);

                foreach (string key in pro.Rules)
                {
                    Console.WriteLine("{0}->{1}\r\n", key, pro.Rules[key]);
                }
            }

            director.ClearProjects();
        }


        private static void CreateNewProject()
        {
            Project prj = new Project();
            prj.Id = "ifengmainland";
            prj.Name = "凤凰网-大陆新闻";
            prj.RequestEncoding = "utf-8";
            prj.ListUriRule = "http://news.ifeng.com/mainland/rt-channel/rtlist_20110825/{0}.shtml";
            prj.ListBlockRule = "<div\\s+class=\"newsList\">\\s+([\\s\\S]+?)\\s+</div>";
            prj.PageUriRule = "http://news.ifeng.com/mainland/detail_\\d+_\\d+/\\d+/\\d+_\\d+.shtml";
            prj.Rules = new PropertyRule();

            prj.Rules.Add("title", "<h1\\s+id=\"artical_topic\">\\s*([\\s\\S]+?)\\s*</h1>");
            prj.Rules.Add("content", "<div\\s+id=\"artical_real\">\\s*([\\s\\S]+?)\\s*<span class=\"ifengLogo\">");

            Console.WriteLine(director.CreateProject(prj));
        }


        private static void Test()
        {
            PropertyRule rule = new PropertyRule();
            rule.Add("title", "^title$");
            rule.Add("content", "^content$");


            DataPack pack = new DataPack(rule,"");


            foreach (KeyValuePair<string, string> pair in pack)
            {

                Console.WriteLine(pair.Key + "->" + pair.Value);
            }
        }


    }
}
