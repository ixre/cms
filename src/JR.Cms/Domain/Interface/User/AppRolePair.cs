using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Cms.Domain.Interface.User
{
    public class AppRolePair : IValueObject
    {
        private List<int> _flags;

        /// <summary>
        /// 特定于系统的角色，为0表示所有系统都拥有的角色
        /// </summary>
        public int AppId { get; set; }

        public List<int> Flags => _flags ?? (_flags = new List<int>());

        public int GetFlag()
        {
            var finalFlag = 0;
            foreach (var flag in Flags) finalFlag |= flag;
            return finalFlag;
        }


        public bool Equal(IValueObject that)
        {
            throw new NotImplementedException();
        }
    }
}