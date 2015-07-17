using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace J6.Cms.Domain.Interface.User
{
   public  class AppRoleBind:IValueObject
    {
       public int Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 特定于系统的角色，为0表示所有系统都拥有的角色
        /// </summary>
        public int AppId { get; set; }

       /// <summary>
       /// 应用名称
       /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 标志位
        /// </summary>
        public int Flag { get; set; }

        public bool Equal(IValueObject that)
        {
            return ((RoleValue)that).Flag == this.Flag;
        }

    }
}
