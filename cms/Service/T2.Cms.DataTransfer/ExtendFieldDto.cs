/*
* Copyright(C) 2010-2012 TO2.NET
* 
* File Name	: ExtendAttr
* author_id	: Newmin (new.min@msn.com)
* Create	: 2012/9/30 9:54:29
* Description	:
*
*/

using JR.DevFw.Framework.Automation;

namespace T2.Cms.DataTransfer
{
    /// <summary>
    /// 模块属性
    /// </summary>
    [EntityForm]
    public class ExtendFieldDto
    {

        /// <summary>
        /// 编号
        /// </summary>
        [FormField("Id", Text = "编号", DisableEdit = true,Hidden = true)]
        public int Id { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        [FormField("Name", Text = "属性名称", IsRequired = true, Length = "[0,30]", Descript = "属性的唯一标识")]
        public string Name { get; set; }

        /// <summary>
        /// UI类型
        /// </summary>
        [FormField("Type", Text = "属性类型", IsRequired = true)]
        [SelectField(UseDrop = true, Data = "一请选择一=;文本=1;多行文本=2;数值=3;上传域=5")]
        public string Type { get; set; }


        /// <summary>
        /// 验证数据正则表达式
        /// </summary>
        [FormField("Regex", Text = "数据验证", Descript = "正则表达式匹配（可不填）")]
        public string Regex { get; set; }

        /// <summary>
        /// 验证提示信息
        /// </summary>
        [FormField("Message", Text = "验证提示")]
        public string Message { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        [FormField("DefaultValue", Text = "默认值", MultLine=true)]
        public string DefaultValue { get; set; }

        /*
        /// <summary>
        /// 属性是否可用
        /// </summary>
        [FormField("Enabled", Text = "是否可用")]
        [SelectField(UseDrop = false, Data = "启用:1;禁用:0")]
        public bool Enabled { get; set; }
         */
    }
}
