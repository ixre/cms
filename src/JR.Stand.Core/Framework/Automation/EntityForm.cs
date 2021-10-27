using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Google.Protobuf.WellKnownTypes;
using JR.Stand.Core.Framework.Web;
using JR.Stand.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using CollectionExtensions = JR.Stand.Core.Framework.Extensions.CollectionExtensions;
using Type = System.Type;

namespace JR.Stand.Core.Framework.Automation
{
    /// <summary>
    /// 实体表单
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class EntityFormAttribute : Attribute
    {
    }

    /// <summary>
    /// 表单列
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class FormFieldAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public FormFieldAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 列分组名称
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 列描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否可以编辑
        /// </summary>
        public bool DisableEdit { get; set; }

        /// <summary>
        /// 是否隐藏显示
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// 是否为密码输入项
        /// </summary>
        public bool IsPassword { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 正则匹配
        /// </summary>
        public string Regex { get; set; }

        /// <summary>
        /// 是否为数字
        /// </summary>
        public bool IsNumber { get; set; }

        /// <summary>
        /// 长度限制，如5-10,0-10
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// 是否多行，多行用TextBox显示
        /// </summary>
        public bool MultiLine { get; set; }
    }

    /// <summary>
    /// 选择列
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class SelectFieldAttribute : Attribute
    {
        /// <summary>
        /// 数据字典，多个用;或|隔开。如：是=1;否=0
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 是否使用下拉，如果为否，则用radiobox
        /// </summary>
        public bool UseDrop { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EntityForm
    {
        /// <summary>
        /// 创建HTML,包含表单及按钮
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="full"></param>
        /// <param name="btnText"></param>
        /// <returns></returns>
        public static string Build<T>(T t, bool full, string btnText) where T : new()
        {
            if (t == null) throw new ArgumentNullException("实体" + typeof (T).Name + "不能为空!");
            StringBuilder sb = new StringBuilder();
            if (full)
            {
                sb.Append("<div class=\"entityForm\">\r\n").Append("<form action=\"\" method=\"post\">\r\n");
            }

            Type type = typeof (T);
            FormFieldAttribute ffa;
            SelectFieldAttribute sfa;
            string value;

            EntityFormAttribute[] eta =
                type.GetCustomAttributes(typeof (EntityFormAttribute), true) as EntityFormAttribute[];
            if (eta != null && eta.Length != 0)
            {
                //读取属性
                PropertyInfo[] prolist = type.GetProperties();
                foreach (PropertyInfo pro in prolist)
                {
                    //枚举则获取对应的数值
                    if (pro.PropertyType.IsEnum)
                    {
                        value = Convert.ToInt32(pro.GetValue(t, null)).ToString();
                    }
                    else
                    {
                        value = (pro.GetValue(t, null) ?? String.Empty).ToString();
                    }

                    FormFieldAttribute[] fieldAttrs =
                        pro.GetCustomAttributes(typeof (FormFieldAttribute), false) as FormFieldAttribute[];
                    if (fieldAttrs == null || fieldAttrs.Length == 0)
                    {
                        continue;
                    }
                    else
                    {
                        ffa = fieldAttrs[0];

                        //设置标题
                        sb.Append("<dl")
                            .Append(ffa.Hidden ? " style=\"display:none\"" : "")
                            .Append(String.IsNullOrEmpty(ffa.Group) ? ">" : " group=\"" + ffa.Group + "\">")
                            .Append("<dt>").Append(ffa.IsRequired ? "<span class=\"required\">*</span>" : "")
                            .Append(String.IsNullOrEmpty(ffa.Text) ? ffa.Name : ffa.Text).Append(":</dt>")
                            .Append("<dd>");

                        if (!ffa.DisableEdit)
                        {
                            //设置UI
                            SelectFieldAttribute[] sfAttrs =
                                pro.GetCustomAttributes(typeof (SelectFieldAttribute), false) as SelectFieldAttribute[];
                            if (sfAttrs != null && sfAttrs.Length != 0)
                            {
                                sfa = sfAttrs[0];
                                //输出SELECT或RADIO
                                if (sfa.UseDrop)
                                {
                                    sb.Append("<select class=\"ui-validate ui-box tb_")
                                        .Append(ffa.Name).Append("\" field=\"")
                                        .Append(ffa.Name)
                                        .Append("\" name=\"field_")
                                        .Append(ffa.Name)
                                        .Append("\"");

                                    if (ffa.IsRequired)
                                    {
                                        sb.Append(" required=\"true\"");
                                    }

                                    sb.Append(">");

                                    //输出选项
                                    string[] data = sfa.Data.Split(';', '|');
                                    string[] opt;
                                    foreach (string dstr in data)
                                    {
                                        opt = dstr.Split('=');
                                        if (opt.Length == 2)
                                        {
                                            sb.Append("<option value=\"").Append(opt[1]).Append("\"")
                                                .Append(value == opt[1] ? " selected=\"selected\"" : "")
                                                .Append(">").Append(opt[0]).Append("</option>");
                                        }
                                    }
                                    sb.Append("</select>");
                                }
                                else
                                {
                                    //输出选项
                                    string[] data = sfa.Data.Split(';', '|');
                                    string[] opt;
                                    foreach (string dstr in data)
                                    {
                                        opt = dstr.Split('=');
                                        if (opt.Length == 2)
                                        {
                                            String groupId = (ffa.Group + ffa.Name).GetHashCode().ToString();
                                            String id = groupId + opt[1];
                                            sb.Append("<input type=\"radio\" name=\"")
                                                .Append(groupId).Append("\" value=\"").Append(opt[1]).Append("\"")
                                                .Append(value == opt[1] ? " selected=\"selected\"" : "")
                                                .Append(" id=\"").Append(id).Append("\"")
                                                .Append("/><label for=\"")
                                                .Append(id).Append("\">").Append(opt[0]).Append("</label>");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (ffa.MultiLine)
                                {
                                    //输出Textbox
                                    sb.Append("<textarea class=\"ui-validate ui-box tb_")
                                        .Append(ffa.Name).Append("\" field=\"")
                                        .Append(ffa.Name)
                                        .Append("\" rows=\"3\" name=\"field_")
                                        .Append(ffa.Name)
                                        .Append("\"");
                                }
                                else
                                {
                                    //输出INPUT
                                    sb.Append("<input class=\"ui-validate ui-box tb_")
                                        .Append(ffa.Name).Append("\" type=\"")
                                        .Append(ffa.IsPassword ? "password" : "text")
                                        .Append("\" field=\"")
                                        .Append(ffa.Name)
                                        .Append("\" name=\"field_")
                                        .Append(ffa.Name)
                                        .Append("\"").Append(" value=\"").Append(value).Append("\"");
                                }

                                if (ffa.IsRequired)
                                {
                                    sb.Append(" required=\"true\"");
                                }

                                if (ffa.IsNumber)
                                {
                                    sb.Append(" isnumber=\"true\"");
                                }

                                if (!String.IsNullOrEmpty(ffa.Length))
                                {
                                    sb.Append(" length=\"").Append(ffa.Length).Append("\"");
                                }

                                if (!String.IsNullOrEmpty(ffa.Regex))
                                {
                                    sb.Append(" regex=\"").Append(ffa.Regex).Append("\"");
                                }

                                if (ffa.MultiLine)
                                {
                                    //关闭Textarea
                                    sb.Append(">").Append(value).Append("</textarea>");
                                }
                                else
                                {
                                    //关闭INPUT
                                    sb.Append(" />");
                                }
                            }
                        }
                        else
                        {
                            sb.Append(value)
                                .Append("<input type=\"hidden\" field=\"")
                                .Append(ffa.Name)
                                .Append("\" name=\"field_")
                                .Append(ffa.Name)
                                .Append("\" value=\"").Append(value).Append("\"/>");
                        }

                        //列描述
                        if (!String.IsNullOrEmpty(ffa.Description))
                        {
                            sb.Append("<span class=\"descript\">").Append(ffa.Description).Append("</span>");
                        }

                        //关闭列
                        sb.Append("</dd></dl>\r\n");
                    }
                }
            }
            else
            {
                sb.Append("<strong style=\"color:red;font-size:14px;\">该实体不支持EntityForm!</strong>");
            }


            if (full)
            {
                sb.Append("<dl><dt>&nbsp;</dt><dd><a id=\"btn\" class=\"btn\" href=\"javascript:;\">")
                    .Append(btnText).Append("</a></dd></dl>")
                    .Append("</form></div>");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 创建HTML，仅包含字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string Build<T>(T t) where T : new()
        {
            return Build(t, false, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static T GetEntity<T>(IDictionary<string,StringValues> form) where T : new()
        {
            const bool allowError = false;

            Type type = typeof (T);
            object t = Activator.CreateInstance(type);
            PropertyInfo[] pros = type.GetProperties();


            Type strType = typeof (String); //字符串类型
            Type bitType = typeof (Boolean); //布尔值类型


            foreach (PropertyInfo pro in pros)
            {
                if (!pro.CanWrite) continue;
                var fieldAttrs = pro.GetCustomAttributes(typeof (FormFieldAttribute), false) as FormFieldAttribute[];
                if (fieldAttrs != null && fieldAttrs.Length != 0)
                {
                    //attr = fieldAttrs[0];
                    string value = form[pro.Name];
                    if (value == null)
                    {
                        value = form["field_" + pro.Name];
                        if (value == null) value = "";
                    }
                    var proType = pro.PropertyType; //属性类型
                    try
                    {
                        var obj = CollectionExtensions.GetPropertyValue<T>(pro, strType, bitType, proType, value);
                        if (obj != null)
                        {
                            pro.SetValue(t, obj, null);
                        }
                    }
                    catch (FormatException exc)
                    {
                        if (allowError)
                        {
                            throw new FormatException("转换错误,属性名：" + pro.Name);
                        }
                    }


                    //if (pro.PropertyType.IsEnum)
                    //{
                    //    int.TryParse(value, out tmpInt);
                    //    pro.SetValue(t, tmpInt, null);
                    //}
                    //else
                    //{
                    //    try
                    //    {
                    //        pro.SetValue(t, Convert.ChangeType(value, pro.PropertyType), null);
                    //    }
                    //    catch (FormatException exc)
                    //    {
                    //        throw new FormatException("转换错误,属性名：" + pro.Name);
                    //    }
                    //}
                }
            }

            return (T) t;
        }
    }
}