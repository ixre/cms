using System.Threading.Tasks;
using JR.Cms.Conf;
using JR.Cms.Domain.Interface.Common.Language;
using JR.Stand.Abstracts.Safety;
using JR.Stand.Abstracts.Web;

namespace JR.Cms.Web.Portal.Comm
{
    public class CmsPkgProxyController
    {
        public Task Change(ICompatibleHttpContext context)
        {
            string langOpt = context.Request.Query("lang");
            string deviceOpt = context.Request.Query("device");
            var isChange = false;
            int i;
            if (!string.IsNullOrEmpty(langOpt))
                if (Cms.Context.SetUserLanguage((int) ParseLang(langOpt)))
                    isChange = true;

            if (!string.IsNullOrEmpty(deviceOpt))
            {
                int.TryParse(deviceOpt, out i);
                if (Cms.Context.SetUserDevice(i)) isChange = true;
            }

            if (isChange)
            {
                string returnUrl = context.Request.Query("return_url");
                if (string.IsNullOrEmpty(returnUrl))
                {
                    context.Request.TryGetHeader("UrlReferrer", out var refer);
                    returnUrl = refer.Count == 0 ? "/" : refer.ToString();
                }

                context.Response.StatusCode(302);
                context.Response.AddHeader("Location", returnUrl);
                return SafetyTask.CompletedTask;
            }

            return context.Response.WriteAsync("error params ! should be  /" + CmsVariables.DEFAULT_CONTROLLER_NAME +
                                               "/change?lang=[1-8]&device=[1-2]");
        }

        private Languages ParseLang(string langOpt)
        {
            switch (langOpt)
            {
                case "zh_CN":
                case "cn":
                    return Languages.zh_CN;
                case "zh_TW":
                case "tw":
                    return Languages.zh_TW;
                case "en":
                case "us":
                case "en_US":
                    return Languages.en_US;
            }

            return Languages.zh_CN;
        }
    }
}