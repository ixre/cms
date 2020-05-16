using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace JR.Stand.Abstracts.Web
{
    public interface IFormValues:IEnumerable<KeyValuePair<string, StringValues>>
    {
        List<string> Keys { get; }
        StringValues this[string key] { get; }
    }
}