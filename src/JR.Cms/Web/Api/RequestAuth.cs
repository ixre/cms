using System;
using System.Collections.Generic;
using JR.Cms.Conf;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JR.Cms.Web.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestAuthorizeAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            String token = context.HttpContext.Request.Headers["Authorization"];
            if (String.IsNullOrEmpty(token))
            {
                throw new Exception("401: access denied");
            }

            token = token.Replace("Bearer ", "");
            var payload = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                .WithSecret(Settings.SYS_PRIVATE_KEY)
                .MustVerifySignature()
                .Decode<IDictionary<string, object>>(token);
            if ((string) payload["iss"] != "JRCms OpenAPI")
            {
                throw new Exception("401: error jwt iss, access denied");
            }

            context.HttpContext.Request.Headers["aud"] = (string)payload["aud"];
        }
    }
}