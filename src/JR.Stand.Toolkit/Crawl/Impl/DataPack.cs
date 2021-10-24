//
//  Copyright 2011 @ S1N1.COM.all rights reserved.
//
//  Project : Untitled
//  File Name : DataPack.cs
//  Date : 2011/8/25
//  Author : 
//
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace JR.Stand.Toolkit.Crawl.Impl
{
    /// <summary>
    /// �ɼ������ݰ�
    /// </summary>
    public class DataPack : ICloneable, IEnumerable<KeyValuePair<string, string>>
    {
        private PropertyRule property;

        /// <summary>
        /// ���ݼ���
        /// </summary>
        private IDictionary<string, string> dict = new Dictionary<string, string>();

        public DataPack(PropertyRule property, string referenceUrl)
        {
            this.property = property;
            this.ReferenceUrl = referenceUrl;

            foreach (string key in property)
            {
                dict.Add(key, key);
            }
        }

        /// <summary>
        /// �ɼ���Դ��ַ
        /// </summary>
        public string ReferenceUrl { get; private set; }

        //��ȡ����
        public string this[string key]
        {
            get { return dict.Keys.Contains(key) ? dict[key] : null; }
            set
            {
                if (dict.Keys.Contains(key)) dict[key] = value;
                else dict.Add(key, value);
            }
        }

        /// <summary>
        /// ��¡һ���µ�DataPack���󣬲�����
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            DataPack pack = new DataPack(property, this.ReferenceUrl);
            return pack;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (KeyValuePair<string, string> pair in dict) yield return pair;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}