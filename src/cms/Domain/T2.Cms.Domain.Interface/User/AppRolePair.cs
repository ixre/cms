using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T2.Cms.Domain.Interface.User
{
   public  class AppRolePair:IValueObject
    {
       private List<int> _flags;
        /// <summary>
        /// 特定于系统的角色，为0表示所有系统都拥有的角色
        /// </summary>
        public int AppId { get; set; }

       public List<int> Flags
       {
           get { return this._flags ?? (this._flags = new List<int>()); }
       }

       public int GetFlag()
       {
           int finalFlag = 0;
           foreach (int flag in this.Flags)
           {
               finalFlag |= flag;
           }
           return finalFlag;
       }


       public bool Equal(IValueObject that)
       {
           throw new NotImplementedException();
       }
    }
}
