using JR.Cms.Domain.Interface.Site.Variable;

namespace JR.Cms.ServiceDto
{
    /// <summary>
    /// 站点变量传输对象
    /// </summary>
    public class SiteVariableDto
    {
        /// <summary>
        /// 编号
        /// </summary>
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
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public SiteVariable ToVariable()
        {
            return new SiteVariable
            {
                Id = this.Id,
                Name = this.Name,
                Value = this.Value,
                Remark = this.Remark,
            };
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static SiteVariableDto ParseFrom(SiteVariable src)
        {
            return new SiteVariableDto
            {
                Id = src.Id,
                Name = src.Name,
                Value = src.Value,
                Remark = src.Remark,
            };
        }
    }
}