using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace J6.Cms.Domain.Interface.Content
{
   public  static class ContentUtil
   {
       private static IDictionary<int, string> _indents;


       public static IDictionary<int, string> GetRelatedIndents()
       {
           if (_indents == null)
           {
              _indents = new Dictionary<int, string>(8);
               _indents.Add(0,"默认分组");
               _indents.Add(1, "#未配置1");
               _indents.Add(2, "#未配置2");
               _indents.Add(3, "#未配置3");
               _indents.Add(4, "#未配置4");
               _indents.Add(5, "#未配置5");
               _indents.Add(6, "#未配置6");
               _indents.Add(7, "#未配置7");
               _indents.Add(8, "#未配置8");
               _indents.Add(9, "#未配置9");
               _indents.Add(10, "#未配置10");
           }
           return _indents;
       }

       /// <summary>
       /// 设置关联类型
       /// </summary>
       /// <param name="indents"></param>
       public static void SetRelatedIndents(IDictionary<int,string> indents)
       {
           if (indents != null && indents.Count != 0)
           {
               _indents = indents;
           }
       }
   }
}
