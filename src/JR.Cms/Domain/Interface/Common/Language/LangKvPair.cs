using System;
using System.Collections.Generic;

namespace JR.Cms.Domain.Interface.Common.Language
{
    /// <summary>
    /// 语言键值对
    /// </summary>
    public class LangKvPair
    {
        public string key;
        public IDictionary<int, string> value;
    }
}