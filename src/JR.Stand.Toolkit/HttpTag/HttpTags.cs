using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using JR.Stand.Core;
using Microsoft.AspNetCore.Http;

namespace JR.Stand.Toolkit.HttpTag
{
    public class HttpTagsHandler
    {
        private HttpRequest request;
        private HttpResponse response;

        /// <summary>
        /// 默认的配置路径
        /// </summary>
        private static string defaultConfigPath = EnvUtil.GetBaseDirectory() + "config/tags.conf";

        //配置路径,如果URI参数config未指定则为默认路径
        private string configPath;

        public TagsManager Tags;

        public HttpTagsHandler():this(null)
        {
        }
        public HttpTagsHandler(string configPath)
        {
            this.configPath = configPath;

            //获取配置文件的路径
            if (String.IsNullOrEmpty(configPath))
            {
                this.configPath = EnvUtil.GetBaseDirectory() + request.GetParameter("config");
            }

            //创建标签管理器对象
            Tags = new TagsManager(configPath);
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pageContent"></param>
        public void ProcessRequest(HttpContext context,string pageContent)
        {
            request = context.Request;
            response = context.Response;



            //获取请求方式，确定执行的方法
            string method = request.Method;
            if (method == "GET")
            {
                string action = request.GetParameter("tag.action");
                if (String.IsNullOrEmpty(action))
                {
                    var assembly = typeof(TagsManager).Assembly;
                    var tagManager = ResourcesReader.Read(assembly, "HttpTag/Assets/tags_manager.html");
                    Dispaly_TagsList(String.IsNullOrEmpty(pageContent)?tagManager:pageContent);
                }
                else if (action == "test")
                {
                    Invoke_TestReplace();
                }
            }
            else
            {
                string action = request.Form["tag.action"];

                switch (action)
                {
                    case "save": SaveTags(); break;
                    case "delete": DeleteTags(); break;
                    case "create": CreateTags(); break;
                }
            }
            
        }

        public void ProcessRequest(HttpContext context)
        {
            this.ProcessRequest(context, null);
        }


        private void CreateTags()
        {
            string msg=String.Empty;

            string name = request.Form["name"],
                   linkUri = request.Form["linkuri"],
                   description = request.Form["description"];

            StringBuilder sb = new StringBuilder();
            //if (string.IsNullOrEmpty(id))
            //{
            //    sb.Append("编号不能为空!");
            //}
            //if(!Regex.IsMatch(id,"^[a-zA-Z0-9]+$"))
            //{
            //    sb.Append("编号必须为字母和数字");
            //}

            if (String.IsNullOrEmpty(name))
            {
                sb.Append("名称不能为空!");
            }
            if (sb.Length != 0)
            {
                msg = sb.ToString();
                goto tip;
            }

            bool result =Tags.Add(new Tag
            {
                Name = name,
                LinkUri = linkUri,
                Description = description
            });

            msg = result ? "添加成功" : "已经存在相同的标签";
        tip:
            response.WriteAsync(String.Format("<script>window.parent.tip('{0}');</script>", msg));

        }

        /// <summary>
        /// 删除Tags
        /// </summary>
        private void DeleteTags()
        {
            string msg;

            /*
            if (tagsManager.Tags.Count == 1)
            {
                msg = "请保留至少一个标签"; goto tip;
            }*/

            Regex reg = new Regex("^(?!action|name|link|des|id|linkuri|description)[a-zA-Z0-9]+$");

            int i = 0;
            foreach (var p in request.Form)
            {
                if (reg.IsMatch(p.Key))
                {
                    Tags.Delete(p.Key);
                    i++;
                }
            }
            msg = i == 0 ? "没选择要删除的项目!" : "删除成功";

            tip:
            response.WriteAsync($"<script>window.parent.tip('{msg}');</script>");
        }

        /// <summary>
        /// 保存Tags
        /// </summary>
        private void SaveTags()
        {
            string msg;

            Regex reg = new Regex("^name_([a-zA-Z0-9]+)$");

            int i = 0;
            string id;
            foreach (var p in request.Form)
            {
                if (reg.IsMatch(p.Key))
                {
                    id = reg.Match(p.Key).Groups[1].Value;

                    Tags.Update(new Tag
                    {
                         Indent=int.Parse(id),
                        Name = request.GetParameter("name_" + id),
                        LinkUri = request.GetParameter("link_" + id),
                        Description = request.GetParameter("des_" + id)
                    });

                    i++;
                }
            }
            msg = i == 0 ? "没有保存的项!" : "保存成功";

            response.WriteAsync($"<script>window.parent.tip('{msg}');</script>");
        }


        /// <summary>
        /// 显示标签列表
        /// </summary>
        private void Dispaly_TagsList(string pageContent)
        {
            string tagsHtml;

            int pageIndex;

            int.TryParse(request.GetParameter("page"), out pageIndex);
            if (pageIndex < 1) pageIndex = 1;

            StringBuilder sb = new StringBuilder();

            Regex reg=new Regex("\\$\\{([a-zA-Z]+)\\}");

            DataSet ds = new DataSet();
            ds.ReadXml(configPath);

            /*
            //获取分页链接
            for (int i = 0; i < ps.PageCount; i++)
            {
                sb.Append("<a href=\"?page=").Append((i + 1).ToString()).Append("\"")
                    .Append(i == pageIndex - 1 ? " style=\"background:white;color:black;text-decoration:none\"" : "").Append(">")
                    .Append((i + 1).ToString()).Append("</a>");
            }

            pagerHtml = sb.ToString();
            sb.Remove(0, sb.Length);
            */

            if (Tags.Tags.Count > 0)
            {
                foreach (Tag tag in Tags.Tags)
                {
                    sb.Append("<tr><td class=\"center\"><input type=\"checkbox\" class=\"ck\" name=\"ck")
                        .Append(tag.Indent.ToString()).Append("\" indent=\"").Append(tag.Indent.ToString()).Append("\"/></td>")
                        .Append("<td class=\"center\">").Append(tag.Name).Append("</td><td>")
                        .Append(tag.LinkUri).Append("</td><td>")
                        .Append(tag.Description).Append("</td><td class=\"center\"><a href=\"javascript:;\" onclick=\"edit(this)\">修改</a></td></tr>");
                }
                tagsHtml = sb.ToString();
            }
            else
            {
                tagsHtml = "<tr><td colspan=\"5\" class=\"center\">暂无tags!</td></tr>";
            }

            string html = reg.Replace(pageContent, match =>
            {
                switch (match.Groups[1].Value)
                {
                    default: return null;
                    case "css": return ReturnStyleLink();
                    case "tagsHtml": return tagsHtml;
                    case "pagetext": return "";

                }
            });
            response.WriteAsync(html);
        }



        private void Invoke_TestReplace()
        {
            TagsManager tagsManager = new TagsManager(EnvUtil.GetBaseDirectory() + "tag.xml");


            const string testContent = @"最近项目有需要<a class=""auto-tag"" target=""_blank"" title="""" href=""tpl1"">开发</a>一个模板的功能，在asp.net mvc项目中使用模板在不压缩代码的模板情况下，性能能提升0.02s左右(本地IIS+FF)";


            const string testContent2 = @"奥博建站系统是<a href=""#"">厦门奥博科技技术</a><p>开发的一款<a href=""#"">奥博科技技术</a>基于asp.net mvc和asp.net <a href=""#"">模板</a>组件的网站管理系统。程序高效，安全，功能强大。从最初的v1.0到v1.5,v1.8,v2.0,v2.2,v2.7

              经过无数次的改进，不仅支持<a href=""#"">模板</a>技术，缓存，自定义路由，全站静态化，更包括一键生成文档文件！<a href=""#"">奥博建站</a>系统包括多用户管理，和会员系统，消息系统，评论系统，同时提供接口方便二次开发。

        今天奥博建站系统升级到3.0了，最大的特色就是：多数据库的支持。您现在可以做为网站的数据库，同时支持数据库的切换。小型网站使用更快。</p>";

            const string testContent1 = @" 淘宝网上卖银耳的卖家很多，但是在这里面很多卖家并不是我们通江人，而且也没有我们通江的银耳，很多卖家是以袋料银耳来充通江银耳，所以买到这样的假货心里一定很不舒服吧，很多买银耳的朋友非常担心这个问题，都想买到品质优良的正宗的通江银耳，我们是通江土特产专营，为了让淘宝网上的买家买到正宗的通江银耳，我们建立了专业的网站和论坛来对通江银耳进行宣传，我们还成立了专业的银耳合作社，和农户签订了购销合同，农民采收，我们包销售，这样农民也就有了相当的积极性。我们这样做也是为保证有大量的货源，能让淘宝网上的买家更多的买到正宗的通江银耳，我们销售的银耳力争是在野生天然环境下生长的银耳，虽然产量很小，但品质优良，所以价格高大家也愿意接受。因为我们不销售假银耳，我们的银耳是正宗地通江野生银耳。";



            response.WriteAsync($"测试内容为:<br />{testContent}<br />");


            string x1 = tagsManager.ReplaceSingleTag(testContent);



            response.WriteAsync($"<br/><br/><span style=\"color:green\">替换标签后的内容为:</span><br />{x1}");


            string x2 = tagsManager.ReplaceSingleTag(x1);


            response.WriteAsync($"<br /><br /><span style=\"color:red\">再次替换后的内容为：</span><br />{x2}");


            response.WriteAsync(
                $"<br /><br /><span style=\"color:red\">移除多余的Tags：</span><br />{tagsManager.RemoveAutoTags(x2)}");


        }

        /// <summary>
        /// 返回样式表链接
        /// </summary>
        /// <returns></returns>
        public virtual string ReturnStyleLink()
        {
            var assembly = typeof(TagsManager).Assembly;
            var style = ResourcesReader.Read(assembly, "HttpTag/Assets/style.html");
            return $"<style type=\"text/css\">{style}</style>";
        }

        #region test

        /*
        /// <summary>
        /// 给关键字加链接，同一关键字只加一次
        /// </summary>
        /// <param name="src">源字符串</param>
        /// <param name="keys">关键字泛型</param>
        /// <returns>替换后结果</returns>
        private string keyAddUrl(string src, List<string> keys)
        {
            Regex reg = new Regex(@"(?i)(?:^|(?<!<a\b[^<>]*)>)[^<>]*(?:<|$)");
            int length = 0;
            string temp = string.Empty;
            return reg.Replace(src, delegate(Match m)
            {
                temp = m.Value;
                length = temp.Length;
                for (int i = keys.Count - 1; i >= 0;i--)
                {
                    temp = Regex.Replace(temp,  String.Format(@"(?is)^((?:(?:(?!{0}|</?a\b).)*<a\b(?:(?!</?a\b).)*</a>)*(?:(?!{0}|</?a\b).)*)(?<tag>{0})", Regex.Escape(keys[i])), @"$1<a href=""http://www.21shipin.com"" target=""_blank"" title=""${tag}"">${tag}</a>");
                    if (length != temp.Length)
                    {
                        keys.Remove(keys[i]);
                    }
                    length = temp.Length;
                }
                return temp;
            });
        }
        private void test()
        {
            //调用
            string str1 = "我想学习c语言教程，我想看的是C语言视频教程，其它什么<a href=\"http://www.21shipin.com\" target=\"_blank\" title=\"C语言\">C语言</a>教程，我都不想看。我喜欢C语言，还是C语言。";
            List<string> keys = new List<string>(new string[] { "c语言", "C语言教程", "c语言视频教程" });
            string result = keyAddUrl(str1, keys);
            response.Write(result);
        }
        */
        #endregion
    }
}