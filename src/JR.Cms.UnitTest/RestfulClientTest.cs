using System;
using System.Collections.Generic;
using JR.Cms.Web.Api;
using JR.Stand.Core.Framework.Api;
using JR.Stand.Core.Framework.Extensions;
using JR.Stand.Core.Framework.Net;
using Newtonsoft.Json;
using NUnit.Framework;

namespace JR.Cms.UnitTest
{

    public class RestfulClientTest
    {
        private RestfulClient client;

        public RestfulClientTest()
        {
            String url = "http://localhost:5000/openapi";
            int expires = 300;
            client = new RestfulClient(url);
            client.UseToken(() =>
            {
                String data = HttpClient.Request(url + "/access_token", "POST", new HttpRequestParam
                {
                    Body = new Dictionary<String, Object>
                    {
                        {"Username", "master"},
                        {"Password", "newmin888".Md5().ToLower()},
                        {"Expires", expires},
                    },
                });
                AccessTokenDataDto data2 = JsonConvert.DeserializeObject<AccessTokenDataDto>(data);
                return data2.AccessToken;
                return "";
            },3000); 
        }
        
        [Test]
        public void TestCms()
        {
           String ret= client.Request("/status", "GET", null);
           Console.WriteLine("result:"+ret);

           AccessTokenDataDto data2 = new AccessTokenDataDto();
           data2.AccessToken = "11212";
           String s = JsonConvert.SerializeObject(data2);
            Console.WriteLine("result:"+s);

        }

        [Test]
        public void TestUploadImage()
        {
            String url = "http://localhost:5000/openapi";

            String data = HttpClient.Request(url + "/1/upload", "POST", new HttpRequestParam
            {
                Body = new Dictionary<String, Object>
                {
                    {"Username", "master"},
                    {"Password", "123456"},
                },
            }); 
        }

        [Test]
        public void TestPostArchive()
        {
            ArchivePostResultDto ret = client.Request<ArchivePostResultDto>("/1/1", "POST", new PostedArchiveDto
            {
                Title = "测试OpenAPI上传文档",
                Content = "测试OpenAPI上传文档",
                Thumbnail = ""
            });
            Console.WriteLine("----"+ret.Url);
        }
    }
}