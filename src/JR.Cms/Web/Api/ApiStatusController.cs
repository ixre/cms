using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JR.Cms.Conf;
using JR.Cms.Infrastructure.Domain;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using JR.Cms.Web.Util;
using JR.Stand.Abstracts.Web;
using JR.Stand.Core.Framework.Web.UI;
using JR.Stand.Core.Web;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Http;
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
        public IDictionary<String, Object> Status()
        {
            IDictionary<String, Object> dict = new Dictionary<String, Object>();
            dict.Add("status", "ok");
            dict.Add("version", Cms.Version);
            return dict;
        }

        /// <summary>
        /// 创建访问密钥
        /// </summary>
        /// <returns></returns>
        [HttpPost("access_token")]
        public AccessTokenDataDto CreateAccessToken([FromBody] RequestAccessTokenDto dto)
        {
            AccessTokenDataDto dst = new AccessTokenDataDto();
            if (dto.Password.Length != 32)
            {
                return new AccessTokenDataDto {Code= 2, Message="密码长度不正确"};
            }
            dto.Password = Generator.CreateUserPwd(dto.Password);
            var ret = ServiceCall.Instance.UserService.TryLogin(dto.Username, dto.Password);
            if (ret.Tag == -1) return new AccessTokenDataDto {Code= 1, Message="用户名或密码不正确"};
            if(ret.Tag == -2) return new AccessTokenDataDto {Code= 3, Message="用户已停用"};
            long expiresTime = DateTimeOffset.UtcNow.AddSeconds(dto.Expires).ToUnixTimeSeconds();
            String token = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                .WithSecret(Settings.SYS_RSA_KEY)
                .AddClaim("aud", ret.Uid.ToString())
                .AddClaim("iss", "JRCms OpenAPI")
                .AddClaim("sub", "jrcms-openapi")
                .AddClaim("exp", expiresTime)
                .Encode();
            dst.AccessToken = "Bearer " + token;
            dst.ExpiresTime = expiresTime;
            return dst;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="siteId">站点编号</param>
        /// <param name="folder"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("{siteId}/upload/{folder}")]
        public UploadResultDto UploadImage(int siteId, string folder, [FromForm] IFormFile file)
        {
            ICompatiblePostedFile icf = new PostedFileImpl(file);
            var dir = UploadUtils.GetUploadDirPath(siteId, "image", true);
            var name = UploadUtils.GetUploadFileName(icf, "image");
            try
            {
                String url = new FileUpload(dir, name, false).Upload(icf);
                return new UploadResultDto
                {
                    Url = url,
                };
            }
            catch (Exception ex)
            {
                return new UploadResultDto
                {
                    Code = 1,
                    Message = ex.Message,
                    Url = "",
                };
            }
        }

        /// <summary>
        /// 提交文档
        /// </summary>
        /// <param name="siteId">站点编号</param>
        /// <param name="catalogId">分类编号</param>
        /// <param name="archive">文档</param>
        [HttpPost("{siteId}/{catalogId}")]
        [RequestAuthorize]
        public ArchivePostResultDto Post(int siteId, int catalogId, [FromBody] PostedArchiveDto archive)
        {
            var ret = ServiceCall.Instance.ArchiveService.SaveArchive(siteId, catalogId, new ArchiveDto
            {
                Title = archive.Title ?? "",
                PublisherId = 0,
                Content = archive.Content ?? "",
                Thumbnail = archive.Thumbnail ?? "",
                //Path = archive.,
                //ExtendValues = null,
            });
            if (ret.ErrCode > 0)
            {
                return new ArchivePostResultDto
                {
                    Code = ret.ErrCode,
                    Message = ret.ErrMsg,
                };
            }

            Dictionary<string, string> dict = (ret.Data as Dictionary<string, string>) ??
                                              new Dictionary<string, string>();
            String path = dict["Path"];
            return new ArchivePostResultDto
            {
                Url = path,
            };
        }
    }
}