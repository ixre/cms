

using JR.Cms.ServiceDto;

namespace JR.Cms.Domain.Interface.Site.Variable
{
    /// <summary>
    /// 站点变量
    /// </summary>
    public class SiteVariable : IValueObject
    {
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否等于
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public bool Equal(IValueObject that)
        {
            SiteVariable t = that as SiteVariable;
            if (t == null) return false;
            return t.Name == this.Name && t.Value == this.Value;
        }

    }
}