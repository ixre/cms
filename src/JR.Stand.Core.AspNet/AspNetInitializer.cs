using JR.Stand.Core.Web;

namespace JR.Stand.Core.AspNet
{
    public static class AspNetInitializer
    {
        public static void Init()
        {
            HttpHosting.Configure(new AspNetHttpContext());
        }
    }
}