using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JR.Cms.Library.CacheService;
using JR.Cms.ServiceDto;
using JR.Stand.Core;
using JR.Stand.Core.Template.Impl;
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
     <div>
              $categories('prod\,uct',{
                    <div class=""col-md-3 col-lg-2 col-sm-4"">
                        {name}
                    </div>
               }) </div>
";
            
            TemplatePage tp = new TemplatePage(null);
            tp.TemplateHandleObject = new TemplateMock();
            tp.OnPreInit += TemplateMock.CompliedTemplate;
            tp.TemplateContent = temp;
            var content = tp.Compile();
            Console.WriteLine(content);
        }
    }

    public class TemplateMock
    {        
        /// <summary>
        /// 编译模板
        /// </summary>
        public static readonly TemplateHandler<object> CompliedTemplate = (object classInstance, ref string html) =>
        {
            var mctpl = new SimpleTplEngine(classInstance, !true);
            html = mctpl.Execute(html);
        };
        
        protected static MicroTemplateEngine TplEngine = new MicroTemplateEngine(null); //模板引擎

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
    }
}