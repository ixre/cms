using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JR.Cms.Conf;
using JR.Cms.Library.CacheService;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Mvc;

namespace JR.Cms.Web.Api
{
    /// <summary>
    /// 开放API状态控制器
    /// </summary>
    [Route("/openapi")]
    [ApiController]
    public class ApiStatusController : Controller
    {
        /// <summary>
        /// 查询API状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("status")]
        public IDictionary<String,Object> Status()
        {
            IDictionary<String, Object> dict = new Dictionary<String, Object>();
            dict.Add("status","ok");
            dict.Add("version",Cms.Version);
            return dict;
        }

        [HttpGet("status2")]
        public AccessTokenDataDto Status2()
        {
            AccessTokenDataDto data2 = new AccessTokenDataDto();
            data2.AccessToken = "11212";
            return data2;
        }
        
        
        /// <summary>
        /// 创建访问密钥
        /// </summary>
        /// <returns></returns>
        [HttpPost("access_token")]
        public JsonResult CreateAccessToken([FromBody] RequestAccessTokenDto dto)
        {
            AccessTokenDataDto dst = new AccessTokenDataDto();
            ServiceCall.Instance.UserService.TryLogin(dto.Username, dto.Password);
            long expiresTime = DateTimeOffset.UtcNow.AddSeconds(dto.Expires).ToUnixTimeSeconds();
            String token = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                .WithSecret(Settings.SYS_PRIVATE_KEY)
                .AddClaim("aud","JR-CMS")
                .AddClaim("iss","JRCms OpenAPI")
                .AddClaim("sub","jrcms-openapi")
                .AddClaim("exp", expiresTime)
                .AddClaim("claim2", "claim2-value")
                .Encode();
            dst.AccessToken = "Bearer "+ token;
            dst.ExpiresTime = expiresTime;
            return Json(dst);
        }
    }
}