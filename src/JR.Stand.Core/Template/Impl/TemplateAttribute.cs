using System;

namespace JR.Stand.Core.Template.Impl
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class TemplateTagAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class TemplateMethodAttribute : Attribute
    {
    }

    /// <summary>
    /// 变量字段属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class TemplateVariableFieldAttribute : Attribute
    {
        public TemplateVariableFieldAttribute()
        {
        }

        public TemplateVariableFieldAttribute(string descript)
        {
            this.Descript = descript;
        }

        public string Descript { get; set; }
    }
}