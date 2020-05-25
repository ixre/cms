/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/14
 * Time: 22:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace JR.Stand.Abstracts.Cache
{
    /// <summary>
    /// Description of ICacheUpdatePolicy.
    /// </summary>
    public interface ICachePolicy
    {
        /// <summary>
        /// 清理缓存
        /// </summary>
        void Clean();
    }
}