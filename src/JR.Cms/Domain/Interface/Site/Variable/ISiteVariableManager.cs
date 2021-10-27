using System.Collections.Generic;

namespace JR.Cms.Domain.Interface.Site.Variable
{
    /// <summary>
    /// 站点变量管理器
    /// </summary>
    public interface ISiteVariableManager
    {
        /// <summary>
        /// 保存变量
        /// </summary>
        /// <param name="v"></param>
        void SaveVariable(SiteVariable v);

        /// <summary>
        /// 删除变量
        /// </summary>
        /// <param name="varId"></param>
        void DeleteVariable(int varId);

        /// <summary>
        /// 获取所有的变量
        /// </summary>
        /// <returns></returns>
        IList<SiteVariable> GetAll();

        /// <summary>
        /// 根据名称获取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        SiteVariable Get(string name);
    }
}