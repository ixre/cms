using System.Collections.Generic;

namespace JR.Cms.Domain.Interface.Content
{
    public static class ContentUtil
    {
        private static IDictionary<int, RelateIndent> _indents;


        public static IDictionary<int, RelateIndent> GetRelatedIndents()
        {
            if (_indents == null)
            {
                _indents = new Dictionary<int, RelateIndent>(8);
                var indents = new RelateIndent[]
                {
                    new RelateIndent("默认类型", "*", "*", true),
                    new RelateIndent("未配置1", "*", "*", false),
                    new RelateIndent("未配置2", "*", "*", false),
                    new RelateIndent("未配置3", "*", "*", false),
                    new RelateIndent("未配置4", "*", "*", false),
                    new RelateIndent("未配置5", "*", "*", false),
                    new RelateIndent("未配置6", "*", "*", false),
                    new RelateIndent("未配置7", "*", "*", false),
                    new RelateIndent("未配置8", "*", "*", false),
                    new RelateIndent("未配置9", "*", "*", false),
                    new RelateIndent("未配置10", "*", "*", false),
                };

                var tmpInt = 0;
                foreach (var relateIndent in indents)
                {
                    _indents.Add(tmpInt, relateIndent);
                    tmpInt++;
                }
            }

            return _indents;
        }

        /// <summary>
        /// 设置关联类型
        /// </summary>
        /// <param name="indents"></param>
        public static void SetRelatedIndents(IDictionary<int, RelateIndent> indents)
        {
            if (indents != null && indents.Count != 0) _indents = indents;
        }

        public static RelateIndent GetRelatedIndentName(int relatedIndent)
        {
            if (_indents != null && _indents.ContainsKey(relatedIndent)) return _indents[relatedIndent];
            return null;
        }
    }
}