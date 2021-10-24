//
//
//  Copyright 2011 (C) S1N1.COM,All rights reserved.
//  -----------------------------
//  Project : OPSoft.Plugin.NetCrawl
//  File Name : Project.cs
//  Date : 2011/8/25
//  Author : Newmin
//  -----------------------------
//  2011-09-06 [+] newmin:��Ӽ�����·���Ĺ��ܣ��ɼ��б��а���"/news/12.html"��������
//
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace JR.Stand.Toolkit.Crawl.Impl
{
    public delegate void DataPackFunc(DataPack datapack);

    public class Project
    {
        private State state = new State();

        public bool SaveResouce;

        public string SaveResourceExtension;

        public string ResouceSavePath;

        /// <summary>
        /// ���·���ĸ�·��
        /// </summary>
        private string basePath;

        /// <summary>
        /// ��Ե�ַƥ����ʽ
        /// </summary>
        private static Regex absoluteUriRegex = new Regex("^(?!http)", RegexOptions.IgnoreCase);

        /// <summary>
        /// ��Ŀ���
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// ��Ŀ����
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ĭ��30�볬ʱ
        /// </summary>
        public int RequestTimeOut = 30000;

        private string _formatedPageUriRule;

        /// <summary>
        /// �������
        /// </summary>
        public string RequestEncoding { get; set; }

        /// <summary>
        /// �Ƿ�ʹ�ö��̣߳�Ĭ�϶��߳�
        /// </summary>
        public bool UseMultiThread { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        private Encoding Encode
        {
            get
            {
                int codepage;
                switch (RequestEncoding.ToLower())
                {
                    default:
                        codepage = 65001;
                        break;
                    case "default":
                        return Encoding.Default;
                    case "gb2312":
                        codepage = 936;
                        break;
                    case "big5":
                        codepage = 950;
                        break;
                }
                return Encoding.GetEncoding(codepage);
            }
        }

        /// <summary>
        /// �б�ҳURI����
        /// </summary>
        public string ListUriRule { get; set; }

        /// <summary>
        /// �б�����,���ڻ�ȡҳ���URI�б�
        /// </summary>
        public string ListBlockRule { get; set; }

        /// <summary>
        /// ҳ��URI����
        /// </summary>
        public string PageUriRule { get; set; }

        /// <summary>
        /// ��ʽ����
        /// </summary>
        public string FormatedPageUriRule
        {
            get
            {
                return this._formatedPageUriRule ??
                       (this._formatedPageUriRule = RuleFormat.Format(this.PageUriRule));
            }
        }

        /// <summary>
        /// Ҫ���˵Ĵ������,���ᱻ�滻��{filter:���˵Ĵ���}
        /// </summary>
        public string FilterWordsRule { get; set; }

        /// <summary>
        /// �������Թ���
        /// </summary>
        public PropertyRule Rules { get; set; }

        /// <summary>
        /// �ɼ�״̬
        /// </summary>
        public State State
        {
            get { return state; }
        }

        /// <summary>
        /// ���ü���״̬
        /// </summary>
        public void ResetState()
        {
            state.TotalCount = 0;
            state.SuccessCount = 0;
            state.FailCount = 0;
        }


        /// <summary>
        /// �ɼ��б�ҳ���������ݼ���
        /// </summary>
        /// <param name="listUriParameter"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        public IList<DataPack> Collect(object listUriParameter, bool reverse)
        {
            //ReadPage(parameter);
            string uri = String.Format(this.ListUriRule, listUriParameter);
            IList<DataPack> packs = new List<DataPack>();

            AnalysisListPage(uri, reverse, dp => { packs.Add(dp); });


#if DEBUG
    // ��ȡ��ҳ����
    // int i=0;
    // DataPack pack= GetPageData("http://news.163.com/11/0824/10/7C7DG91H00011229.html", ref i);
#endif

            return packs;
        }

        /// <summary>
        /// �ɼ��б�ҳ�����Բɼ��Ľ��ִ�в���
        /// </summary>
        /// <param name="listUriParameter">�б�URI�����еĲ���"{0}"��ֵ</param>
        /// <param name="reverse"></param>
        /// <param name="func"></param>
        public void InvokeList(object listUriParameter, bool reverse, DataPackFunc func)
        {
            string uri = String.Format(this.ListUriRule, listUriParameter);
            AnalysisListPage(uri, reverse, func);
        }

        /// <summary>
        /// �ɼ��б�ҳ�����Բɼ��Ľ��ִ�в���
        /// </summary>
        /// <param name="listUri">�б�ҳ��ַ</param>
        /// <param name="reverse"></param>
        /// <param name="func"></param>
        public void InvokeList(string listUri, bool reverse, DataPackFunc func)
        {
            AnalysisListPage(listUri, reverse, func);
        }

        /// <summary>
        /// �ɼ���ƪ����
        /// </summary>
        /// <param name="pageUri"></param>
        /// <param name="func"></param>
        public void InvokeSingle(string pageUri, DataPackFunc func)
        {
            int i = 0;
            this.State.TotalCount = 1;
            GetPageData(pageUri, ref i, func);
        }


        /// <summary>
        /// �����б�ҳ��,���Խ��ִ�л�ִ����
        /// </summary>
        /// <param name="pageUri"></param>
        /// <param name="reverse"></param>
        /// <param name="func"></param>
        private void AnalysisListPage(string pageUri, bool reverse, DataPackFunc func)
        {
            int taskCount = 0,
                //������
                taskNumbers = 0; //һ�����������ж������Ƿ����

            string html; //���ص��б�ҳ��Html

            int bufferLength = 1;
            byte[] buffer = new byte[bufferLength]; //���ص����ݻ�����
            StringBuilder sb = new StringBuilder(); //���췵�صĽ��
            MatchCollection listMatches; //�б��ƥ�估ҳ���ַƥ��


#if DEBUG
            Console.WriteLine("��ʼ��:{0}��������...", pageUri);
#endif


            //�����б�ҳ����
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pageUri);
            request.Timeout = this.RequestTimeOut;

            Stream stream = request.GetResponse().GetResponseStream();


            using (StreamReader sr = new StreamReader(stream, this.Encode))
            {
                html = sr.ReadToEnd();
            }

#if DEBUG
            Console.WriteLine("���ص�����Ϊ:{0}", html);
#endif


            //�����б�ҳ����
            listMatches = Regex.Matches(html, RuleFormat.Format(this.ListBlockRule));


            //û���ҵ�ƥ��
            if (listMatches.Count == 0)
            {
#if DEBUG
                Console.WriteLine("û�ҵ�ƥ��!");
#endif
                return;
            }


            //����ƥ������

#if DEBUGS
            Console.WriteLine("\r\n------------------------------\r\n�õ�ƥ����б�����Ϊ:\r\n");
#endif

            Regex pageUriRegex = new Regex(this.FormatedPageUriRule);

            //�����ʵ�
            IList<string> pageUrls = new List<string>();

            foreach (Match m in listMatches)
            {
#if DEBUG
                Console.WriteLine("\r\n------------------------------------------------\r\n{0}", m.Value);
#endif
                foreach (Match pm in pageUriRegex.Matches(m.Value))
                {
#if DEBUG
                    Console.WriteLine(pm.Value);
#endif
                    pageUrls.Add(pm.Value);


                    //��ȡҳ�����ݣ��������ִ��������

                    //���̻߳�ȡ
                    //if (!UseSingleThread)
                    //{
                    //    new Thread(() =>
                    //    {
                    //        //���û�ִ����
                    //        GetPageData(pm.Value, ref taskNumbers, func);
                    //    }
                    //    ).Start();
                    //}
                    //else   //���̵߳���
                    //{
                    //    //���û�ִ����
                    //    GetPageData(pm.Value, ref taskNumbers, func);
                    //}
                }
            }

            //����������
            taskCount = pageUrls.Count;

            // ��ת˳��
            if (reverse)
            {
                pageUrls = new List<string>(pageUrls.Reverse());
            }

            if (!this.UseMultiThread) //���߳�
            {
                foreach (string pageUrl in pageUrls)
                {
                    //���û�ִ����
                    GetPageData(pageUrl, ref taskNumbers, func);
                }
            }
            else
            {
                MultiThreadProcess mp = new MultiThreadProcess(5, taskCount);
                mp.Start<IList<string>>(urls =>
                {
                    lock (urls)
                    {
                        //���û�ִ����
                        GetPageData(urls[0], ref taskNumbers, func);
                        pageUrls.Remove(urls[0]);
                    }
                }, pageUrls);
            }

            //������������
            state.TotalCount = taskCount;

            //ֱ���߳̾�ִ����ϣ��򷵻�
            do
            {
            } while (taskNumbers != taskCount);


#if DEBUG
            Console.WriteLine("�������....!���ɼ���{0}��", taskCount);
#endif
        }

        /// <summary>
        /// ��ȡһ��ҳ������ݲ�����
        /// </summary>
        /// <param name="pageUri">ҳ���ַ</param>
        /// <param name="number">ά��һ������,�ж������Ƿ����</param>
        /// <returns></returns>
        private DataPack GetPageData(string pageUri, ref int number, DataPackFunc func)
        {
            DataPack dp;
            int bufferLength = 10;
            byte[] buffer = new byte[bufferLength]; //���ص����ݻ�����
            StringBuilder sb = new StringBuilder(); //���췵�صĽ��
            Match match; //����ƥ��


            //ҳ���ַ��ҳ���ַ����ƥ�䣡

            if (!Regex.IsMatch(pageUri, this.FormatedPageUriRule))
            {
                ++number;
                state.FailCount++;
                return null;

                //throw new ArgumentException("ҳ���ַ��ҳ���ַ����ƥ�䣡", pageUri);
            }

            //���ҳ���ַΪ���·�������������
            if (absoluteUriRegex.IsMatch(pageUri)) pageUri = GetBasePath(pageUri) + pageUri;


            //����ҳ���HTML
            string html = String.Empty;

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(pageUri);
                req.Timeout = this.RequestTimeOut;


                Stream stream = req.GetResponse().GetResponseStream();

                html = sb.ToString();
                using (StreamReader sr = new StreamReader(stream, this.Encode))
                {
                    html = sr.ReadToEnd();
                }
            }
            catch (Exception exc)
            {
                state.FailCount++;
                return null;
            }

            //������ص�����
#if DEBUG
            Console.WriteLine("\r\n------------------------------\r\n�õ�ƥ����б�����Ϊ:{0}",html);
#endif
            dp = new DataPack(Rules, pageUri);


            foreach (string propertyName in this.Rules)
            {
                match = Regex.Match(html, this.Rules[propertyName]);
                if (match != null)
                {
                    dp[propertyName] = match.Groups[1].Value;
                }
            }

#if DEBUG
            Console.WriteLine("\r\n-------------------------\r\n");
            foreach (KeyValuePair<string, string> pair in dp)
            {
                Console.WriteLine("{0}->{1}\r\n", pair.Key, pair.Value);
            }
#endif


            //���¼���
            ++number;


#if DEBUG
            Console.WriteLine("flish");
#endif
            //ִ�л�ִ����
            if (func != null) func(dp);


            //���һ���ɹ��ļ���
            state.SuccessCount++;

            return dp;
        }

        private string GetBasePath(string pageUri)
        {
            //����Ѿ������·������ֱ�ӷ���
            if (basePath != null) return basePath;

            //�������·����"/"��ͷ
            if (pageUri.StartsWith("/"))
            {
                Regex reg = new Regex("^(http://[^/]+/)", RegexOptions.IgnoreCase);
                if (reg.IsMatch(ListUriRule))
                {
                    basePath = reg.Match(ListUriRule).Groups[1].Value;
                }
            }
            else
            {
                Regex reg = new Regex("([^/]+)$", RegexOptions.IgnoreCase);
                if (reg.IsMatch(ListUriRule))
                {
                    string filePath = reg.Match(ListUriRule).Value;
                    basePath = ListUriRule.Replace(filePath, String.Empty);
                }
            }
            return basePath;
        }
    }
}