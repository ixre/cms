using AtNet.Cms.Domain.Interface.Site.Extend;

namespace AtNet.Cms.Domain.Implement.Site.Extend
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseExtendFieldRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IExtendField CreateExtendField(int id, string name)
        {
            return new ExtendField(id, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extendField"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IExtendValue CreateExtendValue(IExtendField extendField,int id,string value)
        {
            return new ExtendValue(id, extendField,value);
        }
    }
}
