using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using JR.Cms.Web.Portal.Template.Model;
using JR.Stand.Core;
using JR.Stand.Core.Template.Impl;
using JR.Stand.Core.Web;
using NUnit.Framework;

namespace JR.Cms.UnitTest
{
    [TestFixture]
    public class TemplateTest
    {
        [Test]
        public void TestCompileFunction()
        {
            var temp = @"
$lang(home) 
$navigator()
${archive.map(视频)}
     <div>
              $categories('prod\,uct',{
                    <div class=""col-md-3 col-lg-2 col-sm-4"">
                        {name}
                    </div>
                    <div style=""background-image:url('{name}')""></div>
        }) </div>
";
            IDataContainer dc = new NormalDataContainer();
            TemplatePage tp = new TemplatePage(dc);
            tp.TemplateHandleObject = new TemplateMock();
            tp.OnPreInit += TemplateMock.CompliedTemplate;
            tp.OnBeforeCompile += (TemplatePage page, ref String content) =>
            {
                var pageArchive = new PageArchive(new ArchiveDto());
                page.AddVariable("archive", pageArchive);
            };
            tp.SetTemplateContent(temp);
            var content = tp.Compile();
            Console.WriteLine(content);
        }
        [Test]
        public void TestVariable()
        {
            //const string expressionPattern = "\\$([a-z_0-9\u4e00-\u9fa5]+)\\((((?!}\\)).)+\\}*)\\)";
            const string expressionPattern = "\\$([a-z_0-9\u4e00-\u9fa5]+)\\(((\\{[^}]*\\})|([^)]*))\\)";
            Regex reg = new Regex(expressionPattern);
            MatchCollection matches = reg.Matches("$archive({ (haha) *(sese2)})  $lang(home)$navigator()");
            var i = 0;
            foreach (Match match in matches)
            {
                Console.WriteLine((i++)+ match.Value+"/"+match.Groups[2]);
            }
        }
    }

    public class TemplateMock
    {        
        /// <summary>
        /// 编译模板
        /// </summary>
        public static readonly TemplateHandler<object> CompliedTemplate = (object classInstance, ref string html) =>
        {
            var microTpl = new SimpleTplEngine(classInstance, !true);
            html = microTpl.Execute(html);
        };

        private static readonly MicroTemplateEngine TplEngine = new MicroTemplateEngine(null); //模板引擎

        [TemplateTag]
        public string Categories(string catPath,string format)
        {
            IList<CategoryDto> categories = new List<CategoryDto>();
            categories.Add(new CategoryDto
            {
                Name = "分类A",
            });
            categories.Add(new CategoryDto
            {
                Name = "分类N",
            });
            var sb = new StringBuilder(400);
            var i = 0;

            foreach (var c in categories.OrderBy(a => a.SortNumber))
            {
                    sb.Append(TplEngine.ResolveHolderFields(format, field =>
                    {
                        switch (field)
                        {
                            default: return string.Empty;

                            case "name": return c.Name;
                            case "tag": return c.Tag;
                            case "path": return c.Path;
                            case "thumbnail":
                            case "id": return c.ID.ToString();

                            //case "pid":  return c.PID.ToString();

                            case "description": return c.Description;
                            case "keywords": return c.Keywords;
                            case "index": return (i + 1).ToString();
                        }
                    }));

                ++i;
            }

            return sb.ToString();
        }

        [TemplateTag]
        public string Lang(string catPath)
        {
            return "haha";
        }

        [TemplateTag]
        public string Navigator()
        {
            return "navigator";
        }
    }
}