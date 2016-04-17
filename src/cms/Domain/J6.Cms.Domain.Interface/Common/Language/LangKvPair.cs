using System;
using System.Collections.Generic;

namespace J6.Cms.Domain.Interface.Common.Language
{
    /// <summary>
    /// 语言键值对
    /// </summary>
    public class LangKvPair
    {
        public String key;
        public IDictionary<int, String> value;
    }
}
