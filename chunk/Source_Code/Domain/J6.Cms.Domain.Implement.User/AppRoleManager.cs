using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using J6.Cms.Domain.Interface.User;

namespace J6.Cms.Domain.Implement.User
{
   internal class AppRoleManager:IAppRoleManager
    {
        private IUserRepository rep;
        private int appId;

       public AppRoleManager(IUserRepository rep, int appId)
       {
           // TODO: Complete member initialization
           this.rep = rep;
           this.appId = appId;
       }

        public IList<RoleValue> GetAppRoles()
        {
            IList<RoleValue> list = this.rep.GetAppRoles(this.appId);
            list.Add(Role.Master);
            list.Add(Role.SiteOwner);
            list.Add(Role.Publisher);
            return list;
        }
    }
}
