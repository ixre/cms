using System;

namespace JR.Stand.Core.Template
{
    public class TemplateException:Exception
    {
        public TemplateException(string templateId, string err):base($"{err}; 文件:{templateId}")
        {
            
        }

        public TemplateException(string err) : base(err)
        {
        }
    }
}