/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/2/22
 * Time: 13:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace Ops.Cms.Domain.Interface
{
	/// <summary>
	/// Description of IDomain.
	/// </summary>
	public interface IDomain<T>
	{
        /// <summary>
        /// 编号
        /// </summary>
        T Id { get; set; }

	}
}
