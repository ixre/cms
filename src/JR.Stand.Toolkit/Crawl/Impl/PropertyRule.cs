//
//
//  Copyright 2011 @ S1N1.COM.all rights reseved.
//
//  Project : Untitled
//  File Name : PropertyRule.cs
//  Date : 2011/8/25
//  Author : 
//
//

using System.Collections;
using System.Collections.Generic;

namespace JR.Stand.Toolkit.Crawl.Impl
{
    /// <summary>
    /// �������Թ���������ʽ��
    /// </summary>
    public class PropertyRule : IEnumerable<string>
    {
        private IDictionary<string, string> dict = new Dictionary<string, string>();

        public string ID { get; set; }

        /// <summary>
        /// ������Թ���
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string value)
        {
            dict.Add(key, value);
        }

        /// <summary>
        /// ��ȡ���Թ���
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return dict.Keys.Contains(key) ? dict[key] : null; }
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (string key in dict.Keys) yield return key;
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}