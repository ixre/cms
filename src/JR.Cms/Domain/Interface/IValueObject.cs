/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/2/22
 * Time: 13:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace JR.Cms.Domain.Interface
{
    /// <summary>
    /// Description of IValueObject.
    /// </summary>
    public interface IValueObject
    {
        /// <summary>
        /// 比较值对象是否相等
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        bool Equal(IValueObject that);
    }
}