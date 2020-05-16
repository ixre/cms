using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Cms.Domain.Interface.Content
{
    public class RelateIndent : MarshalByRefObject, IValueObject
    {
        public RelateIndent(string name, string siteLimit, string categoryLimit, bool enabled)
        {
            Name = name;
            SiteLimit = siteLimit;
            CategoryLimit = categoryLimit;
            Enabled = enabled;
            if (CategoryLimit != null && CategoryLimit != "*" && (SiteLimit == "*" || SiteLimit == " "))
                throw new FormatException("Category limit need site limit first!");
        }

        public RelateIndent(string strSign)
        {
            var data = strSign.Split(' ');
            if (data.Length != 4) throw new FormatException("Relate indent string should like:\"video * * 1\"");
            Name = data[0];
            SiteLimit = data[1];
            CategoryLimit = data[2];
            Enabled = data[3] == "1" || data[3] == "true";
            if (CategoryLimit != null && CategoryLimit != "*" && (SiteLimit == "*" || SiteLimit == " "))
                throw new FormatException("Category limit need site limit first!");
        }

        /// <summary>
        /// 标识名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 站点限制,* 表示不限制,"-"表示当前站点
        /// </summary>
        public string SiteLimit { get; set; }

        /// <summary>
        /// 栏目限制,* 表示不限制
        /// </summary>
        public string CategoryLimit { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        public override string ToString()
        {
            return string.Join(" ", Name, SiteLimit, CategoryLimit, Enabled ? "1" : "0");
        }


        public bool Equal(IValueObject that)
        {
            return false;
        }
    }
}