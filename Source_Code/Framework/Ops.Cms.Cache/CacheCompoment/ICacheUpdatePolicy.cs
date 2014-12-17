/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 22:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace Ops.Cms.Cache.CacheCompoment
{
	/// <summary>
	/// Description of ICacheUpdatePolicy.
	/// </summary>
	public interface ICacheUpdatePolicy
	{
		/// <summary>
		/// 清理缓存
		/// </summary>
		void Clear();
	}
}
