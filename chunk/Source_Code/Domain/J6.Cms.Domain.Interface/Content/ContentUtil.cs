using System.Collections.Generic;

namespace J6.Cms.Domain.Interface.Content
{
   public  static class ContentUtil
   {
       private static IDictionary<int, RelateIndent> _indents;


       public static IDictionary<int, RelateIndent> GetRelatedIndents()
       {
           if (_indents == null)
           {
               _indents = new Dictionary<int, RelateIndent>(8);
               RelateIndent[] indents = new RelateIndent[]
               {
                   new RelateIndent("默认类型","*","*",true),
                   new RelateIndent("未配置1","*","*",true),
                   new RelateIndent("未配置2","*","*",true),
                   new RelateIndent("未配置3","*","*",true),
                   new RelateIndent("未配置4","*","*",true),
                   new RelateIndent("未配置5","*","*",true),
                   new RelateIndent("未配置6","*","*",true),
                   new RelateIndent("未配置7","*","*",true),
                   new RelateIndent("未配置8","*","*",true),
                   new RelateIndent("未配置9","*","*",true),
                   new RelateIndent("未配置10","*","*",true),
               };

               int tmpInt = 0;
               foreach (var relateIndent in indents)
               {
                   _indents.Add(tmpInt,relateIndent);
                   tmpInt++;
               }
           }
           return _indents;
       }

       /// <summary>
       /// 设置关联类型
       /// </summary>
       /// <param name="indents"></param>
       public static void SetRelatedIndents(IDictionary<int,RelateIndent> indents)
       {
           if (indents != null && indents.Count != 0)
           {
               _indents = indents;
           }
       }

       public static RelateIndent GetRelatedIndentName(int relatedIndent)
       {
           if (_indents != null && _indents.ContainsKey(relatedIndent))
           {
               return _indents[relatedIndent];
           }
           return null;
       }
   }
}
