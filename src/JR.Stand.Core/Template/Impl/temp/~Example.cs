/********************* 使用TemplatePage类 ************/
/*
            
            string html;
            TemplatePage page = new TemplatePage(templateID,
                new {
                    title = "关于我们",
                    keywords = a.Tags,
                    description = ArchiveUtility.GetOutline(a, 180),
                    content=a.Content
                });
            html = page.ToString();
*/
/********************* 直接替换 ***********************/
/*
            string html = TemplateContext.Regex.Replace(templateID,
                match =>
                {
                    switch (match.Groups[1].Value)
                    {
                        case "title": return "关于我们";
                        case "keywords": return a.Tags;
                        case "description": return ArchiveUtility.GetOutline(a, 180);
                        case "content": return a.Content;
                        default:return TemplateCache.Tags[match.Groups[1].Value];
                    }
                });
*/

