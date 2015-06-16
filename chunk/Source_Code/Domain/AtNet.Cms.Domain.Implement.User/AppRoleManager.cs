using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtNet.Cms.Domain.Interface.User;

namespace AtNet.Cms.Domain.Implement.User
{
   internal class AppRoleManager:CreateAppRoleManager
    {
        private IUserRepository rep;
        private int appId;

       public AppRoleManager(IUserRepository rep, int appId)
       {
           // TODO: Complete member initialization
           this.rep = rep;
           this.appId = appId;
       }
        public IList<IRole> GetAppRoles(int appId)
        {
            throw new NotImplementedException();
        }
    }
}
