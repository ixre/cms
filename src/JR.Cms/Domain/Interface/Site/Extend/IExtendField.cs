namespace JR.Cms.Domain.Interface.Site.Extend
{
    public interface IExtendField : IDomain<int>
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// UI类型
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        string DefaultValue { get; set; }

        /// <summary>
        /// 验证数据正则表达式
        /// </summary>
        string Regex { get; set; }

        /// <summary>
        /// 验证提示信息
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// 属性是否可用
        /// </summary>
        //bool Enabled { get; set; }
    }
}