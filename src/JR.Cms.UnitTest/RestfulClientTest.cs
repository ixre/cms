using System;
using System.Collections.Generic;
using JR.Cms.Web.Api;
using JR.Stand.Core.Framework.Api;
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
                    Data = new Dictionary<String, Object>
                    {
                        {"Username", "master"},
                        {"Password", "123456"},
                        {"Expires", expires},
                    },
                });
                AccessTokenDataDto data2 = JsonConvert.DeserializeObject<AccessTokenDataDto>(data);
                if (data2 != null) return data2.AccessToken;
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
    }
}