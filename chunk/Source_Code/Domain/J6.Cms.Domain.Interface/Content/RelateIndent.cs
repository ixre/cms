using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace J6.Cms.Domain.Interface.Content
{
   public class RelateIndent:MarshalByRefObject, IValueObject
    {
       public RelateIndent(string name, string siteLimit, string categoryLimit,bool enabled)
       {
           this.Name = name;
           this.SiteLimit = siteLimit;
           this.CategoryLimit = categoryLimit;
           this.Enabled = enabled;
           if (!String.IsNullOrEmpty(this.CategoryLimit) &&( this.SiteLimit == "*" || this.SiteLimit ==" "))
           {
               throw new FormatException("Category limit need site limit first!");
           }
       }

       public RelateIndent(string strSign)
       {
           String[] data = strSign.Split(' ');
           if (data.Length != 4)
           {
               throw new FormatException("Relate indent string should like:\"video * * 1\"");
           }
           this.Name = data[0];
           this.SiteLimit = data[1];
           this.CategoryLimit = data[2];
           this.Enabled = data[3] == "1" || data[3] == "true";
           if (!String.IsNullOrEmpty(this.CategoryLimit) && (this.SiteLimit == "*" || this.SiteLimit == " "))
           {
               throw new FormatException("Category limit need site limit first!");
           }
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
           return String.Join(" ", this.Name, this.SiteLimit, this.CategoryLimit, this.Enabled ? "1" : "0");
       }


       public bool Equal(IValueObject that)
       {
           return false;
       }
    }
}
