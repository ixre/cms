using System.Collections;

namespace Ops.Cms.Extend.SSO
{
    /// <summary>
    /// 人员
    /// </summary>
    public class Person
    {
        private Hashtable _extends;

        /// <summary>
        /// 人员编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }


        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enabled { get; set; }

        ///// <summary>
        ///// 用户信息
        ///// </summary>
        //public User User { get; set; }


        /// <summary>
        /// 扩展信息
        /// </summary>
        public Hashtable Extends
        {
            get
            {
                return this._extends ?? (this._extends = new Hashtable());
            }
        }
    }
}

