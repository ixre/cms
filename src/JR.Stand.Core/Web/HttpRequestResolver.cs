using System;
using System.Collections.Generic;
using System.Text;

namespace JR.Stand.Core.Web
{
    public class HttpRequestResolver
    {
        public T FromBody<T>(ICompatibleHttpContext context)
        {
            Stream stream = context.request.
        }
    }
}
