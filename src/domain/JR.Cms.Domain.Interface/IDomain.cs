/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/2/22
 * Time: 13:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace JR.Cms.Domain.Interface
{
	/// <summary>
	/// Description of IDomain.
	/// </summary>
	public interface IDomain<T>
	{
        /// <summary>
        /// 获取领域对象编号
        /// </summary>
        T GetDomainId();
	}
}
